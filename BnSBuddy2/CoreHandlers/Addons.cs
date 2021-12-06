using BnSBuddy2.Functions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace BnSBuddy2.CoreHandlers
{
    public static class Addons
    {
        // Globals
        private static Dictionary<string, Dictionary<string, string[]>> filesToEdit;
        private static string TempPath = Path.GetTempPath();
        private static AutoResetEvent waitPatchingTask = new AutoResetEvent(initialState: false);
        private static bool isBusyWorking = false;
        // End Globals

        public static void OpenAddonsFolder(string AddonsPath) =>
            Process.Start("explorer.exe", AddonsPath);

        public static void SelectAllCheckBox(CheckState checkState)
        {
            if (checkState == CheckState.Checked)
            {
                foreach (TreeNode node in Form1.CurrentForm.treeView3.Nodes)
                {
                    node.Checked = true;
                    if (node.Nodes.Count > 0)
                        foreach (TreeNode childnode in node.Nodes)
                            childnode.Checked = true;
                }
            }
            else if (checkState == CheckState.Unchecked)
            {
                foreach (TreeNode node in Form1.CurrentForm.treeView3.Nodes)
                {
                    node.Checked = false;
                    if (node.Nodes.Count > 0)
                        foreach (TreeNode childnode in node.Nodes)
                            childnode.Checked = false;
                }
            }
        }

        private static string FixXmlString(string input)
        {
            if (input.Contains("<!-- "))
                input = input.Replace("<!-- ", "<!--");
            if (input.Contains(" -->"))
                input = input.Replace(" -->", "-->");
            // Fix missing efbbbf at start
            input = "﻿" + input;
            return input;
        }

        public static void ManualRestoreGameAddons()
        {
            if (CheckIfChecked())
            {
                Form1.CurrentForm.materialMultiLineTextBox4.Text = "";
                Form1.CurrentForm.AddLauncherLog("Reverting Addons...", true);
                RestoreAddons();
            }
            else
                Prompt.Popup("No patch selected.");
        }

        private static bool CheckIfChecked()
        {
            foreach (TreeNode node in Form1.CurrentForm.treeView3.Nodes)
            {
                if (node != null && node.Checked)
                {
                    return true;
                }
            }
            return false;
        }

        public static void ManualStartGameAddons()
        {
            if (CheckIfChecked())
            {
                Form1.CurrentForm.materialMultiLineTextBox4.Text = "";
                Form1.CurrentForm.AddLauncherLog("Applying Addons...", true);
                StartGameAddons();
            }
            else
                Prompt.Popup("No patch selected.");
        }

        public static void AfterSelect(string AddonFullPath)
        {
            string text = "";
            string str = Path.GetFileName(AddonFullPath);

            Dictionary<string, List<Dictionary<string, List<string>>>> description = new Dictionary<string, List<Dictionary<string, List<string>>>>();
            if (File.Exists(AddonFullPath))
            {
                if (str.EndsWith(".xml"))
                    description = new AddonParser.XmlAddons(AddonFullPath, str, true).XmlAddon;
                else if (str.EndsWith(".patch"))
                    description = new AddonParser.LegacyAddons(AddonFullPath, str, true).PatchAddon;
            }
            foreach (string FileName in description.Keys)
                if (!text.Contains(description[FileName][2]["Description"][0]))
                    text += description[FileName][2]["Description"][0] + Environment.NewLine;
            if (text.Length > 0)
                Form1.CurrentForm.materialMultiLineTextBox4.Text = text;
            else
                Form1.CurrentForm.materialMultiLineTextBox4.Text = "No description provided!";
        }

        public static void AfterCheck()
        {
            int num = 0;
            foreach (TreeNode node in Form1.CurrentForm.treeView3.Nodes)
                if (node != null && node.Checked)
                    num++;
            if (Form1.CurrentForm.treeView3.Nodes.Count == num && num != 0)
                Form1.CurrentForm.materialCheckbox4.Checked = true;
            else if (Form1.CurrentForm.treeView3.Nodes.Count > num && num == 0)
                Form1.CurrentForm.materialCheckbox4.Checked = false;
        }

        public static void StartGameAddons()
        {
            filesToEdit = new Dictionary<string, Dictionary<string, string[]>>();
            foreach (TreeNode node in Form1.CurrentForm.treeView3.Nodes)
            {
                if (node != null && node.Checked)
                {
                    Task.Delay(50).ContinueWith(delegate
                    {
                        Control.CheckForIllegalCrossThreadCalls = false;
                        // do job
                        StartPatching(node.FullPath, undo: false);
                        // unfreeze
                        waitPatchingTask.Set();
                    });
                    // freeze
                    waitPatchingTask.Reset();
                    waitPatchingTask.WaitOne();
                }
            }
            int o = 0;
            foreach (KeyValuePair<string, Dictionary<string, string[]>> item in filesToEdit)
            {
                int i = 0;
                foreach (KeyValuePair<string, string[]> item2 in item.Value)
                {
                    if (item2.Value[0] != item2.Value[1])
                    {
                        File.WriteAllBytes(Path.GetTempPath() + item.Key + "\\" + item2.Key, Encoding.Unicode.GetBytes(item2.Value[1]));
                        i++;
                    }
                }
                if (i > 0)
                {
                    Form1.CurrentForm.AddLauncherLog("Compiling " + item.Key, true);
                    isBusyWorking = true;
                    Task.Delay(0).ContinueWith(delegate
                    {
                        Control.CheckForIllegalCrossThreadCalls = false;
                        Pilao.FastColoredHandler.DatHandler.RepackDat(TempPath + item.Key, item.Key);
                        Form1.CurrentForm.AddLauncherLog("Compiled.", true);
                        o++;
                        isBusyWorking = false;
                    });
                    while (true) { Application.DoEvents(); if (!isBusyWorking) { break; } }
                }
                else
                {
                    Form1.CurrentForm.AddLauncherLog("Skipped " + item.Key, true);
                }
            }
            if (o > 0)
                PAK.PakHandler.PakRepack(PAK.PakHandler.ExtractedDir);
            foreach (TreeNode node2 in Form1.CurrentForm.treeView3.Nodes)
            {
                if (node2 != null && node2.Checked)
                {
                    node2.Checked = false;
                }
            }
        }

        public static void RestoreAddons()
        {
            filesToEdit = new Dictionary<string, Dictionary<string, string[]>>();
            foreach (TreeNode node in Form1.CurrentForm.treeView3.Nodes)
            {
                if (node != null && node.Checked)
                {
                    Task.Delay(50).ContinueWith(delegate
                    {
                        Control.CheckForIllegalCrossThreadCalls = false;
                        // do job
                        StartPatching(node.FullPath, undo: true);
                        // unfreeze
                        waitPatchingTask.Set();
                    });
                    // freeze
                    waitPatchingTask.Reset();
                    waitPatchingTask.WaitOne();
                }
            }
            int o = 0;
            foreach (KeyValuePair<string, Dictionary<string, string[]>> item in filesToEdit)
            {
                int i = 0;
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                foreach (KeyValuePair<string, string[]> item2 in item.Value)
                {
                    if (item2.Value[0] != item2.Value[1])
                    {
                        File.WriteAllBytes(Path.GetTempPath() + item.Key + "\\" + item2.Key, Encoding.Unicode.GetBytes(item2.Value[1]));
                        i++;
                    }
                }
                if (dictionary.Count > 0)
                {
                    Form1.CurrentForm.AddLauncherLog("Compiling " + item.Key, true);
                    Pilao.FastColoredHandler.DatHandler.RepackDat(TempPath + item.Key, item.Key);
                    o++;
                    Form1.CurrentForm.AddLauncherLog("Compiled.", true);
                }
                else
                {
                    Form1.CurrentForm.AddLauncherLog("Skipped " + item.Key, true);
                }
            }
            if (o > 0)
                PAK.PakHandler.PakRepack(PAK.PakHandler.ExtractedDir);
            foreach (TreeNode node2 in Form1.CurrentForm.treeView3.Nodes)
            {
                if (node2 != null && node2.Checked)
                {
                    node2.Checked = false;
                }
            }
        }

        public static void StartPatching(string filename, bool undo)
        {
            Dictionary<string, List<Dictionary<string, List<string>>>> topatch = new Dictionary<string, List<Dictionary<string, List<string>>>>();
            if (filename.EndsWith(" (.patch)"))
            {
                filename = filename.Replace(" (.patch)", ".patch");
                topatch = new AddonParser.LegacyAddons((Form1.CurrentForm.AddonsPaths.AddonsPath + "\\" + filename).Replace("\\\\", "\\"), filename).PatchAddon;
            }
            else if (filename.EndsWith(" (.xml)"))
            {
                filename = filename.Replace(" (.xml)", ".xml");
                topatch = new AddonParser.XmlAddons((Form1.CurrentForm.AddonsPaths.AddonsPath + "\\" + filename).Replace("\\\\", "\\"), filename).XmlAddon;
            }
            if (topatch.Count > 0)
            {
                foreach (string key in topatch.Keys)
                {
                    string text = key;
                    string text2;
                    string text3;
                    var Search = undo ? topatch[key][1]["Replace"] : topatch[key][0]["Search"];
                    var Replace = undo ? topatch[key][0]["Search"] : topatch[key][1]["Replace"];
                    if (undo)
                    {
                        Search.Reverse();
                        Replace.Reverse();
                    }
                    try
                    {
                        string[] array = text.Replace("\\\\", "\\").Split(new char[1] { '\\' }, 2);
                        text2 = array[0].Replace(".files", "").Trim();
                        text3 = array[1].ToLower().Trim();
                        if (!text2.Contains("[bit]"))
                        {
                            Form1.CurrentForm.AddLauncherLog(string.Format("Skipped addon: {0} because it did not contain [bit] to support both 32 & 64 bit files", filename.Replace(".patch", "").Replace(".xml", "")), true);
                            return;
                        }
                        text2 = text2.Replace("[bit]", "64");
                    }
                    catch
                    {
                        Form1.CurrentForm.AddLauncherLog(string.Format("Skipped addon: {0} because FileName field was incorrectly formatted", filename.Replace(".patch", "").Replace(".xml", "")), true);
                        return;
                    }
                    if (!PAK.PakHandler.FileList.ContainsKey(text2))
                    {
                        Form1.CurrentForm.AddLauncherLog(string.Format("Skipped Addon: {0} because the file {1} is missing", filename.Replace(".patch", "").Replace(".xml", ""), text2), true);
                        return;
                    }
                    if (!filesToEdit.ContainsKey(text2) || !filesToEdit[text2].ContainsKey(text3))
                    {
                        string text4 = PAK.PakHandler.FileList[text2];
                        if (!File.Exists(text4))
                        {
                            Form1.CurrentForm.AddLauncherLog(string.Format("Skipped addon: {0} because the file {1} is missing", filename.Replace(".patch", "").Replace(".xml", ""), text2), true);
                            return;
                        }
                        Dictionary<string, string> dictionary = new Dictionary<string, string>(); // string = internal file path, byte[] text in bytes
                        try
                        {
                            dictionary.Add(text3, Pilao.TreeViewHandler.ReadStringFromFile(text3, text4));
                            // dictionary = bNSDat.ExtractFile(text4, new List<string> { text3 }, text2.Contains("64"));
                        }
                        catch
                        {
                            Form1.CurrentForm.AddLauncherLog(string.Format("Error: file {2} is damaged/corrupted. Please do a file repair then try again", filename.Replace(".patch", "").Replace(".xml", ""), text3, text2), true);
                            return;
                        }
                        if (dictionary.Count <= 0)
                        {
                            Form1.CurrentForm.AddLauncherLog(string.Format("Skipped addon: {0} because the file {1} was not found in {2}", filename.Replace(".patch", "").Replace(".xml", ""), text3, text2), true);
                            return;
                        }
                        foreach (KeyValuePair<string, string> item in dictionary)
                        {
                            if (filesToEdit.ContainsKey(text2))
                            {
                                Dictionary<string, string[]> dictionary2 = filesToEdit[text2];
                                string @string = item.Value;
                                dictionary2.Add(item.Key.ToLower(), new string[2] { @string, @string });
                                filesToEdit[text2] = dictionary2;
                            }
                            else
                            {
                                Dictionary<string, string[]> dictionary3 = new Dictionary<string, string[]>();
                                string string2 = item.Value;
                                dictionary3.Add(item.Key.ToLower(), new string[2] { string2, string2 });
                                filesToEdit.Add(text2, dictionary3);
                            }
                        }
                    }
                    string text7 = filesToEdit[text2][text3][1];
                    // Beautify if on
                    if (Form1.CurrentForm.materialCheckbox5.Checked)
                    {
                        if (text3.EndsWith(".xml") || text3.EndsWith(".x16"))
                        {
                            text7 = Pilao.FastColoredHandler.FormatXml(text7);
                            //text7 = FixXmlString(text7); // dont know if needed
                            Form1.CurrentForm.AddLauncherLog("Beautified xml file", true);
                        }
                    }
                    for (int i = 0; Search.Count > i; i++)
                    {
                        if (text7.Contains(Search[i]))
                        {
                            text7 = text7.Replace(Search[i], Replace[i]);
                            Form1.CurrentForm.AddLauncherLog("Patched: " + (i + 1) + "/" + Replace.Count, true);
                        }
                        else
                        {
                            Form1.CurrentForm.AddLauncherLog("Error: Could not find " + (i + 1) + "/" + Replace.Count, true);
                            Form1.CurrentForm.AddLauncherLog(string.Format("Maybe already {0}?", undo ? "reverted" : "applied"), true);
                        }
                    }
                    if (text7 != filesToEdit[text2][text3][1])
                    {
                        try
                        {
                            if (text3.EndsWith(".xml") || text3.EndsWith(".x16"))
                            {
                                XmlDocument xmlDocument2 = new XmlDocument();
                                xmlDocument2.LoadXml(text7);
                            }
                            filesToEdit[text2][text3][1] = text7;
                        }
                        catch
                        {
                            Form1.CurrentForm.AddLauncherLog(string.Format("Restored {2}: {0} because the file {1} was invalid after replacing the text", filename.Replace(".patch", "").Replace(".xml", ""), text3, undo ? "addon" : "file"), true);
                            return;
                        }
                    }
                    Form1.CurrentForm.AddLauncherLog(string.Format("{0} Addon: {1}", undo ? "Reverted" : "Applied", filename.Replace(".patch", "").Replace(".xml", "")), true);
                }
            }
            else
            {
                Form1.CurrentForm.AddLauncherLog(string.Format("Skipped addon: {0} | Please verify xml.", filename.Replace(".patch", "").Replace(".xml", "")), true);
            }
        }
    }
}
