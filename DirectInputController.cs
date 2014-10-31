using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.DirectInput;
using UnityEngine;
using DeviceType = SharpDX.DirectInput.DeviceType;

namespace KSPAdvancedFlyByWire
{

    class DirectInputController : IController
    {

        private int m_ControllerIndex = 0;
        private static DirectInput directInput;
        private static bool sharpDXInitialized = false;

        private JoystickState m_State = null;
        private Joystick m_Joystick = null;
        private int m_ButtonsCount = 0;
        private int m_AxesCount = 0;

        public DirectInputController(int controllerIndex)
        {
            InitializeSharpDX();

            m_ControllerIndex = controllerIndex;
            
            var devices = directInput.GetDevices();

            int id = 0;
            foreach (var device in devices)
            {
                if (device.Type != DeviceType.Joystick && device.Type != DeviceType.Gamepad)
                {
                    continue;
                }

                if (id != controllerIndex)
                {
                    id++;
                    continue;
                }

                id++;
                m_Joystick = new Joystick(directInput, device.InstanceGuid);
                break;
            }

            m_Joystick.Acquire();

            if (m_Joystick == null)
            {
                return;
            }

            m_ButtonsCount = m_Joystick.GetObjects(DeviceObjectTypeFlags.Button).Count();
            m_AxesCount = m_Joystick.GetObjects(DeviceObjectTypeFlags.Axis).Count();
            InitializeStateArrays(m_ButtonsCount, m_AxesCount);

            for (int i = 0; i < m_ButtonsCount; i++)
            {
                buttonStates[i] = false;
            }

            for (int i = 0; i < m_AxesCount; i++)
            {
                axisStates[i].m_NegativeDeadZone = float.MaxValue;
                axisStates[i].m_PositiveDeadZone = float.MaxValue;
            }

            for (int i = 0; i < m_AxesCount; i++)
            {
                axisStates[i].m_Left = -1.0f;
                axisStates[i].m_Identity = 0.0f;
                axisStates[i].m_Right = 1.0f;
            }

            m_State = m_Joystick.GetCurrentState();
        }

        public static void InitializeSharpDX()
        {
            if (sharpDXInitialized)
            {
                return;
            }

            directInput = new DirectInput();

            sharpDXInitialized = true;
        }

        public override string GetControllerName()
        {
            return m_Joystick.Information.ProductName;                           
        }

        public static List<KeyValuePair<int, string>> EnumerateControllers()
        {
            InitializeSharpDX();

            List<KeyValuePair<int, string>> controllers = new List<KeyValuePair<int, string>>();

            var devices = directInput.GetDevices();

            int id = 0;
            foreach (var device in devices)
            {
                if (device.Type != DeviceType.Joystick && device.Type != DeviceType.Gamepad)
                {
                    continue;
                }

                if (!directInput.IsDeviceAttached(device.InstanceGuid))
                {
                    id++;
                    continue;
                }

                controllers.Add(new KeyValuePair<int, string>(id, String.Format("{0} #{1}", device.ProductName, id)));
                id++;
            }

            return controllers;
        }

        public static bool IsControllerConnected(int id)
        {
            return true;
        }

        public override void Update(FlightCtrlState state)
        {
            m_State = m_Joystick.GetCurrentState();
            base.Update(state);
        }

        public override int GetButtonsCount()
        {
            return m_ButtonsCount;
        }

        public override string GetButtonName(int id)
        {
            return m_Joystick.GetObjects(DeviceObjectTypeFlags.Button)[id].Name;
        }

        public override int GetAxesCount()
        {
            return m_AxesCount;
        }

        public override string GetAxisName(int id)
        {
            return m_Joystick.GetObjects(DeviceObjectTypeFlags.Axis)[id].Name;
        }

        public override bool GetButtonState(int id)
        {
            return m_State.Buttons[id];
        }

        public override float GetRawAxisState(int id)
        {
            switch (m_Joystick.GetObjects(DeviceObjectTypeFlags.Axis)[id].Usage)
            {
            case 48:
                return m_State.X;
            case 49:      
                return m_State.Y;
            case 50:      
                return m_State.Z;
            case 51:      
                return m_State.RotationX;
            case 52:      
                return m_State.RotationY;
            case 53:      
                return m_State.RotationZ;
            case 54:
                return m_State.Sliders[0];
            }

            return 0.0f;
        }

    }

}
