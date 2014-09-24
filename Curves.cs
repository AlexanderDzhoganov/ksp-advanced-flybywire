using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSPAdvancedFlyByWire
{

    public class Curve
    {
        public virtual float Evaluate(float x)
        {
            return x;
        }
    }

    public class XSquared : Curve
    {
        public override float Evaluate(float x)
        {
            return x * x;
        }
    }

    public class SqrtX : Curve
    {
        public override float Evaluate(float x)
        {
            return (float)Math.Sqrt(x);
        }
    }

    public class SqrtOfOneMinusXSquared : Curve
    {
        public override float Evaluate(float x)
        {
            return (float)Math.Sqrt(1.0 - x * x);
        }
    }

}
