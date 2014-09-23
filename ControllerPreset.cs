using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSPAdvancedFlyByWire
{

    public enum DiscreteAction
    {
        None,
        // yaw, pitch, roll, x, y, z, throttle
        YawPlus,
        YawMinus,
        PitchPlus,
        PitchMinus,
        RollPlus,
        RollMinus,
        XPlus,
        XMinus,
        YPlus,
        YMinus,
        ZPlus,
        ZMinus,
        ThrottlePlus,
        ThrottleMinus,
        // Action groups
        Stage,
        Gear,
        Light,
        RCS,
        SAS,
        Brakes,
        Abort,
        Custom01,
        Custom02,
        Custom03,
        Custom04,
        Custom05,
        Custom06,
        Custom07,
        Custom08,
        Custom09,
        Custom10,
        // 
        KillThrottle,
        NextPreset,
        PreviousPreset
    }
    public enum ContinuousAction
    {
        None,
        Yaw,
        Pitch,
        Roll,
        X,
        Y,
        Z,
        Throttle
    }

    class ControllerPreset
    {

        public ControllerPreset()
        {
            discreteActionsMap[(int)Button.Start] = DiscreteAction.SAS;
        }

        public string name = "Default preset";
        public DiscreteAction[] discreteActionsMap = new DiscreteAction[15];
        public ContinuousAction[] continousActionsMap = new ContinuousAction[6];

        public void SetButton(Button button, DiscreteAction action)
        {
            discreteActionsMap[(int)button] = action;
        }

        public DiscreteAction GetButton(Button button)
        {
            return discreteActionsMap[(int)button];
        }

        public void SetAnalogInput(AnalogInput input, ContinuousAction action)
        {
            continousActionsMap[(int)input] = action;
        }

        public ContinuousAction GetAnalogInput(AnalogInput input)
        {
            return continousActionsMap[(int)input];
        }

    }

}
