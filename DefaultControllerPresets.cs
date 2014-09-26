using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace KSPAdvancedFlyByWire
{

    class DefaultControllerPresets
    {

        public static List<ControllerPreset> GetDefaultPresets(IController controller)
        {
            List<ControllerPreset> presets = new List<ControllerPreset>();

            if (controller is XInputController)
            {
                presets.Add(GetXInputDefaultRocketPreset(controller));
            }

            return presets;
        }

        public static ControllerPreset GetXInputDefaultRocketPreset(IController controller)
        {
            ControllerPreset preset = new ControllerPreset();
            preset.name = "XInput Default";

            int buttonsCount = controller.GetButtonsCount();
            preset.SetDiscreteBinding(new Bitset(buttonsCount, (int)XInput.Button.DPadLeft), DiscreteAction.SAS);
            preset.SetDiscreteBinding(new Bitset(buttonsCount, (int)XInput.Button.DPadRight), DiscreteAction.Light);
            preset.SetDiscreteBinding(new Bitset(buttonsCount, (int)XInput.Button.DPadUp), DiscreteAction.RCS);
            preset.SetDiscreteBinding(new Bitset(buttonsCount, (int)XInput.Button.DPadDown), DiscreteAction.Gear);

            preset.SetDiscreteBinding(new Bitset(buttonsCount, (int)XInput.Button.Back), DiscreteAction.Abort);
            preset.SetDiscreteBinding(new Bitset(buttonsCount, (int)XInput.Button.Start), DiscreteAction.QuickSave);

            preset.SetDiscreteBinding(new Bitset(buttonsCount, (int)XInput.Button.X), DiscreteAction.NextPreset);
            preset.SetDiscreteBinding(new Bitset(buttonsCount, (int)XInput.Button.Y), DiscreteAction.FullThrottle);
            preset.SetDiscreteBinding(new Bitset(buttonsCount, (int)XInput.Button.A), DiscreteAction.Stage);
            preset.SetDiscreteBinding(new Bitset(buttonsCount, (int)XInput.Button.B), DiscreteAction.CutThrottle);

            preset.SetDiscreteBinding(new Bitset(buttonsCount, (int)XInput.Button.LeftStick), DiscreteAction.CameraViewToggle);
            preset.SetDiscreteBinding(new Bitset(buttonsCount, (int)XInput.Button.RightStick), DiscreteAction.NavballToggle);

            preset.SetDiscreteBinding(new Bitset(buttonsCount, (int)XInput.Button.Guide), DiscreteAction.OrbitMapToggle);

            preset.SetContinuousBinding((int)XInput.AnalogInput.LeftStickX, new Bitset(buttonsCount), ContinuousAction.CameraX);
            preset.SetContinuousBinding((int)XInput.AnalogInput.LeftStickY, new Bitset(buttonsCount), ContinuousAction.CameraY);
            preset.SetContinuousBinding((int)XInput.AnalogInput.LeftStickYInverted, new Bitset(buttonsCount, (int)XInput.Button.LB), ContinuousAction.CameraZoom);

            preset.SetContinuousBinding((int)XInput.AnalogInput.RightStickX, new Bitset(buttonsCount), ContinuousAction.Yaw);
            preset.SetContinuousBinding((int)XInput.AnalogInput.RightStickY, new Bitset(buttonsCount), ContinuousAction.Pitch);
            preset.SetContinuousBinding((int)XInput.AnalogInput.RightStickX, new Bitset(buttonsCount, (int)XInput.Button.LB), ContinuousAction.Roll);

            preset.SetContinuousBinding((int)XInput.AnalogInput.RightStickX, new Bitset(buttonsCount, (int)XInput.Button.RB), ContinuousAction.X);
            preset.SetContinuousBinding((int)XInput.AnalogInput.RightStickY, new Bitset(buttonsCount, (int)XInput.Button.RB), ContinuousAction.Y);
            preset.SetContinuousBinding((int)XInput.AnalogInput.RightStickX, new Bitset(buttonsCount, (int)XInput.Button.LB | (int)XInput.Button.RB), ContinuousAction.Z);

            preset.SetContinuousBinding((int)XInput.AnalogInput.LeftTrigger, new Bitset(buttonsCount), ContinuousAction.ThrottleDecrement);
            preset.SetContinuousBinding((int)XInput.AnalogInput.RightTrigger, new Bitset(buttonsCount), ContinuousAction.ThrottleIncrement);

            return preset;
        }

    }

}
