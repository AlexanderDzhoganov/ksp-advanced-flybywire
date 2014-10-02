using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using SDL2;

namespace KSPAdvancedFlyByWire
{

    enum HatAxes
    {
        Centered = 0,
        Up = 1,
        Right = 2,
        Down = 3,
        Left = 4,
        RightUp = 5, 
        RightDown = 6,
        LeftUp = 7, 
        LeftDown = 8
    }

    public class SDLController : IController
    {

        private int m_AxesCount = 0;
        private int m_ButtonsCount = 0;
        private int m_HatsCount = 0;

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
            m_HatsCount = SDL2.SDL.SDL_JoystickNumHats(m_Joystick);

            if (treatHatsAsButtons)
            {
                InitializeStateArrays(m_ButtonsCount + m_HatsCount * 8, m_AxesCount);
            }
            else
            {
                InitializeStateArrays(m_ButtonsCount, m_AxesCount + m_HatsCount * 2);
            }

            for (int i = 0; i < m_ButtonsCount; i++)
            {
                buttonStates[i] = false;
            }

            if (treatHatsAsButtons)
            {
                for (int i = m_ButtonsCount; i < m_ButtonsCount + m_HatsCount * 8; i++)
                {
                    buttonStates[i] = false;
                }
            }

            for (int i = 0; i < m_AxesCount; i++)
            {
                axisNegativeDeadZones[i] = float.MaxValue;
                axisPositiveDeadZones[i] = float.MaxValue;
                axisLeft[i] = -1.0f;
                axisIdentity[i] = 0.0f;
                axisRight[i] = 1.0f;
            }

            if (!treatHatsAsButtons)
            {
                for (int i = m_AxesCount; i < m_AxesCount + m_HatsCount * 2; i++)
                {
                    axisNegativeDeadZones[i] = float.MaxValue;
                    axisPositiveDeadZones[i] = float.MaxValue;
                    axisLeft[i] = -1.0f;
                    axisIdentity[i] = 0.0f;
                    axisRight[i] = 1.0f;
                }
            }
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
            return SDL2.SDL.SDL_JoystickGetAttached(m_Joystick) == SDL.SDL_bool.SDL_TRUE;
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
            if (treatHatsAsButtons)
            {
                return m_ButtonsCount + m_HatsCount * 8;
            }

            return m_ButtonsCount;
        }

        public override string GetButtonName(int id)
        {
            if (id < m_ButtonsCount)
            {
                return String.Format("Button #{0}", id);
            }
            else if(treatHatsAsButtons)
            {
                int hatId = (id - m_ButtonsCount) / 8;
                int buttonId = (id - m_ButtonsCount) % 8;
                return String.Format("Hat #{0} Button {1}", hatId, buttonId);
            }

            return "unknown";
        }

        public override int GetAxesCount()
        {
            if (treatHatsAsButtons)
            {
                return m_AxesCount;
            }

            return m_AxesCount + m_HatsCount * 2;
        }

        public override string GetAxisName(int id)
        {
            if (id < m_AxesCount)
            {
                return String.Format("Axis #{0}", id);
            }
            else if(!treatHatsAsButtons)
            {
                bool xOrY = ((id - m_AxesCount) % 2) == 0;
                return String.Format("Hat #{0} {1}", id - m_AxesCount, xOrY ? "X" : "Y");
            }

            return "unknown";
        }

        public override bool GetButtonState(int button)
        {
            if (button < m_ButtonsCount)
            {
                return SDL2.SDL.SDL_JoystickGetButton(m_Joystick, button) != 0;
            }
            else if(treatHatsAsButtons)
            {
                int hatId = (button - m_ButtonsCount) / 8;
                int buttonId = (button - m_ButtonsCount) % 8;
                return SDL2.SDL.SDL_JoystickGetHat(m_Joystick, hatId) == buttonId;
            }

            return false;
        }

        public override float GetRawAxisState(int analogInput)
        {
            if (analogInput < m_AxesCount)
            {
                return SDL2.SDL.SDL_JoystickGetAxis(m_Joystick, analogInput);
            }
            else if(!treatHatsAsButtons)
            {
                int hatId = (analogInput - m_AxesCount) / 2;
                int axisId = (analogInput - m_AxesCount) % 2;
                HatAxes state = (HatAxes) SDL2.SDL.SDL_JoystickGetHat(m_Joystick, hatId);

                if (axisId == 0)
                {
                    switch (state)
                    {
                        case HatAxes.Left:
                            return -1.0f;
                        case HatAxes.LeftDown:
                            return -1.0f;
                        case HatAxes.LeftUp:
                            return -1.0f;
                        case HatAxes.Right:
                            return 1.0f;
                        case HatAxes.RightDown:
                            return 1.0f;
                        case HatAxes.RightUp:
                            return 1.0f;
                        default:
                            return 0.0f;
                    }
                }
                else
                {
                    switch (state)
                    {
                        case HatAxes.Up:
                            return 1.0f;
                        case HatAxes.LeftDown:
                            return -1.0f;
                        case HatAxes.LeftUp:
                            return 1.0f;
                        case HatAxes.Down:
                            return -1.0f;
                        case HatAxes.RightDown:
                            return -1.0f;
                        case HatAxes.RightUp:
                            return 1.0f;
                        default:
                            return 0.0f;
                    }
                }
            }

            return 0.0f;
        }

    }

}
