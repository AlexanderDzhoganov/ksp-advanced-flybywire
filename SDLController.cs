using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using SDL2;

namespace KSPAdvancedFlyByWire
{

    public class SDLController : IController
    {

        private bool m_Initialized = false;
        private int m_AxesCount = 0;
        private int m_ButtonsCount = 0;

        private IntPtr m_Joystick = IntPtr.Zero;
        int m_ControllerIndex = 0;

        static bool SDLInitialized = false;

        public SDLController(int controllerIndex)
        {
            if (!SDLInitialized)
            {
                SDL2.SDL.SDL_Init(SDL2.SDL.SDL_INIT_JOYSTICK);
                SDLInitialized = true;
            }

            m_ControllerIndex = controllerIndex;
            
            m_Joystick = SDL2.SDL.SDL_JoystickOpen(controllerIndex);

            if (m_Joystick == IntPtr.Zero)
            {
                return;
            }

            m_AxesCount = SDL2.SDL.SDL_JoystickNumAxes(m_Joystick);
            m_ButtonsCount = SDL2.SDL.SDL_JoystickNumButtons(m_Joystick);

            buttonStates = new bool[m_ButtonsCount];
            axisPositiveDeadZones = new float[m_AxesCount];
            axisNegativeDeadZones = new float[m_AxesCount];

            for (int i = 0; i < m_ButtonsCount; i++)
            {
                buttonStates[i] = false;
            }

            for (int i = 0; i < m_AxesCount; i++)
            {
                axisNegativeDeadZones[i] = 0.0f;
                axisPositiveDeadZones[i] = 0.0f;
            }

            m_Initialized = true;
        }

        public override string GetControllerName()
        {
            if (m_Joystick == IntPtr.Zero)
            {
                return "Uninitialized";
            }

            return SDL2.SDL.SDL_JoystickName(m_Joystick);
        }

        bool IsConnected()
        {
            return m_Initialized;
        }

        public static List<KeyValuePair<int, string>> EnumerateControllers()
        {
            if (!SDLInitialized)
            {
                SDL2.SDL.SDL_Init(SDL2.SDL.SDL_INIT_JOYSTICK);
                SDLInitialized = true;
            }

            List<KeyValuePair<int, string>> controllers = new List<KeyValuePair<int, string>>();

            int numControllers = SDL2.SDL.SDL_NumJoysticks();
            for (int i = 0; i < numControllers; i++)
            {
                controllers.Add(new KeyValuePair<int, string>(i, SDL2.SDL.SDL_JoystickNameForIndex(i)));
            }

            return controllers;
        }

        public static bool IsControllerConnected(int id)
        {
            if (!SDLInitialized)
            {
                SDL2.SDL.SDL_Init(SDL2.SDL.SDL_INIT_JOYSTICK);
                SDLInitialized = true;
            }

            var joystick = SDL2.SDL.SDL_JoystickOpen(id);

            if (joystick == IntPtr.Zero)
            {
                return false;
            }

            SDL2.SDL.SDL_JoystickClose(joystick);

            return true;
        }

        void Deinitialize()
        {
            if (SDLInitialized)
            {
                if (m_Joystick != IntPtr.Zero)
                {
                    SDL2.SDL.SDL_JoystickClose(m_Joystick);
                }

                SDL2.SDL.SDL_Quit();
            }
        }

        public override void Update(FlightCtrlState state)
        {
            SDL2.SDL.SDL_JoystickUpdate();
            base.Update(state);
        }

        public override int GetButtonsCount()
        {
            return m_ButtonsCount;
        }

        public override string GetButtonName(int id)
        {
            return String.Format("Button {0}", id);
        }

        public override int GetAxesCount()
        {
            return m_AxesCount;
        }

        public override string GetAxisName(int id)
        {
            return String.Format("Axis {0}", id);
        }

        public override bool GetButtonState(int button)
        {
            return SDL2.SDL.SDL_JoystickGetButton(m_Joystick, button) != 0;
        }

        public override float GetAxisState(int analogInput)
        {
            float sign = 1.0f;
            if (analogInput >= m_AxesCount)
            {
                analogInput -= m_AxesCount;
                sign = -1.0f;
            }

            float value = SDL2.SDL.SDL_JoystickGetAxis(m_Joystick, analogInput) * sign;

            if (value > 0.0f)
            {
                if (value < axisPositiveDeadZones[analogInput])
                {
                    axisPositiveDeadZones[analogInput] = value;
                }

                float deadZone = axisPositiveDeadZones[analogInput];
                value = (value - axisPositiveDeadZones[analogInput]) * (1.0f + deadZone);
            }
            else
            {
                if (Math.Abs(value) < axisNegativeDeadZones[analogInput])
                {
                    axisNegativeDeadZones[analogInput] = Math.Abs(value);
                }

                float deadZone = axisPositiveDeadZones[analogInput];
                value = (Math.Abs(value) - axisPositiveDeadZones[analogInput]) * (1.0f + deadZone);
                value *= -1.0f;
            }

            return Math.Sign(value) * analogEvaluationCurve.Evaluate(Math.Abs(value));
        }

    }

}
