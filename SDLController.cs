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

        private int m_AxesCount = 0;
        private int m_ButtonsCount = 0;
        private IntPtr m_Joystick = IntPtr.Zero;

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
                //  print("couldn't open joystick " + controllerIndex);
                return;
            }

            m_AxesCount = SDL.SDL_JoystickNumAxes(m_Joystick);
            m_ButtonsCount = SDL.SDL_JoystickNumButtons(m_Joystick);
        }

        static bool SDLInitialized = false;

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
            return SDL.SDL_JoystickGetButton(m_Joystick, button) != 0;
        }

        public override float GetAnalogInputState(int analogInput)
        {
            return SDL.SDL_JoystickGetAxis(m_Joystick, analogInput);
        }

    }

}
