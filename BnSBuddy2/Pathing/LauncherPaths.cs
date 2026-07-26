using BnSBuddy2.Functions;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BnSBuddy2.Pathing
{
    public class LauncherPaths
    {
        // Globals
        List<string> regs = new List<string> { "SOFTWARE\\plaync\\Purple\\", "SOFTWARE\\Wow6432Node\\plaync\\Purple\\", "SOFTWARE\\plaync\\NCLauncherS\\", "SOFTWARE\\Wow6432Node\\plaync\\NCLauncherS\\", "SOFTWARE\\NCJapan\\NCLauncher\\", "SOFTWARE\\Wow6432Node\\NCJapan\\NCLauncher\\", "SOFTWARE\\plaync\\NCLauncherS\\", "SOFTWARE\\Wow6432Node\\plaync\\NCLauncherS\\", "SOFTWARE\\PlayNC\\NCLauncherW\\", "SOFTWARE\\Wow6432Node\\PlayNC\\NCLauncherW\\" };
        Dictionary<string, string> References = new Dictionary<string, string>(StringComparer.CurrentCulture);
        public Dictionary<string, string> Launchers = new Dictionary<string, string>(); 
        RegistryKey localMachine = Registry.LocalMachine;
        // End Globals

        public string PurpleVersionNGP = "2.25.226.10"; // Last updated manually

        public LauncherPaths()
        {
            // North America & Europe (Purple)
            if (Form1.CurrentForm.ClientPaths.Installs.ContainsKey("NA/EU Neo") || Form1.CurrentForm.ClientPaths.Installs.ContainsKey("NA/EU Live"))
            {
                References.Add("SOFTWARE\\Wow6432Node\\plaync\\Purple\\", "NA/EU Purple");
                References.Add("SOFTWARE\\plaync\\Purple\\", "NA/EU Purple");
            }
            // North America & Europe (Non Purple)
            if (Form1.CurrentForm.ClientPaths.Installs.ContainsKey("NA/EU"))
            {
                References.Add("SOFTWARE\\Wow6432Node\\PlayNC\\NCLauncherW\\", "NA/EU");
                References.Add("SOFTWARE\\PlayNC\\NCLauncherW\\", "NA/EU");
            }
            // Taiwan
            if (Form1.CurrentForm.ClientPaths.Installs.ContainsKey("Taiwan"))
            {
                References.Add("SOFTWARE\\Wow6432Node\\plaync\\NCLauncherS\\", "Taiwan");
                References.Add("SOFTWARE\\plaync\\NCLauncherS\\", "Taiwan");
            }
            // Japan
            if (Form1.CurrentForm.ClientPaths.Installs.ContainsKey("Japan"))
            {
                References.Add("SOFTWARE\\Wow6432Node\\NCJapan\\NCLauncher\\", "Japan");
                References.Add("SOFTWARE\\NCJapan\\NCLauncher\\", "Japan");
            }
            // Korean
            if (Form1.CurrentForm.ClientPaths.Installs.ContainsKey("Korean") && (!CCOR("SOFTWARE\\Wow6432Node\\plaync\\NCLauncherS\\") || !CCOR("SOFTWARE\\plaync\\NCLauncherS\\")))
            {
                References.Add("SOFTWARE\\Wow6432Node\\plaync\\NCLauncherS\\", "Korean");
                References.Add("SOFTWARE\\plaync\\NCLauncherS\\", "Korean");
            }
            // Korean Test
            if (Form1.CurrentForm.ClientPaths.Installs.ContainsKey("Korean Test") && (!CCOR("SOFTWARE\\Wow6432Node\\plaync\\NCLauncherS\\") || !CCOR("SOFTWARE\\plaync\\NCLauncherS\\")))
            {
                if (!References.ContainsValue("Korean"))
                {
                    References.Add("SOFTWARE\\Wow6432Node\\plaync\\NCLauncherS\\", "Korean Test");
                    References.Add("SOFTWARE\\plaync\\NCLauncherS\\", "Korean Test");
                }
            }
        }

        private bool CCOR(string refstring)
        {
            if (References.ContainsKey(refstring))
                return true;
            return false;
        }

        // Find all Launchers from NCSoft to read needed user settings
        public void FindLauncherDirs()
        {
            Launchers = new Dictionary<string, string>();
            foreach (string regis in regs)
            {
                localMachine = Registry.LocalMachine.OpenSubKey(regis);
                if (localMachine != null)
                {
                    string path = (string)localMachine.GetValue("BaseDir");
                    if (path != null && path.Length > 0)
                    {
                        if (References.ContainsKey(regis) && !Launchers.ContainsKey(References[regis]))
                        {
                            Form1.CurrentForm.AddLauncherLog($"Found {References[regis]} Launcher" + Environment.NewLine + path);
                            Launchers.Add(References[regis], path);

                            // Get Purple NGPVersion for Login
                            if (References[regis] == "NA/EU Purple")
                            {
                                string found = Directory.GetDirectories(path, $"*.*.*.*").FirstOrDefault();
                                string version = Path.GetFileName(found);
                                if (version != string.Empty)
                                {
                                    Form1.CurrentForm.AddLauncherLog($"NGPVersion: {version}");
                                    PurpleVersionNGP = version;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
