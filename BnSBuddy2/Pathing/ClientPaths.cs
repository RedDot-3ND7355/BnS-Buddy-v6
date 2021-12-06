using BnSBuddy2.Functions;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace BnSBuddy2.Pathing
{
    public class ClientPaths
    {
        // Globals
        List<string> regs = new List<string> { "SOFTWARE\\Wow6432Node\\NCWest\\BnS_UE4\\", "SOFTWARE\\NCWest\\BnS_UE4\\", "SOFTWARE\\Wow6432Node\\NCTaiwan\\TWBNS22\\", "SOFTWARE\\NCTaiwan\\TWBNS22\\", "SOFTWARE\\Wow6432Node\\INNOVA Co. SARL\\4game2.0\\Games\\Blade and Soul\\", "SOFTWARE\\INNOVA Co. SARL\\4game2.0\\Games\\Blade and Soul\\", "SOFTWARE\\Wow6432Node\\PlayNC\\BNS_JPN_UE4\\", "SOFTWARE\\PlayNC\\BNS_JPN_UE4\\", "SOFTWARE\\Wow6432Node\\Tencent\\BNS\\", "SOFTWARE\\Tencent\\BNS\\", "SOFTWARE\\Wow6432Node\\plaync\\BNS_LIVE\\", "SOFTWARE\\plaync\\BNS_LIVE\\", "SOFTWARE\\Wow6432Node\\plaync\\BNS_TEST\\", "SOFTWARE\\plaync\\BNS_TEST\\" };
        Dictionary<string, string> References = new Dictionary<string, string>();
        public Dictionary<string, string> Installs = new Dictionary<string, string>();
        RegistryKey localMachine = Registry.LocalMachine;
        // End Globals

        // Build References
        public ClientPaths()
        {
            // North America & Europe
            References.Add("SOFTWARE\\Wow6432Node\\NCWest\\BnS_UE4\\", "NA/EU");
            References.Add("SOFTWARE\\NCWest\\BnS_UE4\\", "NA/EU");
            // Taiwan
            References.Add("SOFTWARE\\Wow6432Node\\NCTaiwan\\TWBNS22\\", "Taiwan");
            References.Add("SOFTWARE\\NCTaiwan\\TWBNS22\\", "Taiwan");
            // Russia
            References.Add("SOFTWARE\\Wow6432Node\\INNOVA Co. SARL\\4game2.0\\Games\\Blade and Soul\\", "Russia");
            References.Add("SOFTWARE\\INNOVA Co. SARL\\4game2.0\\Games\\Blade and Soul\\", "Russia");
            // Japan
            References.Add("SOFTWARE\\Wow6432Node\\PlayNC\\BNS_JPN_UE4\\", "Japanese");
            References.Add("SOFTWARE\\PlayNC\\BNS_JPN_UE4\\", "Japanese");
            // China
            References.Add("SOFTWARE\\Wow6432Node\\Tencent\\BNS\\", "Chinese");
            References.Add("SOFTWARE\\Tencent\\BNS\\", "Chinese");
            // Korea
            References.Add("SOFTWARE\\Wow6432Node\\plaync\\BNS_LIVE\\", "Korean");
            References.Add("SOFTWARE\\plaync\\BNS_LIVE\\", "Korean");
            // Korea Test
            References.Add("SOFTWARE\\Wow6432Node\\plaync\\BNS_TEST\\", "Korean Test");
            References.Add("SOFTWARE\\plaync\\BNS_TEST\\", "Korean Test");
            // Garena
            References.Add("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FeatureUsage\\AppSwitched", "Garena");
            References.Add("Software\\Classes\\Local Settings\\Software\\Microsoft\\Windows\\Shell\\MuiCache", "Garena");
        }

        // Find all game servers via registry
        public void FindGameInstalls()
        {
            foreach (string regis in regs)
            {
                localMachine = Registry.LocalMachine.OpenSubKey(regis);
                if (localMachine != null)
                {
                    string path = (string)localMachine.GetValue("BaseDir");
                    if (path != null && path.Length > 0)
                    {
                        if (!Installs.ContainsKey(References[regis]))
                        {
                            Form1.CurrentForm.AddLauncherLog($"Found {References[regis]} Client" + Environment.NewLine + path);
                            if (References[regis] == "Japanese" && Directory.Exists(path + "\\UE4\\BNSR"))
                                Installs.Add(References[regis], path + "\\UE4\\");
                            else
                                Installs.Add(References[regis], path);
                        }
                    }
                }
            }
            FindGarena();
            FixPaths();
            /*
            * Add existing clients to server selection
            * North America
            * Europe
            * Taiwan
            * Japan
            * Korea
            * Russia
            * Garena
            * China
            * KR
            * - Live
            * - Test
            */
            foreach (string region in Installs.Keys)
            {
                if (region == "NA/EU")
                {
                    Form1.CurrentForm.materialComboBox2.Items.Add("North America");
                    Form1.CurrentForm.materialComboBox2.Items.Add("Europe");
                }
                else if (region.Contains("Korean"))
                {
                    if (!Form1.CurrentForm.materialComboBox2.Items.Contains("Korean"))
                        Form1.CurrentForm.materialComboBox2.Items.Add("Korean");
                    if (region.EndsWith("Test"))
                        Form1.CurrentForm.materialComboBox4.Items.Add("Test");
                    else
                        Form1.CurrentForm.materialComboBox4.Items.Add("Live");
                    Form1.CurrentForm.materialComboBox4.SelectedIndex = 0;
                }
                else
                    Form1.CurrentForm.materialComboBox2.Items.Add(region);
            }
            // Continue
            if (Installs.Count > 1 && Form1.CurrentForm.routine.CurrentSettings["defaultset"].ToString() == "false")
            {
                Form1.CurrentForm.AddLauncherLog("Multiple Installations Found!");
                string server = Prompt.MultipleInstalltion(Installs);
                Form1.CurrentForm.materialComboBox2.SelectedIndex = Form1.CurrentForm.materialComboBox2.FindString(server);
                server = RegionConvert.Convert(server);
                SaveDefault(Installs[server]);
                Form1.CurrentForm.routine.CurrentSettings["lastserver"] = Form1.CurrentForm.materialComboBox2.SelectedItem.ToString();
                Prompt.Popup("Notice: You can always reset the default installation in settings.");
            }
            else if (Installs.Count == 1)
            {
                foreach (string _server in Installs.Keys)
                {
                    string server = "";
                    SaveDefault(Installs[_server]);
                    // NA/EU
                    if (_server == "NA/EU")
                        server = "North America";
                    else // ALL
                        server = _server;
                    Form1.CurrentForm.materialComboBox2.SelectedIndex = Form1.CurrentForm.materialComboBox2.FindString(server);
                }
            }
            else if (Form1.CurrentForm.routine.CurrentSettings["defaultset"].ToString() == "true" && Form1.CurrentForm.routine.CurrentSettings["lastserver"].ToString().Length > 0 && Form1.CurrentForm.materialComboBox2.Items.Contains(Form1.CurrentForm.routine.CurrentSettings["lastserver"].ToString()))
                Form1.CurrentForm.materialComboBox2.SelectedIndex = Form1.CurrentForm.materialComboBox2.FindString(Form1.CurrentForm.routine.CurrentSettings["lastserver"]);
            else
                Form1.CurrentForm.materialComboBox2.SelectedIndex = 0;
            string selectedserver = Form1.CurrentForm.materialComboBox2.SelectedItem.ToString();
            selectedserver = RegionConvert.Convert(selectedserver);
            FindLanguages(Installs[selectedserver]);
        }

        private void FixPaths()
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>(Installs);
            foreach (string Key in keyValuePairs.Keys)
                if (Installs[Key] != null)
                    if (!Installs[Key].ToString().EndsWith(@"\"))
                        Installs[Key] = Installs[Key].ToString() + @"\";
        }

        // Save new server selected
        public void SaveDefault(string val)
        {
            Form1.CurrentForm.routineChanger("defaultset", "true", true);
            Form1.CurrentForm.routineChanger("_default", val, true);
            Form1.CurrentForm.materialTextBox3.Text = val;
        }

        // Find Languages when switching Region
        public void FindLanguages(string ClientPath)
        {
            if (Form1.CurrentForm.materialComboBox3.Items.Count > 0)
                Form1.CurrentForm.materialComboBox3.Items.Clear();
            if (Form1.CurrentForm.materialComboBox6.Items.Count > 0)
                Form1.CurrentForm.materialComboBox6.Items.Clear();
            if (Directory.Exists(ClientPath + "\\BNSR\\Content\\local"))
            {
                DirectoryInfo[] directories = new DirectoryInfo(ClientPath + "\\BNSR\\Content\\local").GetDirectories();
                foreach (DirectoryInfo dir in directories)
                {
                    string autodir = Path.GetFileName(dir.ToString());
                    if (Directory.Exists(ClientPath + "\\BNSR\\Content\\local\\" + autodir + "\\data"))
                    {
                        DirectoryInfo[] directoryInfo = new DirectoryInfo(ClientPath + "\\BNSR\\Content\\local\\" + autodir).GetDirectories();
                        foreach (DirectoryInfo lang in directoryInfo)
                        {
                            if (Directory.Exists(ClientPath + "\\BNSR\\Content\\local\\" + autodir + "\\" + lang.ToString() + "\\data"))
                            {
                                Form1.CurrentForm.materialComboBox3.Items.Add(lang.ToString());
                                Form1.CurrentForm.materialComboBox6.Items.Add(lang.ToString());
                            }
                        }
                    }
                }
                if (Form1.CurrentForm.materialComboBox3.Items.Count > 1 && Form1.CurrentForm.routine.CurrentSettings["langset"] == "false")
                {
                    Form1.CurrentForm.AddLauncherLog("Multiple Languages Found!");
                    string Language = Prompt.MultipleLang(Form1.CurrentForm.materialComboBox3.Items.Cast<Object>().Select(item => item.ToString()).ToList());
                    Form1.CurrentForm.materialComboBox3.SelectedIndex = Form1.CurrentForm.materialComboBox3.FindStringExact(Language);
                    Form1.CurrentForm.materialComboBox6.SelectedIndex = Form1.CurrentForm.materialComboBox3.SelectedIndex;
                    Form1.CurrentForm.routineChanger("langpath", Language, true);
                    Form1.CurrentForm.routineChanger("langset", "true", true);
                }
                else if (Form1.CurrentForm.routine.CurrentSettings["langset"] == "false")
                {
                    Form1.CurrentForm.materialComboBox3.SelectedIndex = 0;
                    Form1.CurrentForm.materialComboBox6.SelectedIndex = 0;
                    Form1.CurrentForm.routineChanger("langpath", Form1.CurrentForm.materialComboBox3.SelectedItem.ToString(), true);
                    Form1.CurrentForm.routineChanger("langset", "true", true);
                }
                else if (Form1.CurrentForm.routine.CurrentSettings["langset"] == "true")
                {
                    if (Form1.CurrentForm.materialComboBox3.Items.Contains(Form1.CurrentForm.routine.CurrentSettings["langpath"]))
                    {
                        Form1.CurrentForm.materialComboBox3.SelectedIndex = Form1.CurrentForm.materialComboBox3.FindStringExact(Form1.CurrentForm.routine.CurrentSettings["langpath"]);
                        Form1.CurrentForm.materialComboBox6.SelectedIndex = Form1.CurrentForm.materialComboBox3.SelectedIndex;
                    }
                    else
                    {
                        Form1.CurrentForm.materialComboBox3.SelectedIndex = 0;
                        Form1.CurrentForm.materialComboBox6.SelectedIndex = Form1.CurrentForm.materialComboBox3.SelectedIndex;
                    }
                }
            }
        }

        // Find Garena via seperate registry
        private void FindGarena()
        {
            // Check if already found
            if (Form1.CurrentForm.routine.CurrentSettings["garenaclientpath"].ToString().Length > 0)
            {
                Installs.Add("Garena", Form1.CurrentForm.routine.CurrentSettings["garenaclientpath"].ToString());
                return;
            }
            // Continue
            string ServicePath = "\\32835\\BNSR";
            string GarenaLocalMachinePaths = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FeatureUsage\\AppSwitched";
            string GarenaCurrentUserPaths = "Software\\Classes\\Local Settings\\Software\\Microsoft\\Windows\\Shell\\MuiCache";
            // Check LocalMachine Cache
            localMachine = Registry.LocalMachine.OpenSubKey(GarenaLocalMachinePaths);
            if (localMachine != null)
            {
                foreach (string pathname in localMachine.GetSubKeyNames())
                {
                    if (pathname.Contains(ServicePath))
                    {
                        string tmp_path = Path.GetDirectoryName(pathname);
                        tmp_path = tmp_path.Substring(0, tmp_path.IndexOf("32835") + 1);
                        string path = tmp_path + "\\";
                        path = path.Replace(@"\\", @"\");
                        Form1.CurrentForm.AddLauncherLog($"Found {References[GarenaLocalMachinePaths]} Client" + Environment.NewLine + path);
                        Installs.Add("Garena", path);
                        Form1.CurrentForm.routineChanger("garenaclientpath", path, true);
                    }
                }
            }
            // Check CurrentUser Cache
            localMachine = Registry.CurrentUser.OpenSubKey(GarenaCurrentUserPaths);
            if (!Installs.ContainsKey("Garena") && localMachine != null)
            {
                foreach (string pathname in localMachine.GetSubKeyNames())
                {
                    if (pathname.Contains(ServicePath))
                    {
                        string tmp_path = Path.GetDirectoryName(pathname);
                        tmp_path = tmp_path.Substring(0, tmp_path.IndexOf("32835") + 1);
                        string path = tmp_path + "\\";
                        path = path.Replace(@"\\", @"\");
                        Form1.CurrentForm.AddLauncherLog($"Found {References[GarenaCurrentUserPaths]} Client" + Environment.NewLine + path);
                        Installs.Add("Garena", path);
                        Form1.CurrentForm.routineChanger("garenaclientpath", path, true);
                    }
                }
            }
            // Manually Get Folder
            List<RegistryKey> keys = new List<RegistryKey>() { Registry.LocalMachine.OpenSubKey("SOFTWARE\\Garena\\gxx\\"), Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node\\Garena\\gxx\\") };
            foreach (RegistryKey key in keys)
            {
                localMachine = key;
                if (localMachine != null && !Installs.ContainsKey("Garena") && Form1.CurrentForm.routine.CurrentSettings["garenaclientpath"].ToString().Length == 0)
                {
                    string text5 = (string)localMachine.GetValue("Path");
                    if (text5.Length > 0 && File.Exists(text5))
                    {
                        FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                        folderBrowserDialog.Description = "Select 32835 folder for garena region game location";
                        if (folderBrowserDialog.ShowDialog().ToString() == "OK")
                        {
                            if (Directory.Exists(folderBrowserDialog.SelectedPath + "\\BNSR"))
                            {
                                text5 = folderBrowserDialog.SelectedPath + "\\";
                                Form1.CurrentForm.AddLauncherLog($"Found {References[GarenaCurrentUserPaths]} Client" + Environment.NewLine + text5);
                                Installs.Add("Garena", text5);
                                Form1.CurrentForm.routineChanger("garenaclientpath", text5, true);
                            }
                            else
                                Prompt.Popup("Error: Invalid Path! Browse for BnS Folder. | Path: " + folderBrowserDialog.SelectedPath);
                        }
                        else
                        {
                            Prompt.Popup("Error: Cancelled operation!");
                            Form1.CurrentForm.KillApp();
                        }
                    }
                }
            }
        }
    }
}
