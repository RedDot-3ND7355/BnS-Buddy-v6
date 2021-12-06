using BnSBuddy2.Functions;

namespace BnSBuddy2.Pathing
{
    public static class RedoPathing
    {
        public static void ResetPaths(bool AppStarted, string region)
        {
            if (AppStarted)
            {
                try
                {
                    Form1.CurrentForm.AddLauncherLog("Resetting Paths...");
                    region = RegionConvert.Convert(region);
                    string ClientPath = Form1.CurrentForm.ClientPaths.Installs[region];
                    Form1.CurrentForm.ClientPaths.FindLanguages(ClientPath);
                    Form1.CurrentForm.SplashPaths.ReturnSplashPath(Form1.CurrentForm.materialComboBox2.SelectedItem.ToString(), Form1.CurrentForm.materialComboBox3.SelectedItem.ToString());
                    Form1.CurrentForm.PakPaths.ReturnPakPath(Form1.CurrentForm.materialComboBox2.SelectedItem.ToString());
                    Form1.CurrentForm.ExecuteableFile.ReturnExecFileLocation(Form1.CurrentForm.materialComboBox2.SelectedItem.ToString());
                    Form1.CurrentForm.Cleaner.MessCleaner();
                    Form1.CurrentForm.AddLauncherLog("Updated Paths!");
                    Form1.CurrentForm.Refresh();
                } 
                catch
                {
                    Form1.CurrentForm.AddLauncherLog("Reverting region! Game not entirely installed?");
                    Form1.CurrentForm.materialComboBox2.SelectedIndex = Form1.CurrentForm.materialComboBox2.FindStringExact(Form1.CurrentForm.routine.CurrentSettings["lastserver"]);
                }
            }
        }
    }
}
