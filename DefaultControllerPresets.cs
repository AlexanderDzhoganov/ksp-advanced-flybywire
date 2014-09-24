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

            preset.SetButton((int)Button.DPadLeft, DiscreteAction.SAS, false);
            preset.SetButton((int)Button.DPadRight, DiscreteAction.Light, false);
            preset.SetButton((int)Button.DPadUp, DiscreteAction.RCS, false);
            preset.SetButton((int)Button.DPadDown, DiscreteAction.Gear, false);

            preset.SetButton((int)Button.Back, DiscreteAction.Abort, false);
            preset.SetButton((int)Button.Start, DiscreteAction.QuickSave, false);

            preset.SetButton((int)Button.LeftShoulder, DiscreteAction.Modifier, false);
            preset.SetButton((int)Button.RightShoulder, DiscreteAction.NextPreset, false);

            //         preset.SetButton((int)Button.X, DiscreteAction.SAS, false);
            //         preset.SetButton((int)Button.Y, DiscreteAction.SAS, false);
            preset.SetButton((int)Button.A, DiscreteAction.Stage, false);
            preset.SetButton((int)Button.B, DiscreteAction.CutThrottle, false);

            preset.SetButton((int)Button.LeftStick, DiscreteAction.NavballToggle, false);
            //         preset.SetButton((int)Button.RightStick, DiscreteAction.SAS, false);

            preset.SetButton((int)Button.Guide, DiscreteAction.OrbitMapToggle, false);

            preset.SetAnalogInput((int)AnalogInput.LeftStickX, ContinuousAction.CameraX, false);
            preset.SetAnalogInput((int)AnalogInput.LeftStickY, ContinuousAction.CameraY, false);

            preset.SetAnalogInput((int)AnalogInput.RightStickX, ContinuousAction.Yaw, false);
            preset.SetAnalogInput((int)AnalogInput.RightStickY, ContinuousAction.Pitch, false);
            preset.SetAnalogInput((int)AnalogInput.RightStickY, ContinuousAction.Roll, true);

            preset.SetAnalogInput((int)AnalogInput.LeftTrigger, ContinuousAction.ThrottleDecrement, false);
            preset.SetAnalogInput((int)AnalogInput.RightTrigger, ContinuousAction.ThrottleIncrement, false);

            return preset;
        }

    }

}
