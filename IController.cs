using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSPAdvancedFlyByWire
{

    public enum InputWrapper
    {
        XInput = 0,
        SDL = 1,
        KeyboardMouse = 2,
    }

    public abstract class IController
    {

        public delegate void ButtonPressedCallback(IController controller, int button, FlightCtrlState state);
        public delegate void ButtonReleasedCallback(IController controller, int button, FlightCtrlState state);

        public static List<KeyValuePair<InputWrapper, KeyValuePair<int, string>>> EnumerateAllControllers()
        {
            List<KeyValuePair<InputWrapper, KeyValuePair<int, string>>> controllers = new List<KeyValuePair<InputWrapper, KeyValuePair<int, string>>>();

            foreach (var controllerName in XInputController.EnumerateControllers())
            {
                controllers.Add(new KeyValuePair<InputWrapper, KeyValuePair<int, string>>(InputWrapper.XInput, controllerName));
            }

            foreach (var controllerName in SDLController.EnumerateControllers())
            {
                controllers.Add(new KeyValuePair<InputWrapper, KeyValuePair<int, string>>(InputWrapper.SDL, controllerName));
            }

            controllers.Add(new KeyValuePair<InputWrapper, KeyValuePair<int, string>>(InputWrapper.KeyboardMouse, new KeyValuePair<int, string>(0, "Mouse&Keyboard")));
            return controllers;
        }

        public ButtonPressedCallback buttonPressedCallback = null;
        public ButtonReleasedCallback buttonReleasedCallback = null;

        public Curve analogEvaluationCurve = new Curve();

        public bool[] buttonStates;
        public float[] axisPositiveDeadZones;
        public float[] axisNegativeDeadZones;

        public Bitset lastUpdateMask;

        public virtual void Update(FlightCtrlState state)
        {
            for (int i = 0; i < 15; i++)
            {
                if (GetButtonState(i) && !buttonStates[i])
                {
                    buttonStates[i] = true;

                    if (buttonPressedCallback != null)
                    {
                        buttonPressedCallback(this, i, state);
                    }
                }
                else if (!GetButtonState(i) && buttonStates[i])
                {
                    buttonStates[i] = false;

                    if (buttonReleasedCallback != null)
                    {
                        buttonReleasedCallback(this, i, state);
                    }
                }
            }

            lastUpdateMask = GetButtonsMask();
        }

        public abstract int GetButtonsCount();

        public abstract string GetButtonName(int id);

        public abstract int GetAxesCount();

        public abstract string GetAxisName(int id);

        public abstract bool GetButtonState(int button);

        public abstract float GetAnalogInputState(int analogInput);

        public virtual Bitset GetButtonsMask()
        {
            int buttonsCount = GetButtonsCount();
            Bitset mask = new Bitset(buttonsCount);

            for (int i = 0; i < buttonsCount; i++)
            {
                if (GetButtonState(i))
                {
                    mask.Set(i);
                }
            }

            return mask;
        }

        public virtual string ConvertMaskToName(Bitset mask, bool hasAxis = false, int axisIndex = 0)
        {
            string result = "";
            bool isFirst = true;

            for (int i = 0; i < GetButtonsCount(); i++)
            {
                if (mask.Get(i))
                {
                    if (!isFirst)
                    {
                        result += " + ";
                    }
                    else
                    {
                        isFirst = false;
                    }

                    result += GetButtonName(i);
                }
            }

            if (hasAxis)
            {
                if (!isFirst)
                {
                    result += " + ";
                }

                result += GetAxisName(axisIndex);
            }

            return result;
        }

    }

}
