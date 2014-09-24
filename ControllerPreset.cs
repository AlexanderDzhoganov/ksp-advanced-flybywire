using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

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
        FullThrottle,
        
        NextPreset,
        PreviousPreset,
        
        // Camera
        CameraXPlus,
        CameraXMinus,
        CameraYPlus,
        CameraYMinus,
        CameraZoomPlus,
        CameraZoomMinus,

        OrbitMapToggle,
        TimeWarpPlus,
        TimeWarpMinus,
        NavballToggle,
        Screenshot,
        QuickSave,
        IVAViewToggle,
        CameraViewToggle,
        SASHold,
        LockStage,
        TogglePrecisionControls,
        ResetTrim,
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
        CameraX,
        CameraY,
        CameraZoom
    }

    class ControllerPreset
    {

        public ControllerPreset(IController controller)
        {
            int buttonsCount = controller.GetButtonsCount();
            int axesCount = controller.GetAxesCount();
        }

        public string name = "Default preset";

        public List<KeyValuePair<int, DiscreteAction>> discreteActionsMap = new List<KeyValuePair<int, DiscreteAction>>();
        public Dictionary<int, List<KeyValuePair<int, ContinuousAction>>> continuousActionsMap = new Dictionary<int, List<KeyValuePair<int, ContinuousAction>>>();

        public void SetDiscreteBinding(int state, DiscreteAction action)
        {
            discreteActionsMap.Add(new KeyValuePair<int,DiscreteAction>(state, action));
        }

        public DiscreteAction GetDiscreteBinding(int state)
        {
            foreach (var maskActionPair in discreteActionsMap)
            {
                int expectedState = maskActionPair.Key;

                bool match = true;

                for (int i = 0; i < 32; i++)
                {
                    if (((1 << i) & expectedState) != 0)
                    {
                        if (((1 << i) & state) == 0)
                        {
                            match = false;
                            break;
                        }
                    }
                }

                if(match)
                {
                    return maskActionPair.Value;
                }
            }

            return DiscreteAction.None;
        }

        public void SetContinuousBinding(int axis, int state, ContinuousAction action)
        {
            if(!continuousActionsMap.ContainsKey(axis))
            {
                continuousActionsMap[axis] = new List<KeyValuePair<int, ContinuousAction>>();
            }

            continuousActionsMap[axis].Add(new KeyValuePair<int, ContinuousAction>(state, action));
        }

        public List<ContinuousAction> GetContinuousBinding(int axis, int state)
        {
            if (!continuousActionsMap.ContainsKey(axis))
            {
                return null;
            }

            List<KeyValuePair<int, ContinuousAction>> matches = new List<KeyValuePair<int, ContinuousAction>>();

            foreach (var stateActionPair in continuousActionsMap[axis])
            {
                int expectedState = stateActionPair.Key;
                bool match = true;
                    
                for (int i = 0; i < 32; i++)
                {
                    if (((1 << i) & expectedState) != 0)
                    {
                        if (((1 << i) & state) == 0)
                        {
                            match = false;
                            break;
                        }
                    }
                }
                    
                if(match)
                {
                    matches.Add(stateActionPair);
                }
            }

            for (int i = 0; i < matches.Count; i++)
            {
                int bits = 0;
                for(int q = 0; q < 32; q++)
                {
                    if((matches[i].Key & (1 << q)) != 0)
                    {
                        bits++;
                    }
                }

                var value = matches[i].Value;
                matches[i] = new KeyValuePair<int, ContinuousAction>(bits, value);
            }

            if(matches.Count == 0)
            {
                return null;
            }

            matches.Sort(CompareKeys);

            int minBits = matches[matches.Count - 1].Key;
            List<ContinuousAction> actions = new List<ContinuousAction>();

            for (var i = 0; i < matches.Count; i++)
            {
                if(matches[i].Key >= minBits)
                {
                    actions.Add(matches[i].Value);
                }
            }
            
            return actions;
        }

        private static int CompareKeys(KeyValuePair<int, ContinuousAction> a, KeyValuePair<int, ContinuousAction> b)
        {
            return a.Key.CompareTo(b.Key);
        }

    }

}
