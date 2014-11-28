using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace KSPAdvancedFlyByWire
{
    class EVAController
    {
        /**
         * Current EVA Bugs:
         * Interact on ladder gives NullReference if not near a hatch
         */

        private const float EVARotationStep = 57.29578f; // From KerbalEVA.UpdateHeading()

        private List<FieldInfo> vectorFields;
        private List<FieldInfo> floatFields;
        private List<FieldInfo> eventFields;
        private FieldInfo colliderListField;
        private KFSMEventCondition runStopCondition;
        private KFSMEventCondition ladderStopCondition;
        private KFSMEventCondition swimStopCondition;
        private KFSMEventCondition eventConditionDisabled = ((KFSMState s) => false);
        private bool m_autoRunning = false;

        private static EVAController instance;

        public static EVAController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EVAController();
                }
                return instance;
            }
        }

        public EVAController()
        {
            LoadReflectionFields();
        }

        public void UpdateEVAFlightProperties(ControllerConfiguration config, FlightCtrlState state)
        {
            if (!FlightGlobals.ActiveVessel.isEVA)
                return;

            KerbalEVA eva = GetKerbalEVA();

            if (this.m_autoRunning)
            {
                state.Z = 1f;
            }

            if (!state.isNeutral)
            {
                Vector3 moveDirection = Vector3.zero;
                Vector3 ladderDirection = Vector3.zero;

                //Debug.Log("State: " + eva.fsm.currentStateName + " (< " + eva.fsm.lastEventName + ")");
                //Debug.Log("MovementMode: " + eva.CharacterFrameMode);
                //Debug.Log("Landed? " + eva.part.GroundContact + ", " + eva.vessel.LandedOrSplashed + ", " + eva.vessel.Landed);

                if (state.Y != 0)
                {
                    moveDirection += (!eva.CharacterFrameMode ? eva.fUp : eva.transform.up) * state.Y;
                    ladderDirection += state.Y > 0 ? eva.transform.up : -eva.transform.up;
                }
                if (state.X != 0)
                {
                    moveDirection += (!eva.CharacterFrameMode ? eva.fRgt : eva.transform.right) * state.X;
                    ladderDirection += (state.X > 0 ? eva.transform.right : -eva.transform.right);
                }
                if (state.Z != 0)
                {
                    moveDirection += (!eva.CharacterFrameMode ? eva.fFwd : eva.transform.forward) * state.Z;
                    ladderDirection += (state.Z > 0 ? eva.transform.up : -eva.transform.up);
                }

                if (eva.vessel.LandedOrSplashed && !eva.JetpackDeployed && !eva.OnALadder && !eva.isRagdoll)
                {
                    float moveSpeed = moveDirection.magnitude;
                    // Decrease max moveSpeed when not moving in a forwards direction.
                    if (Vector3.Angle(moveDirection, eva.transform.forward) > 45)
                    {
                        moveSpeed = Mathf.Clamp(moveSpeed, 0, 0.25f);
                    }
                    this.SetMoveSpeed(moveSpeed);
                }

                // We disable "Run/Ladder/Swim Stop" check while moving.
                if (eva.OnALadder && (state.Y != 0 || state.Z != 0))
                {
                    DisableLadderStopCondition(eva);
                }
                if (eva.vessel.Splashed && (state.X != 0 || state.Z != 0))
                {
                    DisableSwimStopCondition(eva);
                }

                //Debug.Log("Runspeed: " + moveDirection.magnitude);
                moveDirection.Normalize();
                //Debug.Log("MoveDirection: " + moveDirection);
                //Debug.Log("LadderDirection: "+ ladderDirection);
                this.vectorFields[0].SetValue(eva, moveDirection);              //vector3_0 = MoveDirection
                this.vectorFields[2].SetValue(eva, moveDirection);              //vector3_2 = JetpackDirection
                this.vectorFields[6].SetValue(eva, ladderDirection);            //vector3_6 = LadderDirection

                Quaternion rotation = Quaternion.identity;
                rotation *= Quaternion.AngleAxis(eva.turnRate * state.pitch * EVARotationStep * Time.deltaTime, -eva.transform.right);
                rotation *= Quaternion.AngleAxis(eva.turnRate * state.yaw * EVARotationStep * Time.deltaTime, eva.transform.up);
                rotation *= Quaternion.AngleAxis(eva.turnRate * state.roll * EVARotationStep * Time.deltaTime, -eva.transform.forward);
                if (rotation != Quaternion.identity)
                {
                    this.vectorFields[8].SetValue(eva, rotation * (Vector3)this.vectorFields[8].GetValue(eva));
                    this.vectorFields[13].SetValue(eva, rotation * (Vector3)this.vectorFields[13].GetValue(eva));
                }

                if (!moveDirection.IsZero() && !eva.OnALadder)
                {
                    if (eva.CharacterFrameMode)
                    {
                        this.vectorFields[8].SetValue(eva, eva.fFwd);           //vector3_8
                        this.vectorFields[13].SetValue(eva, eva.fUp);           //vector3_13
                    }
                    else
                    {
                        this.vectorFields[8].SetValue(eva, moveDirection);      //vector3_8
                        this.vectorFields[13].SetValue(eva, eva.fUp);           //vector3_13
                    }
                }
            }
            else
            {
                ReEnableStopConditions(eva);
            }
        }

        public void DoInteract()
        {
            if (FlightGlobals.ActiveVessel.isEVA)
            {
                KerbalEVA eva = GetKerbalEVA();

                List<Collider> colliders = this.GetEVAColliders(eva);
                if (colliders.Count > 0)
                {
                    if (eva.OnALadder)
                    {
                        eva.fsm.RunEvent((KFSMEvent)this.eventFields[34].GetValue(eva)); // Board Vessel
                    }
                    else
                    {
                        eva.fsm.RunEvent((KFSMEvent)this.eventFields[22].GetValue(eva)); // Grab Ladder
                    }
                }
            }
            else
            {
                GoEVA();
            }
        }

        public void DoJump()
        {
            if (!FlightGlobals.ActiveVessel.isEVA)
                return;

            KerbalEVA eva = GetKerbalEVA();
            if (eva.OnALadder)
            {
                eva.fsm.RunEvent((KFSMEvent)this.eventFields[27].GetValue(eva)); // Ladder Let Go
            }
            else
            {
                eva.fsm.RunEvent((KFSMEvent)this.eventFields[9].GetValue(eva)); // Jump Start
            }
        }

        public void DoPlantFlag()
        {
            if (!FlightGlobals.ActiveVessel.isEVA)
                return;

            KerbalEVA eva = GetKerbalEVA();
            if (eva.part.GroundContact)
                eva.PlantFlag();
        }

        public void ToggleJetpack()
        {
            if (!FlightGlobals.ActiveVessel.isEVA)
                return;

            KerbalEVA eva = GetKerbalEVA();
            KFSMEvent togglePackEvent = (KFSMEvent)eventFields[17].GetValue(eva);
            togglePackEvent.GoToStateOnEvent = eva.fsm.CurrentState;
            eva.fsm.RunEvent(togglePackEvent);
        }

        public void SetMoveSpeed(float runSpeed)
        {
            if (!FlightGlobals.ActiveVessel.isEVA)
                return;

            KerbalEVA eva = GetKerbalEVA();
            if (IsVesselActive(eva.vessel)
                && !eva.JetpackDeployed && !eva.OnALadder && !eva.isRagdoll && !eva.vessel.Splashed)
            {
                //Debug.Log("RunSpeed is: " + runSpeed);
                if (runSpeed > 0.75f)
                {
                    eva.fsm.RunEvent((KFSMEvent)eventFields[2].GetValue(eva)); // Start Run
                    DisableRunStopCondition(eva);
                }
                else
                {
                    eva.fsm.RunEvent((KFSMEvent)eventFields[3].GetValue(eva)); // Stop Run
                }
                floatFields[6].SetValue(eva, runSpeed * eva.runSpeed);
            }
        }

        public void ToggleHeadlamp()
        {
            if (!FlightGlobals.ActiveVessel.isEVA)
                return;

            KerbalEVA eva = GetKerbalEVA();
            eva.headLamp.SetActive(!eva.lampOn);
            eva.lampOn = !eva.lampOn;
        }

        public void ToggleAutorun()
        {
            this.m_autoRunning = !this.m_autoRunning;
            ScreenMessages.PostScreenMessage("AutoRun: " + (this.m_autoRunning ? "ON" : "OFF"), 2f, ScreenMessageStyle.LOWER_CENTER);
        }

        public void GoEVA()
        {
            if (HighLogic.CurrentGame.Parameters.Flight.CanEVA &&
                CameraManager.Instance.currentCameraMode == CameraManager.CameraMode.IVA)
            {
                foreach (ProtoCrewMember pcm in FlightGlobals.ActiveVessel.GetVesselCrew())
                {
                    if (pcm.KerbalRef.eyeTransform == InternalCamera.Instance.transform.parent)
                    {
                        CameraManager.Instance.SetCameraFlight();
                        //Delay EVA spawn until next FixedUpdate. Prevents push to vessel.
                        FlightEVA.fetch.StartCoroutine(GoEVADelayed(pcm.KerbalRef));
                        break;
                    }
                }
            }
        }


        // PRIVATE METHODS //

        private IEnumerator GoEVADelayed(Kerbal kerbal)
        {
            yield return new WaitForFixedUpdate();
            FlightEVA.SpawnEVA(kerbal);
        }

        private void DisableRunStopCondition(KerbalEVA eva)
        {
            if (this.runStopCondition == null)
            {
                //Debug.Log("Disabling RunStop");
                KFSMEvent eRunStop = (KFSMEvent)eventFields[3].GetValue(eva); // End Run
                this.runStopCondition = eRunStop.OnCheckCondition;
                eRunStop.OnCheckCondition = this.eventConditionDisabled;
            }
        }
        private void DisableLadderStopCondition(KerbalEVA eva)
        {
            if (this.ladderStopCondition == null)
            {
                //Debug.Log("Disabling LadderStop");
                KFSMEvent eLadderStop = (KFSMEvent)eventFields[26].GetValue(eva); // Ladder Stop
                this.ladderStopCondition = eLadderStop.OnCheckCondition;
                eLadderStop.OnCheckCondition = this.eventConditionDisabled;
            }
        }

        private void DisableSwimStopCondition(KerbalEVA eva)
        {
            if (this.swimStopCondition == null)
            {
                //Debug.Log("Disabling SwimStop");
                KFSMEvent eSwimStop = (KFSMEvent)eventFields[21].GetValue(eva); // Swim Stop
                this.swimStopCondition = eSwimStop.OnCheckCondition;
                eSwimStop.OnCheckCondition = this.eventConditionDisabled;
            }
        }

        private void ReEnableStopConditions(KerbalEVA eva)
        {
            if (this.ladderStopCondition != null)
            {
                //Debug.Log("Re-enable LadderStop");
                KFSMEvent eLadderStop = (KFSMEvent)eventFields[26].GetValue(eva);
                eLadderStop.OnCheckCondition = this.ladderStopCondition;
                this.ladderStopCondition = null;
            }
            if (this.swimStopCondition != null)
            {
                //Debug.Log("Re-enable SwimStop");
                KFSMEvent eSwimStop = (KFSMEvent)eventFields[21].GetValue(eva);
                eSwimStop.OnCheckCondition = this.swimStopCondition;
                this.swimStopCondition = null;
            }
            if (this.runStopCondition != null)
            {
                //Debug.Log("Re-enable RunStop");
                KFSMEvent eRunStop = (KFSMEvent)eventFields[3].GetValue(eva);
                eRunStop.OnCheckCondition = this.runStopCondition;
                this.runStopCondition = null;
            }
        }

        private List<Collider> GetEVAColliders(KerbalEVA eva)
        {
            return (List<Collider>)this.colliderListField.GetValue(eva);
        }

        private bool IsVesselActive(Vessel vessel)
        {
            return vessel.state == Vessel.State.ACTIVE && !vessel.packed;
        }

        private KerbalEVA GetKerbalEVA()
        {
            return FlightGlobals.ActiveVessel.GetComponent<KerbalEVA>();
        }

        private void LoadReflectionFields()
        {
            List<FieldInfo> fields = new List<FieldInfo>(typeof(KerbalEVA).GetFields(
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
            this.vectorFields = new List<FieldInfo>(fields.Where<FieldInfo>(f => f.FieldType.Equals(typeof(Vector3))));
            this.floatFields = new List<FieldInfo>(fields.Where<FieldInfo>(f => f.FieldType.Equals(typeof(float))));
            this.eventFields = new List<FieldInfo>(fields.Where<FieldInfo>(f => f.FieldType.Equals(typeof(KFSMEvent))));
            this.colliderListField = new List<FieldInfo>(fields.Where<FieldInfo>(f => f.FieldType.Equals(typeof(List<Collider>))))[0];
        }
    }
}
