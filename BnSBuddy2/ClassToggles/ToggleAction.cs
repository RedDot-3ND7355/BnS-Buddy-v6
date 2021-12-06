using System.Collections.Generic;
using System.IO;
using BnSBuddy2.Functions;
using MaterialSkin.Controls;

namespace BnSBuddy2.ClassToggles
{
    public class ToggleAction
    {
        private Dictionary<string, Dictionary<string, List<string>>> ClassUpks = Form1.CurrentForm.StartupToggles._UpkNumbs.Classes_Toggles;
        private Dictionary<string, List<MaterialSwitch>> MetroToggles = Form1.CurrentForm.StartupToggles._TogglesHandler.MetroToggle;
        private Dictionary<string, List<MaterialButton>> MetroButtons = Form1.CurrentForm.StartupToggles._ButtonsHandler.MetroButton;
        private string RegPath = "";
        private string AppPath = Form1.CurrentForm.AppPath;

        public ToggleAction(string region)
        {
            region = RegionConvert.Convert(region);
            RegPath = Form1.CurrentForm.ClientPaths.Installs[region];
        }

        public void _ToggleAction(string ClassName, string Type, bool toggle = false)
        {
            foreach (string Upk_name in ClassUpks[Type][ClassName])
            {
                string name = Upk_name + ".upk";
                string Backup = toggle ? RegPath + "\\contents\\bns\\CookedPC\\" : AppPath + "\\backup\\";
                string Game = toggle ? AppPath + "\\backup\\" : RegPath + "\\contents\\bns\\CookedPC\\";
                if (File.Exists(Game + name))
                {
                    File.Copy(Game + name, Backup + name, overwrite: true);
                    File.Delete(Game + name);
                }
            }
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
