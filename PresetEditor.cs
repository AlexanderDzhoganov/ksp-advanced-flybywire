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

        Rect windowRect = new Rect(128, 128, 512, 512);

        private DropDownList m_PresetsDropDown = new DropDownList();

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
                        m_CurrentlyEditingContinuousAction = ContinuousAction.None;
                    }

                    m_CurrentMask = null;
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
                GUILayout.Label(action.ToString());
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

            GUI.Window(1337, windowRect, DoWindow, "Fly-By-Wire Preset Editor");
        }

    }

}
