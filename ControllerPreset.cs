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
        ZoomIn,
        ZoomOut,

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
        public DiscreteAction[] discretizedContinousActionsMap = new DiscreteAction[6];

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

        public void SetDiscretizedAnalogInput(AnalogInput input, DiscreteAction action)
        {
            discretizedContinousActionsMap[(int)input] = action;
        }

        public DiscreteAction GetDiscretizedAnalogInput(AnalogInput input)
        {
            return discretizedContinousActionsMap[(int)input];
        }

    }

}
