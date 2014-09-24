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

        public ControllerPreset(IController controller)
        {
            int buttonsCount = controller.GetButtonsCount();
            int axesCount = controller.GetAxesCount();

            discreteActionsMap = new DiscreteAction [buttonsCount * 2];
            continousActionsMap = new ContinuousAction [axesCount * 2];
            discretizedContinousActionsMap = new DiscreteAction [axesCount * 2];

            for (int i = 0; i < buttonsCount * 2; i++)
            {
                discreteActionsMap[i] = DiscreteAction.None;
            }

            for (int i = 0; i < axesCount * 2; i++)
            {
                continousActionsMap[i] = ContinuousAction.None;
                discretizedContinousActionsMap[i] = DiscreteAction.None;
            }
        }

        public string name = "Default preset";
        public DiscreteAction[] discreteActionsMap = null;
        public ContinuousAction[] continousActionsMap = null;
        public DiscreteAction[] discretizedContinousActionsMap = null;

        public void SetButton(int button, DiscreteAction action, bool modifier = false)
        {
            discreteActionsMap[button + (modifier ? 15 : 0)] = action;
        }

        public DiscreteAction GetButton(int button, bool modifier = false)
        {
            return discreteActionsMap[button + (modifier ? 15 : 0)];
        }

        public void SetAnalogInput(int input, ContinuousAction action, bool modifier = false)
        {
            continousActionsMap[input + (modifier ? 6 : 0)] = action;
        }

        public ContinuousAction GetAnalogInput(int input, bool modifier = false)
        {
            return continousActionsMap[input + (modifier ? 6 : 0)];
        }

        public void SetDiscretizedAnalogInput(int input, DiscreteAction action, bool modifier = false)
        {
            discretizedContinousActionsMap[input + (modifier ? 6 : 0)] = action;
        }

        public DiscreteAction GetDiscretizedAnalogInput(int input, bool modifier = false)
        {
            return discretizedContinousActionsMap[input + (modifier ? 6 : 0)];
        }

    }

}
