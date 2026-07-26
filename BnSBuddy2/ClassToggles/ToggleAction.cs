using BnSBuddy2.Properties;
using MaterialSkin.Controls;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace BnSBuddy2.ClassToggles
{
    public class ToggleAction
    {
        private Dictionary<string, Dictionary<string, List<string>>> ClassUpks = Form1.CurrentForm.StartupToggles._UpkNumbs.Classes_Toggles;
        private Dictionary<string, List<MaterialSwitch>> MetroToggles = Form1.CurrentForm.StartupToggles._TogglesHandler.MetroToggle;
        private Dictionary<string, List<MaterialButton>> MetroButtons = Form1.CurrentForm.StartupToggles._ButtonsHandler.MetroButton;

        public void _ToggleAction(string ClassName, string Type, MaterialSwitch control)
        {
            if (!Form1.CurrentForm.AppStarted)
                return;
            bool toggle = !control.Checked;
            foreach (string Upk_name in ClassUpks[Type][ClassName])
            {
                List<string> filestoextract = new List<string>();
                filestoextract.Add(Upk_name + ".pak");
                filestoextract.Add(Upk_name + ".sig");
                string ModPakPath = Form1.CurrentForm.PakPaths.ModsPath;
                if (File.Exists(ModPakPath + Upk_name + ".pak") && !toggle)
                {
                    File.Delete(ModPakPath + Upk_name + ".pak");
                    File.Delete(ModPakPath + Upk_name + ".sig");
                }
                else
                    InstallPakFilesFile(ModPakPath, filestoextract);
            }
        }

        private void InstallPakFilesFile(string modPakPath, List<string> filestoextract)
        {
            MemoryStream stream = new MemoryStream(Resources.Removes);
            using (ZipArchive archive = new ZipArchive(stream))
                foreach (string file in filestoextract)
                    foreach (ZipArchiveEntry entry in archive.Entries.Where(e => e.FullName.Contains(file)))
                        using (Stream s = new FileStream(modPakPath + entry.FullName, FileMode.Create))
                            entry.Open().CopyTo(s);
        }

        int POS = 0;
        public void _ToggleAll(string Type, bool ON_or_OFF)
        {
            if (Type == "Animations")
                POS = 0;
            if (Type == "Effects")
                POS = 1;

            foreach (string ClassName in MetroToggles.Keys)
            {
                if (MetroToggles[ClassName][POS] != null)
                    if (MetroToggles[ClassName][POS].Checked == !ON_or_OFF && MetroToggles[ClassName][POS].Enabled)
                        MetroToggles[ClassName][POS].Checked = ON_or_OFF;
            }
        }

        public void _CheckToggleForButtonAvailability(string Type)
        {
            if (Type == "Animations")
                POS = 0;
            if (Type == "Effects")
                POS = 1;

            int num = 0;
            int num2 = 0;
            foreach (string ClassName in MetroToggles.Keys)
            {
                if (MetroToggles[ClassName][POS] != null)
                {
                    if (MetroToggles[ClassName][POS].Enabled)
                    {
                        num++;
                        if (MetroToggles[ClassName][POS].Checked)
                            num2++;
                    }
                }
            }

            if (num2 == num && num != 0)
            {
                MetroButtons[Type][0].Enabled = true;
                MetroButtons[Type][1].Enabled = false;
            }
            else if (num2 == 0 && num != 0)
            {
                MetroButtons[Type][0].Enabled = false;
                MetroButtons[Type][1].Enabled = true;
            }
            else if (num2 < num && num2 != 0 && num != 0 && num2 > 0)
            {
                MetroButtons[Type][0].Enabled = true;
                MetroButtons[Type][1].Enabled = true;
            }
            else
            {
                MetroButtons[Type][0].Enabled = false;
                MetroButtons[Type][1].Enabled = false;
            }
        }
    }
}
