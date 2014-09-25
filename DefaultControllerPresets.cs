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
            ControllerPreset preset = new ControllerPreset(controller);

            preset.name = "XInput Default";

            preset.SetDiscreteBinding((int)Button.DPadLeft, DiscreteAction.SAS);
            preset.SetDiscreteBinding((int)Button.DPadRight, DiscreteAction.Light);
            preset.SetDiscreteBinding((int)Button.DPadUp, DiscreteAction.RCS);
            preset.SetDiscreteBinding((int)Button.DPadDown, DiscreteAction.Gear);

            preset.SetDiscreteBinding((int)Button.Back, DiscreteAction.Abort);
            preset.SetDiscreteBinding((int)Button.Start, DiscreteAction.QuickSave);

            preset.SetDiscreteBinding((int)Button.X, DiscreteAction.NextPreset);
            preset.SetDiscreteBinding((int)Button.Y, DiscreteAction.FullThrottle);
            preset.SetDiscreteBinding((int)Button.A, DiscreteAction.Stage);
            preset.SetDiscreteBinding((int)Button.B, DiscreteAction.CutThrottle);

            preset.SetDiscreteBinding((int)Button.LeftStick, DiscreteAction.CameraViewToggle);
            preset.SetDiscreteBinding((int)Button.RightStick, DiscreteAction.NavballToggle);

            preset.SetDiscreteBinding((int)Button.Guide, DiscreteAction.OrbitMapToggle);

            preset.SetContinuousBinding((int)AnalogInput.LeftStickX, 0, ContinuousAction.CameraX);
            preset.SetContinuousBinding((int)AnalogInput.LeftStickY, 0, ContinuousAction.CameraY);
            preset.SetContinuousBinding((int)AnalogInput.LeftStickYInverted, (int)Button.LB, ContinuousAction.CameraZoom);

            preset.SetContinuousBinding((int)AnalogInput.RightStickX, 0, ContinuousAction.Yaw);
            preset.SetContinuousBinding((int)AnalogInput.RightStickY, 0, ContinuousAction.Pitch);
            preset.SetContinuousBinding((int)AnalogInput.RightStickX, (int)Button.LB, ContinuousAction.Roll);

            preset.SetContinuousBinding((int)AnalogInput.RightStickX, (int)Button.RB, ContinuousAction.X);
            preset.SetContinuousBinding((int)AnalogInput.RightStickY, (int)Button.RB, ContinuousAction.Y);
            preset.SetContinuousBinding((int)AnalogInput.RightStickX, (int)Button.LB | (int)Button.RB, ContinuousAction.Z);

            preset.SetContinuousBinding((int)AnalogInput.LeftTrigger, 0, ContinuousAction.ThrottleDecrement);
            preset.SetContinuousBinding((int)AnalogInput.RightTrigger, 0, ContinuousAction.ThrottleIncrement);

            return preset;
        }

    }

}
