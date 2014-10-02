using System;
using System.Collections.Generic;

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

            if (Utility.CheckXInputSupport())
            {
                foreach (var controllerName in XInputController.EnumerateControllers())
                {
                    controllers.Add(new KeyValuePair<InputWrapper, KeyValuePair<int, string>>(InputWrapper.XInput, controllerName));
                }
            }

            if (Utility.CheckSDLSupport())
            {
                foreach (var controllerName in SDLController.EnumerateControllers())
                {
                    controllers.Add(new KeyValuePair<InputWrapper, KeyValuePair<int, string>>(InputWrapper.SDL, controllerName));
                }
            }

            controllers.Add(new KeyValuePair<InputWrapper, KeyValuePair<int, string>>(InputWrapper.KeyboardMouse, new KeyValuePair<int, string>(0, "Mouse&Keyboard")));
            return controllers;
        }

        public ButtonPressedCallback buttonPressedCallback = null;
        public ButtonReleasedCallback buttonReleasedCallback = null;

        public Curve analogEvaluationCurve = new Curve();
        public bool treatHatsAsButtons = false;

        public bool[] buttonStates;
        public float[] axisPositiveDeadZones;
        public float[] axisNegativeDeadZones;
        
        public float[] axisLeft;
        public float[] axisIdentity;
        public float[] axisRight;

        public bool[] axisInvert;

        public Bitset lastUpdateMask;

        public void InitializeStateArrays(int buttons, int axes)
        {
            buttonStates = new bool[buttons];
            axisPositiveDeadZones = new float[axes];
            axisNegativeDeadZones = new float[axes];

            axisLeft = new float[axes];
            axisIdentity = new float[axes];
            axisRight = new float[axes];

            axisInvert = new bool[axes];
        }

        public abstract string GetControllerName();

        public virtual void Update(FlightCtrlState state)
        {
            for (int i = 0; i < GetButtonsCount(); i++)
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

        public float GetAxisState(int analogInput)
        {
            return RescaleAxis(analogInput, GetRawAxisState(analogInput));
        }

        public abstract float GetRawAxisState(int analogInput);

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

        public float RescaleAxis(int input, float value)
        {
            if (value < axisLeft[input])
            {
                axisLeft[input] = value;
            }
            else if (value > axisRight[input])
            {
                axisRight[input] = value;
            }

            float eps = 1e-4f;

            if(value + eps > axisIdentity[input] && value - eps < axisIdentity[input])
            {
                value = 0.0f;
            }
            else if(value < axisIdentity[input])
            {
                value = (value - axisIdentity[input]) / (axisIdentity[input] - axisLeft[input]);
            }
            else if(value > axisIdentity[input])
            {
                value = (value - axisIdentity[input]) / (axisRight[input] - axisIdentity[input]);
            }

            if (Math.Abs(value) < 1e-4)
            {
                value = 0.0f;
            }

            if (value > 0.0f)
            {
                if (value < axisPositiveDeadZones[input])
                {
                    axisPositiveDeadZones[input] = value;
                }

                float deadZone = axisPositiveDeadZones[input];
                float t = (1.0f - deadZone);
                if(t != 0.0f)
                {
                    value = (value - deadZone) / t;
                }
            }
            else if(value < 0.0f)
            {
                if (Math.Abs(value) < axisNegativeDeadZones[input])
                {
                    axisNegativeDeadZones[input] = Math.Abs(value);
                }

                float deadZone = axisNegativeDeadZones[input];
                float t = (1.0f - deadZone);
                if (t != 0.0f)
                {
                    value = -1.0f * (Math.Abs(value) - deadZone) / t;
                }
            }

            return (axisInvert[input] ? -1.0f : 1.0f) * Math.Sign(value) * analogEvaluationCurve.Evaluate(Math.Abs(value));
        }

    }

}
