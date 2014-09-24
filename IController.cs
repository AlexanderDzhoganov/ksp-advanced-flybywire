using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSPAdvancedFlyByWire
{

    public enum Button
    {
        DPadLeft = 0,
        DPadRight = 1,
        DPadUp = 2,
        DPadDown = 3,
        Back = 4,
        Start = 5,
        LeftShoulder = 6,
        RightShoulder = 7,
        X = 8,
        Y = 9,
        A = 10,
        B = 11,
        LeftStick = 12,
        RightStick = 13,
        Guide = 14
    }

    public enum AnalogInput
    {
        LeftStickX = 0,
        LeftStickY = 1,
        RightStickX = 2,
        RightStickY = 3,
        LeftTrigger = 4,
        RightTrigger = 5
    }

    public class IController
    {

        public delegate void ButtonPressedCallback(Button button, FlightCtrlState state);
        public delegate void ButtonReleasedCallback(Button button, FlightCtrlState state);
        public delegate void DiscretizedAnalogInputPressedCallback(AnalogInput input, FlightCtrlState state);
        public delegate void DiscretizedAnalogInputReleasedCallback(AnalogInput input, FlightCtrlState state);

        public ButtonPressedCallback buttonPressedCallback = null;
        public ButtonReleasedCallback buttonReleasedCallback = null;
        public DiscretizedAnalogInputPressedCallback discretizedAnalogInputPressedCallback = null;
        public DiscretizedAnalogInputReleasedCallback discretizedAnalogInputReleasedCallback = null;

        public Curve analogInputEvaluationCurve = new Curve();
        public float analogDiscretizationCutoff = 0.8f;

        public virtual void Update(FlightCtrlState state) { }

        public virtual bool GetButton(Button button) { return false; }

        public virtual float GetAnalogInput(AnalogInput input) { return 0.0f; }

        public virtual bool GetDiscreteAnalogInput(AnalogInput input, float cutoff = 0.5f) { return false; }

    }

}
