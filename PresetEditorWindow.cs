using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using UnityEngine;

namespace KSPAdvancedFlyByWire
{

    public class PresetEditorWindow
    {

        private ControllerConfiguration m_Controller;
        private int m_EditorId;

        private Rect windowRect = new Rect(128, 128, 512, 512);

        private Bitset m_CurrentMask = null;

        private DiscreteAction m_CurrentlyEditingDiscreteAction = DiscreteAction.None;
        private ContinuousAction m_CurrentlyEditingContinuousAction = ContinuousAction.None;

        private Vector2 m_ScrollPosition = new Vector2(0, 0);

        public bool shouldBeDestroyed = false;

        public string inputLockHash;

        public PresetEditorWindow(ControllerConfiguration controller, int editorId)
        {
            m_Controller = controller;
            m_EditorId = editorId;
            inputLockHash = "PresetEditor " + m_Controller.wrapper.ToString() + " - " + m_Controller.controllerIndex.ToString();
        }

        public void SetCurrentBitmask(Bitset mask)
        {
            m_CurrentMask = mask;
        }

        public void DoWindow(int window)
        {
            GUI.DragWindow(new Rect(0, 0, 10000, 20));

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Close window"))
            {
                shouldBeDestroyed = true;
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

            if (GUILayout.Button("+"))
            {
                m_Controller.presets.Add(new ControllerPreset());
                m_Controller.currentPreset = m_Controller.presets.Count - 1;
            }

            GUILayout.EndHorizontal();

            m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition);

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

                    if (m_CurrentMask != null)
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

                    for (int i = 0; i < m_Controller.iface.GetAxesCount(); i++ )
                    {
                        if(m_Controller.iface.GetAxisState(i) != 0.0f)
                        {
                            currentPreset.SetContinuousBinding(i, m_Controller.iface.GetButtonsMask(), action);
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

            GUILayout.EndScrollView();
        }

        public void OnGUI()
        {
            if (windowRect.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y)))
            {
                InputLockManager.SetControlLock(inputLockHash);
            }
            else
            {
                InputLockManager.RemoveControlLock(inputLockHash);
            }

            windowRect = GUI.Window(1337 + m_EditorId, windowRect, DoWindow, "Fly-By-Wire Preset Editor");
        }

    }

}
