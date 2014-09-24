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
        Modifier,
        
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

        // EVA
        EVAJetpackActivate,
        EVAJetCounterClockwise,
        EVAJetpackClockwise,
        EVAJetPitchPlus,
        EVAJetPitchMinus,
        EVAJump,
        EVAReorientAttitude,
        EVAUseBoard,
        EVADirectionJump,
        EVASprint,
        EVAHeadlamps,

        // Various
        CutThrottle,
        NextPreset,
        PreviousPreset,
        ZoomIn,
        ZoomOut,
        CameraX,
        CameraY,
        OpenDebugConsole,
        OrbitMapToggle,
        ReverseCycleFocusOrbitMap,
        ResetFocusOrbitMap,
        TimeWarpPlus,
        TimeWarpMinus,
        PhysicalTimeWarpPlus,
        PhysicalTimeWarpMinus,
        NavballToggle,
        CycleActiveShipsForward,
        CycleActiveShipsBackward,
        Screenshot,
        QuickSave,
        LoadQuickSave,
        LoadSaveGameStateDialog,
        DebugCheatMenu,
        IVAViewToggle,
        CameraViewToggle,
        SASHold,
        LockStage,
        TogglePrecisionControls,
        ResetTrim,
        PartInfo,
        FuelInfo,
        ScrollStageUp,
        ScrollStageDown,
        UndoAction,
        RedoAction,
        DuplicatePart,
        AngleSnap,
        CycleSymmetry,
        ViewUp,
        ViewDown,
        MoveShip,
        ZoomShipIn,
        ZoomShipOut,
        RotatePartBackwards,
        RotatePartForwards,
        RotatePartCC,
        RotateParkClockwise,
        ResetPartRotation
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
        Throttle,
        ThrottleIncrement,
        ThrottleDecrement,
        ZoomIn,
        ZoomOut,
        CameraX,
        CameraY
    }

    class ControllerPreset
    {

        public ControllerPreset()
        {
            for (int i = 0; i < 30; i++)
            {
                discreteActionsMap[i] = DiscreteAction.None;
            }

            for (int i = 0; i < 12; i++)
            {
                continousActionsMap[i] = ContinuousAction.None;
                discretizedContinousActionsMap[i] = DiscreteAction.None;
            }
        }

        public string name = "Default preset";
        public DiscreteAction[] discreteActionsMap = new DiscreteAction[30];
        public ContinuousAction[] continousActionsMap = new ContinuousAction[12];
        public DiscreteAction[] discretizedContinousActionsMap = new DiscreteAction[12];

        public void SetButton(Button button, DiscreteAction action, bool modifier = false)
        {
            discreteActionsMap[(int)button + (modifier ? 15 : 0)] = action;
        }

        public DiscreteAction GetButton(Button button, bool modifier = false)
        {
            return discreteActionsMap[(int)button + (modifier ? 15 : 0)];
        }

        public void SetAnalogInput(AnalogInput input, ContinuousAction action, bool modifier = false)
        {
            continousActionsMap[(int)input + (modifier ? 6 : 0)] = action;
        }

        public ContinuousAction GetAnalogInput(AnalogInput input, bool modifier = false)
        {
            return continousActionsMap[(int)input + (modifier ? 6 : 0)];
        }

        public void SetDiscretizedAnalogInput(AnalogInput input, DiscreteAction action, bool modifier = false)
        {
            discretizedContinousActionsMap[(int)input + (modifier ? 6 : 0)] = action;
        }

        public DiscreteAction GetDiscretizedAnalogInput(AnalogInput input, bool modifier = false)
        {
            return discretizedContinousActionsMap[(int)input + (modifier ? 6 : 0)];
        }

    }

}
