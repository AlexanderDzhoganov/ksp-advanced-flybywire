using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using XInputDotNetPure;

namespace KSPAdvancedFlyByWire
{

    [KSPAddon(KSPAddon.Startup.Flight, true)]
    public class FlyByWire : MonoBehaviour
    {

        public void Awake()
        {
            print("KSPAdvancedFlyByWire: initialized");
        }

        public void OnDestroy()
        {
            print("KSPAdvancedFlyByWire: destroyed");
        }

        void DoMainWindow(int index)
        {
            GamePadState state = GamePad.GetState(PlayerIndex.One);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Lock inputs");
            m_InputsLocked = GUILayout.Toggle(m_InputsLocked, "");
            GUILayout.EndHorizontal();


            GUILayout.Label("debug info:");
            GUILayout.Label(String.Format("IsConnected {0} Packet #{1}", state.IsConnected, state.PacketNumber));
            GUILayout.Label(String.Format("Triggers {0} {1}", state.Triggers.Left, state.Triggers.Right));
            GUILayout.Label(String.Format("D-Pad {0} {1} {2} {3}", state.DPad.Up, state.DPad.Right, state.DPad.Down, state.DPad.Left));
            
            GUILayout.Label(String.Format("Start {0}", state.Buttons.Start));
            GUILayout.Label(String.Format("Back {0}", state.Buttons.Back));
            GUILayout.Label(String.Format("LeftStick {0}", state.Buttons.LeftStick));
            GUILayout.Label(String.Format("RightStick {0}", state.Buttons.RightStick));
            GUILayout.Label(String.Format("LeftShoulder {0}", state.Buttons.LeftShoulder));
            GUILayout.Label(String.Format("RightShoulder {0}", state.Buttons.RightShoulder));
            GUILayout.Label(String.Format("Guide {0}", state.Buttons.Guide));
            GUILayout.Label(String.Format("A {0}", state.Buttons.A));
            GUILayout.Label(String.Format("B {0}", state.Buttons.B));
            GUILayout.Label(String.Format("X {0}", state.Buttons.X));
            GUILayout.Label(String.Format("Y {0}", state.Buttons.Y));

            GUILayout.Label(String.Format("Left stick {0} {1}", state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y));
            GUILayout.Label(String.Format("Right stick {0} {1}", state.ThumbSticks.Right.X, state.ThumbSticks.Right.Y));
            GamePad.SetVibration(PlayerIndex.One, state.Triggers.Left, state.Triggers.Right);
        }

        private bool m_CallbackSet = false;

        private bool m_InputsLocked = false;

        private void OnFlyByWire(FlightCtrlState state)
        {
            GamePadState input = GamePad.GetState(PlayerIndex.One);

            if(input.Buttons.X == ButtonState.Pressed)
            {
                m_InputsLocked = !m_InputsLocked;
            }

            if(m_InputsLocked)
            {
                return;
            }

            state.pitch = input.ThumbSticks.Right.Y;

            if(input.Buttons.LeftShoulder == ButtonState.Pressed)
            {
                state.roll = input.ThumbSticks.Right.X;
            }
            else
            {
                state.yaw = input.ThumbSticks.Right.X;
            }
        }

        private void Update()
        {
            if(!m_CallbackSet && FlightGlobals.ActiveVessel != null)
            {
                FlightGlobals.ActiveVessel.OnFlyByWire += new FlightInputCallback(OnFlyByWire);
                m_CallbackSet = true;
            }
        }

        void OnGUI()
        {
            GUI.Window(0, new Rect(32, 32, 400, 600), DoMainWindow, "Advanced FlyByWire");
        }

    }

}
