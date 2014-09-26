using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace KSPAdvancedFlyByWire
{

    class ControllerTest
    {

        private ControllerConfiguration m_Controller;
        private int m_EditorId;

        private Rect windowRect = new Rect(128, 128, 512, 512);

        private Vector2 m_ScrollPosition = new Vector2(0, 0);

        public bool shouldBeDestroyed = false;

        public ControllerTest(ControllerConfiguration controller, int editorId)
        {
            m_Controller = controller;
            m_EditorId = editorId;
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

            m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition);

            GUILayout.Label("Axes:");

            for (int i = 0; i < m_Controller.iface.GetAxesCount(); i++)
            {
                GUILayout.Label(m_Controller.iface.GetAxisName(i) + ": " + m_Controller.iface.GetAxisState(i).ToString());
            }

            GUILayout.Label("Buttons:");

            for (int i = 0; i < m_Controller.iface.GetButtonsCount(); i++)
            {
                GUILayout.Label(m_Controller.iface.GetButtonName(i) + ": " + m_Controller.iface.GetButtonState(i).ToString());
            }

            GUILayout.EndScrollView();

            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }

        public void OnGUI()
        {
            string hash = "Controller Test (" + m_Controller.wrapper.ToString() + " - " + m_Controller.controllerIndex.ToString() + ")";

            if (windowRect.Contains(Input.mousePosition))
            {
                InputLockManager.SetControlLock(hash);
            }
            else
            {
                InputLockManager.RemoveControlLock(hash);
            }

            GUI.Window(2672 + m_EditorId, windowRect, DoWindow, "Fly-By-Wire Preset Editor");
        }

    }

}
