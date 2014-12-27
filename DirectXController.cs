using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX.DirectInput;

namespace KSPAdvancedFlyByWire
{

    enum DirectXAxis
    {
        X = 0,
        Y = 1,
        Z = 2,
        Rx = 3,
        Ry = 4,
        Rz = 5,
        Ax = 6,
        Ay = 7,
        Az = 8,
        Tx = 9,
        Ty = 10,
        Tz = 11,
        Slider0 = 12,
        Slider1 = 13,
    }

    enum DirectXHatAxes
    {
        Centered = -1,
        Up = 0,
        Right = 9000,
        Down = 18000,
        Left = 27000,
        RightUp = 4500,
        RightDown = 13500,
        LeftUp = 31500,
        LeftDown = 22500
    }

    public class DirectXController : IController
    {

        private int m_AxesCount = 0;
        private int m_ButtonsCount = 0;
        private int m_HatsCount = 0;

        int m_ControllerIndex = 0;

        private Joystick m_Joystick = null;
        private JoystickState m_State = null;

        private static DirectInput m_DirectInput = null;

        private static void InitializeDirectInput()
        {
            if (m_DirectInput != null)
            {
                return;
            }

            m_DirectInput = new DirectInput();
        }

        public DirectXController(int controllerIndex)
        {
            m_ControllerIndex = controllerIndex;

            InitializeDirectInput();

            var devices = m_DirectInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AllDevices);
            if (devices.Count < m_ControllerIndex)
            {
                return;
            }

            m_Joystick = new Joystick(m_DirectInput, devices[m_ControllerIndex].InstanceGuid);
            m_Joystick.Properties.AxisMode = DeviceAxisMode.Absolute;
            m_Joystick.Acquire();

            m_AxesCount = m_Joystick.Capabilities.AxeCount;
            m_ButtonsCount = m_Joystick.Capabilities.ButtonCount;
            m_HatsCount = m_Joystick.Capabilities.PovCount;

            int buttonsCount = m_ButtonsCount + m_HatsCount * 8;
            int axesCount = Enum.GetNames(typeof(DirectXAxis)).Count();

            InitializeStateArrays(buttonsCount, axesCount);

            for (int i = 0; i < buttonsCount; i++)
            {
                buttonStates[i] = false;
            }

            for (int i = 0; i < axesCount; i++)
            {
                axisStates[i].m_NegativeDeadZone = float.MaxValue;
                axisStates[i].m_PositiveDeadZone = float.MaxValue;
                axisStates[i].m_Left = -1.0f;
                axisStates[i].m_Identity = 0.0f;
                axisStates[i].m_Right = 1.0f;
            }
        }

      
        public override string GetControllerName()
        {
            var devices = m_DirectInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AllDevices);
            if (devices.Count < m_ControllerIndex)
            {
                return "";
            }

            return devices[m_ControllerIndex].InstanceName + " (DX)";
        }

        bool IsConnected()
        {
            var devices = m_DirectInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AllDevices);
            if (devices.Count < m_ControllerIndex)
            {
                return false;
            }

            return m_DirectInput.IsDeviceAttached(devices[m_ControllerIndex].InstanceGuid);
        }

        public static List<KeyValuePair<int, string>> EnumerateControllers()
        {
            InitializeDirectInput();
            List<KeyValuePair<int, string>> controllers = new List<KeyValuePair<int, string>>();

            int index = 0;
            foreach (
                DeviceInstance instance in
                    m_DirectInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AllDevices))
            {
                controllers.Add(new KeyValuePair<int, string>(index, instance.InstanceName + " (DX)"));
                index++;
            }

            return controllers;
        }

        public static bool IsControllerConnected(int id)
        {
            var devices = m_DirectInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AllDevices);
            if (devices.Count < id)
            {
                return false;
            }

            return m_DirectInput.IsDeviceAttached(devices[id].InstanceGuid);
        }

        void Deinitialize()
        {
            m_Joystick.Unacquire();
            m_Joystick.Dispose();
            m_Joystick = null;
        }

        public override void Update(FlightCtrlState state)
        {
            try
            {
                m_Joystick.Poll();
                m_State = m_Joystick.GetCurrentState();
            }
            catch (Exception) {}

            base.Update(state);
        }

        public override int GetButtonsCount()
        {
            return m_ButtonsCount + m_HatsCount * 8;
        }

        public override string GetButtonName(int id)
        {
            if (id < m_ButtonsCount)
            {
                return String.Format("Button #{0}", id);
            }
            
            int hatId = (id - m_ButtonsCount) / 8;
            if (hatId < m_HatsCount)
            {
                int buttonId = (id - m_ButtonsCount) % 8;
                return String.Format("Hat #{0} Button {1}", hatId, buttonId);
            }
            
            return "unknown";
        }

        public override int GetAxesCount()
        {
            return m_AxesCount;
        }

        public override string GetAxisName(int id)
        {
            if (id < m_AxesCount)
            {
                return String.Format("Axis #{0}", id);
            }
         
            return "unknown";
        }

        public override bool GetButtonState(int button)
        {
            if (button < m_ButtonsCount)
            {
                return m_State.Buttons[button];
            }

            int hatId = (button - m_ButtonsCount) / 8;
            if (hatId < m_HatsCount)
            {
                int buttonId = (button - m_ButtonsCount) % 8;

                return (DirectXHatAxes)Enum.GetValues(typeof (DirectXHatAxes)).GetValue(buttonId) ==
                       (DirectXHatAxes) m_State.PointOfViewControllers[hatId];
            }

            return false;
        }

        public override float GetRawAxisState(int analogInput)
        {
            if (analogInput < m_AxesCount)
            {
                switch ((DirectXAxis)analogInput)
                {
                    case DirectXAxis.X:
                        return m_State.X;
                    case DirectXAxis.Y:
                        return m_State.Y;
                    case DirectXAxis.Z:
                        return m_State.Z;
                    case DirectXAxis.Rx:
                        return m_State.RotationX;
                    case DirectXAxis.Ry:
                        return m_State.RotationY;
                    case DirectXAxis.Rz:
                        return m_State.RotationZ;
                    case DirectXAxis.Ax:
                        return m_State.AccelerationX;
                    case DirectXAxis.Ay:
                        return m_State.AccelerationY;
                    case DirectXAxis.Az:
                        return m_State.AccelerationZ;
                    case DirectXAxis.Tx:
                        return m_State.TorqueX;
                    case DirectXAxis.Ty:
                        return m_State.TorqueY;
                    case DirectXAxis.Tz:
                        return m_State.TorqueZ;
                    case DirectXAxis.Slider0:
                        return m_State.Sliders[0];
                    case DirectXAxis.Slider1:
                        return m_State.Sliders[1];
                }
            }

            return 0.0f;
        }

    }

}
