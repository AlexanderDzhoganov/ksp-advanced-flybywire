using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSPAdvancedFlyByWire
{

    class KeyboardMouseController : IController
    {

        public override void Update(FlightCtrlState state)
        {
            base.Update(state);
        }

        public override int GetButtonsCount()
        {
            return 0;
        }

        public override string GetButtonName(int id)
        {
            return "";
        }

        public override int GetAxesCount()
        {
            return 0;
        }

        public override string GetAxisName(int id)
        {
            return "";
        }

        public override bool GetButtonState(int button)
        {
            return false;
        }

        public override float GetAnalogInputState(int analogInput)
        {
            return 0.0f;
        }

    }

}
