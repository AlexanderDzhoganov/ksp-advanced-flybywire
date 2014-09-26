using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace KSPAdvancedFlyByWire
{

    public class PresetEditor
    {

        public PresetEditor(ControllerConfiguration controller)
        {
            m_Controller = controller;
        }

        private ControllerConfiguration m_Controller;

        public Vector2 position = new Vector2(128, 128);
        public Vector2 size = new Vector2(512, 512);

        private DropDownList m_PresetsDropDown = new DropDownList();

        private void SelectionChanged(int index, int oldIndex)
        {

        }

        private Bitset m_CurrentMask = null;

        public void SetCurrentBitmask(Bitset mask)
        {
            m_CurrentMask = mask;
        }

        private DiscreteAction m_CurrentlyEditingDiscreteAction = DiscreteAction.None;
        private ContinuousAction m_CurrentlyEditingContinuousAction = ContinuousAction.None;

        private Vector2 m_ScrollPosition = new Vector2(0, 0);

        public void DoWindow(int window)
        {
            m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition);

            GUILayout.Label("Discrete actions:");

            var currentPreset = m_Controller.presets[m_Controller.currentPreset];
            foreach(var action in (DiscreteAction[])Enum.GetValues(typeof(DiscreteAction)))
            {
                if(action == DiscreteAction.None)
                {
                    continue;
                }

                GUILayout.BeginHorizontal();

                GUILayout.Label(action.ToString());

                GUILayout.FlexibleSpace();

                string label = "";

                var bitset = currentPreset.GetBitsetForDiscreteBinding(action);
                if(m_CurrentlyEditingDiscreteAction == action)
                {
                    label = "Press desired combination";

                    if(m_CurrentMask != null)
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

                if(GUILayout.Button(label, GUILayout.Width(256)))
                {
                    if (m_CurrentlyEditingDiscreteAction != action)
                    {
                        m_CurrentlyEditingDiscreteAction = action;
                    }

                    m_CurrentMask = null;
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.Label("Continuous actions:");

            foreach (var action in (ContinuousAction[])Enum.GetValues(typeof(ContinuousAction)))
            {
                if(action == ContinuousAction.None)
                {
                    continue;
                }

                GUILayout.BeginHorizontal();

                GUILayout.Label(action.ToString());

                GUILayout.FlexibleSpace();

                string label = "FIXME!!!!!";

                var bitset = currentPreset.GetBitsetForContinuousBinding(action);
                if (bitset.Value == null)
                {
                    if (m_CurrentlyEditingContinuousAction == action)
                    {
                        label = "Press desired combination";
                    }
                    else
                    {
                        label = "Click to assign";
                    }
                }
                else
                {
                    label = m_Controller.iface.ConvertMaskToName(bitset.Value, true, bitset.Key);
                }

                GUILayout.Button(label);

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }

        public void OnGUI()
        {
            var rect = new Rect(position.x, position.y, size.x, size.y);
            string hash = "PresetEditor " + m_Controller.wrapper.ToString() + " - " + m_Controller.controllerIndex.ToString();

            if (rect.Contains(Input.mousePosition))
            {
                InputLockManager.SetControlLock(hash);
            }
            else
            {
                InputLockManager.RemoveControlLock(hash);
            }

            GUI.Window(1337, rect, DoWindow, "Fly-By-Wire Preset Editor");
        }

    }

}
