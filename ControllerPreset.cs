using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

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
        YawTrim,
        Pitch,
        PitchTrim,
        Roll,
        RollTrim,
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

    [Serializable]
    public class ControllerPreset
    {

        public ControllerPreset()
        {
        }

        public string name = "Default preset";

        public delegate void OnCustomActionCallback();

        public List<KeyValuePair<Bitset, DiscreteAction>> discreteActionsMap = new List<KeyValuePair<Bitset, DiscreteAction>>();

        [XmlIgnore()]
        public Dictionary<int, List<KeyValuePair<Bitset, ContinuousAction>>> continuousActionsMap = new Dictionary<int, List<KeyValuePair<Bitset, ContinuousAction>>>();

        public List<KeyValuePair<int, List<KeyValuePair<Bitset, ContinuousAction>>>> serialiazableContinuousActionMap = new List<KeyValuePair<int, List<KeyValuePair<Bitset, ContinuousAction>>>>();

        [XmlIgnore()]
        public List<KeyValuePair<Bitset, OnCustomActionCallback>> customActionsMap = new List<KeyValuePair<Bitset, OnCustomActionCallback>>();

        public void OnPreSerialize()
        {
            serialiazableContinuousActionMap.Clear();

            foreach(var keyValue in continuousActionsMap)
            {
                serialiazableContinuousActionMap.Add(keyValue);
            }
        }

        public void OnPostDeserialize()
        {
            continuousActionsMap.Clear();

            foreach(var keyValue in serialiazableContinuousActionMap)
            {
                continuousActionsMap.Add(keyValue.Key, keyValue.Value);
            }
        }

        public void SetDiscreteBinding(Bitset state, DiscreteAction action)
        {
            discreteActionsMap.Add(new KeyValuePair<Bitset, DiscreteAction>(state, action));
        }

        public DiscreteAction GetDiscreteBinding(Bitset state)
        {
            foreach (var maskActionPair in discreteActionsMap)
            {
                Bitset expectedState = maskActionPair.Key;
                bool match = true;

                for (int i = 0; i < 32; i++)
                {
                    if (expectedState.Get(i))
                    {
                        if (!state.Get(i))
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

        public void SetContinuousBinding(int axis, Bitset state, ContinuousAction action)
        {
            if(!continuousActionsMap.ContainsKey(axis))
            {
                continuousActionsMap[axis] = new List<KeyValuePair<Bitset, ContinuousAction>>();
            }

            continuousActionsMap[axis].Add(new KeyValuePair<Bitset, ContinuousAction>(state, action));
        }

        public List<ContinuousAction> GetContinuousBinding(int axis, Bitset state)
        {
            if (!continuousActionsMap.ContainsKey(axis))
            {
                return null;
            }

            List<KeyValuePair<Bitset, ContinuousAction>> matches = new List<KeyValuePair<Bitset, ContinuousAction>>();

            foreach (var stateActionPair in continuousActionsMap[axis])
            {
                Bitset expectedState = stateActionPair.Key;
                bool match = true;
                    
                for (int i = 0; i < 32; i++)
                {
                    if (expectedState.Get(i))
                    {
                        if (!state.Get(i))
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

            List<KeyValuePair<int, ContinuousAction>> matchResults = new List<KeyValuePair<int, ContinuousAction>>();
            for (int i = 0; i < matches.Count; i++)
            {
                int bits = 0;
                for(int q = 0; q < 32; q++)
                {
                    if(matches[i].Key.Get(q))
                    {
                        bits++;
                    }
                }

                var value = matches[i].Value;
                matchResults.Add(new KeyValuePair<int, ContinuousAction>(bits, value));
            }

            if(matches.Count == 0)
            {
                return null;
            }

            matchResults.Sort(CompareKeys);

            int minBits = matchResults[matches.Count - 1].Key;
            List<ContinuousAction> actions = new List<ContinuousAction>();

            for (var i = 0; i < matches.Count; i++)
            {
                if (matchResults[i].Key >= minBits)
                {
                    actions.Add(matches[i].Value);
                }
            }
            
            return actions;
        }

        public void SetCustomBinding(Bitset state, OnCustomActionCallback action)
        {
            customActionsMap.Add(new KeyValuePair<Bitset, OnCustomActionCallback>(state, action));
        }

        public OnCustomActionCallback GetCustomBinding(Bitset state)
        {
            foreach (var maskActionPair in customActionsMap)
            {
                Bitset expectedState = maskActionPair.Key;
                bool match = true;

                for (int i = 0; i < 32; i++)
                {
                    if (expectedState.Get(i))
                    {
                        if (!state.Get(i))
                        {
                            match = false;
                            break;
                        }
                    }
                }

                if (match)
                {
                    return maskActionPair.Value;
                }
            }

            return null;
        }

        private static int CompareKeys(KeyValuePair<int, ContinuousAction> a, KeyValuePair<int, ContinuousAction> b)
        {
            return a.Key.CompareTo(b.Key);
        }

    }

}
