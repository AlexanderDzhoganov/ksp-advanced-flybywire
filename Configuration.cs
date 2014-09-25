using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;

namespace KSPAdvancedFlyByWire
{

    [Serializable]
    public class ControllerConfiguration
    {
        public InputWrapper wrapper = InputWrapper.XInput;
        public int controllerIndex = 0;
        public List<ControllerPreset> presets = new List<ControllerPreset>();
        public int currentPreset = 0;
        public CurveType analogInputCurve = CurveType.XSquared;
        public float discreteActionStep = 0.15f;
        public float incrementalThrottleSensitivity = 0.05f;

        [NonSerialized]
        public IController iface;

        [NonSerialized]
        public HashSet<Bitset> evaluatedDiscreteActionMasks = new HashSet<Bitset>();
    }

    [Serializable]
    public class Configuration
    {

        public List<ControllerConfiguration> controllers = new List<ControllerConfiguration>();

        public ControllerConfiguration GetConfigurationByIController(IController controller)
        {
            foreach (ControllerConfiguration ctrlr in controllers)
            {
                if(ctrlr.iface == controller)
                {
                    return ctrlr;
                }
            }

            return null;
        }

        public static void Serialize(string filename, Configuration config)
        {
            var serializer = new XmlSerializer(typeof(Configuration));

            using (var writer = new StreamWriter(filename))
            {
                serializer.Serialize(writer, config);
            }
        }

        public static Configuration Deserialize(string filename)
        {
            var serializer = new XmlSerializer(typeof(Configuration));

            using (var reader = new StreamReader(filename))
            {
                return (Configuration)serializer.Deserialize(reader);
            }

            return null;
        }


    }

}
