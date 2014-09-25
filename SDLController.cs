using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using SDL2;

namespace KSPAdvancedFlyByWire
{

    class SDLController : IController
    {

        private bool m_Initialized = false;
        private int m_AxesCount = 0;
        private int m_ButtonsCount = 0;

        private IntPtr m_Joystick = IntPtr.Zero;

        static bool SDLInitialized = false;

        SDLController(int controllerIndex)
        {
            if (!SDLInitialized)
            {
                SDL.SDL_Init(SDL.SDL_INIT_JOYSTICK);
                SDLInitialized = true;
            }
            
            m_Joystick = SDL.SDL_JoystickOpen(controllerIndex);

            if (m_Joystick == IntPtr.Zero || SDL.SDL_JoystickOpened(controllerIndex) == 0)
            {
                return;
            }

            m_AxesCount = SDL.SDL_JoystickNumAxes(m_Joystick);
            m_ButtonsCount = SDL.SDL_JoystickNumButtons(m_Joystick);

            buttonStates = new bool[m_ButtonsCount];
            axisPositiveDeadZones = new float[m_AxesCount * 2];
            axisNegativeDeadZones = new float[m_AxesCount * 2];

            for (int i = 0; i < m_ButtonsCount; i++)
            {
                buttonStates[i] = false;
            }

            for (int i = 0; i < m_AxesCount * 2; i++)
            {
                axisNegativeDeadZones[i] = 0.0f;
                axisPositiveDeadZones[i] = 0.0f;
            }

            m_Initialized = true;
        }

        bool IsConnected()
        {
            return m_Initialized;
        }

        static int EnumerateControllers()
        {
            if (!SDLInitialized)
            {
                SDL.SDL_Init(SDL.SDL_INIT_JOYSTICK);
                SDLInitialized = true;
            }
            
            int numControllers = SDL.SDL_NumJoysticks();
            SDLInitialized = true;
            return numControllers;
        }

        static bool IsControllerConnected(int id)
        {
            if (!SDLInitialized)
            {
                SDL.SDL_Init(SDL.SDL_INIT_JOYSTICK);
                SDLInitialized = true;
            }

            var joystick = SDL.SDL_JoystickOpen(id);

            if (joystick == IntPtr.Zero || SDL.SDL_JoystickOpened(id) == 0)
            {
                return false;
            }

            SDL.SDL_JoystickClose(joystick);

            return true;
        }

        void Deinitialize()
        {
            if (SDLInitialized)
            {
                if (m_Joystick != IntPtr.Zero)
                {
                    SDL.SDL_JoystickClose(m_Joystick);
                }

                SDL.SDL_Quit();
            }
        }

        public override void Update(FlightCtrlState state)
        {
            SDL.SDL_JoystickUpdate();
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
            return m_AxesCount * 2;
        }

        public override string GetAxisName(int id)
        {
            if (id >= m_AxesCount)
            {
                return String.Format("Axis {0} Inverted", id - m_AxesCount);
            }

            return String.Format("Axis {0}", id);
        }

        public override bool GetButtonState(int button)
        {
            return SDL.SDL_JoystickGetButton(m_Joystick, button) != 0;
        }

        public override float GetAnalogInputState(int analogInput)
        {
            float sign = 1.0f;
            if (analogInput >= m_AxesCount)
            {
                analogInput -= m_AxesCount;
                sign = -1.0f;
            }

            float value = SDL.SDL_JoystickGetAxis(m_Joystick, analogInput) * sign;

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
