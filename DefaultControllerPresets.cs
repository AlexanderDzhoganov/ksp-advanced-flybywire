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

            preset.name = "Rocket (default)";

            preset.SetDiscreteBinding((int)Button.DPadLeft, DiscreteAction.SAS);
            preset.SetDiscreteBinding((int)Button.DPadRight, DiscreteAction.Light);
            preset.SetDiscreteBinding((int)Button.DPadUp, DiscreteAction.RCS);
            preset.SetDiscreteBinding((int)Button.DPadDown, DiscreteAction.Gear);

            preset.SetDiscreteBinding((int)Button.Back, DiscreteAction.Abort);
            preset.SetDiscreteBinding((int)Button.Start, DiscreteAction.QuickSave);

           // preset.SetDiscreteBinding((int)Button.LB, DiscreteAction.PreviousPreset);
            preset.SetDiscreteBinding((int)Button.RB, DiscreteAction.NextPreset);

            //         preset.SetButton((int)Button.X, DiscreteAction.SAS, false);
            //         preset.SetButton((int)Button.Y, DiscreteAction.SAS, false);
            preset.SetDiscreteBinding((int)Button.A, DiscreteAction.Stage);
            preset.SetDiscreteBinding((int)Button.B, DiscreteAction.CutThrottle);

            preset.SetDiscreteBinding((int)Button.LeftStick, DiscreteAction.NavballToggle);
            //         preset.SetButton((int)Button.RightStick, DiscreteAction.SAS, false);

            preset.SetDiscreteBinding((int)Button.Guide, DiscreteAction.OrbitMapToggle);

            preset.SetContinuousBinding((int)AnalogInput.LeftStickX, 0, ContinuousAction.CameraX);
            preset.SetContinuousBinding((int)AnalogInput.LeftStickY, 0, ContinuousAction.CameraY);

            preset.SetContinuousBinding((int)AnalogInput.RightStickX, 0, ContinuousAction.Yaw);
            preset.SetContinuousBinding((int)AnalogInput.RightStickY, 0, ContinuousAction.Pitch);
            preset.SetContinuousBinding((int)AnalogInput.RightStickX, (int)Button.LB, ContinuousAction.Roll);

            preset.SetContinuousBinding((int)AnalogInput.LeftTrigger, 0, ContinuousAction.ThrottleDecrement);
            preset.SetContinuousBinding((int)AnalogInput.RightTrigger, 0, ContinuousAction.ThrottleIncrement);

            return preset;
        }

    }

}
