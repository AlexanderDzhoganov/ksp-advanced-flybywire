using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace KSPAdvancedFlyByWire
{
    class XMLConfigReader
    {

        public void FromFile(string xmlPath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlPath);

            SortedList<int, string> hooks = new SortedList<int, string>();

            XmlNode cameraHooks = doc.SelectSingleNode("KSPGraphicsOverhaul/CameraHooks");
            foreach(XmlNode hook in cameraHooks.ChildNodes)
            {
                var name = hook.Attributes["name"].Value;
                hooks.Add(int.Parse(hook.Attributes["blitOrder"].Value), name);
            }

            m_CameraHooks = hooks.Values.ToList();

            XmlNode shaderPaths = doc.SelectSingleNode("KSPGraphicsOverhaul/ShaderPaths");
            foreach(XmlNode shader in shaderPaths.ChildNodes)
            {
                var name = shader.Attributes["name"].Value;
                var path = shader.InnerXml;
                m_ShaderPaths[name] = path;
            }

            XmlNode replaceShaders = doc.SelectSingleNode("KSPGraphicsOverhaul/ReplaceShaders");
            foreach (XmlNode shader in replaceShaders.ChildNodes)
            {
                var name = shader.Attributes["name"].Value;
                var path = shader.InnerXml;
                m_ShaderReplacements[name] = path;
            }
        }

        public Dictionary<string, string> ShaderPaths
        {
            get
            {
                return m_ShaderPaths;
            }
        }
        
        public Dictionary<string, string> ShaderReplacements
        {
            get
            {
                return m_ShaderReplacements;
            }
        }

        public List<string> CameraHooks
        {
            get
            {
                return m_CameraHooks;
            }
        }

        private Dictionary<string, string> m_ShaderPaths = new Dictionary<string, string>();
        private Dictionary<string, string> m_ShaderReplacements = new Dictionary<string, string>();
        private List<string> m_CameraHooks = new List<string>();

    }

}
