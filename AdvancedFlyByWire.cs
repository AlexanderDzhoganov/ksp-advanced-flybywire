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

        private float m_Yaw = 0.0f;
        private int m_YawIncrement = 0;

        private float m_Pitch = 0.0f;
        private int m_PitchIncrement = 0;

        private float m_Roll = 0.0f;
        private int m_RollIncrement = 0;

        private float m_X = 0.0f;
        private int m_XIncrement = 0;

        private float m_Y = 0.0f;
        private int m_YIncrement = 0;

        private float m_Z = 0.0f;
        private int m_ZIncrement = 0;

        private float m_Throttle = 0.0f;
        private int m_ThrottleIncrement = 0;

        private bool m_CallbackSet = false;

        private FlightInputCallback m_Callback;

        private bool m_UIActive = true;
        private bool m_UIHidden = false;

        private List<PresetEditorWindow> m_PresetEditors = new List<PresetEditorWindow>();
        private List<ControllerConfigurationWindow> m_ControllerTests = new List<ControllerConfigurationWindow>();

        private Toolbar.IButton m_ToolbarButton = null;

        private Rect windowRect = new Rect(0, 64, 432, 576);

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

        private static AdvancedFlyByWire m_Instance = null;

        public static AdvancedFlyByWire Instance
        {
            get
            {
                return m_Instance;
            }
        }

        public void Awake()
        {
            m_Instance = this;

            GameEvents.onShowUI.Add(OnShowUI);
            GameEvents.onHideUI.Add(OnHideUI);
            GameEvents.onGameStateSave.Add(new EventData<ConfigNode>.OnEvent(SaveState));
            GameEvents.onGameStateLoad.Add(new EventData<ConfigNode>.OnEvent(LoadState));
            GameEvents.onGUIRecoveryDialogSpawn.Add(new EventData<MissionRecoveryDialog>.OnEvent(OnGUIRecoveryDialogSpawn));
            GameEvents.onGamePause.Add(new EventVoid.OnEvent(OnGamePause));
            GameEvents.onGameUnpause.Add(new EventVoid.OnEvent(OnGameUnpause));

            LoadState(null);

            m_ToolbarButton = Toolbar.ToolbarManager.Instance.add("advancedflybywire", "mainButton");
            m_ToolbarButton.TexturePath = "000_Toolbar/img_buttonAdvancedFlyByWire";
            m_ToolbarButton.ToolTip = "Advanced Fly-By-Wire";
            m_ToolbarButton.Visibility = new Toolbar.GameScenesVisibility(GameScenes.FLIGHT);
            m_ToolbarButton.OnClick += new Toolbar.ClickHandler(OnToolbarButtonClick);

            m_UIHidden = false;
            m_UIActive = false;

            print("KSPAdvancedFlyByWire: Initialized");
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

            GameEvents.onShowUI.Remove(OnShowUI);
            GameEvents.onHideUI.Remove(OnHideUI);
            GameEvents.onGameStateSave.Remove(new EventData<ConfigNode>.OnEvent(SaveState));
            GameEvents.onGameStateLoad.Remove(new EventData<ConfigNode>.OnEvent(LoadState));
            GameEvents.onGUIRecoveryDialogSpawn.Remove(new EventData<MissionRecoveryDialog>.OnEvent(OnGUIRecoveryDialogSpawn));
            GameEvents.onGamePause.Remove(new EventVoid.OnEvent(OnGamePause));
            GameEvents.onGameUnpause.Remove(new EventVoid.OnEvent(OnGameUnpause));

            print("KSPAdvancedFlyByWire: Deinitialized");
        }

        void OnToolbarButtonClick(Toolbar.ClickEvent ev)
        {
            this.gameObject.SetActive(true);
            m_UIActive = true;
            m_UIHidden = false;
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
            
            var actions = config.GetCurrentPreset().GetDiscreteBinding(controller.GetButtonsMask());

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

                m_Yaw += m_YawIncrement * config.discreteActionStep * Time.deltaTime;
                m_Yaw = Utility.Clamp(m_Yaw, -1.0f, 1.0f);
                state.yaw = Utility.Clamp(state.yaw + m_Yaw, -1.0f, 1.0f);

                m_Pitch += m_PitchIncrement * config.discreteActionStep * Time.deltaTime;
                m_Pitch = Utility.Clamp(m_Pitch, -1.0f, 1.0f);
                state.pitch = Utility.Clamp(state.pitch + m_Pitch, -1.0f, 1.0f);

                m_Roll += m_RollIncrement * config.discreteActionStep * Time.deltaTime;
                m_Roll = Utility.Clamp(m_Roll, -1.0f, 1.0f);
                state.roll = Utility.Clamp(state.roll + m_Roll, -1.0f, 1.0f);

                m_X += m_XIncrement * config.discreteActionStep * Time.deltaTime;
                m_X = Utility.Clamp(m_X, -1.0f, 1.0f);
                state.X = Utility.Clamp(state.X + m_X, -1.0f, 1.0f);

                m_Y += m_YIncrement * config.discreteActionStep * Time.deltaTime;
                m_Y = Utility.Clamp(m_Y, -1.0f, 1.0f);
                state.Y = Utility.Clamp(state.Y + m_Y, -1.0f, 1.0f);

                m_Z += m_ZIncrement * config.discreteActionStep * Time.deltaTime;
                m_Z = Utility.Clamp(m_Z, -1.0f, 1.0f);
                state.Z = Utility.Clamp(state.Z + m_Z, -1.0f, 1.0f);

                m_Throttle += m_ThrottleIncrement * config.discreteActionStep * Time.deltaTime;
                m_Throttle = Utility.Clamp(m_Throttle, -1.0f, 1.0f);
                state.mainThrottle = Utility.Clamp(state.mainThrottle + m_Throttle, -1.0f, 1.0f);
                
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
            case DiscreteAction.CyclePresets:
                controller.currentPreset++;
                if(controller.currentPreset >= controller.presets.Count)
                {
                    controller.currentPreset = 0;
                }
                ScreenMessages.PostScreenMessage("PRESET: " + controller.GetCurrentPreset().name.ToUpper(), 1.0f, ScreenMessageStyle.LOWER_CENTER);
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
                if (TimeWarp.WarpMode == TimeWarp.Modes.HIGH)
                {
                    if (TimeWarp.CurrentRateIndex >= TimeWarp.fetch.warpRates.Length - 1)
                    {
                        return;
                    }
                }
                else
                {
                    if (TimeWarp.CurrentRateIndex >= TimeWarp.fetch.physicsWarpRates.Length - 1)
                    {
                        return;
                    }
                }

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
            case DiscreteAction.QuickSave:
                GamePersistence.SaveGame("persistent", HighLogic.SaveFolder, SaveMode.OVERWRITE);
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
                    m_Yaw += value;
                    m_Yaw = Utility.Clamp(m_Yaw, -1.0f, 1.0f);
                    return;
                case ContinuousAction.YawTrim:
                    state.yawTrim = value;
                    state.yawTrim = Utility.Clamp(state.yawTrim, -1.0f, 1.0f);
                    return;
                case ContinuousAction.Pitch:
                    m_Pitch += value;
                    m_Pitch = Utility.Clamp(m_Pitch, -1.0f, 1.0f);
                    return;
                case ContinuousAction.PitchTrim:
                    state.pitchTrim = value;
                    state.pitchTrim = Utility.Clamp(state.pitchTrim, -1.0f, 1.0f);
                    return;
                case ContinuousAction.Roll:
                    m_Roll += value;
                    m_Roll = Utility.Clamp(m_Roll, -1.0f, 1.0f);
                    return;
                case ContinuousAction.RollTrim:
                    state.rollTrim = value;
                    state.rollTrim = Utility.Clamp(state.rollTrim, -1.0f, 1.0f);
                    return;
                case ContinuousAction.X:
                    m_X += value;
                    m_X = Utility.Clamp(m_X, -1.0f, 1.0f);
                    return;
                case ContinuousAction.Y:
                    m_Y += value;
                    m_Y = Utility.Clamp(m_Y, -1.0f, 1.0f);
                    return;
                case ContinuousAction.Z:
                    m_Z += value;
                    m_Z = Utility.Clamp(m_Z, -1.0f, 1.0f);
                    return;
                case ContinuousAction.Throttle:
                    m_Throttle += value;
                    m_Throttle = Utility.Clamp(m_Throttle, 0.0f, 1.0f - state.mainThrottle);
                    return;
                case ContinuousAction.ThrottleIncrement:
                    m_Throttle += value * controller.incrementalThrottleSensitivity;
                    m_Throttle = Utility.Clamp(m_Throttle, 0.0f, 1.0f - state.mainThrottle);
                    return;
                case ContinuousAction.ThrottleDecrement:
                    m_Throttle -= value * controller.incrementalThrottleSensitivity;
                    m_Throttle = Utility.Clamp(m_Throttle, 0.0f, 1.0f - state.mainThrottle);
                    return;
                case ContinuousAction.CameraX:
                    FlightCamera.CamHdg += value * controller.incrementalThrottleSensitivity;
                    return;
                case ContinuousAction.CameraY:
                    FlightCamera.CamPitch += value * controller.incrementalThrottleSensitivity;
                    return;
                case ContinuousAction.CameraZoom:
                    FlightCamera.fetch.SetDistance(FlightCamera.fetch.Distance + value);
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
