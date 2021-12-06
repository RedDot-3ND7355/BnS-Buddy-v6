using BnSBuddy2.Functions;
using System;
using System.IO;

namespace BnSBuddy2.PluginLoader
{
    class AddonHandler
    {
        private string BnSBuddy_Addons_Path = PLAddonManager.CurrentForm.BnSBuddy_Addons_Path;
        private string BnSPatch_Addons_Path = PLAddonManager.CurrentForm.BnSPatch_Addons_Path;

        public void Install(string AddonName)
        {
            try
            {
                File.Copy(BnSBuddy_Addons_Path + "\\" + AddonName, BnSPatch_Addons_Path + "\\" + AddonName, true);
            }
            catch (Exception ex)
            {
                Prompt.Popup("Error: " + ex.ToString());
            }
        }

        public void Uninstall(string AddonName)
        {
            try
            {
                if (!File.Exists(BnSBuddy_Addons_Path + "\\" + AddonName))
                    File.Copy(BnSPatch_Addons_Path + "\\" + AddonName, BnSBuddy_Addons_Path + "\\" + AddonName, true);
                File.Delete(BnSPatch_Addons_Path + "\\" + AddonName);
            }
            catch (Exception ex)
            {
                Prompt.Popup("Error: " + ex.ToString());
            }
        }
    }
}
