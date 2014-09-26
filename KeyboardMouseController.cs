using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace KSPAdvancedFlyByWire
{

    class KeyboardMouseController : IController
    {
        private float m_MouseDeltaX = 0.0f;
        private float m_MouseDeltaY = 0.0f;

        private Dictionary<int, string> m_NameLookupTable = new Dictionary<int, string>();

        public KeyboardMouseController()
        {
             buttonStates = new bool[GetButtonsCount()];
             axisPositiveDeadZones = new float[GetAxesCount()];
             axisNegativeDeadZones = new float[GetAxesCount()];

            for (int i = 0; i < 10; i++)
            {
                m_NameLookupTable.Add(i, i.ToString());
            }
            m_NameLookupTable.Add(10, "a");
            m_NameLookupTable.Add(11, "b");
            m_NameLookupTable.Add(12, "c");
            m_NameLookupTable.Add(13, "d");
            m_NameLookupTable.Add(14, "e");
            m_NameLookupTable.Add(15, "f");
            m_NameLookupTable.Add(16, "g");
            m_NameLookupTable.Add(17, "h");
            m_NameLookupTable.Add(18, "i");
            m_NameLookupTable.Add(19, "j");
            m_NameLookupTable.Add(20, "k");
            m_NameLookupTable.Add(21, "l");
            m_NameLookupTable.Add(22, "m");
            m_NameLookupTable.Add(23, "n");
            m_NameLookupTable.Add(24, "o");
            m_NameLookupTable.Add(25, "p");
            m_NameLookupTable.Add(26, "q");
            m_NameLookupTable.Add(27, "r");
            m_NameLookupTable.Add(28, "s");
            m_NameLookupTable.Add(29, "t");
            m_NameLookupTable.Add(30, "u");
            m_NameLookupTable.Add(31, "v");
            m_NameLookupTable.Add(32, "w");
            m_NameLookupTable.Add(33, "x");
            m_NameLookupTable.Add(34, "y");
            m_NameLookupTable.Add(35, "z");
            m_NameLookupTable.Add(36, "up");
            m_NameLookupTable.Add(37, "down");
            m_NameLookupTable.Add(38, "left");
            m_NameLookupTable.Add(39, "right");
            m_NameLookupTable.Add(40, "f1");
            m_NameLookupTable.Add(41, "f2");
            m_NameLookupTable.Add(42, "f3");
            m_NameLookupTable.Add(43, "f4");
            m_NameLookupTable.Add(44, "f5");
            m_NameLookupTable.Add(45, "f6");
            m_NameLookupTable.Add(46, "f7");
            m_NameLookupTable.Add(47, "f8");
            m_NameLookupTable.Add(48, "f9");
            m_NameLookupTable.Add(49, "f10");
            m_NameLookupTable.Add(50, "f11");
            m_NameLookupTable.Add(51, "f12");
            m_NameLookupTable.Add(52, "[1]");
            m_NameLookupTable.Add(53, "[2]");
            m_NameLookupTable.Add(54, "[3]");
            m_NameLookupTable.Add(55, "[4]");
            m_NameLookupTable.Add(56, "[5]");
            m_NameLookupTable.Add(57, "[6]");
            m_NameLookupTable.Add(58, "[7]");
            m_NameLookupTable.Add(59, "[8]");
            m_NameLookupTable.Add(60, "[9]");
            m_NameLookupTable.Add(61, "[0]");
            m_NameLookupTable.Add(62, "[+]");
            m_NameLookupTable.Add(64, "enter");
            m_NameLookupTable.Add(65, "escape");
            m_NameLookupTable.Add(66, "left shift");
            m_NameLookupTable.Add(67, "right shift");
            m_NameLookupTable.Add(68, "right ctrl");
            m_NameLookupTable.Add(69, "left ctrl");
            m_NameLookupTable.Add(70, "right alt");
            m_NameLookupTable.Add(71, "left alt");
        }

        public override string GetControllerName()
        {
            return "Keyboard/ Mouse";
        }

        public override void Update(FlightCtrlState state)
        {
            m_MouseDeltaX = Input.GetAxis("Mouse X") / Screen.width;
            m_MouseDeltaY = Input.GetAxis("Mouse Y") / Screen.height;
            base.Update(state);

        }

        public override int GetButtonsCount()// chosen by a fair dice
        {
            return 104;
        }

        public override string GetButtonName(int id)
        {
            if (!m_NameLookupTable.ContainsKey(id)) 
            {
                return "unknown"; 
            }

            return m_NameLookupTable[id];
        }

        public override int GetAxesCount()
        {
            return 2;
        }

        public override string GetAxisName(int id) // 0 is X, 1 is Y 
        {
            switch(id)
            {
                case 0:
                    return "Mouse X";
                case 1:
                    return "Mouse Y";

                default:
                    return "";
            }
        }

        public override bool GetButtonState(int button)
        {
            var name = GetButtonName(button);
            if (name == "unknown") return false;

            return Input.GetKey(name);
        }

        public override float GetAxisState(int analogInput) // 0 is X, 1 is Y 
        {
            switch (analogInput)
            {
                case 0:
                    return m_MouseDeltaX;
                    
                case 1:
                    return m_MouseDeltaY;

                default:
                    break;
            }

            return 0.0f;
        }

    }

}
