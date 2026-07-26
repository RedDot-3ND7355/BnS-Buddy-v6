using BnSBuddy2.Functions;
using MaterialSkin.Controls;

namespace BnSBuddy2.Pathing
{
    public static class RedoPathing
    {
        public static void ResetPaths(bool AppStarted, string region)
        {
            if (AppStarted || (!AppStarted && Form1.CurrentForm.materialComboBox2.SelectedItem != null))
            {
                try
                {
                    Form1.CurrentForm.AddLauncherLog("Resetting Paths...");
                    region = RegionConvert.Convert(region);
                    string ClientPath = Form1.CurrentForm.ClientPaths.Installs[region];
                    Form1.CurrentForm.ClientPaths.FindLanguages(ClientPath);
                    Form1.CurrentForm.SplashPaths.ReturnSplashPath(Form1.CurrentForm.materialComboBox2.SelectedItem, Form1.CurrentForm.materialComboBox3.SelectedItem);
                    Form1.CurrentForm.PakPaths.ReturnPakPath(Form1.CurrentForm.materialComboBox2.SelectedItem);
                    Form1.CurrentForm.ExecuteableFile.ReturnExecFileLocation(Form1.CurrentForm.materialComboBox2.SelectedItem);
                    Form1.CurrentForm.Cleaner.MessCleaner();
                    if (Form1.CurrentForm.materialComboBox3.SelectedItem != null)
                        FasterToggle.CheckState(Form1.CurrentForm.materialComboBox2.SelectedItem, Form1.CurrentForm.materialComboBox3.SelectedItem);
                    Form1.CurrentForm.AddLauncherLog("Updated Paths!");
                    Form1.CurrentForm.Refresh();
                } 
                catch
                {
                    Form1.CurrentForm.AddLauncherLog("Reverting region! Game not entirely installed? Update Game or File Scan.");
                    Form1.CurrentForm.materialComboBox2.SelectedIndex = Form1.CurrentForm.materialComboBox2.FindStringExact(Form1.CurrentForm.routine.CurrentSettings["lastserver"].Replace(" Neo", "").Replace(" Live", "").Replace(" Test", ""));
                }
            }
        }
    }
}
