using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSPAdvancedFlyByWire
{

    class Utility
    {

        public static float Clamp(float x, float min, float max)
        {
            return x < min ? min : x > max ? max : x;
        }

    }

}
