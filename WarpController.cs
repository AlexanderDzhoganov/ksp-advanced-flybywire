// gratitiously copy/pasted from MechJeb's source
// all credit goes to original author

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace KSPAdvancedFlyByWire
{

    class WarpController
    {

        public static void IncreaseWarp()
        {
            if (CheckRegularWarp())
            {
                IncreaseRegularWarp();
                return;
            }

            if (CheckPhysicsWarp())
            {
                IncreasePhysicsWarp();
            }
        }

        public static void DecreaseWarp()
        {
            if (CheckRegularWarp())
            {
                DecreaseRegularWarp();
                return;
            }

            if (CheckPhysicsWarp())
            {
                DecreasePhysicsWarp();
            }
        }

        public static bool CheckRegularWarp()
        {
            if (TimeWarp.WarpMode != TimeWarp.Modes.HIGH)
            {
                return false;
            }

            return true;
        }

        private static bool CheckPhysicsWarp()
        {
            if (TimeWarp.WarpMode != TimeWarp.Modes.LOW)
            {
                return false;
            }

            return true;
        }

        public static bool IncreaseRegularWarp(bool instant = false)
        {
            if (!CheckRegularWarp()) return false; //make sure we are in regular warp

            //do a bunch of checks to see if we can increase the warp rate:
            if (TimeWarp.CurrentRateIndex + 1 == TimeWarp.fetch.warpRates.Length) return false; //already at max warp
            
            if (!FlightGlobals.ActiveVessel.LandedOrSplashed)
            {
                double instantAltitudeASL = (FlightGlobals.ActiveVessel.CoM - FlightGlobals.ActiveVessel.mainBody.position).magnitude - FlightGlobals.ActiveVessel.mainBody.Radius;
                if (TimeWarp.fetch.GetAltitudeLimit(TimeWarp.CurrentRateIndex + 1, FlightGlobals.ActiveVessel.mainBody) > instantAltitudeASL)
                {
                    return false;
                } //altitude too low to increase warp
            }

            if (TimeWarp.fetch.warpRates[TimeWarp.CurrentRateIndex] != TimeWarp.CurrentRate)
            {
                return false; //most recent warp change is not yet complete
            }

            TimeWarp.SetRate(TimeWarp.CurrentRateIndex + 1, instant);
            return true;
        }

        public static bool IncreasePhysicsWarp(bool instant = false)
        {
            if (!CheckPhysicsWarp()) return false; //make sure we are in regular warp

            //do a bunch of checks to see if we can increase the warp rate:
            if (TimeWarp.CurrentRateIndex + 1 == TimeWarp.fetch.physicsWarpRates.Length)
            {
                return false; //already at max warp
            }

            if (TimeWarp.fetch.physicsWarpRates[TimeWarp.CurrentRateIndex] != TimeWarp.CurrentRate)
            {
                return false; //most recent warp change is not yet complete
            }

            TimeWarp.SetRate(TimeWarp.CurrentRateIndex + 1, instant);
            return true;
        }

        public static bool DecreaseRegularWarp(bool instant = false)
        {
            if (!CheckRegularWarp()) return false;

            if (TimeWarp.CurrentRateIndex == 0) return false; //already at minimum warp

            TimeWarp.SetRate(TimeWarp.CurrentRateIndex - 1, instant);
            return true;
        }

        public static bool DecreasePhysicsWarp(bool instant = false)
        {
            if (!CheckPhysicsWarp()) return false;

            if (TimeWarp.CurrentRateIndex == 0) return false; //already at minimum warp

            TimeWarp.SetRate(TimeWarp.CurrentRateIndex - 1, instant);
            return true;
        }

    }

}
