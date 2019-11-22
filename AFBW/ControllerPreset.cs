using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using UnityEngine;

namespace KSPAdvancedFlyByWire
{

    using DiscreteActionsMap = Dictionary<DiscreteAction, Bitset>;
    using ContinuousActionsMap = Dictionary<ContinuousAction, KeyValuePair<Bitset, int>>;
    using ContinuousActionsInversionMap = Dictionary<ContinuousAction, bool>;

    using SerializableDiscreteActionsMap = List<KeyValuePair<DiscreteAction, Bitset>>;
    using SerializableContinuousActionsMap = List<KeyValuePair<ContinuousAction, KeyValuePair<Bitset, int>>>;
    using SerializableContinuousActionsInvMap = List<KeyValuePair<ContinuousAction, bool>>;

    public enum DiscreteAction
    {
        None,

        NextPreset,
        PreviousPreset,
        CyclePresets,
        // yaw, pitch, roll, x, y, z, throttle

        YawPlus,
        YawMinus,
        PitchPlus,
        PitchMinus,
        PitchTrimPlus,
        PitchTrimMinus,
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
        SASHold,
        SASStabilityAssist,
        SASPrograde,
        SASRetrograde,
        SASNormal,
        SASAntinormal,
        SASRadialIn,
        SASRadialOut,
        SASManeuver,
        SASTarget,
        SASAntiTarget,
        SASManeuverOrTarget,
        Brakes,
        BrakesHold,
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
        LockStage,

        // EVA
        EVAToggleJetpack,
        EVAInteract,
        EVAJump,
        EVAPlantFlag,
        EVAAutoRunToggle,

        // Various
        CutThrottle,
        FullThrottle,

        // Camera
        CameraXPlus,
        CameraXMinus,
        CameraYPlus,
        CameraYMinus,
        CameraZoomPlus,
        CameraZoomMinus,

        TimeWarpPlus,
        TimeWarpMinus,
        PhysicsTimeWarpPlus,
        PhysicsTimeWarpMinus,
        StopWarp,

        OrbitMapToggle,
        IVAViewToggle,
        CameraViewToggle,
        NavballToggle,

        IVANextCamera,
        IVALookWindow, // has issues, disabled for now
        TogglePrecisionControls,
        ResetTrim,
    }

    public enum ContinuousAction
    {
        None,
        Yaw,
        NegativeYaw,
        YawTrim,
        Pitch,
        NegativePitch,
        PitchTrim,
        Roll,
        NegativeRoll,
        RollTrim,
        X,
        NegativeX,
        Y,
        NegativeY,
        Z,
        NegativeZ,
        Throttle,
        ThrottleIncrement,
        ThrottleDecrement,
        WheelSteer,
        WheelSteerTrim,
        WheelThrottle,
        WheelThrottleTrim,
        CameraX,
        CameraY,
        CameraZoom
    }

    public class ControllerPreset
    {

        public string name = "New preset";

        public delegate void OnCustomActionCallback();

        [XmlIgnore()]
        public DiscreteActionsMap discreteActionsMap = new DiscreteActionsMap();
        [XmlIgnore()]
        public ContinuousActionsMap continuousActionsMap = new ContinuousActionsMap();
        [XmlIgnore()]
        public ContinuousActionsInversionMap continuousActionsInvMap = new ContinuousActionsInversionMap();

        public SerializableDiscreteActionsMap serializableDiscreteActionMap = new SerializableDiscreteActionsMap();
        public SerializableContinuousActionsMap serialiazableContinuousActionMap = new SerializableContinuousActionsMap();
        public SerializableContinuousActionsInvMap serializableContinuousActionInvMap = new SerializableContinuousActionsInvMap();

        public void OnPreSerialize()
        {
            serialiazableContinuousActionMap.Clear();
            foreach(var keyValue in continuousActionsMap)
            {
                serialiazableContinuousActionMap.Add(keyValue);
            }

            serializableContinuousActionInvMap.Clear();
            foreach (var keyValue in continuousActionsInvMap)
            {
                serializableContinuousActionInvMap.Add(keyValue);
            }

            serializableDiscreteActionMap.Clear();
            foreach(var keyValue in discreteActionsMap)
            {
                serializableDiscreteActionMap.Add(keyValue);
            }
        }

        public void OnPostDeserialize()
        {
            continuousActionsMap.Clear();
            foreach(var keyValue in serialiazableContinuousActionMap)
            {
                continuousActionsMap.Add(keyValue.Key, keyValue.Value);
            }

            continuousActionsInvMap.Clear();
            foreach (var keyValue in serializableContinuousActionInvMap)
            {
                continuousActionsInvMap.Add(keyValue.Key, keyValue.Value);
            }

            discreteActionsMap.Clear();
            foreach(var keyValue in serializableDiscreteActionMap)
            {
                discreteActionsMap.Add(keyValue.Key, keyValue.Value);
            }
        }

        public void SetDiscreteBinding(Bitset state, DiscreteAction action)
        {
            foreach (var keyVal in discreteActionsMap)
            {
                if(keyVal.Value.Equals(state))
                {
                    discreteActionsMap.Remove(keyVal.Key);
                    break;
                }
            }

            discreteActionsMap[action] = state;
        }

        public void UnsetDiscreteBinding(DiscreteAction action)
        {
            if (!discreteActionsMap.ContainsKey(action))
            {
                return;
            }

            discreteActionsMap.Remove(action);
        }

        public List<DiscreteAction> GetDiscreteBinding(Bitset state)
        {
            List<KeyValuePair<Bitset, DiscreteAction>> matches = new List<KeyValuePair<Bitset, DiscreteAction>>();

            foreach (var maskActionPair in discreteActionsMap)
            {
                Bitset expectedState = maskActionPair.Value;
                bool match = true;

                for (int i = 0; i < state.m_NumBits; i++)
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
                    matches.Add(new KeyValuePair<Bitset, DiscreteAction>(maskActionPair.Value, maskActionPair.Key));
                }
            }

            List<KeyValuePair<int, DiscreteAction>> matchResults = new List<KeyValuePair<int, DiscreteAction>>();
            for (int i = 0; i < matches.Count; i++)
            {
                int bits = 0;
                for (int q = 0; q < state.m_NumBits; q++)
                {
                    if (matches[i].Key.Get(q))
                    {
                        bits++;
                    }
                }

                var value = matches[i].Value;
                matchResults.Add(new KeyValuePair<int, DiscreteAction>(bits, value));
            }

            if (matches.Count == 0)
            {
                return null;
            }

            matchResults.Sort(CompareKeys2);

            int minBits = matchResults[matches.Count - 1].Key;
            List<DiscreteAction> actions = new List<DiscreteAction>();

            for (var i = 0; i < matches.Count; i++)
            {
                if (matchResults[i].Key >= minBits)
                {
                    actions.Add(matchResults[i].Value);
                }
            }

            return actions;
        }

        public Bitset GetBitsetForDiscreteBinding(DiscreteAction action)
        {
            if (!discreteActionsMap.ContainsKey(action)) return null;
            return discreteActionsMap[action];
        }

        public void SetContinuousBinding(int axis, Bitset state, ContinuousAction action, bool isInverted)
        {
            //Debug.Log("SetContBind: " + action.ToString() + " to axis " + axis + " and bitset " + state.ToString());
            continuousActionsMap[action] = new KeyValuePair<Bitset, int>(state, axis);
            if (isInverted)
            {
                continuousActionsInvMap[action] = true;
            }
            //TODO: Remove from map if false and containsKey? Saves a little space when serializing.
        }

        public void UnsetContinuousBinding(ContinuousAction action)
        {
            continuousActionsMap.Remove(action);
            continuousActionsInvMap.Remove(action);
        }

        public List<ContinuousAction> GetContinuousBinding(int axis, Bitset state)
        {
            List<KeyValuePair<Bitset, ContinuousAction>> matches = new List<KeyValuePair<Bitset, ContinuousAction>>();
            foreach (var continuousActionPair in continuousActionsMap)
            {
                if (continuousActionPair.Value.Key == null || continuousActionPair.Value.Value != axis)
                {
                    continue;
                }

                Bitset expectedState = continuousActionPair.Value.Key;
                bool match = true;

                for (int i = 0; i < state.m_NumBits; i++)
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
                    matches.Add(new KeyValuePair<Bitset, ContinuousAction>(expectedState, continuousActionPair.Key));
                }
            }

            if (matches.Count == 0)
            {
                return null;
            }

            List<KeyValuePair<int, ContinuousAction>> matchResults = new List<KeyValuePair<int, ContinuousAction>>();
            for (int i = 0; i < matches.Count; i++)
            {
                int bits = 0;
                for (int q = 0; q < matches[i].Key.m_NumBits; q++)
                {
                    if(matches[i].Key.Get(q))
                    {
                        bits++;
                    }
                }

                var value = matches[i].Value;
                matchResults.Add(new KeyValuePair<int, ContinuousAction>(bits, value));
            }

            matchResults.Sort(CompareKeys);

            int minBits = matchResults[0].Key;
            List<ContinuousAction> actions = new List<ContinuousAction>();

            for (var i = 0; i < matchResults.Count; i++)
            {
                if (matchResults[i].Key >= minBits)
                {
                    actions.Add(matchResults[i].Value);
                }
            }
            
            return actions;
        }
        
        public KeyValuePair<int, Bitset> GetBitsetForContinuousBinding(ContinuousAction action)
        {
            if(!continuousActionsMap.ContainsKey(action))
            {
                return new KeyValuePair<int, Bitset>(0, null);
            }

            return new KeyValuePair<int,Bitset>(continuousActionsMap[action].Value, continuousActionsMap[action].Key);
        }

        public bool IsContinuousBindingInverted(ContinuousAction action)
        {
            return continuousActionsInvMap.ContainsKey(action) ? continuousActionsInvMap[action] : false;
        }

        public void SetContinuousBindingInverted(ContinuousAction action, bool isInverted)
        {
            continuousActionsInvMap[action] = isInverted;
        }

        private static int CompareKeys(KeyValuePair<int, ContinuousAction> a, KeyValuePair<int, ContinuousAction> b)
        {
            return b.Key.CompareTo(a.Key);
        }

        private static int CompareKeys2(KeyValuePair<int, DiscreteAction> a, KeyValuePair<int, DiscreteAction> b)
        {
            return a.Key.CompareTo(b.Key);
        }

    }

}
