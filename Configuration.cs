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

        [XmlIgnore()]
        public IController iface;

        [XmlIgnore()]
        public HashSet<Bitset> evaluatedDiscreteActionMasks = new HashSet<Bitset>();
    }

    [Serializable]
    public class Configuration
    {

        public Configuration() {}

        public void OnPreSerialize()
        {
            foreach (ControllerConfiguration config in controllers)
            {
                foreach (var preset in config.presets)
                {
                    preset.OnPreSerialize();
                }
            }
        }

        public void OnPostDeserialize()
        {
            foreach (ControllerConfiguration config in controllers)
            {
                if (config.wrapper == InputWrapper.XInput)
                {
                    config.iface = new XInputController(config.controllerIndex);
                }
                else if (config.wrapper == InputWrapper.SDL)
                {
                    config.iface = new SDLController(config.controllerIndex);
                }

                config.evaluatedDiscreteActionMasks = new HashSet<Bitset>();

                foreach (var preset in config.presets)
                {
                    preset.OnPostDeserialize();
                }
            }
        }

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
                config.OnPreSerialize();
                serializer.Serialize(writer, config);
            }
        }

        public static Configuration Deserialize(string filename)
        {
            var serializer = new XmlSerializer(typeof(Configuration));

            try
            {
                using (var reader = new StreamReader(filename))
                {
                    Configuration config = (Configuration)serializer.Deserialize(reader);
                    config.OnPostDeserialize();
                    return config;
                }
            }
            catch
            {
                
            }

            return null;
        }
    }

}
