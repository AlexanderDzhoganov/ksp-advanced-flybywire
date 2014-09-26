using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using UnityEngine;

namespace KSPAdvancedFlyByWire
{

    public class PresetEditor
    {

        private ControllerConfiguration m_Controller;
        private int m_EditorId;

        private Rect windowRect = new Rect(128, 128, 512, 512);

        private Bitset m_CurrentMask = null;

        private DiscreteAction m_CurrentlyEditingDiscreteAction = DiscreteAction.None;
        private ContinuousAction m_CurrentlyEditingContinuousAction = ContinuousAction.None;

        private Vector2 m_ScrollPosition = new Vector2(0, 0);

        public bool shouldBeDestroyed = false;

        public PresetEditor(ControllerConfiguration controller, int editorId)
        {
            m_Controller = controller;
            m_EditorId = editorId;
        }

        public void SetCurrentBitmask(Bitset mask)
        {
            m_CurrentMask = mask;
        }

        public void DoWindow(int window)
        {
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

            if (m_Controller.currentPreset < m_Controller.presets.Count)
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

            GUILayout.Label("Discrete actions:");

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

            GUILayout.Label("Continuous actions:");

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

                    if (m_CurrentMask != null)
                    {
                        currentPreset.SetContinuousBinding(axisBitsetPair.Key, m_CurrentMask, action);
                        m_CurrentMask = null;
                        m_CurrentlyEditingDiscreteAction = DiscreteAction.None;
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
                    currentPreset.UnsetContinuousBinding(axisBitsetPair.Key, action);
                    m_CurrentlyEditingContinuousAction = ContinuousAction.None;
                    m_CurrentlyEditingDiscreteAction = DiscreteAction.None;
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();

            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }

        public void OnGUI()
        {
            string hash = "PresetEditor " + m_Controller.wrapper.ToString() + " - " + m_Controller.controllerIndex.ToString();

            if (windowRect.Contains(Input.mousePosition))
            {
                InputLockManager.SetControlLock(hash);
            }
            else
            {
                InputLockManager.RemoveControlLock(hash);
            }

            GUI.Window(1337 + m_EditorId, windowRect, DoWindow, "Fly-By-Wire Preset Editor");
        }

    }

}
