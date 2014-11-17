using System;
using System.Collections.Generic;

using UnityEngine;

namespace KSPAdvancedFlyByWire
{

    class ModSettingsWindow
    {
        private Rect windowRect = new Rect(0, 648, 420, 256);

        private Vector2 m_ScrollPosition = new Vector2(0, 0);

        public bool shouldBeDestroyed = false;

        public string inputLockHash = "AFBW Settings";

        private bool FloatField(float value, out float retValue)
        {
            string text = GUILayout.TextField(value.ToString("0.00"), GUILayout.Width(128));

            float newValue;
            if (float.TryParse(text, out newValue))
            {
                retValue = newValue;
                return true;
            }

            retValue = value;
            return false;
        }

        public void DoWindow(int window)
        {
            GUI.DragWindow(new Rect(0, 0, 10000, 20));

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("X", GUILayout.Height(16)))
            {
                shouldBeDestroyed = true;
                return;
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Use stock skin");
            AdvancedFlyByWire.Instance.m_UseKSPSkin = GUILayout.Toggle(AdvancedFlyByWire.Instance.m_UseKSPSkin, "");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Use old presets editor");
            AdvancedFlyByWire.Instance.m_UseOldPresetsWindow = GUILayout.Toggle(AdvancedFlyByWire.Instance.m_UseOldPresetsWindow, "");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("PrecisionMode Factor");
            AdvancedFlyByWire.Instance.m_UsePrecisionModeFactor = GUILayout.Toggle(AdvancedFlyByWire.Instance.m_UsePrecisionModeFactor, "");
            GUILayout.FlexibleSpace();
            GUILayout.Label(AdvancedFlyByWire.Instance.m_PrecisionModeFactor.ToString("0.000"));
            AdvancedFlyByWire.Instance.m_PrecisionModeFactor = GUILayout.HorizontalSlider(AdvancedFlyByWire.Instance.m_PrecisionModeFactor, 0.0f, 1.0f, GUILayout.Width(150));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save configuration"))
            {
                AdvancedFlyByWire.Instance.SaveState(null);
            }
            GUILayout.EndHorizontal();
        }

        public void OnGUI()
        {
            if (Utility.RectContainsMouse(windowRect))
            {
                InputLockManager.SetControlLock(inputLockHash);
            }
            else
            {
                InputLockManager.RemoveControlLock(inputLockHash);
            }

            windowRect = GUI.Window(8721, windowRect, DoWindow, inputLockHash);
            windowRect = Utility.ClampRect(windowRect, new Rect(0, 0, Screen.width, Screen.height));
        }

    }

}
