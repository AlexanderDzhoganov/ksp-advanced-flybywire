using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace KSPAdvancedFlyByWire
{

    class Utility
    {

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

    }

}
