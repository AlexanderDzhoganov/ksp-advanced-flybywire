using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace KSPAdvancedFlyByWire
{

    [KSPAddon(KSPAddon.Startup.Flight, true)]
    public class FlyByWire : MonoBehaviour
    {

        private XMLConfigReader configReader;

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
            GUILayout.Label("hello world");
        }

        void OnGUI()
        {
            GUI.Window(0, new Rect(32, 32, 400, 600), DoMainWindow, "Advanced FlyByWire");
        }

    }

}
