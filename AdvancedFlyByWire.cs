using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace KSPAdvancedFlyByWire
{

    [KSPAddon(KSPAddon.Startup.Flight, true)]
    public class AdvancedFlyByWire : MonoBehaviour
    {

        private KSP.IO.PluginConfiguration m_Config;
        private ControllerWrapper m_Controller = null;
        private List<ControllerPreset> m_Presets = new List<ControllerPreset>();
        private ControllerPreset m_CurrentPreset = null;

        private float m_DiscreteActionStep = 0.15f;

        private float m_Throttle = 0.0f;

        private bool m_CallbackSet = false;

        public void Awake()
        {
            print("KSPAdvancedFlyByWire: initialized");

            m_Config = KSP.IO.PluginConfiguration.CreateForType<AdvancedFlyByWire>();
            m_Config.load();

            int presetsCount = m_Config.GetValue<int>("PresetsCount", 0);
            int selectedPreset = m_Config.GetValue<int>("SelectedPreset", 0);

            if(presetsCount == 0)
            {
                m_Config.SetValue("Preset0", new ControllerPreset());
            }

            m_Config.save();

            for (int i = 0; i < presetsCount; i++ )
            {
                m_Presets.Add(m_Config.GetValue<ControllerPreset>("Preset" + i));
            }

            m_CurrentPreset = m_Presets[selectedPreset];

            m_Controller = new ControllerWrapper();
            m_Controller.buttonPressedCallback = new ControllerWrapper.ButtonPressedCallback(ButtonPressedCallback);
            m_CurrentPreset = new ControllerPreset();
        }

        public void OnDestroy()
        {
            print("KSPAdvancedFlyByWire: destroyed");
        }

        void DoMainWindow(int index)
        {
        }

        void ButtonPressedCallback(Button button, FlightCtrlState state)
        {
            EvaluateDiscreteAction(m_CurrentPreset.GetButton(button), state);
        }

        private void OnFlyByWire(FlightCtrlState state)
        {
            FlightGlobals.ActiveVessel.VesselSAS.ManualOverride(true);
            m_Controller.Update(state);
            
            state.mainThrottle = m_Throttle;
            
            for(int i = 0; i < 6; i++)
            {
                AnalogInput input = (AnalogInput)i;
                EvaluateContinuousAction(m_CurrentPreset.GetAnalogInput(input), m_Controller.GetAnalogInput(input), state);
            }

            FlightGlobals.ActiveVessel.VesselSAS.ManualOverride(false);
        }
        
        private void EvaluateDiscreteAction(DiscreteAction action, FlightCtrlState state)
        {
            switch(action)
            {
            case DiscreteAction.None:
                return;
            case DiscreteAction.YawPlus:
                state.yaw += m_DiscreteActionStep;
                return;
            case DiscreteAction.YawMinus:
                state.yaw -= m_DiscreteActionStep;
                return;
            case DiscreteAction.PitchPlus:
                state.pitch += m_DiscreteActionStep;
                return;
            case DiscreteAction.PitchMinus:
                state.pitch -= m_DiscreteActionStep;
                return;
            case DiscreteAction.RollPlus:
                state.roll += m_DiscreteActionStep;
                return;
            case DiscreteAction.RollMinus:
                state.roll -= m_DiscreteActionStep;
                return;
            case DiscreteAction.XPlus:
                state.X += m_DiscreteActionStep;
                return;
            case DiscreteAction.XMinus:
                state.X -= m_DiscreteActionStep;
                return;
            case DiscreteAction.YPlus:
                state.Y += m_DiscreteActionStep;
                return;
            case DiscreteAction.YMinus:
                state.Y -= m_DiscreteActionStep;
                return;
            case DiscreteAction.ZPlus:
                state.Z += m_DiscreteActionStep;
                return;
            case DiscreteAction.ZMinus:
                state.Z -= m_DiscreteActionStep;
                return;
            case DiscreteAction.ThrottlePlus:
                m_Throttle += m_DiscreteActionStep;
                return;
            case DiscreteAction.ThrottleMinus:
                m_Throttle -= m_DiscreteActionStep;
                return;
            case DiscreteAction.Stage:
                FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Stage);
                return;
            case DiscreteAction.Gear:
                FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Gear);
                return;
            case DiscreteAction.Light:
                FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Light);
                return;
            case DiscreteAction.RCS:
                FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.RCS);
                return;
            case DiscreteAction.SAS:
                FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.SAS);
                return;
            case DiscreteAction.Brakes:
                FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Brakes);
                return;
            case DiscreteAction.Abort:
                FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Abort);
                return;
            case DiscreteAction.Custom01:
                FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Custom01);
                return;
            case DiscreteAction.Custom02:
                FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Custom02);
                return;
            case DiscreteAction.Custom03:
                FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Custom03);
                return;
            case DiscreteAction.Custom04:
                FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Custom04);
                return;
            case DiscreteAction.Custom05:
                FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Custom05);
                return;
            case DiscreteAction.Custom06:
                FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Custom06);
                return;
            case DiscreteAction.Custom07:
                FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Custom07);
                return;
            case DiscreteAction.Custom08:
                FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Custom08);
                return;
            case DiscreteAction.Custom09:
                FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Custom09);
                return;
            case DiscreteAction.Custom10:
                FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Custom10);
                return;
            case DiscreteAction.KillThrottle:
                m_Throttle = 0.0f;
                return;
            case DiscreteAction.NextPreset:
                return;
            case DiscreteAction.PreviousPreset:
                return;
            }
        }

        private void EvaluateContinuousAction(ContinuousAction action, float value, FlightCtrlState state)
        {
            switch(action)
            {
                case ContinuousAction.None:
                    return;
                case ContinuousAction.Yaw:
                    state.yaw += value;
                    return;
                case ContinuousAction.Pitch:
                    state.pitch += value;
                    return;
                case ContinuousAction.Roll:
                    state.roll += value;
                    return;
                case ContinuousAction.X:
                    state.X += value;
                    return;
                case ContinuousAction.Y:
                    state.Y += value;
                    return;
                case ContinuousAction.Z:
                    state.Z += value;
                    return;
                case ContinuousAction.Throttle:
                    m_Throttle += value;
                    return;
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
