using BnSBuddy2.Functions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace BnSBuddy2.AddonParser
{
    class LegacyAddons
    {
        public Dictionary<string, List<Dictionary<string, List<string>>>> PatchAddon = new Dictionary<string, List<Dictionary<string, List<string>>>>();
        // 
        //  FileName
        //  L - (string)Search = List<string> (Dic)
        //  L - (string)Replace = List<string> (Dic)
        //  L - (string)Description = List<string> (Dic)
        //

        public LegacyAddons(string AddonPatchFile, string FileName, bool Description = false)
        {
            if (AddonPatchFile != "" && FileName != "")
            {
                if (File.Exists(AddonPatchFile))
                {
                    if (File.ReadAllLines(AddonPatchFile).Length < 4)
                    {
                        Prompt.Popup(String.Format("Skipping Addon: {0} miscoded patch file!", FileName.Replace(".patch", "")));
                        return;
                    }
                    PatchParser(AddonPatchFile, FileName, Description);
                }
                else
                {
                    Prompt.Popup(String.Format("Skipping Addon: {0} no longer exists in the folder", FileName.Replace(".patch", "")));
                }
            }
        }

        private void PatchParser(string FilePath, string FileName, bool Description)
        {
            string FileNames = "";
            Dictionary<string, List<string>> Descriptions = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> Search = new Dictionary<string, List<string>>();
            List<string> DD = new List<string>();
            List<string> SS = new List<string>();
            List<string> RR = new List<string>();
            Dictionary<string, List<string>> Replace = new Dictionary<string, List<string>>();
            string[] Lines = File.ReadAllLines(FilePath);
            int apos = 0;
            foreach(string Line in Lines)
            {
                if (Line.StartsWith("FileName = "))
                {
                    apos++;
                    if (FileNames != "" && Line.StartsWith("FileName = "))
                    {
                        Search.Add("Search", SS);
                        Replace.Add("Replace", RR);
                        if (DD.Count > 0)
                            Descriptions.Add("Description", DD);

                        List<Dictionary<string, List<string>>> pairs = new List<Dictionary<string, List<string>>>();
                        pairs.Add(Search);
                        pairs.Add(Replace);
                        pairs.Add(Descriptions);

                        string[] test = null;
                        List<string> testfinal = new List<string>();
                        string searchz = "";
                        string activedatafile = "";
                        if (!Description)
                            PAK.PakHandler.PakUnpack(Form1.CurrentForm.PakPaths.PakPath + "Pak0-Local.pak");
                        if (FileNames.Contains("*") && !Description)
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
                        if (Description)
                        {
                            PatchAddon.Add(FileNames, pairs);
                            return;
                        }
                        if (testfinal.Count > 0)
                        {
                            foreach (string FileWildCard in testfinal)
                            {
                                activedatafile = activedatafile.Replace("64", "[bit]");
                                if (!PatchAddon.ContainsKey(activedatafile + "\\" + FileWildCard))
                                    PatchAddon.Add(activedatafile + "\\" + FileWildCard, pairs);
                                else
                                    Prompt.Popup("Duplicate of FileNames, please recode patch file: " + FileName.Replace(".patch", ""));
                            }
                        }
                        else
                        {
                            if (!PatchAddon.ContainsKey(FileNames))
                                PatchAddon.Add(FileNames, pairs);
                            else
                                Prompt.Popup("Duplicate of FileNames, please recode patch file: " + FileName.Replace(".patch", ""));
                        }
                    }

                    FileNames = Line.Replace("FileName = ", "");
                    DD = new List<string>();
                    SS = new List<string>();
                    RR = new List<string>();
                    Search = new Dictionary<string, List<string>>();
                    Replace = new Dictionary<string, List<string>>();
                    Descriptions = new Dictionary<string, List<string>>();
                }
                else
                {
                    if (Line.StartsWith("Search = "))
                        SS.Add(Line.Replace("Search = ", "").Replace("[NewLine]", Environment.NewLine).Replace("NewLine", Environment.NewLine));
                    if (Line.StartsWith("Replace = "))
                    {
                        if (Line.Replace("Replace = ", "") != "")
                            RR.Add(Line.Replace("Replace = ", "").Replace("[NewLine]", Environment.NewLine).Replace("NewLine", Environment.NewLine));
                        else
                            RR.Add("<!--" + SS[SS.Count - 1] + "-->");
                    }
                    if (Line.StartsWith("Description = "))
                        DD.Add(Line.Replace("Description = ", ""));
                }
            }
            if (apos == 1 && PatchAddon.Count == 0)
            {
                Search.Add("Search", SS);
                Replace.Add("Replace", RR);
                if (DD.Count > 0)
                    Descriptions.Add("Description", DD);

                List<Dictionary<string, List<string>>> pairs = new List<Dictionary<string, List<string>>>();
                pairs.Add(Search);
                pairs.Add(Replace);
                pairs.Add(Descriptions);


                string[] test = null;
                List<string> testfinal = new List<string>();
                string searchz = "";
                string activedatafile = "";
                if (FileNames.Contains("*") && !Description)
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
                if (testfinal.Count > 0)
                {
                    foreach (string FileWildCard in testfinal)
                    {
                        activedatafile = activedatafile.Insert(activedatafile.IndexOf(".dat"), "[bit]");
                        if (!PatchAddon.ContainsKey(activedatafile + ".files\\" + FileWildCard))
                            PatchAddon.Add(activedatafile + ".files\\" + FileWildCard, pairs);
                        else
                            Prompt.Popup("Duplicate of FileNames, please recode patch file: " + FileName.Replace(".patch", ""));
                    }
                }
                else
                {
                    if (!PatchAddon.ContainsKey(FileNames))
                        PatchAddon.Add(FileNames, pairs);
                    else
                        Prompt.Popup("Duplicate of FileNames, please recode patch file: " + FileName.Replace(".patch", ""));
                }
            }
            if (apos != PatchAddon.Count && !FileNames.Contains("*"))
            {
                PatchAddon = new Dictionary<string, List<Dictionary<string, List<string>>>>();
                Prompt.Popup(String.Format("Skipping Addon: {0} Bad Count of FileNames!", FileName.Replace(".patch", "")));
                return;
            }
            List<string> ToRemove = new List<string>();
            int Dcount = 0;
            foreach (string key in PatchAddon.Keys)
            {
                int Scount = 0;
                int Rcount = 0;
                foreach (Dictionary<string,List<string>> LLore in PatchAddon[key])
                {
                    if (LLore.ContainsKey("Search"))
                        Scount = LLore["Search"].Count;
                    if (LLore.ContainsKey("Replace"))
                        Rcount = LLore["Replace"].Count;
                    if (LLore.ContainsKey("Description"))
                        Dcount += LLore["Description"].Count;
                }
                if (Scount != Rcount)
                {
                    Prompt.Popup(String.Format("Skipping FileName: {0} Seach and Replace count does not match!", FileName.Replace(".patch", "")));
                    if (!ToRemove.Contains(key))
                        ToRemove.Add(key);
                    return;
                }
            }
            if (PatchAddon.Count != Dcount)
            {
                Prompt.Popup(String.Format("Skipping FileName: {0} FileName does not have Description!", FileName.Replace(".patch", "")));
            }
            foreach (string rkey in ToRemove)
            {
                if (PatchAddon.ContainsKey(rkey))
                    PatchAddon.Remove(rkey);
            }
        }
    }
}
