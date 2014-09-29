using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace KSPAdvancedFlyByWire
{

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class AdvancedFlyByWire : MonoBehaviour
    {
        private Configuration m_Configuration = null;
        private string m_ConfigurationPath = "GameData/ksp-advanced-flybywire/advanced_flybywire_config.xml";

        private bool m_CallbackSet = false;

        private FlightInputCallback m_Callback = null;

        private bool m_UIActive = true;
        private bool m_UIHidden = false;

        private List<PresetEditorWindow> m_PresetEditors = new List<PresetEditorWindow>();
        private List<ControllerConfigurationWindow> m_ControllerTests = new List<ControllerConfigurationWindow>();

        private Toolbar.IButton m_ToolbarButton = null;

        private Rect windowRect = new Rect(0, 64, 432, 576);

        private static AdvancedFlyByWire m_Instance = null;

        private FlightProperty m_Yaw = new FlightProperty(-1.0f, 1.0f);
        private int m_YawIncrement = 0;
        private FlightProperty m_Pitch = new FlightProperty(-1.0f, 1.0f);
        private int m_PitchIncrement = 0;
        private FlightProperty m_Roll = new FlightProperty(-1.0f, 1.0f);
        private int m_RollIncrement = 0;
        private FlightProperty m_X = new FlightProperty(-1.0f, 1.0f);
        private int m_XIncrement = 0;
        private FlightProperty m_Y = new FlightProperty(-1.0f, 1.0f);
        private int m_YIncrement = 0;
        private FlightProperty m_Z = new FlightProperty(-1.0f, 1.0f);
        private int m_ZIncrement = 0;
        private FlightProperty m_Throttle = new FlightProperty(0.0f, 1.0f);
        private int m_ThrottleIncrement = 0;
        private FlightProperty m_CameraPitch = new FlightProperty(-1.0f, 1.0f);
        private int m_CameraPitchIncrement = 0;
        private FlightProperty m_CameraHeading = new FlightProperty(-1.0f, 1.0f);
        private int m_CameraHeadingIncrement = 0;
        private FlightProperty m_CameraZoom = new FlightProperty(-1.0f, 1.0f);
        private int m_CameraZoomIncrement = 0;

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

        public static AdvancedFlyByWire Instance
        {
            get
            {
                return m_Instance;
            }
        }

        public void Awake()
        {
            Utility.CheckLibrarySupport();

            m_Instance = this;
            RegisterCallbacks(); 
            LoadState(null);
            InitializeToolbarButton();

            m_UIHidden = false;
            m_UIActive = false;
            
            print("Advanced Fly-By-Wire: Initialized");
        }

        private void RegisterCallbacks()
        {
            GameEvents.onShowUI.Add(OnShowUI);
            GameEvents.onHideUI.Add(OnHideUI);
            GameEvents.onGameStateSave.Add(new EventData<ConfigNode>.OnEvent(SaveState));
            GameEvents.onGameStateLoad.Add(new EventData<ConfigNode>.OnEvent(LoadState));
            GameEvents.onGUIRecoveryDialogSpawn.Add(new EventData<MissionRecoveryDialog>.OnEvent(OnGUIRecoveryDialogSpawn));
            GameEvents.onGamePause.Add(new EventVoid.OnEvent(OnGamePause));
            GameEvents.onGameUnpause.Add(new EventVoid.OnEvent(OnGameUnpause));
            GameEvents.onFlightReady.Add(new EventVoid.OnEvent(OnFlightReady));
        }

        private void UnregisterCallbacks()
        {
            GameEvents.onShowUI.Remove(OnShowUI);
            GameEvents.onHideUI.Remove(OnHideUI);
            GameEvents.onGameStateSave.Remove(SaveState);
            GameEvents.onGameStateLoad.Remove(LoadState);
            GameEvents.onGUIRecoveryDialogSpawn.Remove(OnGUIRecoveryDialogSpawn);
            GameEvents.onGamePause.Remove(OnGamePause);
            GameEvents.onGameUnpause.Remove(OnGameUnpause);
            GameEvents.onFlightReady.Remove(OnFlightReady);
        }

        private void InitializeToolbarButton()
        {
            m_ToolbarButton = Toolbar.ToolbarManager.Instance.add("advancedflybywire", "mainButton");
            m_ToolbarButton.TexturePath = "000_Toolbar/img_buttonAdvancedFlyByWire";
            m_ToolbarButton.ToolTip = "Advanced Fly-By-Wire";
            m_ToolbarButton.Visibility = new Toolbar.GameScenesVisibility(GameScenes.FLIGHT);
            m_ToolbarButton.OnClick += new Toolbar.ClickHandler(OnToolbarButtonClick);
        }

        void OnToolbarButtonClick(Toolbar.ClickEvent ev)
        {
            this.gameObject.SetActive(true);
            m_UIActive = true;
            m_UIHidden = false;
        }

        void OnFlightReady()
        {
        }

        void OnGUIRecoveryDialogSpawn(MissionRecoveryDialog dialog)
        {
            m_UIHidden = true;
        }

        void OnGamePause()
        {
            m_UIHidden = true;
        }

        void OnGameUnpause()
        {
            m_UIHidden = false;
        }

        public void OnDestroy()
        {
            m_Instance = null;

            if (m_ToolbarButton != null)
            {
                m_ToolbarButton.Destroy();
            }

            SaveState(null);
            UnregisterCallbacks();

            print("Advanced Fly-By-Wire: Deinitialized");
        }

        public void ButtonPressedCallback(IController controller, int button, FlightCtrlState state)
        {
            var config = m_Configuration.GetConfigurationByIController(controller);

            Bitset mask = controller.GetButtonsMask();

            if (config.evaluatedDiscreteActionMasks.Contains(mask))
            {
                return;
            }

            List<DiscreteAction> actions = config.GetCurrentPreset().GetDiscreteBinding(mask);

            if(actions != null)
            {
                foreach (DiscreteAction action in actions)
                {
                    EvaluateDiscreteAction(config, action, state);
                    config.evaluatedDiscreteActionMasks.Add(mask);
                }
            }
        }

        public void ButtonReleasedCallback(IController controller, int button, FlightCtrlState state)
        {
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

            if(controller.lastUpdateMask != null)
            {
                foreach (var presetEditor in m_PresetEditors)
                {
                    Bitset bitset = controller.lastUpdateMask.Copy();
                    bitset.Set(button);
                    presetEditor.SetCurrentBitmask(bitset);
                }
            }

            var mask = controller.GetButtonsMask();
            mask.Set(button);
            var actions = config.GetCurrentPreset().GetDiscreteBinding(mask);

            if(actions != null)
            {
                foreach (DiscreteAction action in actions)
                {
                    EvaluateDiscreteActionRelease(config, action, state);
                }
            }
        }

        private void OnFlyByWire(FlightCtrlState state)
        {
            FlightGlobals.ActiveVessel.VesselSAS.ManualOverride(true);

            foreach (ControllerConfiguration config in m_Configuration.controllers)
            {
                config.iface.Update(state);

                if(m_YawIncrement == 0)
                {
                    m_Yaw.SetValue(0.0f);
                }
                else
                {
                    m_Yaw.SetAcceleration(m_YawIncrement * config.discreteActionStep);
                }

                if (m_PitchIncrement == 0)
                {
                    m_Pitch.SetValue(0.0f);
                }
                else
                {
                    m_Pitch.SetAcceleration(m_PitchIncrement * config.discreteActionStep);
                }

                if(m_RollIncrement == 0)
                {
                    m_Roll.SetValue(0.0f);
                }
                else
                {
                    m_Roll.SetAcceleration(m_RollIncrement * config.discreteActionStep);
                }

                if (m_XIncrement == 0)
                {
                    m_X.SetValue(0.0f);
                }
                else
                {
                    m_X.SetAcceleration(m_XIncrement * config.discreteActionStep);
                }

                if (m_YIncrement == 0)
                {
                    m_Y.SetValue(0.0f);
                }
                else
                {
                    m_Y.SetAcceleration(m_YIncrement * config.discreteActionStep);
                }

                if (m_ZIncrement == 0)
                {
                    m_Z.SetValue(0.0f);
                }
                else
                {
                    m_Z.SetAcceleration(m_ZIncrement * config.discreteActionStep);
                }

                if (m_ThrottleIncrement == 0)
                {
                    m_Throttle.SetVelocity(0.0f);
                    m_Throttle.SetAcceleration(0.0f);
                }
                else
                {
                    m_Throttle.SetAcceleration(m_ThrottleIncrement * config.discreteActionStep);
                }

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
                        if (input != 0.0f)
                        {
                            EvaluateContinuousAction(config, action, config.iface.GetAxisState(i), state);
                        }
                    }
                }

                state.yaw = Utility.Clamp(state.yaw + m_Yaw.Update(), -1.0f, 1.0f);
                state.pitch = Utility.Clamp(state.pitch + m_Pitch.Update(), -1.0f, 1.0f);
                state.roll = Utility.Clamp(state.roll + m_Roll.Update(), -1.0f, 1.0f);

                state.X = Utility.Clamp(state.X + m_X.Update(), -1.0f, 1.0f);
                state.Y = Utility.Clamp(state.Y + m_Y.Update(), -1.0f, 1.0f);
                state.Z = Utility.Clamp(state.Z + m_Z.Update(), -1.0f, 1.0f);

                state.mainThrottle = Utility.Clamp(state.mainThrottle + m_Throttle.Update(), 0.0f, 1.0f);

                if (m_CameraHeadingIncrement == 0)
                {
                    m_CameraHeading.SetVelocity(0.0f);
                    m_CameraHeading.SetAcceleration(0.0f);
                }
                else
                {
                    m_CameraHeading.SetAcceleration(m_CameraHeadingIncrement * config.discreteActionStep);
                }

                if (m_CameraPitchIncrement == 0)
                {
                    m_CameraPitch.SetVelocity(0.0f);
                    m_CameraPitch.SetAcceleration(0.0f);
                }
                else
                {
                    m_CameraPitch.SetAcceleration(m_CameraPitchIncrement * config.discreteActionStep);
                }

                if (m_CameraZoomIncrement == 0)
                {
                    m_CameraZoom.SetVelocity(0.0f);
                    m_CameraZoom.SetAcceleration(0.0f);
                }
                else
                {
                    m_CameraZoom.SetAcceleration(m_CameraZoomIncrement * config.discreteActionStep);
                }

                FlightCamera.CamHdg += m_CameraHeading.Update() * config.cameraSensitivity;
                FlightCamera.CamPitch += m_CameraPitch.Update() * config.cameraSensitivity;
                FlightCamera.fetch.SetDistance(FlightCamera.fetch.Distance + m_CameraZoom.Update());
            }

           FlightGlobals.ActiveVessel.VesselSAS.ManualOverride(false);
        }

        private float ApplyChangeAndClamp(float x, float change, float clampMin = -1.0f, float clampMax = 1.0f)
        {
            x += change;
            x = Utility.Clamp(x, clampMin, clampMax);
            return x;
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
                m_YawIncrement = 1;
                return;
            case DiscreteAction.YawMinus:
                m_YawIncrement = -1;
                return;
            case DiscreteAction.PitchPlus:
                m_PitchIncrement = 1;
                return;
            case DiscreteAction.PitchMinus:
                m_PitchIncrement = -1;
                return;
            case DiscreteAction.RollPlus:
                m_RollIncrement = 1;
                return;
            case DiscreteAction.RollMinus:
                m_RollIncrement = -1;
                return;
            case DiscreteAction.XPlus:
                m_XIncrement = 1;
                return;
            case DiscreteAction.XMinus:
                m_XIncrement = -1;
                return;
            case DiscreteAction.YPlus:
                m_YIncrement = 1;
                return;
            case DiscreteAction.YMinus:
                m_YIncrement = -1;
                return;
            case DiscreteAction.ZPlus:
                m_ZIncrement = 1;
                return;
            case DiscreteAction.ZMinus:
                m_ZIncrement = -1;
                return;
            case DiscreteAction.ThrottlePlus:
                m_ThrottleIncrement = 1;
                return;
            case DiscreteAction.ThrottleMinus:
                m_ThrottleIncrement = -1;
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
                m_Throttle.SetZero();
                return;
            case DiscreteAction.FullThrottle:
                m_Throttle.SetMax();
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
                if(controller.currentPreset >= controller.presets.Count)
                {
                    controller.currentPreset = 0;
                }
                ScreenMessages.PostScreenMessage("PRESET: " + controller.GetCurrentPreset().name.ToUpper(), 1.0f, ScreenMessageStyle.LOWER_CENTER);
                return;
            case DiscreteAction.CameraZoomPlus:
                m_CameraZoomIncrement = 1;
                return; 
            case DiscreteAction.CameraZoomMinus:
                m_CameraZoomIncrement = -1;
                return;
            case DiscreteAction.CameraXPlus:
                m_CameraHeadingIncrement = 1;
                return;
            case DiscreteAction.CameraXMinus:
                m_CameraHeadingIncrement = -1;
                return;
            case DiscreteAction.CameraYPlus:
                m_CameraPitchIncrement = 1;
                return;
            case DiscreteAction.CameraYMinus:
                m_CameraPitchIncrement = -1;
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
                if (MapView.MapIsEnabled)
                {
                    MapView.fetch.maneuverModeToggle.OnPress.Invoke();
                }
                return;
            case DiscreteAction.IVAViewToggle:
                CameraManager.Instance.SetCameraIVA();
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
            case DiscreteAction.YawPlus:
                m_YawIncrement = 0;
                return;
            case DiscreteAction.YawMinus:
                m_YawIncrement = 0;
                return;
            case DiscreteAction.PitchPlus:
                m_PitchIncrement = 0;
                return;
            case DiscreteAction.PitchMinus:
                m_PitchIncrement = 0;
                return;
            case DiscreteAction.RollPlus:
                m_RollIncrement = 0;
                return;
            case DiscreteAction.RollMinus:
                m_RollIncrement = 0;
                return;
            case DiscreteAction.XPlus:
                m_XIncrement = 0;
                return;
            case DiscreteAction.XMinus:
                m_XIncrement = 0;
                return;
            case DiscreteAction.YPlus:
                m_YIncrement = 0;
                return;
            case DiscreteAction.YMinus:
                m_YIncrement = 0;
                return;
            case DiscreteAction.ZPlus:
                m_ZIncrement = 0;
                return;
            case DiscreteAction.ZMinus:
                m_ZIncrement = 0;
                return;
            case DiscreteAction.ThrottlePlus:
                m_ThrottleIncrement = 0;
                return;
            case DiscreteAction.ThrottleMinus:
                m_ThrottleIncrement = 0;
                return;
            case DiscreteAction.CameraZoomPlus:
                m_CameraZoomIncrement = 0;
                return;
            case DiscreteAction.CameraZoomMinus:
                m_CameraZoomIncrement = 0;
                return;
            case DiscreteAction.CameraXPlus:
                m_CameraHeadingIncrement = 0;
                return;
            case DiscreteAction.CameraXMinus:
                m_CameraHeadingIncrement = 0;
                return;
            case DiscreteAction.CameraYPlus:
                m_CameraPitchIncrement = 0;
                return;
            case DiscreteAction.CameraYMinus:
                m_CameraPitchIncrement = 0;
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
                    m_Yaw.SetValue(value);
                    return;
                case ContinuousAction.YawTrim:
                    state.yawTrim = value;
                    state.yawTrim = Utility.Clamp(state.yawTrim, -1.0f, 1.0f);
                    return;
                case ContinuousAction.Pitch:
                    m_Pitch.SetValue(value);
                    return;
                case ContinuousAction.PitchTrim:
                    state.pitchTrim = value;
                    state.pitchTrim = Utility.Clamp(state.pitchTrim, -1.0f, 1.0f);
                    return;
                case ContinuousAction.Roll:
                    m_Roll.SetValue(value);
                    return;
                case ContinuousAction.RollTrim:
                    state.rollTrim = value;
                    state.rollTrim = Utility.Clamp(state.rollTrim, -1.0f, 1.0f);
                    return;
                case ContinuousAction.X:
                    m_X.SetValue(value);
                    return;
                case ContinuousAction.Y:
                    m_Y.SetValue(value);
                    return;
                case ContinuousAction.Z:
                    m_Z.SetValue(value);
                    return;
                case ContinuousAction.Throttle:
                    m_Throttle.SetValue(value);
                    return;
                case ContinuousAction.ThrottleIncrement:
                    m_Throttle.SetValue(m_Throttle.GetValue() + value * controller.incrementalActionSensitivity);
                    return;
                case ContinuousAction.ThrottleDecrement:
                    m_Throttle.SetValue(m_Throttle.GetValue() - value * controller.incrementalActionSensitivity);
                    return;
                case ContinuousAction.CameraX:
                    m_CameraHeading.SetValue(m_CameraHeading.GetValue() + value * controller.incrementalActionSensitivity);
                    return;
                case ContinuousAction.CameraY:
                    m_CameraPitch.SetValue(m_CameraPitch.GetValue() + value * controller.incrementalActionSensitivity);
                    return;
                case ContinuousAction.CameraZoom:
                    m_CameraZoom.SetValue(m_CameraZoom.GetValue() + value * controller.incrementalActionSensitivity);
                    return;
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

            if(Input.GetKey("left shift") && Input.GetKey("k"))
            {
                m_UIActive = true;
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

        private Vector2 m_ScrollPosition = new Vector2(0, 0);

        void DoMainWindow(int index)
        {
            GUI.DragWindow(new Rect(0, 0, 10000, 20));

            if (GUILayout.Button("Close window"))
            {
                m_UIActive = false;
                InputLockManager.RemoveControlLock("AdvancedFlyByWireMainWindow");
            }

            m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition);

            GUILayout.Label("Controllers");

            var controllers = IController.EnumerateAllControllers();
            foreach (var controller in controllers)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(controller.Key.ToString() + " #" + controller.Value.Key.ToString() + " - " + controller.Value.Value);

                GUILayout.FlexibleSpace();

                ControllerConfiguration config = m_Configuration.GetConfigurationByControllerType(controller.Key, controller.Value.Key);
                bool isEnabled = config != null && config.iface != null;

                if (!isEnabled && GUILayout.Button("Enable"))
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
                    if (GUILayout.Button("Disable"))
                    {
                        m_Configuration.DeactivateController(controller.Key, controller.Value.Key);
                    }
                }

                if (isEnabled)
                {
                    if (config.presetEditorOpen)
                    {
                        GUI.enabled = false;
                    }

                    if (GUILayout.Button("Presets"))
                    {
                        m_PresetEditors.Add(new PresetEditorWindow(config, m_PresetEditors.Count));
                        config.presetEditorOpen = true;
                    }

                    GUI.enabled = true;

                    if (config.controllerConfigurationOpen)
                        GUI.enabled = false;

                    if (GUILayout.Button("Configuration"))
                    {
                        m_ControllerTests.Add(new ControllerConfigurationWindow(config, m_ControllerTests.Count));
                        config.controllerConfigurationOpen = true;
                    }

                    GUI.enabled = true;

                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();

                    GUILayout.Label("Preset: " + config.GetCurrentPreset().name);

                    if (config.currentPreset > 0)
                    {
                        if (GUILayout.Button("<"))
                        {
                            config.currentPreset--;
                        }
                    }
                    else
                    {
                        GUI.enabled = false;
                        GUILayout.Button("<");
                        GUI.enabled = true;
                    }

                    if (config.currentPreset < config.presets.Count - 1)
                    {
                        if (GUILayout.Button(">"))
                        {
                            config.currentPreset++;
                        }
                    }
                    else
                    {
                        GUI.enabled = false;
                        GUILayout.Button(">");
                        GUI.enabled = true;
                    }
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }

        void OnGUI()
        {
            if (m_UIHidden || !m_UIActive)
            {
                return;
            }

            if (windowRect.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y)))
            {
                InputLockManager.SetControlLock(ControlTypes.All, "AdvancedFlyByWireMainWindow");
            }
            else
            {
                InputLockManager.RemoveControlLock("AdvancedFlyByWireMainWindow");
            }

            windowRect = GUI.Window(0, windowRect, DoMainWindow, "Advanced Fly-By-Wire");
            windowRect = Utility.ClampRect(windowRect, new Rect(0, 0, Screen.width, Screen.height));

            for (int i = 0; i < m_PresetEditors.Count; i++)
            {
                if(m_PresetEditors[i].shouldBeDestroyed)
                {
                    InputLockManager.RemoveControlLock(m_PresetEditors[i].inputLockHash);
                    m_PresetEditors.RemoveAt(i);
                    break;
                }
            }

            foreach (var presetEditor in m_PresetEditors)
            {
                presetEditor.OnGUI();
            }

            for (int i = 0; i < m_ControllerTests.Count; i++)
            {
                if (m_ControllerTests[i].shouldBeDestroyed)
                {
                    InputLockManager.RemoveControlLock(m_ControllerTests[i].inputLockHash);
                    m_ControllerTests.RemoveAt(i);
                    break;
                }
            }

            foreach (var controllerTest in m_ControllerTests)
            {
                controllerTest.OnGUI();
            }
        }

    }

}
