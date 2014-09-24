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

        public static Vector2 position = new Vector2(128, 128);
        public static Vector2 size = new Vector2(512, 512);

        private static DropDownList m_PresetsDropDown = new DropDownList();

        private static void SelectionChanged(int index, int oldIndex)
        {

        }

        public static void DoWindow(int window)
        {
            m_PresetsDropDown.SelectionChanged += SelectionChanged;
            m_PresetsDropDown.Items = new List<string>();
            m_PresetsDropDown.Items.Add("hello");
            m_PresetsDropDown.Items.Add("world");
            m_PresetsDropDown.Items.Add("!");

            m_PresetsDropDown.DrawBlockingSelector();
            GUILayout.BeginVertical();
            m_PresetsDropDown.DrawButton();
            GUILayout.Label("hello world");

            GUILayout.EndVertical();
            m_PresetsDropDown.DrawDropDown();
            m_PresetsDropDown.CloseOnOutsideClick();

            GUI.DragWindow();
        }

        public static void OnGUI()
        {
            GUI.Window(1337, new Rect(position.x, position.y, size.x, size.y), DoWindow, "Fly-By-Wire Preset Editor");
        }

    }

}
