using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BnSBuddy2.Pathing
{
    public class LauncherPaths
    {
        // Globals
        List<string> regs = new List<string> { "SOFTWARE\\plaync\\NCLauncherS\\", "SOFTWARE\\Wow6432Node\\plaync\\NCLauncherS\\", "SOFTWARE\\NCJapan\\NCLauncher\\", "SOFTWARE\\Wow6432Node\\NCJapan\\NCLauncher\\", "SOFTWARE\\plaync\\NCLauncherS\\", "SOFTWARE\\Wow6432Node\\plaync\\NCLauncherS\\", "SOFTWARE\\PlayNC\\NCLauncherW\\", "SOFTWARE\\Wow6432Node\\PlayNC\\NCLauncherW\\" };
        Dictionary<string, string> References = new Dictionary<string, string>(StringComparer.CurrentCulture);
        public Dictionary<string, string> Launchers = new Dictionary<string, string>(); 
        RegistryKey localMachine = Registry.LocalMachine;
        // End Globals

        public LauncherPaths()
        {
            // North America & Europe
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
            if (Form1.CurrentForm.ClientPaths.Installs.ContainsKey("Korean"))
            {
                References.Add("SOFTWARE\\Wow6432Node\\plaync\\NCLauncherS\\", "Korean");
                References.Add("SOFTWARE\\plaync\\NCLauncherS\\", "Korean");
            }
            // Korean Test
            if (Form1.CurrentForm.ClientPaths.Installs.ContainsKey("Korean Test"))
            {
                if (!References.ContainsValue("Korean"))
                {
                    References.Add("SOFTWARE\\Wow6432Node\\plaync\\NCLauncherS\\", "Korean Test");
                    References.Add("SOFTWARE\\plaync\\NCLauncherS\\", "Korean Test");
                }
            }
        }

        // Find all Launchers from NCSoft to read needed user settings
        public void FindLauncherDirs()
        {
            foreach (string regis in regs)
            {
                localMachine = Registry.LocalMachine.OpenSubKey(regis);
                if (localMachine != null)
                {
                    string path = (string)localMachine.GetValue("BaseDir", "", RegistryValueOptions.DoNotExpandEnvironmentNames);
                    if (path != null && path.Length > 0)
                    {
                        if (References.ContainsKey(regis) && !Launchers.ContainsKey(References[regis]))
                        {
                            Form1.CurrentForm.AddLauncherLog($"Found {References[regis]} Launcher" + Environment.NewLine + path);
                            Launchers.Add(References[regis], path);
                        }
                    }
                }
            }
        }
    }
}
