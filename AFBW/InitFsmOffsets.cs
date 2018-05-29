using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace KSPAdvancedFlyByWire
{
    internal class InitFsmOffsets
    {
        internal int BoardVessel = -1;
        internal int GrabLadder = -1;
        internal int LadderLetGo = -1;
        internal int JumpStart = -1;
        internal int StartRun = -1;
        internal int StopRun = -1;
        internal int EndRun = -1;
        internal int TogglePackEvent = -1;
        internal int LadderStop = -1;
        internal int SwimStop = -1;

        internal InitFsmOffsets()
        {
            int c = 0;
            List<FieldInfo> fields = new List<FieldInfo>(typeof(KerbalEVA).GetFields(BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));

            foreach (FieldInfo FI in fields) //.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                switch (FI.Name)
                {
                    case "On_boardPart":
                        BoardVessel = c; break;
                    case "On_ladderGrabStart":
                        GrabLadder = c; break;
                    case "On_ladderLetGo":
                        LadderLetGo = c; break;
                    case "On_jump_start":
                        JumpStart = c; break;
                    case "On_startRun":
                        StartRun = c; break;
                    case "On_endRun":
                        EndRun = c;
                        StopRun = c;
                        break;
                    case "On_packToggle":
                        TogglePackEvent = c; break;
                    case "On_ladderStop":
                        LadderStop = c; break;
                    case "On_swim_stop":
                        SwimStop = c; break;
                }
                //Debug.Log("FieldInfo[" + c + "].Name: " + FI.Name);
                c++;
            }
        }

    }
}
