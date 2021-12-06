using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;

namespace BnSBuddy2.Functions
{
    public class FTH
    {
        // Globals
        private Dictionary<string, string> CompatEntriesLocal = new Dictionary<string, string>();
        private Dictionary<string, string> CompatEntriesUser = new Dictionary<string, string>();
        private Dictionary<string, string> FTHEntries = new Dictionary<string, string>();
        // End Globals

        public void Warning()
        {
            int count = CompatEntriesLocal.Count + CompatEntriesUser.Count;
            string str = "entry";
            if (count > 1)
                str = "entries";
            Prompt.Popup($"  You have {count} {str}. \r\n  Gameplay performance might be affected. \r\n  You can either Ignore this warning, it is NOT an error. \r\n  Or to remove the warning, pressing the \"Clr Compatibility\" button under \"Extras\".  ");
        }

        public void FTHToggle(bool enable)
        {
            try
            {
                Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\FTH\\", writable: true).SetValue("Enabled", enable ? 0x00000001 : 0x00000000, RegistryValueKind.DWord);
                Prompt.Popup("A reboot of your computer may be required before this takes effect.");
            }
            catch
            {
                Prompt.Popup("Failed to Toggle FTH.");
            }
        }

        public void Exclude(string PathToExec)
        {
            string NameOfExec = Path.GetFileName(PathToExec);
            RegistryKey localMachine = Registry.LocalMachine;
            localMachine = localMachine.OpenSubKey("SOFTWARE\\Microsoft\\FTH\\", writable: true);
            if (localMachine != null && localMachine.GetValue("ExclusionList") != null) 
            {
                List<string> val = new List<string>((string[])localMachine.GetValue("ExclusionList"));
                val.Add(NameOfExec);
                localMachine.SetValue("ExclusionList", val.ToArray(), RegistryValueKind.MultiString);
            }
            Form1.CurrentForm.materialButton59.Enabled = false;
            Form1.CurrentForm.materialLabel10.Text = "Y";
        }

        public void ClearCompat()
        {
            foreach (string key in CompatEntriesLocal.Keys)
            {
                try
                {
                    if (key != null)
                    {
                        string text2 = CompatEntriesLocal[key].Replace("HKEY_LOCAL_MACHINE\\", "");
                        RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey(text2, writable: true);
                        registryKey2.DeleteValue(key, throwOnMissingValue: false);
                    }
                }
                catch (Exception ex)
                {
                    Prompt.Popup("Could not delete the key: " + CompatEntriesLocal[key] + " | Value: " + key + Environment.NewLine + "Error: " + ex.ToString());
                }
            }
            foreach (string key in CompatEntriesUser.Keys)
            {
                try
                {
                    if (key != null)
                    {
                        string text2 = CompatEntriesUser[key].Replace("HKEY_CURRENT_USER\\", "");
                        RegistryKey registryKey2 = Registry.CurrentUser.OpenSubKey(text2, writable: true);
                        registryKey2.DeleteValue(key, throwOnMissingValue: false);
                    }
                }
                catch (Exception ex)
                {
                    Prompt.Popup("Could not delete the key: " + CompatEntriesUser[key] + " | Value: " + key + Environment.NewLine + "Error: " + ex.ToString());
                }
            }
            Form1.CurrentForm.pictureBox4.Visible = false;
            Form1.CurrentForm.materialButton17.Enabled = false;
        }

        public void ClearFTH()
        {
            foreach (string key in FTHEntries.Keys)
            {
                try
                {
                    if (key != null)
                    {
                        string text2 = key.Replace("HKEY_LOCAL_MACHINE\\", "");
                        RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey(text2, writable: true);
                        registryKey2.DeleteValue(FTHEntries[key], throwOnMissingValue: false);
                    }
                }
                catch (Exception ex)
                {
                    Prompt.Popup("Could not delete the key: " + key + " | Value: " + FTHEntries[key] + Environment.NewLine + "Error: " + ex.ToString());
                }
            }
            Form1.CurrentForm.materialButton16.Enabled = false;
            Form1.CurrentForm.materialLabel37.Text = "0";
            Prompt.Popup("A reboot of your computer may be required before this takes effect.");
        }

        // Check Overall FTH
        public void CheckFTH(string PathToExec)
        {
            if (File.Exists(PathToExec))
            {
                string NameOfExec = Path.GetFileName(PathToExec);
                FTHStatus(NameOfExec);
                CheckFTHEntries(NameOfExec);
                CheckCompatEntries(PathToExec);
            }
        }

        // Check FTH Entries
        private void CheckFTHEntries(string NameOfExec)
        {
            RegistryKey localMachine = Registry.LocalMachine;
            localMachine = localMachine.OpenSubKey("SOFTWARE\\Microsoft\\FTH\\State\\", writable: true);
            if (localMachine != null)
                foreach (string text2 in localMachine.GetValueNames())
                    if (text2 != null && text2.ToString().Contains(NameOfExec) && !FTHEntries.ContainsKey("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\FTH\\State\\"))
                        FTHEntries.Add("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\FTH\\State\\", text2);
            if (FTHEntries.Count > 0)
            {
                Form1.CurrentForm.materialLabel37.Text = FTHEntries.Count.ToString();
                Form1.CurrentForm.materialButton16.Enabled = true;
                Prompt.Popup($"You have {FTHEntries.Count} FTH entr{(FTHEntries.Count > 1 ? "ies" : "y")} \r\n Please, follow these steps: \r\n - Go to the Extras tab \r\n - Clear the entr{(FTHEntries.Count > 1 ? "ies" : "y")} \r\n - Turn off FTH (recommended) \r\n - Reboot your PC \r\n If you don't follow these steps, you may have performance issues.");
            }
        }

        // Check Compatibility Entries
        private void CheckCompatEntries(string PathToExec)
        {
            CompatEntriesLocal = new Dictionary<string, string>();
            CompatEntriesUser = new Dictionary<string, string>();
            // Local Machine
            RegistryKey localMachine = Registry.LocalMachine;
            localMachine = localMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\AppCompatFlags\\Layers\\");
            if (localMachine != null)
                foreach (string text in localMachine.GetValueNames())
                    if (text != null)
                        if (!CompatEntriesLocal.ContainsKey(text))
                            if (PathToExec == text.ToString())
                                CompatEntriesLocal.Add(text, "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\AppCompatFlags\\Layers\\");
            // Current User
            localMachine = Registry.CurrentUser;
            localMachine = localMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\AppCompatFlags\\Layers\\");
            if (localMachine != null)
                foreach (string text3 in localMachine.GetValueNames())
                    if (text3 != null)
                        if (!CompatEntriesUser.ContainsKey(text3))
                            if (PathToExec == text3.ToString())
                                CompatEntriesUser.Add(text3, "HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\AppCompatFlags\\Layers\\");
            if ((CompatEntriesLocal.Count + CompatEntriesUser.Count) > 0)
            {
                Form1.CurrentForm.materialButton17.Enabled = true;
                Form1.CurrentForm.pictureBox4.Visible = true;
            }
        }

        // Check FTH Status
        private void FTHStatus(string NameOfExec)
        {
            RegistryKey localMachine = Registry.LocalMachine;
            localMachine = localMachine.OpenSubKey("SOFTWARE\\Microsoft\\FTH\\", true);
            if (localMachine != null)
            {
                // Check IF enabled
                if (localMachine.GetValue("Enabled") != null)
                {
                    if ((int)localMachine.GetValue("Enabled") == 0x00000000)
                        Form1.CurrentForm.materialSwitch5.Checked = false;
                }
                else
                    Form1.CurrentForm.materialSwitch5.Checked = false;
                // Check IF excluded
                if (localMachine.GetValue("ExclusionList") != null)
                {
                    List<string> list = new List<string>((string[])localMachine.GetValue("ExclusionList"));
                    if (list.Contains(NameOfExec))
                    {
                        Form1.CurrentForm.materialButton59.Enabled = false;
                        Form1.CurrentForm.materialLabel10.Text = "Y";
                    }
                }
                else
                    Form1.CurrentForm.materialButton59.Enabled = false;
            }
            else
            {
                Form1.CurrentForm.materialSwitch5.Checked = false;
                Form1.CurrentForm.materialSwitch5.Enabled = false;
            }
        }

    }
}
