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

        private DiscreteAction m_CurrentlyEditingDiscreteAction = DiscreteAction.None;

        private Vector2 m_ScrollPosition = new Vector2(0, 0);

        public void DoWindow(int window)
        {
            m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition);

            GUILayout.Label("Discrete actions:");

            var currentPreset = m_Controller.presets[m_Controller.currentPreset];
            foreach(var discreteAction in (DiscreteAction[])Enum.GetValues(typeof(DiscreteAction)))
            {
                GUILayout.BeginHorizontal();

                GUILayout.Label(discreteAction.ToString());

                GUILayout.FlexibleSpace();

                string label = "FIXME!!!!!";

                var bitset = currentPreset.GetBitsetForDiscreteBinding(discreteAction);
                if (bitset == null)
                {
                    if(m_CurrentlyEditingDiscreteAction == discreteAction)
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
                    label = m_Controller.iface.ConvertMaskToName(bitset);
                }

                GUILayout.Button(label);

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }

        public void OnGUI()
        {
            GUI.Window(1337, new Rect(position.x, position.y, size.x, size.y), DoWindow, "Fly-By-Wire Preset Editor");
        }

    }

}
