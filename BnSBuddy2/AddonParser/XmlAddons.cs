using BnSBuddy2.Functions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace BnSBuddy2.AddonParser
{
    class XmlAddons
    {
        public Dictionary<string, List<Dictionary<string, List<string>>>> XmlAddon = new Dictionary<string, List<Dictionary<string, List<string>>>>();
        // 
        //  FileName [Key]
        //  L - (string)Search [Key]  = List<string> [array] (Dic)
        //  L - (string)Replace [Key] = List<string> [array] (Dic)
        //  L - (string)Description [Key] = List<string> [array] (Dic)
        //
        public XmlAddons(string AddonXmlFile, string FileName, bool description = false)
        {
            if (AddonXmlFile != "" && FileName != "")
            {
                if (File.Exists(AddonXmlFile))
                {
                    if (File.ReadAllLines(AddonXmlFile).Length < 4)
                    {
                        Prompt.Popup(String.Format("Skipping Addon: {0} miscoded xml file!", FileName.Replace(".xml", "")));
                        return;
                    }
                    XmlParser(AddonXmlFile, FileName, description);
                }
                else
                {
                    Prompt.Popup(String.Format("Skipping Addon: {0} no longer exists in the folder", FileName.Replace(".xml", "")));
                }
            }
        }

        private bool CompatibilityCheck(XmlDocument xmlfile)
        {
            XmlNodeList files = xmlfile.DocumentElement.GetElementsByTagName("patch");
            if (files.Count > 0)
                return true;
            else
                return false;
        }

        private void XmlParser(string FilePath, string FileName, bool description)
        {
            string FileNames = "";
            Dictionary<string, List<string>> Descriptions = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> Search = new Dictionary<string, List<string>>();
            List<string> DD = new List<string>();
            List<string> SS = new List<string>();
            List<string> RR = new List<string>();
            Dictionary<string, List<string>> Replace = new Dictionary<string, List<string>>();
            //
            // Parse Xml and Add
            //
            try
            {
                XmlDocument xmlfile = new XmlDocument();
                xmlfile.Load(FilePath);
                if (CompatibilityCheck(xmlfile))
                {
                    Prompt.Popup(String.Format("Skipping Addon: {0} | BnS Buddy Does not support BnSPatch xml format!", FileName.Replace(".xml", "")));
                    return;
                }
                XmlNodeList files = xmlfile.DocumentElement.GetElementsByTagName("file");
                foreach (XmlNode file in files)
                {
                    FileNames = file.Attributes["path"].Value;
                    DD = new List<string>();
                    SS = new List<string>();
                    RR = new List<string>();
                    Search = new Dictionary<string, List<string>>();
                    Replace = new Dictionary<string, List<string>>();
                    Descriptions = new Dictionary<string, List<string>>();
                    XmlNodeList Childs = file.SelectNodes("*");
                    foreach (XmlNode srd in Childs)
                    {
                        if (srd.Name == "search")
                            SS.Add(srd.InnerText);
                        if (srd.Name == "replace")
                            RR.Add(srd.InnerText);
                        if (srd.Name == "description")
                            DD.Add(srd.InnerText);
                    }
                    if (SS.Count != RR.Count)
                    {
                        Prompt.Popup(String.Format("Skipping FileName: {0} | Seach and Replace count does not match!", FileName.Replace(".xml", "")));
                        return;
                    }
                    if (DD.Count != 1)
                    {
                        DD.Add("No description provided!");
                    }
                    Search.Add("Search", SS);
                    Replace.Add("Replace", RR);
                    Descriptions.Add("Description", DD);
                    List<Dictionary<string, List<string>>> pairs = new List<Dictionary<string, List<string>>>();
                    pairs.Add(Search);
                    pairs.Add(Replace);
                    pairs.Add(Descriptions);

                    string[] test = null;
                    List<string> testfinal = new List<string>();
                    string searchz = "";
                    string activedatafile = "";
                    if (!description)
                        PAK.PakHandler.PakUnpack(Form1.CurrentForm.PakPaths.PakPath + "Pak0-Local.pak");
                    if (FileNames.Contains("*") && !description)
                    {
                        string[] array = FileNames.Replace("\\\\", "\\").Split(new char[1] { '\\' }, 2);
                        activedatafile = array[0].Replace(".files", "").Trim();
                        searchz = array[1].ToLower().Trim();
                        activedatafile = activedatafile.Contains("[bit]") ? activedatafile.Replace("[bit]", "64") : activedatafile.Replace("[bit]", "");
                        test = Pilao.ComboBoxHandler.GetFileList(activedatafile);
                        foreach (string filez in test)
                        {
                            if (Regex.IsMatch(filez.Replace("\\", "/"), searchz.Replace("\\", "/")))
                                testfinal.Add(filez);
                        }
                        test = null;
                    }
                    if (description)
                    {
                        XmlAddon.Add(FileNames, pairs);
                        return;
                    }
                    if (testfinal.Count > 0)
                    {
                        foreach (string FileWildCard in testfinal)
                        {
                            activedatafile = activedatafile.Replace("64", "[bit]");
                            if (!XmlAddon.ContainsKey(activedatafile + "\\" + FileWildCard))
                                XmlAddon.Add(activedatafile + "\\" + FileWildCard, pairs);
                            else
                                Prompt.Popup("Duplicate of FileNames, please recode patch file: " + FileName.Replace(".patch", ""));
                        }
                    }
                    else
                    {
                        if (!XmlAddon.ContainsKey(FileNames))
                            XmlAddon.Add(FileNames, pairs);
                        else
                            Prompt.Popup("Duplicate of FileNames, please recode xml file: " + FileName.Replace(".xml", ""));
                    }
                }
            }
            catch (XmlException)
            {
                Prompt.Popup("Error: Could not parse xml file: " + FilePath + Environment.NewLine + "Please recode the xml properly.");
            }
            catch (Exception ex)
            {
                Prompt.Popup("Error: Could not parse xml file: " + FilePath + Environment.NewLine + ex.ToString());
            }
        }
    }
}
