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
        RightTrigger = 5,
        LeftStickXInverted = 6,
        LeftStickYInverted = 7,
        RightStickXInverted = 8,
        RightStickYInverted = 9,
        LeftTriggerInverted = 10,
        RightTriggerInverted = 11
    }

    public class XInputController : IController
    {

        private GamePadState m_State;
        private PlayerIndex m_ControllerIndex = PlayerIndex.One;

        public XInputController(int controllerIndex)
        {
            m_ControllerIndex = (PlayerIndex)controllerIndex;

            buttonStates = new bool[15];
            axisPositiveDeadZones = new float[12];
            axisNegativeDeadZones = new float[12];

            for(int i = 0; i < 15; i++)
            {
                buttonStates[i] = false;
            }

            for (int i = 0; i < 12; i++)
            {
                axisNegativeDeadZones[i] = 0.0f;
                axisPositiveDeadZones[i] = 0.0f;
            }
        }

        public static List<KeyValuePair<int, string>> EnumerateControllers()
        {
            List<KeyValuePair<int, string>> controllers = new List<KeyValuePair<int, string>>();

            for (int i = 0; i < 4; i++)
            {
                if (GamePad.GetState((PlayerIndex)i).IsConnected)
                {
                    controllers.Add(new KeyValuePair<int, string>(i, String.Format("XInput Controller #{0}", i)));
                }
            }

            return controllers;
        }

        public static bool IsControllerConnected(int id)
        {
            switch (id)
            {
                case 0:
                    return GamePad.GetState(PlayerIndex.One).IsConnected;
                case 1:
                    return GamePad.GetState(PlayerIndex.Two).IsConnected;
                case 2:
                    return GamePad.GetState(PlayerIndex.Three).IsConnected;
                case 3:
                    return GamePad.GetState(PlayerIndex.Four).IsConnected;
            }

            return false;
        }

        public override void Update(FlightCtrlState state)
        {
            m_State = GamePad.GetState(m_ControllerIndex);
            base.Update(state);
        }

        public override int GetButtonsCount()
        {
            return 15;
        }

        public override string GetButtonName(int id)
        {
            switch ((Button)(1 << id))
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
            default:
                return "bad index - " + id.ToString();
            }
        }

        public override int GetAxesCount()
        {
            return 12;
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
                case AnalogInput.LeftStickXInverted:
                    return "Left stick X Inverted";
                case AnalogInput.LeftStickYInverted:
                    return "Left stick Y Inverted";
                case AnalogInput.RightStickXInverted:
                    return "Right stick X Inverted";
                case AnalogInput.RightStickYInverted:
                    return "Right stick Y Inverted";
                case AnalogInput.LeftTriggerInverted:
                    return "Left trigger Inverted";
                case AnalogInput.RightTriggerInverted:
                    return "Right trigger Inverted";
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
                case AnalogInput.LeftStickXInverted:
                    value = -m_State.ThumbSticks.Left.X;
                    break;
                case AnalogInput.LeftStickYInverted:
                    value = -m_State.ThumbSticks.Left.Y;
                    break;
                case AnalogInput.RightStickXInverted:
                    value = -m_State.ThumbSticks.Right.X;
                    break;
                case AnalogInput.RightStickYInverted:
                    value = -m_State.ThumbSticks.Right.Y;
                    break;
                case AnalogInput.LeftTriggerInverted:
                    value = -m_State.Triggers.Left;
                    break;
                case AnalogInput.RightTriggerInverted:
                    value = -m_State.Triggers.Right;
                    break;
            }

            if (value > 0.0f)
            {
                if(value < axisPositiveDeadZones[input])
                {
                    axisPositiveDeadZones[input] = value;
                }

                float deadZone = axisPositiveDeadZones[input];
                value = (value - axisPositiveDeadZones[input]) * (1.0f + deadZone);
            }
            else
            {
                if(Math.Abs(value) < axisNegativeDeadZones[input])
                {
                    axisNegativeDeadZones[input] = Math.Abs(value);
                }

                float deadZone = axisPositiveDeadZones[input];
                value = (Math.Abs(value) - axisPositiveDeadZones[input]) * (1.0f + deadZone);
                value *= -1.0f;
            }

            return Math.Sign(value) * analogEvaluationCurve.Evaluate(Math.Abs(value));
        }

    }

}
