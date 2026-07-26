using BnSBuddy2.Functions;
using BnSBuddy2.PAK;
using Microsoft.Web.WebView2.Core;
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
        // Stea,
        List<string> steamregs = new List<string> { "SOFTWARE\\WOW6432Node\\Valve\\Steam", "SOFTWARE\\Valve\\Steam" }; // Steam Registry Path for BnS
        // Globals
        List<string> regs = new List<string> 
        {
            "SOFTWARE\\Wow6432Node\\plaync\\TWBNSNEO\\",
            "SOFTWARE\\plaync\\TWBNSNEO\\",
            "SOFTWARE\\Wow6432Node\\plaync\\TWBNSUE4\\",
            "SOFTWARE\\plaync\\TWBNSUE4\\", 
            "SOFTWARE\\Wow6432Node\\INNOVA Co. SARL\\4game2.0\\Games\\Blade and Soul\\", 
            "SOFTWARE\\INNOVA Co. SARL\\4game2.0\\Games\\Blade and Soul\\",
            "SOFTWARE\\Wow6432Node\\plaync\\BNS_JPN_UE4\\",
            "SOFTWARE\\plaync\\BNS_JPN_UE4\\",
            "SOFTWARE\\Wow6432Node\\plaync\\BNS_NEO_JP\\",
            "SOFTWARE\\plaync\\BNS_NEO_JP\\",
            "SOFTWARE\\Wow6432Node\\Tencent\\Blade_and_Soul\\", 
            "SOFTWARE\\Tencent\\Blade_and_Soul\\",
            "SOFTWARE\\Wow6432Node\\plaync\\BNSNEO_LIVE\\",
            "SOFTWARE\\plaync\\BNSNEO_LIVE\\",
            "SOFTWARE\\Wow6432Node\\plaync\\BNS_LIVE\\", 
            "SOFTWARE\\plaync\\BNS_LIVE\\", 
            "SOFTWARE\\Wow6432Node\\plaync\\BNS_TEST\\", 
            "SOFTWARE\\plaync\\BNS_TEST\\", 
            "SOFTWARE\\WOW6432Node\\plaync\\BnS_UE4", 
            "SOFTWARE\\plaync\\BnS_UE4",
            "SOFTWARE\\WOW6432Node\\plaync\\BNSNEO",
            "SOFTWARE\\plaync\\BNSNEO"
        };
        Dictionary<string, string> References = new Dictionary<string, string>();
        public Dictionary<string, string> Installs = new Dictionary<string, string>();
        RegistryKey localMachine = Registry.LocalMachine;
        // End Globals

        // Build References
        public ClientPaths()
        {
            // Purple (NA/EU) LIVE
            References.Add("SOFTWARE\\WOW6432Node\\plaync\\BnS_UE4", "NA/EU Live");
            References.Add("SOFTWARE\\plaync\\BnS_UE4", "NA/EU Live");
            // Purple (NA/EU) NEO
            References.Add("SOFTWARE\\WOW6432Node\\plaync\\BNSNEO", "NA/EU Neo");
            References.Add("SOFTWARE\\plaync\\BNSNEO", "NA/EU Neo");
            // Purple (Taiwan) LIVE
            References.Add("SOFTWARE\\Wow6432Node\\plaync\\TWBNSUE4\\", "Taiwan Live");
            References.Add("SOFTWARE\\plaync\\TWBNSUE4\\", "Taiwan Live");
            // Purple (Taiwan) NEO
            References.Add("SOFTWARE\\Wow6432Node\\plaync\\TWBNSNEO\\", "Taiwan Neo");
            References.Add("SOFTWARE\\plaync\\TWBNSNEO\\", "Taiwan Neo");
            // Russia [No Purple]
            References.Add("SOFTWARE\\Wow6432Node\\INNOVA Co. SARL\\4game2.0\\Games\\Blade and Soul\\", "Russia");
            References.Add("SOFTWARE\\INNOVA Co. SARL\\4game2.0\\Games\\Blade and Soul\\", "Russia");
            // Purple (Japan) LIVE
            References.Add("SOFTWARE\\Wow6432Node\\plaync\\BNS_JPN_UE4\\", "Japanese Live");
            References.Add("SOFTWARE\\plaync\\BNS_JPN_UE4\\", "Japanese Live");
            // Purple (Japan) NEO
            References.Add("SOFTWARE\\Wow6432Node\\plaync\\BNS_NEO_JP\\", "Japanese Neo");
            References.Add("SOFTWARE\\plaync\\BNS_NEO_JP\\", "Japanese Neo");
            // China [No Purple]
            References.Add("SOFTWARE\\Wow6432Node\\Tencent\\Blade_and_Soul\\", "Chinese");
            References.Add("SOFTWARE\\Tencent\\Blade_and_Soul\\", "Chinese");
            // Purple (Korea)
            References.Add("SOFTWARE\\Wow6432Node\\plaync\\BNS_LIVE\\", "Korean Live");
            References.Add("SOFTWARE\\plaync\\BNS_LIVE\\", "Korean Live");
            // Purple (Korea Test)
            References.Add("SOFTWARE\\Wow6432Node\\plaync\\BNS_TEST\\", "Korean Test");
            References.Add("SOFTWARE\\plaync\\BNS_TEST\\", "Korean Test");
            // Purple (Korea Neo)
            References.Add("SOFTWARE\\Wow6432Node\\plaync\\BNSNEO_LIVE\\", "Korean Neo");
            References.Add("SOFTWARE\\plaync\\BNSNEO_LIVE\\", "Korean Neo");
            // Garena [No Purple]
            References.Add("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FeatureUsage\\AppSwitched", "Garena");
            References.Add("Software\\Classes\\Local Settings\\Software\\Microsoft\\Windows\\Shell\\MuiCache", "Garena");
            // Steam [No Purple]
            References.Add("SOFTWARE\\WOW6432Node\\Valve\\Steam", "Steam Neo");
            References.Add("SOFTWARE\\Valve\\Steam", "Steam Neo");
        }

        // Find all game servers via registry
        public void FindGameInstalls()
        {
            foreach (string regis in regs)
            {
                localMachine = Registry.LocalMachine.OpenSubKey(regis);
                if (localMachine != null)
                {
                    string path = "";
                    if (localMachine.GetValue("BaseDir") != null && localMachine.GetValue("BaseDir").GetType() == typeof(String[]))
                    {
                        var obj = localMachine.GetValue("BaseDir") as string[];
                        path = obj[0].Replace("\r\n", "");
                    }
                    else if (localMachine.GetValue("BaseDir") != null)
                        path = localMachine.GetValue("BaseDir").ToString();
                    else if (localMachine.GetValue("InstallPath") != null)
                        path = localMachine.GetValue("InstallPath").ToString();
                    else if (localMachine.GetValue("Path") != null)
                        path = localMachine.GetValue("Path").ToString();

                    if (path != null && path.Length > 0)
                    {
                        if (!Installs.ContainsKey(References[regis]))
                        {
                            if (Directory.Exists(path))
                            {
                                Form1.CurrentForm.AddLauncherLog($"Found {References[regis]} Client" + Environment.NewLine + path);
                                if (References[regis] == "Japanese" && Directory.Exists(path + "\\UE4\\BNSR"))
                                    Installs.Add(References[regis], path.Replace("\r\n", "") + "\\UE4\\");
                                else
                                    Installs.Add(References[regis], path.Replace("\r\n", ""));
                            }
                            else
                            {
                                Prompt.Popup($"The registry path for region {References[regis]} does not exist! \r\n Path: {path} \r\n Please fix the registry value to match current path. \r\n So that you can play on this region! \r\n Or delete the registry path to {regis} if you have previously uninstalled that client.");
                                new KillApp();
                            }
                        }
                    }
                }
            }
            FindSteam();
            FindGarena();
            FixPaths();
            /*
            * Add existing clients to server selection
            * North America
            * Purple (same as na/eu)
            * Europe
            * Taiwan
            * Japan
            * Korea
            * Russia
            * Garena
            * China
            * KR / Purple
            * - Live
            * - Test
            * - Neo
            * Steam
            */
            foreach (string region in Installs.Keys)
            {
                if (region.Contains("Japanese"))
                {
                    if (!Form1.CurrentForm.materialComboBox2.Items.Contains("Japanese"))
                        Form1.CurrentForm.materialComboBox2.Items.Add("Japanese");
                    if (region.EndsWith("Live") && !Form1.CurrentForm.materialComboBox4.Items.Contains("Live"))
                        Form1.CurrentForm.materialComboBox4.Items.Add("Live");
                    else if (region.EndsWith("Neo") && !Form1.CurrentForm.materialComboBox4.Items.Contains("Neo"))
                        Form1.CurrentForm.materialComboBox4.Items.Add("Neo");
                }
                else if (region.Contains("Taiwan"))
                {
                    if (!Form1.CurrentForm.materialComboBox2.Items.Contains("Taiwan"))
                        Form1.CurrentForm.materialComboBox2.Items.Add("Taiwan");
                    if (region.EndsWith("Live") && !Form1.CurrentForm.materialComboBox4.Items.Contains("Live"))
                        Form1.CurrentForm.materialComboBox4.Items.Add("Live");
                    else if (region.EndsWith("Neo") && !Form1.CurrentForm.materialComboBox4.Items.Contains("Neo"))
                        Form1.CurrentForm.materialComboBox4.Items.Add("Neo");
                }
                else if (region.Contains("NA/EU") || region == "Purple")
                {
                    if (!Form1.CurrentForm.materialComboBox2.Items.Contains("North America"))
                        Form1.CurrentForm.materialComboBox2.Items.Add("North America");
                    if (!Form1.CurrentForm.materialComboBox2.Items.Contains("Europe"))
                        Form1.CurrentForm.materialComboBox2.Items.Add("Europe");
                    if (region.EndsWith("Live") && !Form1.CurrentForm.materialComboBox4.Items.Contains("Live"))
                        Form1.CurrentForm.materialComboBox4.Items.Add("Live");
                    else if (region.EndsWith("Neo") && !Form1.CurrentForm.materialComboBox4.Items.Contains("Neo"))
                        Form1.CurrentForm.materialComboBox4.Items.Add("Neo");
                }
                else if (region.Contains("Korean"))
                {
                    if (!Form1.CurrentForm.materialComboBox2.Items.Contains("Korean"))
                        Form1.CurrentForm.materialComboBox2.Items.Add("Korean");
                    if (region.EndsWith("Test") && !Form1.CurrentForm.materialComboBox4.Items.Contains("Test"))
                        Form1.CurrentForm.materialComboBox4.Items.Add("Test");
                    else if (region.EndsWith("Live") && !Form1.CurrentForm.materialComboBox4.Items.Contains("Live"))
                        Form1.CurrentForm.materialComboBox4.Items.Add("Live");
                    else if (region.EndsWith("Neo") && !Form1.CurrentForm.materialComboBox4.Items.Contains("Neo"))
                        Form1.CurrentForm.materialComboBox4.Items.Add("Neo");
                
                } 
                else if (region.Contains("Steam"))
                {
                    if (!Form1.CurrentForm.materialComboBox2.Items.Contains("Steam NA"))
                        Form1.CurrentForm.materialComboBox2.Items.Add("Steam NA");
                    if (!Form1.CurrentForm.materialComboBox2.Items.Contains("Steam EU"))
                        Form1.CurrentForm.materialComboBox2.Items.Add("Steam EU");
                    if (region.EndsWith("Neo") && !Form1.CurrentForm.materialComboBox4.Items.Contains("Neo"))
                        Form1.CurrentForm.materialComboBox4.Items.Add("Neo");
                }
                else
                    Form1.CurrentForm.materialComboBox2.Items.Add(region);
            }
            // Continue
            if (Installs.Count > 1 && Form1.CurrentForm.routine.CurrentSettings["defaultset"].ToString() == "false")
            {
                Form1.CurrentForm.AddLauncherLog("Multiple Installations Found!");
                string server = Prompt.MultipleInstalltion(Installs);
                if (server.Contains("Japanese"))
                {
                    Form1.CurrentForm.materialComboBox2.SelectedIndex = Form1.CurrentForm.materialComboBox2.FindString("Japanese");
                    Form1.CurrentForm.materialComboBox4.SelectedIndex = Form1.CurrentForm.materialComboBox4.FindString(server.Replace("Japanese ", ""));
                }
                else if (server.Contains("Steam"))
                {
                    Form1.CurrentForm.materialComboBox2.SelectedIndex = Form1.CurrentForm.materialComboBox2.FindString("Steam");
                    Form1.CurrentForm.materialComboBox4.SelectedIndex = Form1.CurrentForm.materialComboBox4.FindString(server.Replace("Steam ", ""));
                }
                else if (server.Contains("Taiwan"))
                {
                    Form1.CurrentForm.materialComboBox2.SelectedIndex = Form1.CurrentForm.materialComboBox2.FindString("Taiwan");
                    Form1.CurrentForm.materialComboBox4.SelectedIndex = Form1.CurrentForm.materialComboBox4.FindString(server.Replace("Taiwan ", ""));
                }
                else if (server.Contains("Korean"))
                {
                    Form1.CurrentForm.materialComboBox2.SelectedIndex = Form1.CurrentForm.materialComboBox2.FindString("Korean");
                    Form1.CurrentForm.materialComboBox4.SelectedIndex = Form1.CurrentForm.materialComboBox4.FindString(server.Replace("Korean ", ""));
                }
                else if (server.Contains("NA/EU"))
                {
                    Form1.CurrentForm.materialComboBox2.SelectedIndex = Form1.CurrentForm.materialComboBox2.FindString("North America");
                    Form1.CurrentForm.materialComboBox4.SelectedIndex = Form1.CurrentForm.materialComboBox4.FindString(server.Replace("NA/EU ", "")); ;
                }
                else
                    Form1.CurrentForm.materialComboBox2.SelectedIndex = Form1.CurrentForm.materialComboBox2.FindString(server);
                server = RegionConvert.Convert(server);
                SaveDefault(Installs[server]);
                // Multi Server Support
                string currsel = Form1.CurrentForm.materialComboBox2.SelectedItem.ToString();
                if (currsel == "North America" || currsel == "Europe" || currsel == "Korean" || currsel == "Taiwan" || currsel == "Steam NA" || currsel == "Steam EU" || currsel == "Japanese")
                    Form1.CurrentForm.routine.CurrentSettings["lastserver"] = currsel + " " + Form1.CurrentForm.materialComboBox4.SelectedItem.ToString();
                else
                    Form1.CurrentForm.routine.CurrentSettings["lastserver"] = currsel;
                // Prompt Notice
                Prompt.Popup("Notice: You can always reset the default installation in settings.");
            }
            if (Form1.CurrentForm.routine.CurrentSettings["defaultset"].ToString() == "true" && Form1.CurrentForm.routine.CurrentSettings["lastserver"].ToString().Length > 0 && Form1.CurrentForm.materialComboBox2.Items.Contains(Form1.CurrentForm.routine.CurrentSettings["lastserver"].ToString().Replace(" Neo", "").Replace(" Live", "").Replace(" Test", "")))
            {
                // Multi Server Support
                string currsaved = Form1.CurrentForm.routine.CurrentSettings["lastserver"];
                if (currsaved.Contains("Korean") || currsaved.Contains("North America") || currsaved.Contains("Europe") || currsaved.Contains("Taiwan") || currsaved.Contains("Japanese") || currsaved.Contains("Steam NA") || currsaved.Contains("Steam EU"))
                {
                    Form1.CurrentForm.materialComboBox2.SelectedIndex = Form1.CurrentForm.materialComboBox2.FindString(currsaved.Replace(" Neo", "").Replace(" Live", "").Replace(" Test", ""));
                    string cleanedclientsel = currsaved.Replace("Korean ", "").Replace("North America ", "").Replace("Europe ", "").Replace("Taiwan ", "").Replace("Japanese ", "").Replace("Steam NA", "").Replace("Steam EU", "");
                    if (!cleanedclientsel.Contains("Neo") && !cleanedclientsel.Contains("Live") && !cleanedclientsel.Contains("Test"))
                        Form1.CurrentForm.materialComboBox4.SelectedIndex = 0;
                    else
                        Form1.CurrentForm.materialComboBox4.SelectedIndex = Form1.CurrentForm.materialComboBox4.FindString(cleanedclientsel);
                }
                else
                    Form1.CurrentForm.materialComboBox2.SelectedIndex = Form1.CurrentForm.materialComboBox2.FindString(currsaved);
            }
            else if (Installs.Count == 1)
            {
                foreach (string _server in Installs.Keys)
                {
                    string server = "";
                    SaveDefault(Installs[_server]);
                    // NA/EU
                    if (_server.Contains("NA/EU"))
                        server = "North America";
                    // Steam NA/EU
                    else if (_server.Contains("Steam"))
                        server = "Steam NA";
                    else // ALL
                        server = _server;
                    Form1.CurrentForm.materialComboBox2.SelectedIndex = Form1.CurrentForm.materialComboBox2.FindString(server);
                }
            }
            else if (Form1.CurrentForm.materialComboBox2.Items.Count != 0)
                Form1.CurrentForm.materialComboBox2.SelectedIndex = 0;
            if (Form1.CurrentForm.materialComboBox2.SelectedItem != null)
            {
                string selectedserver = Form1.CurrentForm.materialComboBox2.SelectedItem.ToString();
                selectedserver = RegionConvert.Convert(selectedserver);
                FindLanguages(Installs[selectedserver]);
            }
            else if (Form1.CurrentForm.materialComboBox2.Items.Count == 0)
            {
                Prompt.Popup("No game installation were found!");
                new KillApp();
            }
        }

        private void FindSteam()
        {
            foreach (string regis in steamregs)
            {
                var steamRegPath = Registry.LocalMachine.OpenSubKey(regis);
                string Steampath = "";
                string bnssteampath = "";
                if (steamRegPath != null)
                {
                    if (steamRegPath.GetValue("InstallPath") != null && steamRegPath.GetValue("InstallPath").GetType() == typeof(String[]))
                    {
                        var obj = steamRegPath.GetValue("InstallPath") as string[];
                        Steampath = obj[0].Replace("\r\n", "");
                    }
                    else if (steamRegPath.GetValue("InstallPath") != null)
                        Steampath = steamRegPath.GetValue("InstallPath").ToString();
                    if (Steampath.Length > 0)
                    {
                        string SteamBnSNeoFinder = Steampath + "\\steamapps\\libraryfolders.vdf";
                        if (File.Exists(SteamBnSNeoFinder))
                        {
                            var libfolder = new SteamVDFParser().ParseVdf(File.ReadAllText(SteamBnSNeoFinder));
                            foreach (var LibraryFolders in libfolder.Children)
                            {
                                foreach (var LibraryFolder in LibraryFolders.Value.Children)
                                {
                                    bool foundSteamBnSNeo = false;
                                    foreach (var SubFolder in LibraryFolder.Value.Children)
                                    {
                                        if (SubFolder.Key == "path")
                                        {
                                            string SteamLibPath = SubFolder.Value.Value;
                                            if (Directory.Exists(SteamLibPath + "\\steamapps\\common\\Blade & Soul NEO\\UE4"))
                                            {
                                                bnssteampath = SteamLibPath + "\\steamapps\\common\\Blade & Soul NEO\\UE4";
                                                foundSteamBnSNeo = true;
                                            }
                                        }
                                        if (foundSteamBnSNeo) break;
                                    }
                                    if (foundSteamBnSNeo) break;
                                }
                                if (bnssteampath.Length > 0)
                                {
                                    if (!Installs.ContainsKey(References[regis]))
                                    {
                                        Form1.CurrentForm.AddLauncherLog($"Found {References[regis]} Client" + Environment.NewLine + bnssteampath.Replace(@"\\", @"\"));
                                        Installs.Add(References[regis], bnssteampath.Replace("\r\n", "").Replace(@"\\", @"\"));
                                    }
                                }
                            }
                        }
                    }
                }
            }
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
            // Ini-Reset Locale Dats
            PakHandler.LocaleDats = new Dictionary<string, string>();
            // Continue
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
                                // Save Lang Paths for Dat Usage
                                PakHandler.LocaleDats.Add(lang.ToString(), ClientPath + "\\BNSR\\Content\\local\\" + autodir + "\\" + lang.ToString() + "\\data");
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
