using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSPAdvancedFlyByWire
{

    public class IController
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

        public virtual void Update(FlightCtrlState state) { }

        public virtual int GetButtonsCount() { return 0; }

        public virtual string GetButtonName(int id) { return ""; }

        public virtual int GetAxesCount() { return 0; }

        public virtual string GetAxisName(int id) { return ""; }

        public virtual bool GetButtonState(int button) { return false; }

        public virtual float GetAnalogInputState(int analogInput) { return 0.0f; }

        public virtual bool GetDiscreteAnalogInputState(int analogInput) { return false; }

    }

}
