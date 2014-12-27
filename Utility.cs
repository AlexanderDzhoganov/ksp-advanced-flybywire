using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using UnityEngine;

namespace KSPAdvancedFlyByWire
{

    class Utility
    {

        public static bool RectContainsMouse(Rect windowRect)
        {
            return windowRect.Contains(GetMousePosition());
        }

        public static Vector2 GetMousePosition()
        {
            return new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
        }

        public static float ApplyChangeAndClamp(float x, float change, float clampMin = -1.0f, float clampMax = 1.0f)
        {
            x += change;
            x = Clamp(x, clampMin, clampMax);
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

        public static bool CheckXInputSupport()
        {
#if LINUX
            return false;
#endif

            return true;
        }

        public static bool CheckSDLSupport()
        {
            return true;
        }

        public static bool CheckSharpDXSupport()
        {
#if LINUX
            return false;
#endif

            return true;
        }

    }

}
