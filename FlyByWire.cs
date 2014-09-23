using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace KSPAdvancedFlyByWire
{

    enum RightStickMode
    {
        Orientation,
        Translation
    }

    [KSPAddon(KSPAddon.Startup.Flight, true)]
    public class FlyByWire : MonoBehaviour
    {

        private bool m_CallbackSet = false;

        private ControllerWrapper m_Controller = null;

        private float m_TriggerSensitivity = 0.05f;
        private float m_GlobalSensitivityHigh = 0.8f;
        private float m_GlobalSensitivityLow = 0.05f;

        private bool m_InputsLocked = false;
        private RightStickMode m_RightStickMode = RightStickMode.Orientation;

        private float m_Throttle = 0.0f;

        public void Awake()
        {
            print("KSPAdvancedFlyByWire: initialized");
            m_Controller = new ControllerWrapper();
            m_Controller.buttonPressedCallback = new ControllerWrapper.ButtonPressedCallback(ButtonPressedCallback);
        }

        public void OnDestroy()
        {
            print("KSPAdvancedFlyByWire: destroyed");
        }

        void DoMainWindow(int index)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Lock inputs");
            m_InputsLocked = GUILayout.Toggle(m_InputsLocked, "");
            GUILayout.EndHorizontal();
        }

        void ButtonPressedCallback(Button button)
        {
            if(button == Button.X)
            {
                m_InputsLocked = !m_InputsLocked;
            }
            else if(button == Button.Back)
            {
                if(m_RightStickMode == RightStickMode.Orientation)
                {
                    m_RightStickMode = RightStickMode.Orientation;
                }
                else
                {
                    m_RightStickMode = RightStickMode.Translation;
                }
            }
        }

        private void OnFlyByWire(FlightCtrlState state)
        {
            m_Controller.Update();

            if(m_InputsLocked)
            {
                return;
            }

            float sensitivity = m_GlobalSensitivityHigh;
            if(m_Controller.GetButton(Button.RightShoulder))
            {
                sensitivity = m_GlobalSensitivityLow;
            }

            m_Throttle -= m_Controller.GetAnalogInput(AnalogInput.LeftTrigger) * m_TriggerSensitivity * sensitivity;
            m_Throttle += m_Controller.GetAnalogInput(AnalogInput.RightTrigger) * m_TriggerSensitivity * sensitivity;
            m_Throttle = m_Throttle < 0.0f ? 0.0f : m_Throttle > 1.0f ? 1.0f : m_Throttle;
            state.mainThrottle = m_Throttle;

            if(m_RightStickMode == RightStickMode.Orientation)
            {
                state.pitch = m_Controller.GetAnalogInput(AnalogInput.RightStickY) * sensitivity;

                if (m_Controller.GetButton(Button.LeftShoulder))
                {
                    state.roll = m_Controller.GetAnalogInput(AnalogInput.RightStickX) * sensitivity;
                }
                else
                {
                    state.yaw = m_Controller.GetAnalogInput(AnalogInput.RightStickX) * sensitivity;
                }
            }
            else
            {
                state.X = m_Controller.GetAnalogInput(AnalogInput.RightStickX) * sensitivity;

                if (m_Controller.GetButton(Button.LeftShoulder))
                {
                    state.Z = m_Controller.GetAnalogInput(AnalogInput.RightStickY) * sensitivity;
                }
                else
                {
                    state.Y = m_Controller.GetAnalogInput(AnalogInput.RightStickY) * sensitivity;
                }
            }
        }

        private void Update()
        {
            if(!m_CallbackSet && FlightGlobals.ActiveVessel != null)
            {
                FlightGlobals.ActiveVessel.OnFlyByWire += new FlightInputCallback(OnFlyByWire);
                m_CallbackSet = true;
            }
        }

        void OnGUI()
        {
            GUI.Window(0, new Rect(32, 32, 400, 600), DoMainWindow, "Advanced FlyByWire");
        }

    }

}
