using System;
using System.Collections.Generic;

using UnityEngine;

namespace KSPAdvancedFlyByWire
{

    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
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

        private bool m_UseKSPSkin = true;
        private bool m_UseOldPresetsWindow = false;

        // Configuration
        private Configuration m_Configuration = null;
        private string m_ConfigurationPath =
            "GameData/ksp-advanced-flybywire/advanced_flybywire_config_v" + versionMajor.ToString() + versionMinor.ToString() + ".xml";

        private List<PresetEditorWindow> m_PresetEditors = new List<PresetEditorWindow>();
        private List<ControllerConfigurationWindow> m_ControllerTests = new List<ControllerConfigurationWindow>();

        // Flight state
        private FlightInputCallback m_Callback = null;
        private FlightManager m_FlightManager = new FlightManager();

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
            Utility.CheckLibrarySupport();

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
            if (m_Callback == null && HighLogic.LoadedSceneIsFlight)
            {
                m_Callback = new FlightInputCallback(m_FlightManager.OnFlyByWire);
                FlightGlobals.ActiveVessel.OnFlyByWire += m_Callback;
            }
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
            }
            
            m_Configuration = Configuration.Deserialize(m_ConfigurationPath);
            if (m_Configuration == null)
            {
                m_Configuration = new Configuration();
                Configuration.Serialize(m_ConfigurationPath, m_Configuration);
            }

            m_FlightManager.m_Configuration = m_Configuration;
        }

        public void SaveState(ConfigNode configNode)
        {
            if (configNode != null)
            {
                configNode.SetValue("useStockSkin", m_UseKSPSkin ? "true" : "false");
                configNode.SetValue("useOldPresetEditor", m_UseOldPresetsWindow ? "true" : "false");
            }

            Configuration.Serialize(m_ConfigurationPath, m_Configuration);
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

            if (m_Callback != null && FlightGlobals.ActiveVessel != null)
            {
                FlightGlobals.ActiveVessel.OnFlyByWire -= m_Callback;
            }
        }

        private void InitializeToolbarButton()
        {
            if(Toolbar.ToolbarManager.Instance == null)
            {
                print("Advanced Fly-By-Wire: toolbar instance not available");
                return;
            }

            m_ToolbarButton = Toolbar.ToolbarManager.Instance.add("advancedflybywire", "mainButton");
            m_ToolbarButton.TexturePath = "000_Toolbar/img_buttonAdvancedFlyByWire";
            m_ToolbarButton.ToolTip = "Advanced Fly-By-Wire";

            m_ToolbarButton.Visibility = new Toolbar.GameScenesVisibility
            (
                GameScenes.MAINMENU,
                GameScenes.FLIGHT,
                GameScenes.SPH,
                GameScenes.EDITOR,
                GameScenes.SETTINGS,
                GameScenes.SPACECENTER,
                GameScenes.TRACKSTATION
            );

            m_ToolbarButton.OnClick += new Toolbar.ClickHandler(OnToolbarButtonClick);
        }

        void OnToolbarButtonClick(Toolbar.ClickEvent ev)
        {
            this.gameObject.SetActive(true);
            m_UIActive = true;
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
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Use stock skin");
            m_UseKSPSkin = GUILayout.Toggle(m_UseKSPSkin, "");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Use old presets editor");
            m_UseOldPresetsWindow = GUILayout.Toggle(m_UseOldPresetsWindow, "");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

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
                        new IController.ButtonPressedCallback(ButtonPressedCallback),
                        new IController.ButtonReleasedCallback(ButtonReleasedCallback)
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

            if (m_WindowRect.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y)))
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

            GUI.skin = oldSkin;
        }

    }

}
