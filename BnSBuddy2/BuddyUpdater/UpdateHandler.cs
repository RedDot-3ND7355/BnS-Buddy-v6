using BnSBuddy2.Functions;
using BnSBuddy2.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BnSBuddy2.BuddyUpdater
{
    public class UpdateHandler
    {
        Pages.UpdateTransition _UpdateTransition; 

        public void StartUpdate()
        {
            File.WriteAllBytes(Form1.CurrentForm.AppPath + "\\BnS Buddy Updater.exe", ExtractRessources.ExtractRessource(Resources.BnS_Buddy_Updater));
            Process process = new Process();
            process.StartInfo.FileName = Form1.CurrentForm.AppPath + "\\BnS Buddy Updater.exe";
            process.Start();
            Form1.CurrentForm.KillApp();
        }

        public void UpdateTransition()
        {
            _UpdateTransition = new Pages.UpdateTransition();
            switch (_UpdateTransition.ShowDialog())
            {
                case System.Windows.Forms.DialogResult.OK:
                    StartUpdate();
                    break;
                case System.Windows.Forms.DialogResult.Cancel:
                    Form1.CurrentForm.AddLauncherLog("Update Cancelled.");
                    break;
            }
        }
    }
}
