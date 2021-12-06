using BnSBuddy2.Functions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace BnSBuddy2.Settings
{
    public class ConfigParser
    {
        private Dictionary<string, string> CurrentConfigs = new Dictionary<string, string>();
        private string AppPath = Path.GetDirectoryName(Application.ExecutablePath);
        private readonly string _default = new Default().value();

        private enum InfoType
        {
            NAME = 0,
            VALUE = 1
        }

        public Dictionary<string, string> Get()
        {
            if (CurrentConfigs.Count == 0 && File.Exists(AppPath + "\\Settings.xml"))
            {
                XmlDocument document = new XmlDocument();
                document.Load(AppPath + "\\Settings.xml");
                foreach (XmlNode node in document.GetElementsByTagName("setting"))
                    CurrentConfigs.Add(node.Attributes["name"].Value, node.InnerText);
                document = null;
            }
            return CurrentConfigs;
        }

        public void Save(Dictionary<string, string> input)
        {
            XmlDocument Xml = new XmlDocument();
            Xml.LoadXml(_default);
            foreach (XmlNode node in Xml.DocumentElement.GetElementsByTagName("setting"))
                if (input.ContainsKey(node.Attributes["name"].Value))
                    if (node.InnerText != input[node.Attributes["name"].Value])
                        node.InnerText = input[node.Attributes["name"].Value];
            using (XmlTextWriter writer = new XmlTextWriter(AppPath + "\\Settings.xml", UTF8Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                Xml.Save(writer);
            }
        }

        public void Update()
        {
            Dictionary<string, string> NameValPairs = new Dictionary<string, string>();
            XmlDocument Old = new XmlDocument();
            Old.Load(AppPath + "\\Settings.xml");
            XmlDocument Xml = new XmlDocument();
            Xml.LoadXml(_default);
            if (Old.DocumentElement.GetElementsByTagName("setting").Count < Xml.DocumentElement.GetElementsByTagName("setting").Count)
            {
                Form1.CurrentForm.AddLauncherLog("Updating Settings...");
                foreach (XmlNode node in Old.DocumentElement.GetElementsByTagName("setting"))
                    if (!NameValPairs.ContainsKey(node.Attributes["name"].Value))
                        NameValPairs.Add(node.Attributes["name"].Value, node.InnerText);
                foreach (XmlNode node in Xml.DocumentElement.GetElementsByTagName("setting"))
                    if (NameValPairs.ContainsKey(node.Attributes["name"].Value))
                        if (node.InnerText != NameValPairs[node.Attributes["name"].Value])
                            node.InnerText = NameValPairs[node.Attributes["name"].Value];
                // Save
                Xml.Save(AppPath + "\\Settings.xml");
                Form1.CurrentForm.AddLauncherLog("Updated Settings");
            }
            // Dispose
            NameValPairs = null;
            Old = null;
            Xml = null;
        }

        public void Convert()
        {
            if (File.Exists(AppPath + "\\Settings.ini"))
            {
                Form1.CurrentForm.AddLauncherLog("Converting Settings...");
                Dictionary<string, string> NameValPairs = new Dictionary<string, string>();
                string[] Old = File.ReadAllLines(AppPath + "\\Settings.ini");
                XmlDocument Xml = new XmlDocument();
                Xml.LoadXml(_default);
                foreach (string line in Old)
                {
                    string[] pairs = line.Split(new[] { " = " }, StringSplitOptions.None);
                    if (pairs.Length == 2)
                    {
                        if (pairs[0] == "default")
                            pairs[0] = "_default";
                        if (pairs[0] == "arguements")
                            pairs[0] = "arguments";
                        if (!NameValPairs.ContainsKey(pairs[(int)InfoType.NAME]))
                            NameValPairs.Add(pairs[(int)InfoType.NAME], pairs[(int)InfoType.VALUE]);
                    }
                }
                foreach (XmlNode node in Xml.DocumentElement.GetElementsByTagName("setting"))
                {
                    if (NameValPairs.ContainsKey(node.Attributes["name"].Value))
                    {
                        if (node.InnerText != NameValPairs[node.Attributes["name"].Value])
                            node.InnerText = NameValPairs[node.Attributes["name"].Value];
                    }
                }
                // Save
                using (XmlTextWriter writer = new XmlTextWriter(AppPath + "\\Settings.xml", UTF8Encoding.UTF8))
                {
                    writer.Formatting = Formatting.Indented;
                    Xml.Save(writer);
                }
                File.Delete(AppPath + "\\Settings.ini");
                // Dispose
                NameValPairs = null;
                Old = null;
                Xml = null;
                Form1.CurrentForm.AddLauncherLog("Converted Settings");
            }
        }

        public void NewSettings(bool recreate = false)
        {
            if (recreate)
                File.Delete(AppPath + "\\Settings.xml");
            if (!File.Exists(AppPath + "\\Settings.xml"))
            {
                Form1.CurrentForm.AddLauncherLog("Creating Settings...");
                using (XmlTextWriter writer = new XmlTextWriter(AppPath + "\\Settings.xml", UTF8Encoding.UTF8))
                {
                    XmlDocument xml = new XmlDocument();
                    xml.LoadXml(_default);
                    writer.Formatting = Formatting.Indented;
                    xml.Save(writer);
                    xml = null;
                }
                Form1.CurrentForm.AddLauncherLog("Created Settings");
            }
        }

        public void CheckCorrupt()
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(_default);
            XmlNodeList DefNodes = xml.DocumentElement.GetElementsByTagName("setting");
            xml = new XmlDocument();
            xml.Load(AppPath + "\\Settings.xml");
            int Fixed = 0;
            foreach (XmlNode node in xml.DocumentElement.GetElementsByTagName("setting"))
                for (int i = 0; i > xml.DocumentElement.GetElementsByTagName("setting").Count; i++)
                {
                    if (node.Attributes["name"].Value != DefNodes[i].Attributes["name"].Value)
                    {
                        node.Attributes["name"].Value = DefNodes[i].Attributes["name"].Value;
                        Fixed++;
                    }
                }
            if (Fixed > 0)
            {
                xml.Save(AppPath + "\\Settings.xml");
                Prompt.Popup($"BnS Buddy Detected {Fixed} broken settings and fixed them for you.");
            }
        }
    }
}
