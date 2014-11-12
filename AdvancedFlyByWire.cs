using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace KSPAdvancedFlyByWire
{

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class AdvancedFlyByWire : MonoBehaviour
    {
        public const int versionMajor = 1;
        public const int versionMinor = 1;

        // UI stuff
        private Toolbar.IButton m_ToolbarButton = null;
        private bool m_UIActive = true;
        private bool m_UIHidden = false;
        private Rect m_WindowRect = new Rect(0, 64, 432, 576);
        private Vector2 m_ScrollPosition = new Vector2(0, 0);

        public bool m_UseKSPSkin = true;
        public bool m_UseOldPresetsWindow = false;

        public bool m_UsePrecisionModeFactor = false;
        public float m_PrecisionModeFactor = 0.5f;

        // Configuration
        private Configuration m_Configuration = null;

        private ModSettingsWindow m_ModSettings = null;
        private List<PresetEditorWindow> m_PresetEditors = new List<PresetEditorWindow>();
        private List<ControllerConfigurationWindow> m_ControllerTests = new List<ControllerConfigurationWindow>();

        // Flight state
        private FlightManager m_FlightManager = new FlightManager();

        private static AdvancedFlyByWire m_Instance = null;

        public static AdvancedFlyByWire Instance
        {
            get
            {
                return m_Instance;
            }
        }

        public float GetPrecisionModeFactor()
        {
            bool precisionModeEnabled = FlightInputHandler.fetch != null && FlightInputHandler.fetch.precisionMode;
            return (precisionModeEnabled && m_UsePrecisionModeFactor) ? m_PrecisionModeFactor : 1;
        }

        public string GetAbsoluteConfigurationPath()
        {
            return Path.Combine
            (
                Path.Combine
                (
                    Path.Combine
                    (
                        KSPUtil.ApplicationRootPath, "GameData"
                    ),
                    "ksp-advanced-flybywire"
                ), "advanced_flybywire_config_v" + versionMajor + versionMinor + ".xml"
            );
        }

        public void Awake()
        {
            m_Instance = this;

            RegisterCallbacks(); 
            LoadState(null);
            InitializeToolbarButton();

            m_UIHidden = false;
            m_UIActive = false;
            
            print("Advanced Fly-By-Wire: Initialized");
        }

        public void Start()
        {
            AddFlyByWireCallbackToActiveVessel();
            m_LastChangedActiveVessel = FlightGlobals.ActiveVessel;
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

            m_FlightManager = null;

            print("Advanced Fly-By-Wire: Deinitialized");
        }

        private void LoadState(ConfigNode configNode)
        {
            if (configNode != null)
            {
                if (configNode.HasValue("useStockSkin"))
                {
                    if (configNode.GetValue("useStockSkin") == "true")
                    {
                        configNode.SetValue("useStockSkin", "true");
                    }
                }

                if (configNode.HasValue("useOldPresetEditor"))
                {
                    if (configNode.GetValue("useOldPresetEditor") == "true")
                    {
                        configNode.SetValue("useOldPresetEditor", "true");
                    }
                }

                if (configNode.HasValue("usePrecisionModeFactor"))
                {
                    if (configNode.GetValue("usePrecisionModeFactor") == "true")
                    {
                        configNode.SetValue("usePrecisionModeFactor", "true");
                    }
                }
            }
            
            m_Configuration = Configuration.Deserialize(GetAbsoluteConfigurationPath());
            if (m_Configuration == null)
            {
                m_Configuration = new Configuration();
                Configuration.Serialize(GetAbsoluteConfigurationPath(), m_Configuration);
            }

            m_FlightManager.m_Configuration = m_Configuration;
        }

        public void SaveState(ConfigNode configNode)
        {
            if (configNode != null)
            {
                configNode.SetValue("useStockSkin", m_UseKSPSkin ? "true" : "false");
                configNode.SetValue("useOldPresetEditor", m_UseOldPresetsWindow ? "true" : "false");
                configNode.SetValue("usePrecisionModeFactor", m_UsePrecisionModeFactor ? "true" : "false");
            }

            Configuration.Serialize(GetAbsoluteConfigurationPath(), m_Configuration);
        }

        private void RegisterCallbacks()
        {
            GameEvents.onShowUI.Add(OnShowUI);
            GameEvents.onHideUI.Add(OnHideUI);
            GameEvents.onGameStateSave.Add(SaveState);
            GameEvents.onGameStateLoad.Add(LoadState);
            GameEvents.onGUIRecoveryDialogSpawn.Add(OnGUIRecoveryDialogSpawn);
            GameEvents.onGamePause.Add(OnGamePause);
            GameEvents.onGameUnpause.Add(OnGameUnpause);
            GameEvents.onVesselChange.Add(OnVesselChange);
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
            GameEvents.onVesselChange.Remove(OnVesselChange);

            if (FlightGlobals.ActiveVessel != null)
            {
                FlightGlobals.ActiveVessel.OnFlyByWire -= m_FlightManager.OnFlyByWire;
            }
        }

        void AddFlyByWireCallbackToActiveVessel()
        {
            FlightGlobals.ActiveVessel.OnFlyByWire -= m_FlightManager.OnFlyByWire;
            FlightGlobals.ActiveVessel.OnFlyByWire += m_FlightManager.OnFlyByWire;
        }

        private Vessel m_LastChangedActiveVessel = null;

        private void OnVesselChange(Vessel vessel)
        {
            if (vessel == null)
            {
                return;
            }

            if (m_LastChangedActiveVessel != null)
            {
                m_LastChangedActiveVessel.OnFlyByWire -= m_FlightManager.OnFlyByWire;
                m_LastChangedActiveVessel = FlightGlobals.ActiveVessel;
            }

            m_FlightManager.m_Throttle.SetZero();
            m_FlightManager.m_WheelThrottle.SetZero();
            m_FlightManager.m_Yaw.SetZero();
            m_FlightManager.m_Pitch.SetZero();
            m_FlightManager.m_Roll.SetZero();

            AddFlyByWireCallbackToActiveVessel();
        }

        private void InitializeToolbarButton()
        {
            if(Toolbar.ToolbarManager.Instance == null)
            {
                print("Advanced Fly-By-Wire: toolbar instance not available");
                return;
            }

            m_ToolbarButton = Toolbar.ToolbarManager.Instance.add("advancedflybywire", "mainButton");
            m_ToolbarButton.TexturePath = "ksp-advanced-flybywire/toolbar_btn";
            m_ToolbarButton.ToolTip = "Advanced Fly-By-Wire";

            m_ToolbarButton.Visibility = new Toolbar.GameScenesVisibility
            (
                GameScenes.FLIGHT
            );

            m_ToolbarButton.OnClick += OnToolbarButtonClick;
        }

        void OnToolbarButtonClick(Toolbar.ClickEvent ev)
        {
            this.gameObject.SetActive(true);
            m_UIActive = !m_UIActive;
            m_UIHidden = false;
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

        private void OnShowUI()
        {
            m_UIHidden = false;
        }

        private void OnHideUI()
        {
            m_UIHidden = true;
        }

        public void ButtonPressedCallback(IController controller, int button, FlightCtrlState state)
        {
            if(!HighLogic.LoadedSceneIsFlight)
            {
                return;
            }

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
                    m_FlightManager.EvaluateDiscreteAction(config, action, state);
                    config.evaluatedDiscreteActionMasks.Add(mask);
                }
            }
        }

        public void ButtonReleasedCallback(IController controller, int button, FlightCtrlState state)
        {
            if (!HighLogic.LoadedSceneIsFlight)
            {
                return;
            }

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
                    m_FlightManager.EvaluateDiscreteActionRelease(config, action, state);
                }   
            }
        }

        private void Update()
        {
            SDLController.SDLUpdateState();

            if(HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel != null)
            {
                if(TimeWarp.fetch != null && TimeWarp.fetch.Mode == TimeWarp.Modes.HIGH && TimeWarp.CurrentRateIndex != 0)
                {
                    m_FlightManager.OnFlyByWire(new FlightCtrlState());
                }
            }

            if(Input.GetKey("left shift") && Input.GetKey("l"))
            {
                m_UIActive = true;
            }
        }

        void DoMainWindow(int index)
        {
            GUI.DragWindow(new Rect(0, 0, 10000, 20));

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("X", GUILayout.Height(16)))
            {
                m_UIActive = false;
                InputLockManager.RemoveControlLock("AdvancedFlyByWireMainWindow");
            }

            GUILayout.EndHorizontal();

            if (GUILayout.Button("Mod settings"))
            {
                if (m_ModSettings == null)
                {
                    m_ModSettings = new ModSettingsWindow();
                }
            }
            
            m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Controllers");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            var controllers = IController.EnumerateAllControllers();
            foreach (var controller in controllers)
            {
                GUILayout.BeginHorizontal();
                ControllerConfiguration config = m_Configuration.GetConfigurationByControllerType(controller.Key,
                    controller.Value.Key);
                bool isEnabled = config != null && config.iface != null;
                bool newIsEnabled = GUILayout.Toggle(isEnabled, "");

                GUILayout.Label(controller.Value.Value);
                GUILayout.FlexibleSpace();

                if (!isEnabled && newIsEnabled)
                {
                    isEnabled = true;
                    m_Configuration.ActivateController
                    (
                        controller.Key,
                        controller.Value.Key,
                        ButtonPressedCallback,
                        ButtonReleasedCallback
                    );
                }
                else if (isEnabled && !newIsEnabled)
                {
                    m_Configuration.DeactivateController(controller.Key, controller.Value.Key);
                }

                if (!isEnabled || config.presetEditorOpen)
                {
                    GUI.enabled = false;
                }

                if (GUILayout.Button("Presets", GUILayout.Height(32)))
                {
                    if (m_UseOldPresetsWindow)
                    {
                        m_PresetEditors.Add(new PresetEditorWindow(config, m_PresetEditors.Count));
                    }
                    else
                    {
                        m_PresetEditors.Add(new PresetEditorWindowNG(config, m_PresetEditors.Count));
                    }

                    config.presetEditorOpen = true;
                }

                if (isEnabled)
                {
                    GUI.enabled = true;
                }

                if (!isEnabled || config.controllerConfigurationOpen)
                {
                    GUI.enabled = false;
                }

                if (GUILayout.Button("Configuration", GUILayout.Height(32)))
                {
                    m_ControllerTests.Add(new ControllerConfigurationWindow(config, m_ControllerTests.Count));
                    config.controllerConfigurationOpen = true;
                }

                if (isEnabled)
                {
                    GUI.enabled = true;
                }

                GUILayout.EndHorizontal();

                if (isEnabled)
                {
                    GUILayout.Space(4);

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    
                    GUILayout.Label("Preset: " + config.GetCurrentPreset().name);

                    if (config.currentPreset <= 0)
                    {
                        GUI.enabled = false;
                    }
                  
                    if (GUILayout.Button("<", GUILayout.Width(32)))
                    {
                        config.currentPreset--;
                    }

                    GUI.enabled = true;
                    GUILayout.Space(4);

                    if (config.currentPreset >= config.presets.Count - 1)
                    {
                        GUI.enabled = false;
                    }

                    if (GUILayout.Button(">", GUILayout.Width(32)))
                    {
                        config.currentPreset++;
                    }

                    GUI.enabled = true;
                    GUILayout.EndHorizontal();
                }

                GUILayout.Space(24);
                
                GUI.enabled = true;
            }

            GUILayout.EndScrollView();
        }

        void OnGUI()
        {
            if (m_UIHidden || !m_UIActive)
            {
                return;
            }

            GUISkin oldSkin = GUI.skin;
            if (m_UseKSPSkin)
            {
                GUI.skin = HighLogic.Skin;
            }

            if (Utility.RectContainsMouse(m_WindowRect))
            {
                InputLockManager.SetControlLock(ControlTypes.All, "AdvancedFlyByWireMainWindow");
            }
            else
            {
                InputLockManager.RemoveControlLock("AdvancedFlyByWireMainWindow");
            }

            m_WindowRect = GUI.Window(0, m_WindowRect, DoMainWindow, "Advanced Fly-By-Wire");
            m_WindowRect = Utility.ClampRect(m_WindowRect, new Rect(0, 0, Screen.width, Screen.height));

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

            if (m_ModSettings != null)
            {
                if (m_ModSettings.shouldBeDestroyed)
                {
                    InputLockManager.RemoveControlLock(m_ModSettings.inputLockHash);
                    m_ModSettings = null;
                }
                else
                {
                    m_ModSettings.OnGUI();
                }
            }

            GUI.skin = oldSkin;
        }

    }

}
