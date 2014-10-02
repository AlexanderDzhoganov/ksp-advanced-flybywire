using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using UnityEngine;

namespace KSPAdvancedFlyByWire
{

    class Utility
    {

        public float ApplyChangeAndClamp(float x, float change, float clampMin = -1.0f, float clampMax = 1.0f)
        {
            x += change;
            x = Utility.Clamp(x, clampMin, clampMax);
            return x;
        }

        public static float Clamp(float x, float min, float max)
        {
            return x < min ? min : x > max ? max : x;
        }

        public static Rect ClampRect(Rect rect, Rect container)
        {
            Rect item = rect;

            if(item.x < container.x)
            {
                item.x = container.x;
            }
            else if(item.x + item.width >= container.x + container.width)
            {
                item.x = container.x + container.width - item.width;
            }

            if(item.y < container.y)
            {
                item.y = container.y;
            }
            else if(item.y + item.height > container.y + container.height)
            {
                item.y = container.y + container.height - item.height;
            }

            return item;
        }

        [DllImport("kernel32", SetLastError = true)]
        public static extern IntPtr LoadLibrary(string lpFileName);

        public static bool CheckLibrary(string fileName)
        {
            return LoadLibrary(fileName) == IntPtr.Zero;
        }

        private static bool m_XInputExists = false;
        private static bool m_SDLExists = false;

        public static void CheckLibrarySupport()
        {
            m_XInputExists = CheckLibrary("XInputDotNetPure.dll");
            m_SDLExists = CheckLibrary("SDL2.dll");
        }

        public static bool CheckXInputSupport()
        {
            return true;
//            return m_XInputExists;
        }

        public static bool CheckSDLSupport()
        {
            return true;
//            return m_SDLExists;
        }

    }

}
