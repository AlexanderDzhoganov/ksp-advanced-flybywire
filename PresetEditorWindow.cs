using System;
using System.Collections.Generic;

using UnityEngine;

namespace KSPAdvancedFlyByWire
{

    public class PresetEditorWindow
    {

        public ControllerConfiguration m_Controller;
        public int m_EditorId;

        public Rect windowRect = new Rect(448, 128, 512, 512);

        public Bitset m_CurrentMask = null;

        public DiscreteAction m_CurrentlyEditingDiscreteAction = DiscreteAction.None;
        public ContinuousAction m_CurrentlyEditingContinuousAction = ContinuousAction.None;

        public Vector2 m_ScrollPosition = new Vector2(0, 0);

        public bool shouldBeDestroyed = false;

        public string inputLockHash;

        public float m_ClickSleepTimer = 0.0f;

        public bool m_DestructiveActionWait = false;
        public float m_DestructiveActionTimer = 0.0f;

        public float[] axisSnapshot;

        public PresetEditorWindow(ControllerConfiguration controller, int editorId)
        {
            m_Controller = controller;
            m_EditorId = editorId;
            axisSnapshot = new float[controller.iface.GetAxesCount()];
            inputLockHash = "PresetEditor " + m_Controller.wrapper.ToString() + " - " + m_Controller.controllerIndex.ToString();
        }

        public void SetCurrentBitmask(Bitset mask)
        {
            m_CurrentMask = mask;
        }

        public virtual void DoWindow(int window)
        {
            GUI.DragWindow(new Rect(0, 0, 10000, 20));

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("X", GUILayout.Height(16)) || m_Controller == null || m_Controller.iface == null)
            {
                shouldBeDestroyed = true;
                if (m_Controller != null)
                {
                    m_Controller.presetEditorOpen = false;
                    AdvancedFlyByWire.Instance.SaveState(null);
                }

                return;
            }

            GUILayout.EndHorizontal();

            var currentPreset = m_Controller.GetCurrentPreset();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Preset: ");

            currentPreset.name = GUILayout.TextField(currentPreset.name, GUILayout.Width(256));

            if (m_Controller.currentPreset > 0)
            {
                if (GUILayout.Button("<"))
                {
                    m_Controller.currentPreset--;
                }
            }
            else
            {
                GUI.enabled = false;
                GUILayout.Button("<");
                GUI.enabled = true;
            }

            if (m_Controller.currentPreset < m_Controller.presets.Count - 1)
            {
                if (GUILayout.Button(">"))
                {
                    m_Controller.currentPreset++;
                }
            }
            else
            {
                GUI.enabled = false;
                GUILayout.Button(">");
                GUI.enabled = true;
            }

            if (GUILayout.Button("New"))
            {
                m_Controller.presets.Add(new ControllerPreset());
                m_Controller.currentPreset = m_Controller.presets.Count - 1;
            }

            string destructiveRemoveLabel = "Delete";
            if (m_DestructiveActionWait)
            {
                destructiveRemoveLabel = "Sure?";
            }

            if (m_Controller.presets.Count <= 1 || m_Controller.currentPreset == 0)
            {
                GUI.enabled = false;
            }

            if (GUILayout.Button(destructiveRemoveLabel))
            {
                if(m_DestructiveActionWait)
                {
                    if(m_Controller.presets.Count > 1)
                    {
                        m_Controller.presets.RemoveAt(m_Controller.currentPreset);
                        m_Controller.currentPreset--;
                        if (m_Controller.currentPreset < 0)
                        {
                            m_Controller.currentPreset = 0;
                        }

                        m_DestructiveActionWait = false;
                        m_DestructiveActionTimer = 0.0f;
                    }
                }
                else
                {
                    m_DestructiveActionWait = true;
                    m_DestructiveActionTimer = 2.0f;
                }
            }

            GUI.enabled = true;
            GUILayout.EndHorizontal();

            m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition);

            GUILayout.Label("Continuous actions");

            foreach (var action in (ContinuousAction[])Enum.GetValues(typeof(ContinuousAction)))
            {
                if (action == ContinuousAction.None)
                {
                    continue;
                }

                GUILayout.BeginHorizontal();
                GUILayout.Label(Stringify.ContinuousActionToString(action));
                GUILayout.FlexibleSpace();

                string label = "";

                var axisBitsetPair = currentPreset.GetBitsetForContinuousBinding(action);
                if (m_CurrentlyEditingContinuousAction == action)
                {
                    label = "Press desired combination";

                    var buttonsMask = m_Controller.iface.GetButtonsMask();

                    for (int i = 0; i < m_Controller.iface.GetAxesCount(); i++)
                    {
                        if (Math.Abs(m_Controller.iface.GetAxisState(i) - axisSnapshot[i]) > 0.1 && m_ClickSleepTimer == 0.0f)
                        {
                            currentPreset.SetContinuousBinding(i, buttonsMask, action);
                            m_CurrentlyEditingContinuousAction = ContinuousAction.None;
                        }
                    }
                }

                axisBitsetPair = currentPreset.GetBitsetForContinuousBinding(action);
                if (m_CurrentlyEditingContinuousAction != action)
                {
                    if (axisBitsetPair.Value == null)
                    {
                        label = "Click to assign";
                    }
                    else
                    {
                        label = m_Controller.iface.ConvertMaskToName(axisBitsetPair.Value, true, axisBitsetPair.Key);
                    }
                }

                if (GUILayout.Button(label, GUILayout.Width(256)))
                {
                    if (m_CurrentlyEditingContinuousAction != action)
                    {
                        m_CurrentlyEditingContinuousAction = action;
                        m_CurrentlyEditingDiscreteAction = DiscreteAction.None;
                        m_ClickSleepTimer = 0.25f;

                        for (int i = 0; i < m_Controller.iface.GetAxesCount(); i++)
                        {
                            axisSnapshot[i] = m_Controller.iface.GetAxisState(i);
                        }
                    }

                    m_CurrentMask = null;
                }

                if (GUILayout.Button("X"))
                {
                    currentPreset.UnsetContinuousBinding(action);
                    m_CurrentlyEditingContinuousAction = ContinuousAction.None;
                    m_CurrentlyEditingDiscreteAction = DiscreteAction.None;
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.Label("Discrete actions");

            foreach (var action in (DiscreteAction[])Enum.GetValues(typeof(DiscreteAction)))
            {
                if (action == DiscreteAction.None)
                {
                    continue;
                }

                GUILayout.BeginHorizontal();
                GUILayout.Label(Stringify.DiscreteActionToString(action));
                GUILayout.FlexibleSpace();

                string label = "";

                var bitset = currentPreset.GetBitsetForDiscreteBinding(action);
                if (m_CurrentlyEditingDiscreteAction == action)
                {
                    label = "Press desired combination";

                    if (m_CurrentMask != null && m_ClickSleepTimer == 0.0f)
                    {
                        currentPreset.SetDiscreteBinding(m_CurrentMask, action);
                        m_CurrentMask = null;
                        m_CurrentlyEditingDiscreteAction = DiscreteAction.None;
                    }
                }

                bitset = currentPreset.GetBitsetForDiscreteBinding(action);
                if (m_CurrentlyEditingDiscreteAction != action)
                {
                    if (bitset == null)
                    {
                        label = "Click to assign";
                    }
                    else
                    {
                        label = m_Controller.iface.ConvertMaskToName(bitset);
                    }
                }

                if (GUILayout.Button(label, GUILayout.Width(256)))
                {
                    if (m_CurrentlyEditingDiscreteAction != action)
                    {
                        m_CurrentlyEditingDiscreteAction = action;
                        m_CurrentlyEditingContinuousAction = ContinuousAction.None;
                        m_ClickSleepTimer = 0.25f;
                    }

                    m_CurrentMask = null;
                }

                if (GUILayout.Button("X"))
                {
                    currentPreset.UnsetDiscreteBinding(action);
                    m_CurrentlyEditingContinuousAction = ContinuousAction.None;
                    m_CurrentlyEditingDiscreteAction = DiscreteAction.None;
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }

        public virtual void OnGUI()
        {
            if (m_Controller.iface == null)
            {
                shouldBeDestroyed = true;
                return;
            }

            if (m_ClickSleepTimer > 0.0f)
            {
                m_ClickSleepTimer -= Time.unscaledDeltaTime;
                if (m_ClickSleepTimer < 0.0f)
                {
                    m_CurrentMask = null;
                    m_ClickSleepTimer = 0.0f;
                }
            }

            if (m_DestructiveActionTimer > 0.0f)
            {
                m_DestructiveActionTimer -= Time.unscaledDeltaTime;
                if (m_DestructiveActionTimer < 0.0f)
                {
                    m_DestructiveActionWait = false;
                    m_DestructiveActionTimer = 0.0f;
                }
            }

            if (Utility.RectContainsMouse(windowRect))
            {
                InputLockManager.SetControlLock(inputLockHash);
            }
            else
            {
                InputLockManager.RemoveControlLock(inputLockHash);
            }

            windowRect = GUI.Window(1337 + m_EditorId, windowRect, DoWindow, "Fly-By-Wire Preset Editor");
            windowRect = Utility.ClampRect(windowRect, new Rect(0, 0, Screen.width, Screen.height));
        }

    }

}
