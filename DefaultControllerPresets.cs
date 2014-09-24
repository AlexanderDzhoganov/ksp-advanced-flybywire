using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSPAdvancedFlyByWire
{

    class DefaultControllerPresets
    {

        public static List<ControllerPreset> GetDefaultPresets()
        {
            List<ControllerPreset> presets = new List<ControllerPreset>();

            presets.Add(GetDefaultRocketPreset());

            return presets;
        }

        public static ControllerPreset GetDefaultRocketPreset()
        {
            ControllerPreset preset = new ControllerPreset();

            preset.name = "Rocket (default)";

            preset.SetButton(Button.DPadLeft, DiscreteAction.SAS, false);
            preset.SetButton(Button.DPadRight, DiscreteAction.Light, false);
            preset.SetButton(Button.DPadUp, DiscreteAction.RCS, false);
            preset.SetButton(Button.DPadDown, DiscreteAction.Gear, false);

            preset.SetButton(Button.Back, DiscreteAction.Abort, false);
            preset.SetButton(Button.Start, DiscreteAction.QuickSave, false);

            preset.SetButton(Button.LeftShoulder, DiscreteAction.Modifier, false);
            preset.SetButton(Button.RightShoulder, DiscreteAction.NextPreset, false);

 //         preset.SetButton(Button.X, DiscreteAction.SAS, false);
 //         preset.SetButton(Button.Y, DiscreteAction.SAS, false);
            preset.SetButton(Button.A, DiscreteAction.Stage, false);
            preset.SetButton(Button.B, DiscreteAction.CutThrottle, false);

            preset.SetButton(Button.LeftStick, DiscreteAction.NavballToggle, false);
 //         preset.SetButton(Button.RightStick, DiscreteAction.SAS, false);

            preset.SetButton(Button.Guide, DiscreteAction.OrbitMapToggle, false);

            preset.SetAnalogInput(AnalogInput.LeftStickX, ContinuousAction.CameraX, false);
            preset.SetAnalogInput(AnalogInput.LeftStickY, ContinuousAction.CameraY, false);

            preset.SetAnalogInput(AnalogInput.RightStickX, ContinuousAction.Yaw, false);
            preset.SetAnalogInput(AnalogInput.RightStickY, ContinuousAction.Pitch, false);
            preset.SetAnalogInput(AnalogInput.RightStickY, ContinuousAction.Roll, true);

            preset.SetAnalogInput(AnalogInput.LeftTrigger, ContinuousAction.ThrottleDecrement, false);
            preset.SetAnalogInput(AnalogInput.RightTrigger, ContinuousAction.ThrottleIncrement, false);

            return preset;
        }

    }

}
