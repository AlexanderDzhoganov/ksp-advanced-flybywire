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

    public class AxisConfiguration
    {
        public float m_PositiveDeadZone = 0.0f;
        public float m_NegativeDeadZone = 0.0f;
        public float m_Left = 0.0f;
        public float m_Identity = 0.0f;
        public float m_Right = 0.0f;
        public bool m_Invert = false;

        public float Rescale(float value, Curve analogEvaluationCurve)
        {
            if (value < m_Left)
            {
                m_Left = value;
            }
            else if (value > m_Right)
            {
                m_Right = value;
            }

            float eps = 1e-4f;

            if (value + eps > m_Identity && value - eps < m_Identity)
            {
                value = 0.0f;
            }
            else if (value < m_Identity)
            {
                value = (value - m_Identity) / (m_Identity - m_Left);
            }
            else if (value > m_Identity)
            {
                value = (value - m_Identity) / (m_Right - m_Identity);
            }

            if (Math.Abs(value) < 1e-4)
            {
                value = 0.0f;
            }

            if (value > 0.0f)
            {
                if (value < m_PositiveDeadZone)
                {
                    m_PositiveDeadZone = value;
                }

                float deadZone = m_PositiveDeadZone;
                float t = (1.0f - deadZone);
                if (t != 0.0f)
                {
                    value = (value - deadZone) / t;
                }
            }
            else if (value < 0.0f)
            {
                if (Math.Abs(value) < m_NegativeDeadZone)
                {
                    m_NegativeDeadZone = Math.Abs(value);
                }

                float deadZone = m_NegativeDeadZone;
                float t = (1.0f - deadZone);
                if (t != 0.0f)
                {
                    value = -1.0f * (Math.Abs(value) - deadZone) / t;
                }
            }

            return (m_Invert ? -1.0f : 1.0f) * Math.Sign(value) * analogEvaluationCurve.Evaluate(Math.Abs(value));
        }

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
        public AxisConfiguration[] axisStates;

        public Bitset lastUpdateMask;

        public void InitializeStateArrays(int buttons, int axes)
        {
            buttonStates = new bool[buttons];
            axisStates = new AxisConfiguration[axes];
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
            return axisStates[analogInput].Rescale(GetRawAxisState(analogInput), analogEvaluationCurve);
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

    }

}
