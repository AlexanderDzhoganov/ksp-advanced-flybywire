using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace KSPAdvancedFlyByWire
{

    class ControllerConfigurationWindow
    {

        private ControllerConfiguration m_Controller;
        private int m_EditorId;

        private Rect windowRect = new Rect(976, 128, 385, 512);

        private Vector2 m_ScrollPosition = new Vector2(0, 0);

        private bool m_ShowDetails = false;

        public bool shouldBeDestroyed = false;

        public string inputLockHash;

        public ControllerConfigurationWindow(ControllerConfiguration controller, int editorId)
        {
            m_Controller = controller;
            m_EditorId = editorId;

            inputLockHash = "Controller Configuration (" + m_Controller.wrapper.ToString() + " - " + m_Controller.controllerIndex.ToString() + ")";
        }

        private bool FloatField(float value, out float retValue)
        {
            string text = GUILayout.TextField(value.ToString("0.00"));

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

            if (GUILayout.Button("Close window") || m_Controller == null || m_Controller.iface == null)
            {
                shouldBeDestroyed = true;
                m_Controller.controllerConfigurationOpen = false;
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Discrete action step");
            FloatField(m_Controller.discreteActionStep, out m_Controller.discreteActionStep);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Incremental action sensitivity");
            FloatField(m_Controller.incrementalActionSensitivity, out m_Controller.incrementalActionSensitivity);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Camera sensitivity");
            FloatField(m_Controller.cameraSensitivity, out m_Controller.cameraSensitivity);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Analog input curve");

            string curveLabel = "Quadratic";
            if(m_Controller.analogInputCurve == CurveType.Identity)
            {
                curveLabel = "Linear";
            }
            
            if(GUILayout.Button(curveLabel))
            {
                if(m_Controller.analogInputCurve == CurveType.Identity)
                {
                    m_Controller.SetAnalogInputCurveType(CurveType.XSquared);
                }
                else
                {
                    m_Controller.SetAnalogInputCurveType(CurveType.Identity);
                }
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Treat hats as buttons (requires restart)");
            var state = GUILayout.Toggle(m_Controller.treatHatsAsButtons, "");
            if (state != m_Controller.treatHatsAsButtons)
            {
                m_Controller.treatHatsAsButtons = state;
            }
            GUILayout.EndHorizontal();

            GUILayout.Label("If some axes below are not displaying 0.0 when the controller is left untouched then it needs calibration.");
            GUILayout.Label("Leave the controller and press calibrate, then move around all the axes");

            if(GUILayout.Button("Calibrate"))
            {
                for (int i = 0; i < m_Controller.iface.GetAxesCount(); i++)
                {
                    float value = m_Controller.iface.GetRawAxisState(i);
                    m_Controller.iface.axisLeft[i] = value - 1e-4f;
                    m_Controller.iface.axisIdentity[i] = value;
                    m_Controller.iface.axisRight[i] = value + 1e-4f;
                    m_Controller.iface.axisNegativeDeadZones[i] = float.MaxValue;
                    m_Controller.iface.axisPositiveDeadZones[i] = float.MaxValue;
                }
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Show details");
            m_ShowDetails = GUILayout.Toggle(m_ShowDetails, "");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition);

            GUILayout.Label("Axes");

            for (int i = 0; i < m_Controller.iface.GetAxesCount(); i++)
            {
          
                GUILayout.BeginHorizontal();

                string label = "";
                float value = m_Controller.iface.GetAxisState(i);
                label += m_Controller.iface.GetAxisName(i) + ") ";
                GUILayout.Label(label);

                GUILayout.FlexibleSpace();

                label = value.ToString("0.000");
                if(value >= 0.0f)
                {
                    label += " ";
                }

                if(m_ShowDetails)
                {
                    label += " (min: " + m_Controller.iface.axisLeft[i].ToString() +
                        ", ident: " + m_Controller.iface.axisIdentity[i].ToString() +
                        ", max: " + m_Controller.iface.axisRight[i].ToString() +
                        ", raw: " + m_Controller.iface.GetRawAxisState(i).ToString() +
                        ", deadzone min: " + m_Controller.iface.axisNegativeDeadZones[i].ToString() +
                        ", deadzone max: " + m_Controller.iface.axisPositiveDeadZones[i].ToString() + ")";
                }

                GUILayout.Label(label);
                
                if(!m_ShowDetails)
                {
                    GUI.enabled = false;
                    GUILayout.HorizontalSlider(value, -1.0f, 1.0f, GUILayout.Width(150));
                    GUI.enabled = true;
                }

                GUILayout.Label("Invert");
                m_Controller.iface.axisInvert[i] = GUILayout.Toggle(m_Controller.iface.axisInvert[i], "");

                GUILayout.EndHorizontal();
            }

            GUILayout.Label("Buttons");

            for (int i = 0; i < m_Controller.iface.GetButtonsCount(); i++)
            {
                GUILayout.Label(m_Controller.iface.GetButtonName(i) + ": " + m_Controller.iface.GetButtonState(i).ToString());
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

            if(!m_ShowDetails)
            {
                windowRect.width = 384;
            }
            else
            {
                windowRect.width = 768;
            }

            windowRect = GUI.Window(2672 + m_EditorId, windowRect, DoWindow, inputLockHash);
            windowRect = Utility.ClampRect(windowRect, new Rect(0, 0, Screen.width, Screen.height));
        }

    }

}
