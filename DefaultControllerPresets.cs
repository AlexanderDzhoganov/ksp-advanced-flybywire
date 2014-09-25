using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            preset.SetDiscreteBinding(new Bitset(buttonsCount, (int)Button.DPadLeft), DiscreteAction.SAS);
            preset.SetDiscreteBinding(new Bitset(buttonsCount, (int)Button.DPadRight), DiscreteAction.Light);
            preset.SetDiscreteBinding(new Bitset(buttonsCount, (int)Button.DPadUp), DiscreteAction.RCS);
            preset.SetDiscreteBinding(new Bitset(buttonsCount, (int)Button.DPadDown), DiscreteAction.Gear);

            preset.SetDiscreteBinding(new Bitset(buttonsCount, (int)Button.Back), DiscreteAction.Abort);
            preset.SetDiscreteBinding(new Bitset(buttonsCount, (int)Button.Start), DiscreteAction.QuickSave);

            preset.SetDiscreteBinding(new Bitset(buttonsCount, (int)Button.X), DiscreteAction.NextPreset);
            preset.SetDiscreteBinding(new Bitset(buttonsCount, (int)Button.Y), DiscreteAction.FullThrottle);
            preset.SetDiscreteBinding(new Bitset(buttonsCount, (int)Button.A), DiscreteAction.Stage);
            preset.SetDiscreteBinding(new Bitset(buttonsCount, (int)Button.B), DiscreteAction.CutThrottle);

            preset.SetDiscreteBinding(new Bitset(buttonsCount, (int)Button.LeftStick), DiscreteAction.CameraViewToggle);
            preset.SetDiscreteBinding(new Bitset(buttonsCount, (int)Button.RightStick), DiscreteAction.NavballToggle);

            preset.SetDiscreteBinding(new Bitset(buttonsCount, (int)Button.Guide), DiscreteAction.OrbitMapToggle);

            preset.SetContinuousBinding((int)AnalogInput.LeftStickX, new Bitset(buttonsCount), ContinuousAction.CameraX);
            preset.SetContinuousBinding((int)AnalogInput.LeftStickY, new Bitset(buttonsCount), ContinuousAction.CameraY);
            preset.SetContinuousBinding((int)AnalogInput.LeftStickYInverted, new Bitset(buttonsCount, (int)Button.LB), ContinuousAction.CameraZoom);

            preset.SetContinuousBinding((int)AnalogInput.RightStickX, new Bitset(buttonsCount), ContinuousAction.Yaw);
            preset.SetContinuousBinding((int)AnalogInput.RightStickY, new Bitset(buttonsCount), ContinuousAction.Pitch);
            preset.SetContinuousBinding((int)AnalogInput.RightStickX, new Bitset(buttonsCount, (int)Button.LB), ContinuousAction.Roll);

            preset.SetContinuousBinding((int)AnalogInput.RightStickX, new Bitset(buttonsCount, (int)Button.RB), ContinuousAction.X);
            preset.SetContinuousBinding((int)AnalogInput.RightStickY, new Bitset(buttonsCount, (int)Button.RB), ContinuousAction.Y);
            preset.SetContinuousBinding((int)AnalogInput.RightStickX, new Bitset(buttonsCount, (int)Button.LB | (int)Button.RB), ContinuousAction.Z);

            preset.SetContinuousBinding((int)AnalogInput.LeftTrigger, new Bitset(buttonsCount), ContinuousAction.ThrottleDecrement);
            preset.SetContinuousBinding((int)AnalogInput.RightTrigger, new Bitset(buttonsCount), ContinuousAction.ThrottleIncrement);

            return preset;
        }

    }

}
