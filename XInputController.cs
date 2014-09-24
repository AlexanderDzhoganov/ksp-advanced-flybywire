using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using XInputDotNetPure;

namespace KSPAdvancedFlyByWire
{

    public enum Button
    {
        DPadLeft = 1 << 0,
        DPadRight = 1 << 1,
        DPadUp = 1 << 2,
        DPadDown = 1 << 3,
        Back = 1 << 4,
        Start = 1 << 5,
        LB = 1 << 6,
        RB = 1 << 7,
        X = 1 << 8,
        Y = 1 << 9,
        A = 1 << 10,
        B = 1 << 11,
        LeftStick = 1 << 12,
        RightStick = 1 << 13,
        Guide = 1 << 14
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

    public class XInputController : IController
    {

        private GamePadState m_State;
        private PlayerIndex m_ControllerIndex = PlayerIndex.One;

        public bool[] m_ButtonStates = new bool[15];
        public float[] m_AxisPositiveDeadZones = new float[6];
        public float[] m_AxisNegativeDeadZones = new float[6];

        public XInputController()
        {
            for(int i = 0; i < 15; i++)
            {
                m_ButtonStates[i] = false;
            }

            for (int i = 0; i < 6; i++)
            {
                m_AxisNegativeDeadZones[i] = 0.0f;
                m_AxisPositiveDeadZones[i] = 0.0f;
            }
        }

        public override void Update(FlightCtrlState state)
        {
            m_State = GamePad.GetState(m_ControllerIndex);

            for(int i = 0; i < 15; i++)
            {
                if(GetButtonState(i) && !m_ButtonStates[i])
                {
                    m_ButtonStates[i] = true;

                    if(buttonPressedCallback != null)
                    {
                        buttonPressedCallback(i, state);
                    }
                }
                else if(!GetButtonState(i) && m_ButtonStates[i])
                {
                    m_ButtonStates[i] = false;

                    if(buttonReleasedCallback != null)
                    {
                        buttonReleasedCallback(i, state);
                    }
                }
            }
        }

        public override int GetButtonsCount()
        {
            return 15;
        }

        public override string GetButtonName(int id)
        {
            switch ((Button)id)
            {
            case Button.DPadLeft:
                return "Dpad left";
            case Button.DPadRight:
                return "Dpad right";
            case Button.DPadUp:
                return "Dpad up";
            case Button.DPadDown:
                return "Dpad down";
            case Button.Back:
                return "Back";
            case Button.Start:
                return "Start";
            case Button.LB:
                return "Left shoulder";
            case Button.RB:
                return "Right shoulder";
            case Button.X:
                return "X";
            case Button.Y:
                return "Y";
            case Button.A:
                return "A";
            case Button.B:
                return "B";
            case Button.LeftStick:
                return "Left stick";
            case Button.RightStick:
                return "Right stick";
            case Button.Guide:
                return "Guide";
            }

            return "";
        }

        public override int GetAxesCount()
        {
            return 6;
        }

        public override string GetAxisName(int id)
        {
            switch ((AnalogInput)id)
            {
                case AnalogInput.LeftStickX:
                    return "Left stick X";
                case AnalogInput.LeftStickY:
                    return "Left stick Y";
                case AnalogInput.RightStickX:
                    return "Right stick X";
                case AnalogInput.RightStickY:
                    return "Right stick Y";
                case AnalogInput.LeftTrigger:
                    return "Left trigger";
                case AnalogInput.RightTrigger:
                    return "Right trigger";
            }

            return "";
        }

        public override bool GetButtonState(int button)
        {
            switch ((Button)(1 << button))
            {
                case Button.DPadLeft:
                    return m_State.DPad.Left == ButtonState.Pressed;
                case Button.DPadRight:
                    return m_State.DPad.Right == ButtonState.Pressed;
                case Button.DPadUp:
                    return m_State.DPad.Up == ButtonState.Pressed;
                case Button.DPadDown:
                    return m_State.DPad.Down == ButtonState.Pressed;
                case Button.Back:
                    return m_State.Buttons.Back == ButtonState.Pressed;
                case Button.Start:
                    return m_State.Buttons.Start == ButtonState.Pressed;
                case Button.LB:
                    return m_State.Buttons.LeftShoulder == ButtonState.Pressed;
                case Button.RB:
                    return m_State.Buttons.RightShoulder == ButtonState.Pressed;
                case Button.X:
                    return m_State.Buttons.X == ButtonState.Pressed;
                case Button.Y:
                    return m_State.Buttons.Y == ButtonState.Pressed;
                case Button.A:
                    return m_State.Buttons.A == ButtonState.Pressed;
                case Button.B:
                    return m_State.Buttons.B == ButtonState.Pressed;
                case Button.LeftStick:
                    return m_State.Buttons.LeftStick == ButtonState.Pressed;
                case Button.RightStick:
                    return m_State.Buttons.RightStick == ButtonState.Pressed;
                case Button.Guide:
                    return m_State.Buttons.Guide == ButtonState.Pressed;
            }

            return false;
        }

        public override float GetAnalogInputState(int input)
        {
            float value = 0.0f;

            switch ((AnalogInput)input)
            {
                case AnalogInput.LeftStickX:
                    value = m_State.ThumbSticks.Left.X;
                    break;
                case AnalogInput.LeftStickY:
                    value = m_State.ThumbSticks.Left.Y;
                    break;
                case AnalogInput.RightStickX:
                    value = m_State.ThumbSticks.Right.X;
                    break;
                case AnalogInput.RightStickY:
                    value = m_State.ThumbSticks.Right.Y;
                    break;
                case AnalogInput.LeftTrigger:
                    value = m_State.Triggers.Left;
                    break;
                case AnalogInput.RightTrigger:
                    value = m_State.Triggers.Right;
                    break;
            }

            if (value > 0.0f)
            {
                if(value < m_AxisPositiveDeadZones[input])
                {
                    m_AxisPositiveDeadZones[input] = value;
                }

                float deadZone = m_AxisPositiveDeadZones[input];
                value = (value - m_AxisPositiveDeadZones[input]) * (1.0f + deadZone);
            }
            else
            {
                if(Math.Abs(value) < m_AxisNegativeDeadZones[input])
                {
                    m_AxisNegativeDeadZones[input] = Math.Abs(value);
                }

                float deadZone = m_AxisPositiveDeadZones[input];
                value = (Math.Abs(value) - m_AxisPositiveDeadZones[input]) * (1.0f + deadZone);
                value *= -1.0f;
            }

            return Math.Sign(value) * analogInputEvaluationCurve.Evaluate(Math.Abs(value));
        }

        public override bool GetDiscreteAnalogInputState(int input)
        {
            float value = 0.0f;

            switch ((AnalogInput)input)
            {
                case AnalogInput.LeftStickX:
                    value = m_State.ThumbSticks.Left.X;
                    break;
                case AnalogInput.LeftStickY:
                    value = m_State.ThumbSticks.Left.Y;
                    break;
                case AnalogInput.RightStickX:
                    value = m_State.ThumbSticks.Right.X;
                    break;
                case AnalogInput.RightStickY:
                    value = m_State.ThumbSticks.Right.Y;
                    break;
                case AnalogInput.LeftTrigger:
                    value = m_State.Triggers.Left;
                    break;
                case AnalogInput.RightTrigger:
                    value = m_State.Triggers.Right;
                    break;
            }

            return Math.Abs(value) >= analogDiscretizationCutoff;
        }

        public override int GetButtonsMask()
        {
            int mask = 0;

            for (int i = 0; i < 15; i++)
            {
                if(GetButtonState(i))
                {
                    mask |= 1 << i;
                }
            }

            return mask;
        }

    }

}
