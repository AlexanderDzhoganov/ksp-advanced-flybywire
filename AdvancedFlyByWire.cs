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
        private Configuration m_Configuration = null;
        private string m_ConfigurationPath = "GameData/ksp-advanced-flybywire/advanced_flybywire_config.xml";

        private float m_Throttle = 0.0f;
        private bool m_CallbackSet = false;

        private FlightInputCallback m_Callback;

        private bool m_UIHidden = false;

        private List<PresetEditor> m_PresetEditors = new List<PresetEditor>();

        private void LoadState(ConfigNode configNode)
        {
            m_Configuration = Configuration.Deserialize(m_ConfigurationPath);
            if (m_Configuration == null)
            {
                m_Configuration = new Configuration();
                Configuration.Serialize(m_ConfigurationPath, m_Configuration);
            }
        }

        public void SaveState(ConfigNode configNode)
        {
            Configuration.Serialize(m_ConfigurationPath, m_Configuration);
        }

        public void Awake()
        {
            print("KSPAdvancedFlyByWire: Initialized");

            LoadState(null);

            GameEvents.onShowUI.Add(OnShowUI);
            GameEvents.onHideUI.Add(OnHideUI);
            GameEvents.onGameStateSave.Add(new EventData<ConfigNode>.OnEvent(SaveState));
            GameEvents.onGameStateLoad.Add(new EventData<ConfigNode>.OnEvent(LoadState));
        }

        void ButtonPressedCallback(IController controller, int button, FlightCtrlState state)
        {
            var config = m_Configuration.GetConfigurationByIController(controller);

            Bitset mask = controller.GetButtonsMask();

            if (config.evaluatedDiscreteActionMasks.Contains(mask))
            {
                return;
            }

            List<DiscreteAction> actions = config.GetCurrentPreset().GetDiscreteBinding(mask);
            foreach(DiscreteAction action in actions)
            {
                EvaluateDiscreteAction(config, action, state);
                config.evaluatedDiscreteActionMasks.Add(mask);
            }
        }

        void ButtonReleasedCallback(IController controller, int button, FlightCtrlState state)
        {
            print("button released " + button.ToString());
            var config = m_Configuration.GetConfigurationByIController(controller);

            List<Bitset> masksToRemove = new List<Bitset>();

            foreach (Bitset evaluatedMask in config.evaluatedDiscreteActionMasks)
            {
                for (int i = 0; i < controller.GetButtonsCount(); i++)
                {
                    if (!controller.GetButtonState(i) && evaluatedMask.Get(i))
                   {
                       masksToRemove.Add(evaluatedMask);
                       break;
                   }
                }
            }

            foreach (Bitset maskRemove in masksToRemove)
            {
                config.evaluatedDiscreteActionMasks.Remove(maskRemove);
            }

            foreach (var presetEditor in m_PresetEditors)
            {
                Bitset bitset = controller.lastUpdateMask.Copy();
                bitset.Set(button);
                presetEditor.SetCurrentBitmask(bitset);
            }

            var actions = config.GetCurrentPreset().GetDiscreteBinding(controller.GetButtonsMask());
            foreach (DiscreteAction action in actions)
            {
                EvaluateDiscreteActionRelease(config, action, state);
            }
        }

        private void OnFlyByWire(FlightCtrlState state)
        {
            FlightGlobals.ActiveVessel.VesselSAS.ManualOverride(true);

            foreach (ControllerConfiguration config in m_Configuration.controllers)
            {
                config.iface.Update(state);
                state.mainThrottle = m_Throttle;

                for (int i = 0; i < config.iface.GetAxesCount(); i++)
                {
                    List<ContinuousAction> actions = config.GetCurrentPreset().GetContinuousBinding(i, config.iface.GetButtonsMask());
                    if (actions == null)
                    {
                        continue;
                    }

                    foreach (var action in actions)
                    {
                        float input = config.iface.GetAnalogInputState(i);
                        if (input != 0.0f)
                        {
                            EvaluateContinuousAction(config, action, config.iface.GetAnalogInputState(i), state);
                        }
                    }
                }
            }

           FlightGlobals.ActiveVessel.VesselSAS.ManualOverride(false);
        }

        private void EvaluateDiscreteAction(ControllerConfiguration controller, DiscreteAction action, FlightCtrlState state)
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
                state.yaw += controller.discreteActionStep;
                state.yaw = Utility.Clamp(state.yaw, -1.0f, 1.0f);
                return;
            case DiscreteAction.YawMinus:
                state.yaw -= controller.discreteActionStep;
                state.yaw = Utility.Clamp(state.yaw, -1.0f, 1.0f);
                return;
            case DiscreteAction.PitchPlus:
                state.pitch += controller.discreteActionStep;
                state.pitch = Utility.Clamp(state.pitch, -1.0f, 1.0f);
                return;
            case DiscreteAction.PitchMinus:
                state.pitch -= controller.discreteActionStep;
                state.pitch = Utility.Clamp(state.pitch, -1.0f, 1.0f);
                return;
            case DiscreteAction.RollPlus:
                state.roll += controller.discreteActionStep;
                state.roll = Utility.Clamp(state.roll, -1.0f, 1.0f);
                return;
            case DiscreteAction.RollMinus:
                state.roll -= controller.discreteActionStep;
                state.roll = Utility.Clamp(state.roll, -1.0f, 1.0f);
                return;
            case DiscreteAction.XPlus:
                state.X += controller.discreteActionStep;
                state.X = Utility.Clamp(state.X, -1.0f, 1.0f);
                return;
            case DiscreteAction.XMinus:
                state.X -= controller.discreteActionStep;
                state.X = Utility.Clamp(state.X, -1.0f, 1.0f);
                return;
            case DiscreteAction.YPlus:
                state.Y += controller.discreteActionStep;
                state.Y = Utility.Clamp(state.Y, -1.0f, 1.0f);
                return;
            case DiscreteAction.YMinus:
                state.Y -= controller.discreteActionStep;
                state.Y = Utility.Clamp(state.Y, -1.0f, 1.0f);
                return;
            case DiscreteAction.ZPlus:
                state.Z += controller.discreteActionStep;
                state.Z = Utility.Clamp(state.Z, -1.0f, 1.0f);
                return;
            case DiscreteAction.ZMinus:
                state.Z -= controller.discreteActionStep;
                state.Z = Utility.Clamp(state.Z, -1.0f, 1.0f);
                return;
            case DiscreteAction.ThrottlePlus:
                m_Throttle += controller.discreteActionStep;
                m_Throttle = Utility.Clamp(m_Throttle, -1.0f, 1.0f);
                return;
            case DiscreteAction.ThrottleMinus:
                m_Throttle -= controller.discreteActionStep;
                m_Throttle = Utility.Clamp(m_Throttle, -1.0f, 1.0f);
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
            case DiscreteAction.FullThrottle:
                m_Throttle = 1.0f;
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
            case DiscreteAction.CameraZoomPlus:
                FlightCamera.fetch.SetDistance(FlightCamera.fetch.Distance + controller.discreteActionStep);
                return; 
            case DiscreteAction.CameraZoomMinus:
                FlightCamera.fetch.SetDistance(FlightCamera.fetch.Distance - controller.discreteActionStep);
                return;
            case DiscreteAction.CameraXPlus:
                FlightCamera.CamHdg += controller.discreteActionStep;
                return;
            case DiscreteAction.CameraXMinus:
                FlightCamera.CamHdg -= controller.discreteActionStep;
                return;
            case DiscreteAction.CameraYPlus:
                FlightCamera.CamPitch += controller.discreteActionStep;
                return;
            case DiscreteAction.CameraYMinus:
                FlightCamera.CamPitch -= controller.discreteActionStep;
                return;
            case DiscreteAction.OrbitMapToggle:
                if(!MapView.MapIsEnabled)
                {
                    MapView.EnterMapView();
                }
                else
                {
                    MapView.ExitMapView();
                }
                return;
            case DiscreteAction.TimeWarpPlus:
                TimeWarp.SetRate(TimeWarp.CurrentRateIndex + 1, false);
                return;
            case DiscreteAction.TimeWarpMinus:
                if (TimeWarp.CurrentRateIndex <= 0)
                {
                    break;
                }
                TimeWarp.SetRate(TimeWarp.CurrentRateIndex - 1, false);
                return;
            case DiscreteAction.NavballToggle:
                if (MapView.MapIsEnabled)
                {
                    MapView.fetch.maneuverModeToggle.OnPress.Invoke();
                }
                return;
            case DiscreteAction.Screenshot:
                return;
            case DiscreteAction.QuickSave:
                GamePersistence.SaveGame("persistent", HighLogic.SaveFolder, SaveMode.OVERWRITE);
                return;
            case DiscreteAction.IVAViewToggle:
                return;
            case DiscreteAction.CameraViewToggle:
                FlightCamera.fetch.SetNextMode();
                return;
            case DiscreteAction.SASHold:
                FlightGlobals.ActiveVessel.ActionGroups.SetGroup(KSPActionGroup.SAS, true);
                return;
            case DiscreteAction.LockStage:
                FlightInputHandler.fetch.stageLock = !FlightInputHandler.fetch.stageLock;
                return;
            case DiscreteAction.TogglePrecisionControls:
                FlightInputHandler.fetch.precisionMode = !FlightInputHandler.fetch.precisionMode;
                return;
            case DiscreteAction.ResetTrim:
                state.ResetTrim();
                return;
            }
        }

        private void EvaluateDiscreteActionRelease(ControllerConfiguration controller, DiscreteAction action, FlightCtrlState state)
        {
            switch (action)
            {
            case DiscreteAction.None:
                return;
            case DiscreteAction.SASHold:
                FlightGlobals.ActiveVessel.ActionGroups.SetGroup(KSPActionGroup.SAS, false);
                return;
            }
        }

        private void EvaluateContinuousAction(ControllerConfiguration controller, ContinuousAction action, float value, FlightCtrlState state)
        {
            switch (action)
            {
                case ContinuousAction.None:
                    return;
                case ContinuousAction.Yaw:
                    state.yaw = value;
                    state.yaw = Utility.Clamp(state.yaw, -1.0f, 1.0f);
                    return;
                case ContinuousAction.YawTrim:
                    state.yawTrim = value;
                    state.yawTrim = Utility.Clamp(state.yawTrim, -1.0f, 1.0f);
                    return;
                case ContinuousAction.Pitch:
                    state.pitch = value;
                    state.pitch = Utility.Clamp(state.pitch, -1.0f, 1.0f);
                    return;
                case ContinuousAction.PitchTrim:
                    state.pitchTrim = value;
                    state.pitchTrim = Utility.Clamp(state.pitchTrim, -1.0f, 1.0f);
                    return;
                case ContinuousAction.Roll:
                    state.roll = value;
                    state.roll = Utility.Clamp(state.roll, -1.0f, 1.0f);
                    return;
                case ContinuousAction.RollTrim:
                    state.rollTrim = value;
                    state.rollTrim = Utility.Clamp(state.rollTrim, -1.0f, 1.0f);
                    return;
                case ContinuousAction.X:
                    state.X = value;
                    state.X = Utility.Clamp(state.X, -1.0f, 1.0f);
                    return;
                case ContinuousAction.Y:
                    state.Y = value;
                    state.Y = Utility.Clamp(state.Y, -1.0f, 1.0f);
                    return;
                case ContinuousAction.Z:
                    state.Z = value;
                    state.Z = Utility.Clamp(state.Z, -1.0f, 1.0f);
                    return;
                case ContinuousAction.Throttle:
                    m_Throttle += value;
                    m_Throttle = Utility.Clamp(m_Throttle, -1.0f, 1.0f);
                    return;
                case ContinuousAction.ThrottleIncrement:
                    m_Throttle += value * controller.incrementalThrottleSensitivity;
                    m_Throttle = Utility.Clamp(m_Throttle, -1.0f, 1.0f);
                    return;
                case ContinuousAction.ThrottleDecrement:
                    m_Throttle -= value * controller.incrementalThrottleSensitivity;
                    m_Throttle = Utility.Clamp(m_Throttle, -1.0f, 1.0f);
                    return;
                case ContinuousAction.CameraX:
                    FlightCamera.CamHdg += value;
                    break;
                case ContinuousAction.CameraY:
                    FlightCamera.CamPitch += value;
                    break;
                case ContinuousAction.CameraZoom:
                    FlightCamera.fetch.SetDistance(FlightCamera.fetch.Distance + value);
                    break;
            }
        }

        private void FixedUpdate()
        {
            if(HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel != null)
            {
                if(!m_CallbackSet)
                {
                    m_Callback = new FlightInputCallback(OnFlyByWire);
                }
                else
                {
                    // we have to remove and re-add the callback every frame to make
                    // sure that it's always the last callback
                    // otherwise we break SAS and probably some mods
                    FlightGlobals.ActiveVessel.OnFlyByWire -= m_Callback;
                }

                FlightGlobals.ActiveVessel.OnFlyByWire += m_Callback;
                m_CallbackSet = true;
            }
        }

        private void OnShowUI()
        {
            m_UIHidden = false;
        }

        private void OnHideUI()
        {
            m_UIHidden = true;
        }

        void DoMainWindow(int index)
        {
            var controllers = IController.EnumerateAllControllers();
            foreach (var controller in controllers)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(controller.Key.ToString() + "-" + controller.Value.ToString());

                bool isEnabled = false;
                ControllerConfiguration config = m_Configuration.GetConfigurationByControllerType(controller.Key, controller.Value.Key);

                if (!isEnabled && GUILayout.Button("enable"))
                {
                    m_Configuration.ActivateController
                    (
                        controller.Key,
                        controller.Value.Key,
                        new IController.ButtonPressedCallback(ButtonPressedCallback),
                        new IController.ButtonReleasedCallback(ButtonReleasedCallback)
                    );
                }
                else if (isEnabled)
                {
                    if (GUILayout.Button("disable"))
                    {
                        m_Configuration.DeactivateController(controller.Key, controller.Value.Key);
                    }
                }

                if (isEnabled)
                {
                    if (GUILayout.Button("edit"))
                    {
                        m_PresetEditors.Add(new PresetEditor(config));
                    }
                }

                GUILayout.EndHorizontal();
            }

            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }

        private Rect windowRect = new Rect(32, 32, 400, 600);

        void OnGUI()
        {
            if (m_UIHidden)
            {
                return;
            }


            if (windowRect.Contains(Input.mousePosition))
            {
                InputLockManager.SetControlLock("AdvancedFlyByWire");
            }
            else
            {
                InputLockManager.RemoveControlLock("AdvancedFlyByWire");
            }

            GUI.Window(0, windowRect, DoMainWindow, "Advanced FlyByWire");

            foreach (var presetEditor in m_PresetEditors)
            {
                presetEditor.OnGUI();
            }
        }

    }

}
