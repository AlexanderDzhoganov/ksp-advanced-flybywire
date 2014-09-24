using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace KSPAdvancedFlyByWire
{

    [KSPAddon(KSPAddon.Startup.Flight, true)]
    public class AdvancedFlyByWire : MonoBehaviour
    {

        private KSP.IO.PluginConfiguration m_Config;
        private IController m_Controller = null;

        private List<ControllerPreset> m_Presets = new List<ControllerPreset>();
        private int m_CurrentPreset = 0;

        private float m_DiscreteActionStep = 0.15f;
        private float m_AnalogDiscretizationCutoff = 0.8f;
        private CurveType m_AnalogInputCurveType = CurveType.Identity;

        private float m_Throttle = 0.0f;
        private bool m_Modifier = false;

        private bool m_CallbackSet = false;

        private CameraManager.CameraMode m_OriginalCameraMode;

        private ControllerPreset GetCurrentPreset()
        {
            return m_Presets[m_CurrentPreset];
        }

        void SetAnalogInputCurveType(CurveType type)
        {
            m_AnalogInputCurveType = type;
            m_Controller.analogInputEvaluationCurve = CurveFactory.Instantiate(type);
        }

        void SetAnalogDiscretizationCutoff(float cutoff)
        {
            m_AnalogDiscretizationCutoff = cutoff;
            m_Controller.analogDiscretizationCutoff = cutoff;
        }

        private void SavePresetsToDisk()
        {
            m_Config.SetValue("AnalogInputCurveType", m_AnalogInputCurveType);
            m_Config.SetValue("AnalogInputDiscretizationCutoff", m_AnalogDiscretizationCutoff);

            m_Config.SetValue("PresetsCount", m_Presets.Count);
            m_Config.SetValue("SelectedPreset", m_CurrentPreset);

            for (int i = 0; i < m_Presets.Count; i++)
            {
                m_Config.SetValue("Preset" + i, m_Presets[i]);
            }

            m_Config.save();
        }

        public void Awake()
        {
            print("KSPAdvancedFlyByWire: initialized");

            m_Config = KSP.IO.PluginConfiguration.CreateForType<AdvancedFlyByWire>();
            m_Config.load();

            m_AnalogInputCurveType = m_Config.GetValue<CurveType>("AnalogInputCurveType", CurveType.Identity);
            m_AnalogDiscretizationCutoff = m_Config.GetValue<float>("AnalogInputDiscretizationCutoff", 0.8f);

            int presetsCount = m_Config.GetValue<int>("PresetsCount", 0);
            int selectedPreset = m_Config.GetValue<int>("SelectedPreset", 0);

            if (presetsCount == 0)
            {
                m_Config.SetValue("Preset0", new ControllerPreset());
            }
            
            m_Config.save();

            for (int i = 0; i < presetsCount; i++)
            {
                m_Presets.Add(m_Config.GetValue<ControllerPreset>("Preset" + i));
            }

            m_CurrentPreset = selectedPreset;

            m_Controller = new XInputController();
            m_Controller.analogDiscretizationCutoff = m_AnalogDiscretizationCutoff;
            m_Controller.analogInputEvaluationCurve = CurveFactory.Instantiate(m_AnalogInputCurveType);

            m_Controller.buttonPressedCallback = new XInputController.ButtonPressedCallback(ButtonPressedCallback);
            m_Controller.buttonReleasedCallback = new XInputController.ButtonReleasedCallback(ButtonReleasedCallback);
            m_Controller.discretizedAnalogInputPressedCallback = new XInputController.DiscretizedAnalogInputPressedCallback(DiscretizedAnalogInputPressedCallback);
        }

        public void OnDestroy()
        {
        }

        void DoMainWindow(int index)
        {
        }

        void ButtonPressedCallback(Button button, FlightCtrlState state)
        {
            EvaluateDiscreteAction(GetCurrentPreset().GetButton(button, m_Modifier), state);
        }

        void ButtonReleasedCallback(Button button, FlightCtrlState state)
        {
            EvaluateDiscreteActionRelease(GetCurrentPreset().GetButton(button, m_Modifier), state);
        }

        void DiscretizedAnalogInputPressedCallback(AnalogInput input, FlightCtrlState state)
        {
            EvaluateDiscreteAction(GetCurrentPreset().GetDiscretizedAnalogInput(input, m_Modifier), state);
        }

        private void OnFlyByWire(FlightCtrlState state)
        {
            FlightGlobals.ActiveVessel.VesselSAS.ManualOverride(true);
            m_Controller.Update(state);
            
            state.mainThrottle = m_Throttle;
            
            for (int i = 0; i < 6; i++)
            {
                AnalogInput input = (AnalogInput)i;
                EvaluateContinuousAction(GetCurrentPreset().GetAnalogInput(input, m_Modifier), m_Controller.GetAnalogInput(input), state);
            }

            FlightGlobals.ActiveVessel.VesselSAS.ManualOverride(false);
        }
        
        private void EvaluateDiscreteAction(DiscreteAction action, FlightCtrlState state)
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
            case DiscreteAction.Modifier:
                m_Modifier = true;
                return;
            case DiscreteAction.YawPlus:
                state.yaw += m_DiscreteActionStep;
                return;
            case DiscreteAction.YawMinus:
                state.yaw -= m_DiscreteActionStep;
                return;
            case DiscreteAction.PitchPlus:
                state.pitch += m_DiscreteActionStep;
                return;
            case DiscreteAction.PitchMinus:
                state.pitch -= m_DiscreteActionStep;
                return;
            case DiscreteAction.RollPlus:
                state.roll += m_DiscreteActionStep;
                return;
            case DiscreteAction.RollMinus:
                state.roll -= m_DiscreteActionStep;
                return;
            case DiscreteAction.XPlus:
                state.X += m_DiscreteActionStep;
                return;
            case DiscreteAction.XMinus:
                state.X -= m_DiscreteActionStep;
                return;
            case DiscreteAction.YPlus:
                state.Y += m_DiscreteActionStep;
                return;
            case DiscreteAction.YMinus:
                state.Y -= m_DiscreteActionStep;
                return;
            case DiscreteAction.ZPlus:
                state.Z += m_DiscreteActionStep;
                return;
            case DiscreteAction.ZMinus:
                state.Z -= m_DiscreteActionStep;
                return;
            case DiscreteAction.ThrottlePlus:
                m_Throttle += m_DiscreteActionStep;
                return;
            case DiscreteAction.ThrottleMinus:
                m_Throttle -= m_DiscreteActionStep;
                return;
            case DiscreteAction.Stage:
                FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Stage);
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
            case DiscreteAction.EVAJetpackActivate:
                if (eva != null)
                {
                    eva.JetpackDeployed = !eva.JetpackDeployed;
                }
                return;
            case DiscreteAction.EVAJetCounterClockwise:
                return;
            case DiscreteAction.EVAJetpackClockwise:
                return;
            case DiscreteAction.EVAJetPitchPlus:
                return;
            case DiscreteAction.EVAJetPitchMinus:
                return;
            case DiscreteAction.EVAJump:
                return;
            case DiscreteAction.EVAReorientAttitude:
                return;
            case DiscreteAction.EVAUseBoard:
                return;
            case DiscreteAction.EVADirectionJump:
                return;
            case DiscreteAction.EVASprint:
                return;
            case DiscreteAction.EVAHeadlamps:
                if (eva != null)
                {
                    eva.lampOn = !eva.lampOn;
                }
                return;
            case DiscreteAction.CutThrottle:
                m_Throttle = 0.0f;
                return;
            case DiscreteAction.NextPreset:
                if (m_CurrentPreset >= m_Presets.Count)
                {
                    break;
                }
                m_CurrentPreset++;
                return;
            case DiscreteAction.PreviousPreset:
                if (m_CurrentPreset <= 0)   
                {
                    break;
                }
                m_CurrentPreset--;
                return;
            case DiscreteAction.ZoomIn:
                FlightCamera.fetch.zoomScaleFactor += m_DiscreteActionStep;
                return; 
            case DiscreteAction.ZoomOut:
                FlightCamera.fetch.zoomScaleFactor -= m_DiscreteActionStep;
                return;
            case DiscreteAction.CameraX:
                return;
            case DiscreteAction.CameraY:
                return;
            case DiscreteAction.OpenDebugConsole:
                return;
            case DiscreteAction.OrbitMapToggle:
                if (CameraManager.Instance.currentCameraMode != CameraManager.CameraMode.Map)
                {
                    m_OriginalCameraMode = CameraManager.Instance.currentCameraMode;
                    CameraManager.Instance.SetCameraMode(CameraManager.CameraMode.Map);
                }
                else
                {
                    CameraManager.Instance.SetCameraMode(CameraManager.CameraMode.Map);
                }
                return;
            case DiscreteAction.ReverseCycleFocusOrbitMap:
                return;
            case DiscreteAction.ResetFocusOrbitMap:
                return;
            case DiscreteAction.TimeWarpPlus:
                TimeWarp.SetRate(TimeWarp.CurrentRateIndex + 1, false);
                return;
            case DiscreteAction.TimeWarpMinus:
                if (TimeWarp.CurrentRateIndex <= 0)
                    break;
                TimeWarp.SetRate(TimeWarp.CurrentRateIndex - 1, false);
                return;
            case DiscreteAction.PhysicalTimeWarpPlus:
                return;
            case DiscreteAction.PhysicalTimeWarpMinus:
                return;
            case DiscreteAction.NavballToggle:
                return;
            case DiscreteAction.CycleActiveShipsForward:
                return;
            case DiscreteAction.CycleActiveShipsBackward:
                return;
            case DiscreteAction.Screenshot:
                return;
            case DiscreteAction.QuickSave:
                return;
            case DiscreteAction.LoadQuickSave:
                return;
            case DiscreteAction.LoadSaveGameStateDialog:
                return;
            case DiscreteAction.DebugCheatMenu:
                return;
            case DiscreteAction.IVAViewToggle:
                if (CameraManager.Instance.currentCameraMode != CameraManager.CameraMode.IVA)
                {
                    m_OriginalCameraMode = CameraManager.Instance.currentCameraMode;
                    CameraManager.Instance.SetCameraMode(CameraManager.CameraMode.IVA);
                }
                else
                {
                    CameraManager.Instance.SetCameraMode(CameraManager.CameraMode.IVA);
                }
                return;
            case DiscreteAction.CameraViewToggle:
                CameraManager.Instance.NextCamera();
                return;
            case DiscreteAction.SASHold:
                FlightGlobals.ActiveVessel.ActionGroups.SetGroup(KSPActionGroup.SAS, true);
                return;
            case DiscreteAction.LockStage:
                return;
            case DiscreteAction.TogglePrecisionControls:
                return;
            case DiscreteAction.ResetTrim:
                return;
            case DiscreteAction.PartInfo:
                return;
            case DiscreteAction.FuelInfo:
                return;
            case DiscreteAction.ScrollStageUp:
                return;
            case DiscreteAction.ScrollStageDown:
                return;
            case DiscreteAction.UndoAction:
                return;
            case DiscreteAction.RedoAction:
                return;
            case DiscreteAction.DuplicatePart:
                return;
            case DiscreteAction.AngleSnap:
                return;
            case DiscreteAction.CycleSymmetry:
                return;
            case DiscreteAction.ViewUp:
                return;
            case DiscreteAction.ViewDown:
                return;
            case DiscreteAction.MoveShip:
                return;
            case DiscreteAction.ZoomShipIn:
                return;
            case DiscreteAction.ZoomShipOut:
                return;
            case DiscreteAction.RotatePartBackwards:
                return;
            case DiscreteAction.RotatePartForwards:
                return;
            case DiscreteAction.RotatePartCC:
                return;
            case DiscreteAction.RotateParkClockwise:
                return;
            case DiscreteAction.ResetPartRotation:
                return;
            }
        }

        private void EvaluateDiscreteActionRelease(DiscreteAction action, FlightCtrlState state)
        {
            switch (action)
            {
            case DiscreteAction.None:
                return;
            case DiscreteAction.Modifier:
                m_Modifier = false;
                return;
            case DiscreteAction.SASHold:
                FlightGlobals.ActiveVessel.ActionGroups.SetGroup(KSPActionGroup.SAS, false);
                return;
            }
        }

        private void EvaluateContinuousAction(ContinuousAction action, float value, FlightCtrlState state)
        {
            switch (action)
            {
                case ContinuousAction.None:
                    return;
                case ContinuousAction.Yaw:
                    state.yaw += value;
                    return;
                case ContinuousAction.Pitch:
                    state.pitch += value;
                    return;
                case ContinuousAction.Roll:
                    state.roll += value;
                    return;
                case ContinuousAction.X:
                    state.X += value;
                    return;
                case ContinuousAction.Y:
                    state.Y += value;
                    return;
                case ContinuousAction.Z:
                    state.Z += value;
                    return;
                case ContinuousAction.Throttle:
                    m_Throttle += value;
                    return;
            }
        }

        private void Update()
        {
            if (!m_CallbackSet && HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel != null)
            {
                FlightGlobals.ActiveVessel.OnFlyByWire += new FlightInputCallback(OnFlyByWire);
                m_CallbackSet = true;
            }
        }

        void OnGUI()
        {
            GUI.Window(0, new Rect(32, 32, 400, 600), DoMainWindow, "Advanced FlyByWire");
        }

    }

}
