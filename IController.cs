using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSPAdvancedFlyByWire
{

    public abstract class IController
    {

        public delegate void ButtonPressedCallback(int button, FlightCtrlState state);
        public delegate void ButtonReleasedCallback(int button, FlightCtrlState state);
        public delegate void DiscretizedAnalogInputPressedCallback(int input, FlightCtrlState state);
        public delegate void DiscretizedAnalogInputReleasedCallback(int input, FlightCtrlState state);

        public ButtonPressedCallback buttonPressedCallback = null;
        public ButtonReleasedCallback buttonReleasedCallback = null;
        public DiscretizedAnalogInputPressedCallback discretizedAnalogInputPressedCallback = null;
        public DiscretizedAnalogInputReleasedCallback discretizedAnalogInputReleasedCallback = null;

        public Curve analogInputEvaluationCurve = new Curve();
        public float analogDiscretizationCutoff = 0.8f;

        public abstract void Update(FlightCtrlState state);

        public abstract int GetButtonsCount();

        public abstract string GetButtonName(int id);

        public abstract int GetAxesCount();

        public abstract string GetAxisName(int id);

        public abstract bool GetButtonState(int button);

        public abstract float GetAnalogInputState(int analogInput);

        public abstract bool GetDiscreteAnalogInputState(int analogInput);

        public abstract int GetButtonsMask();

    }

}
