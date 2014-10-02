using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KSPAdvancedFlyByWire
{

    class FlightManager
    {
        public Configuration m_Configuration = null;

        private FlightProperty m_Yaw = new FlightProperty(-1.0f, 1.0f);
        private float m_YawTrim = 0.0f;

        private FlightProperty m_Pitch = new FlightProperty(-1.0f, 1.0f);
        private float m_PitchTrim = 0.0f;

        private FlightProperty m_Roll = new FlightProperty(-1.0f, 1.0f);
        private float m_RollTrim = 0.0f;

        private FlightProperty m_X = new FlightProperty(-1.0f, 1.0f);
        private FlightProperty m_Y = new FlightProperty(-1.0f, 1.0f);
        private FlightProperty m_Z = new FlightProperty(-1.0f, 1.0f);

        private FlightProperty m_Throttle = new FlightProperty(0.0f, 1.0f);

        private FlightProperty m_WheelThrottle = new FlightProperty(-1.0f, 1.0f);
        private float m_WheelThrottleTrim = 0.0f;

        private FlightProperty m_WheelSteer = new FlightProperty(-1.0f, 1.0f);
        private float m_WheelSteerTrim = 0.0f;

        private FlightProperty m_CameraPitch = new FlightProperty(-1.0f, 1.0f);
        private FlightProperty m_CameraHeading = new FlightProperty(-1.0f, 1.0f);
        private FlightProperty m_CameraZoom = new FlightProperty(-1.0f, 1.0f);

        public void OnFlyByWire(FlightCtrlState state)
        {
            m_Throttle.SetMinMaxValues(-state.mainThrottle, 1.0f - state.mainThrottle);

            foreach (ControllerConfiguration config in m_Configuration.controllers)
            {
                config.iface.Update(state);

                for (int i = 0; i < config.iface.GetAxesCount(); i++)
                {
                    List<ContinuousAction> actions = config.GetCurrentPreset().GetContinuousBinding(i, config.iface.GetButtonsMask());
                    if (actions == null)
                    {
                        continue;
                    }

                    foreach (var action in actions)
                    {
                        float input = config.iface.GetAxisState(i);
                        if (input != 0.0f || action == ContinuousAction.Throttle)
                        {
                            EvaluateContinuousAction(config, action, config.iface.GetAxisState(i), state);
                        }
                    }
                }

                state.yawTrim = m_YawTrim;
                state.pitchTrim = m_PitchTrim;
                state.rollTrim = m_RollTrim;

                state.yaw = Utility.Clamp(state.yaw + m_Yaw.Update() + m_YawTrim, -1.0f, 1.0f);
                state.pitch = Utility.Clamp(state.pitch + m_Pitch.Update() + m_PitchTrim, -1.0f, 1.0f);
                state.roll = Utility.Clamp(state.roll + m_Roll.Update() + m_RollTrim, -1.0f, 1.0f);

                state.X = Utility.Clamp(state.X + m_X.Update(), -1.0f, 1.0f);
                state.Y = Utility.Clamp(state.Y + m_Y.Update(), -1.0f, 1.0f);
                state.Z = Utility.Clamp(state.Z + m_Z.Update(), -1.0f, 1.0f);

                state.wheelSteerTrim = m_WheelSteerTrim;
                state.wheelThrottleTrim = m_WheelThrottleTrim;

                state.mainThrottle = Utility.Clamp(state.mainThrottle + m_Throttle.Update(), 0.0f, 1.0f);
                state.wheelSteer = Utility.Clamp(state.wheelSteer + m_WheelSteer.Update() + m_WheelSteerTrim, -1.0f, 1.0f);
                state.wheelThrottle = Utility.Clamp(state.wheelThrottle + m_WheelThrottle.Update() + m_WheelThrottleTrim, 0.0f, 1.0f);
                
                FlightCamera.CamHdg += m_CameraHeading.Update() * config.cameraSensitivity;
                FlightCamera.CamPitch += m_CameraPitch.Update() * config.cameraSensitivity;
                if (FlightCamera.fetch != null)
                {
                    FlightCamera.fetch.SetDistance(FlightCamera.fetch.Distance + m_CameraZoom.Update());
                }
            }

            if (!m_Yaw.HasIncrement())
            {
                m_Yaw.SetValue(0.0f);
            }

            if (!m_Pitch.HasIncrement())
            {
                m_Pitch.SetValue(0.0f);
            }

            if (!m_Roll.HasIncrement())
            {
                m_Roll.SetValue(0.0f);
            }

            if (!m_X.HasIncrement())
            {
                m_X.SetValue(0.0f);
            }

            if (!m_Y.HasIncrement())
            {
                m_Y.SetValue(0.0f);
            }

            if (!m_Z.HasIncrement())
            {
                m_Z.SetValue(0.0f);
            }

            if (!m_Throttle.HasIncrement())
            {
                m_Throttle.SetVelocity(0.0f);
                m_Throttle.SetAcceleration(0.0f);
            }

            m_WheelThrottle.SetVelocity(0.0f);
            m_WheelThrottle.SetAcceleration(0.0f);
            m_WheelSteer.SetValue(0.0f);

            if (!m_CameraHeading.HasIncrement())
            {
                m_CameraHeading.SetValue(0.0f);
            }

            if (!m_CameraPitch.HasIncrement())
            {
                m_CameraPitch.SetValue(0.0f);
            }

            if (!m_CameraZoom.HasIncrement())
            {
                m_CameraZoom.SetValue(0.0f);
            }

            VesselSAS VesselSAS = FlightGlobals.ActiveVessel.vesselSAS;
            bool overrideSAS = (Math.Abs(state.pitch) > VesselSAS.controlDetectionThreshold) ||
                                    (Math.Abs(state.yaw) > VesselSAS.controlDetectionThreshold) ||
                                    (Math.Abs(state.roll) > VesselSAS.controlDetectionThreshold);
            VesselSAS.ManualOverride(overrideSAS);
        }

        public void EvaluateDiscreteAction(ControllerConfiguration controller, DiscreteAction action, FlightCtrlState state)
        {
            KerbalEVA eva = null;
            if (FlightGlobals.ActiveVessel.isEVA)
            {
                eva = FlightGlobals.ActiveVessel.GetComponent<KerbalEVA>();
            }

            switch (action)
            {
                case DiscreteAction.None:
                    return;
                case DiscreteAction.YawPlus:
                    m_Yaw.SetIncrement(1, controller.discreteActionStep);
                    return;
                case DiscreteAction.YawMinus:
                    m_Yaw.SetIncrement(-1, controller.discreteActionStep);
                    return;
                case DiscreteAction.PitchPlus:
                    m_Pitch.SetIncrement(1, controller.discreteActionStep);
                    return;
                case DiscreteAction.PitchMinus:
                    m_Pitch.SetIncrement(-1, controller.discreteActionStep);
                    return;
                case DiscreteAction.RollPlus:
                    m_Roll.SetIncrement(1, controller.discreteActionStep);
                    return;
                case DiscreteAction.RollMinus:
                    m_Roll.SetIncrement(-1, controller.discreteActionStep);
                    return;
                case DiscreteAction.XPlus:
                    m_X.SetIncrement(1, controller.discreteActionStep);
                    return;
                case DiscreteAction.XMinus:
                    m_X.SetIncrement(-1, controller.discreteActionStep);
                    return;
                case DiscreteAction.YPlus:
                    m_Y.SetIncrement(1, controller.discreteActionStep);
                    return;
                case DiscreteAction.YMinus:
                    m_Y.SetIncrement(-1, controller.discreteActionStep);
                    return;
                case DiscreteAction.ZPlus:
                    m_Z.SetIncrement(1, controller.discreteActionStep);
                    return;
                case DiscreteAction.ZMinus:
                    m_Z.SetIncrement(-1, controller.discreteActionStep);
                    return;
                case DiscreteAction.ThrottlePlus:
                    m_Throttle.SetIncrement(1, controller.discreteActionStep);
                    return;
                case DiscreteAction.ThrottleMinus:
                    m_Throttle.SetIncrement(-1, controller.discreteActionStep);
                    return;
                case DiscreteAction.Stage:
                    Staging.ActivateNextStage();
                    return;
                case DiscreteAction.Gear:
                    FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Gear);
                    return;
                case DiscreteAction.Light:
                    FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Light);
                    return;
                case DiscreteAction.RCS:
                    FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.RCS);
                    return;
                case DiscreteAction.SAS:
                    FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.SAS);
                    return;
                case DiscreteAction.Brakes:
                    FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Brakes);
                    return;
                case DiscreteAction.Abort:
                    FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Abort);
                    return;
                case DiscreteAction.Custom01:
                    FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Custom01);
                    return;
                case DiscreteAction.Custom02:
                    FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Custom02);
                    return;
                case DiscreteAction.Custom03:
                    FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Custom03);
                    return;
                case DiscreteAction.Custom04:
                    FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Custom04);
                    return;
                case DiscreteAction.Custom05:
                    FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Custom05);
                    return;
                case DiscreteAction.Custom06:
                    FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Custom06);
                    return;
                case DiscreteAction.Custom07:
                    FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Custom07);
                    return;
                case DiscreteAction.Custom08:
                    FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Custom08);
                    return;
                case DiscreteAction.Custom09:
                    FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Custom09);
                    return;
                case DiscreteAction.Custom10:
                    FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Custom10);
                    return;
                case DiscreteAction.EVAToggleJetpack:
                    if (eva != null)
                    {
                        eva.JetpackDeployed = !eva.JetpackDeployed;
                    }
                    return;
                case DiscreteAction.EVAToggleHeadlamps:
                    if (eva != null)
                    {
                        eva.lampOn = !eva.lampOn;
                    }
                    return;
                case DiscreteAction.CutThrottle:
                    m_Throttle.SetValue(-state.mainThrottle);
                    return;
                case DiscreteAction.FullThrottle:
                    m_Throttle.SetValue(1.0f - state.mainThrottle);
                    return;
                case DiscreteAction.NextPreset:
                    if (controller.currentPreset >= controller.presets.Count - 1)
                    {
                        return;
                    }

                    controller.currentPreset++;
                    ScreenMessages.PostScreenMessage("PRESET: " + controller.GetCurrentPreset().name.ToUpper(), 1.0f, ScreenMessageStyle.UPPER_CENTER);
                    return;
                case DiscreteAction.PreviousPreset:
                    if (controller.currentPreset <= 0)
                    {
                        return;
                    }

                    controller.currentPreset--;
                    ScreenMessages.PostScreenMessage("PRESET: " + controller.GetCurrentPreset().name.ToUpper(), 1.0f, ScreenMessageStyle.UPPER_CENTER);
                    return;
                case DiscreteAction.CyclePresets:
                    controller.currentPreset++;
                    if (controller.currentPreset >= controller.presets.Count)
                    {
                        controller.currentPreset = 0;
                    }
                    ScreenMessages.PostScreenMessage("PRESET: " + controller.GetCurrentPreset().name.ToUpper(), 1.0f, ScreenMessageStyle.LOWER_CENTER);
                    return;
                case DiscreteAction.CameraZoomPlus:
                    m_CameraZoom.SetIncrement(1, controller.discreteActionStep);
                    return;
                case DiscreteAction.CameraZoomMinus:
                    m_CameraZoom.SetIncrement(-1, controller.discreteActionStep);
                    return;
                case DiscreteAction.CameraXPlus:
                    m_CameraHeading.SetIncrement(1, controller.discreteActionStep);
                    return;
                case DiscreteAction.CameraXMinus:
                    m_CameraHeading.SetIncrement(-1, controller.discreteActionStep);
                    return;
                case DiscreteAction.CameraYPlus:
                    m_CameraPitch.SetIncrement(1, controller.discreteActionStep);
                    return;
                case DiscreteAction.CameraYMinus:
                    m_CameraPitch.SetIncrement(-1, controller.discreteActionStep);
                    return;
                case DiscreteAction.OrbitMapToggle:
                    if (!MapView.MapIsEnabled)
                    {
                        MapView.EnterMapView();
                    }
                    else
                    {
                        MapView.ExitMapView();
                    }
                    return;
                case DiscreteAction.TimeWarpPlus:
                    WarpController.IncreaseWarp();
                    return;
                case DiscreteAction.TimeWarpMinus:
                    WarpController.DecreaseWarp();
                    return;
                case DiscreteAction.PhysicsTimeWarpPlus:
                    WarpController.IncreasePhysicsWarp();
                    return;
                case DiscreteAction.PhysicsTimeWarpMinus:
                    WarpController.DecreasePhysicsWarp();
                    return;
                case DiscreteAction.NavballToggle:
                    if (MapView.MapIsEnabled && MapView.fetch != null)
                    {
                        MapView.fetch.maneuverModeToggle.OnPress.Invoke();
                    }
                    return;
                case DiscreteAction.IVAViewToggle:
                    if (CameraManager.Instance != null)
                    {
                        CameraManager.Instance.SetCameraIVA();
                    }
                    return;
                case DiscreteAction.CameraViewToggle:
                    if (FlightCamera.fetch != null)
                    {
                        FlightCamera.fetch.SetNextMode();
                    }
                    return;
                case DiscreteAction.SASHold:
                    FlightGlobals.ActiveVessel.ActionGroups.SetGroup(KSPActionGroup.SAS, true);
                    return;
                case DiscreteAction.LockStage:
                    if (FlightInputHandler.fetch != null)
                    {
                        FlightInputHandler.fetch.stageLock = !FlightInputHandler.fetch.stageLock;
                    }
                    return;
                case DiscreteAction.TogglePrecisionControls:
                    if (FlightInputHandler.fetch != null)
                    {
                        FlightInputHandler.fetch.precisionMode = !FlightInputHandler.fetch.precisionMode;
                    }
                    return;
                case DiscreteAction.ResetTrim:
                    m_YawTrim = 0.0f;
                    m_PitchTrim = 0.0f;
                    m_RollTrim = 0.0f;
                    state.ResetTrim();
                    return;
            }
        }

        public void EvaluateDiscreteActionRelease(ControllerConfiguration controller, DiscreteAction action, FlightCtrlState state)
        {
            switch (action)
            {
                case DiscreteAction.None:
                    return;
                case DiscreteAction.YawPlus:
                    m_Yaw.SetIncrement(0);
                    return;
                case DiscreteAction.YawMinus:
                    m_Yaw.SetIncrement(0);
                    return;
                case DiscreteAction.PitchPlus:
                    m_Pitch.SetIncrement(0);
                    return;
                case DiscreteAction.PitchMinus:
                    m_Pitch.SetIncrement(0);
                    return;
                case DiscreteAction.RollPlus:
                    m_Roll.SetIncrement(0);
                    return;
                case DiscreteAction.RollMinus:
                    m_Roll.SetIncrement(0);
                    return;
                case DiscreteAction.XPlus:
                    m_X.SetIncrement(0);
                    return;
                case DiscreteAction.XMinus:
                    m_X.SetIncrement(0);
                    return;
                case DiscreteAction.YPlus:
                    m_Y.SetIncrement(0);
                    return;
                case DiscreteAction.YMinus:
                    m_Y.SetIncrement(0);
                    return;
                case DiscreteAction.ZPlus:
                    m_Z.SetIncrement(0);
                    return;
                case DiscreteAction.ZMinus:
                    m_Z.SetIncrement(0);
                    return;
                case DiscreteAction.ThrottlePlus:
                    m_Throttle.SetIncrement(0);
                    return;
                case DiscreteAction.ThrottleMinus:
                    m_Throttle.SetIncrement(0);
                    return;
                case DiscreteAction.CameraZoomPlus:
                    m_CameraZoom.SetIncrement(0);
                    return;
                case DiscreteAction.CameraZoomMinus:
                    m_CameraZoom.SetIncrement(0);
                    return;
                case DiscreteAction.CameraXPlus:
                    m_CameraHeading.SetIncrement(0);
                    return;
                case DiscreteAction.CameraXMinus:
                    m_CameraHeading.SetIncrement(0);
                    return;
                case DiscreteAction.CameraYPlus:
                    m_CameraPitch.SetIncrement(0);
                    return;
                case DiscreteAction.CameraYMinus:
                    m_CameraPitch.SetIncrement(0);
                    return;
                case DiscreteAction.SASHold:
                    FlightGlobals.ActiveVessel.ActionGroups.SetGroup(KSPActionGroup.SAS, false);
                    return;
            }
        }

        public void EvaluateContinuousAction(ControllerConfiguration controller, ContinuousAction action, float value, FlightCtrlState state)
        {
            switch (action)
            {
                case ContinuousAction.None:
                    return;
                case ContinuousAction.Yaw:
                    m_Yaw.SetValue(value);
                    return;
                case ContinuousAction.NegativeYaw:
                    m_Yaw.SetValue(-value);
                    return;
                case ContinuousAction.YawTrim:
                    m_YawTrim = Utility.Clamp(m_YawTrim + value, -1.0f, 1.0f);
                    return;
                case ContinuousAction.Pitch:
                    m_Pitch.SetValue(value);
                    return;
                case ContinuousAction.NegativePitch:
                    m_Pitch.SetValue(-value);
                    return;
                case ContinuousAction.PitchTrim:
                    m_PitchTrim = Utility.Clamp(m_PitchTrim + value, -1.0f, 1.0f);
                    return;
                case ContinuousAction.Roll:
                    m_Roll.SetValue(value);
                    return;
                case ContinuousAction.NegativeRoll:
                    m_Roll.SetValue(-value);
                    return;
                case ContinuousAction.RollTrim:
                    m_RollTrim = Utility.Clamp(m_RollTrim + value, -1.0f, 1.0f);
                    return;
                case ContinuousAction.X:
                    m_X.SetValue(value);
                    return;
                case ContinuousAction.NegativeX:
                    m_X.SetValue(-value);
                    return;
                case ContinuousAction.Y:
                    m_Y.SetValue(value);
                    return;
                case ContinuousAction.NegativeY:
                    m_Y.SetValue(-value);
                    return;
                case ContinuousAction.Z:
                    m_Z.SetValue(value);
                    return;
                case ContinuousAction.NegativeZ:
                    m_Z.SetValue(-value);
                    return;
                case ContinuousAction.Throttle:
                    m_Throttle.SetMinMaxValues(-state.mainThrottle, 1.0f - state.mainThrottle);
                    m_Throttle.SetValue(value - state.mainThrottle);
                    return;
                case ContinuousAction.ThrottleIncrement:
                    m_Throttle.Increment(value * controller.incrementalActionSensitivity);
                    return;
                case ContinuousAction.ThrottleDecrement:
                    m_Throttle.Increment(-value * controller.incrementalActionSensitivity);
                    return;
                case ContinuousAction.WheelThrottle:
                    m_WheelThrottle.SetValue(value);
                    return;
                case ContinuousAction.WheelSteer:
                    m_WheelSteer.SetValue(value);
                    return;
                case ContinuousAction.WheelThrottleTrim:
                    m_WheelThrottleTrim = Utility.Clamp(m_WheelThrottleTrim + value, -1.0f, 1.0f);
                    return;
                case ContinuousAction.WheelSteerTrim:
                    m_WheelSteerTrim = Utility.Clamp(m_WheelSteerTrim + value, -1.0f, 1.0f);
                    return;
                case ContinuousAction.CameraX:
                    m_CameraHeading.Increment(value);
                    return;
                case ContinuousAction.CameraY:
                    m_CameraPitch.Increment(value);
                    return;
                case ContinuousAction.CameraZoom:
                    m_CameraZoom.Increment(value);
                    return;
            }
        }

    }

}
