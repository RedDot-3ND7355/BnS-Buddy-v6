using System.Collections.Generic;
using System.IO;
using BnSBuddy2.Functions;
using MaterialSkin.Controls;

namespace BnSBuddy2.ClassToggles
{
    public class Startup
    {
        public TogglesHandler _TogglesHandler = new TogglesHandler();
        public UpkNumbs _UpkNumbs = new UpkNumbs();
        public ButtonsHandler _ButtonsHandler = new ButtonsHandler();
        private string RegPath = "";
        private string AppPath = Form1.CurrentForm.AppPath;

        public Startup(string region)
        {
            region = RegionConvert.Convert(region);
            RegPath = Form1.CurrentForm.ClientPaths.Installs[region];

            Dictionary<string, List<MaterialSwitch>> _MetroToggle = _TogglesHandler.MetroToggle;
            Dictionary<string, Dictionary<string, List<string>>> _ClassNumber = _UpkNumbs.Classes_Toggles;

            if (!Directory.Exists(AppPath + "\\backup"))
                Directory.CreateDirectory(AppPath + "\\backup");

            int TypeInt = 0;
            foreach (string Type in _ClassNumber.Keys)
            {
                int present = 0;
                foreach (string ClassName in _ClassNumber[Type].Keys)
                {
                    present = 0;
                    int expect = _ClassNumber[Type][ClassName].Count;
                    foreach (string upk_number in _ClassNumber[Type][ClassName])
                    {
                        string _upk_number = upk_number + ".upk";
                        if (File.Exists(AppPath + "\\backup\\" + _upk_number))
                        {
                            if (File.Exists(RegPath + "\\contents\\bns\\CookedPC\\" + _upk_number))
                            {
                                File.Copy(RegPath + "\\contents\\bns\\CookedPC\\" + _upk_number, AppPath + "\\backup\\" + _upk_number, overwrite: true);
                                File.Delete(RegPath + "\\contents\\bns\\CookedPC\\" + _upk_number);
                            }
                            present++;
                        }
                        else if (!File.Exists(RegPath + "\\contents\\bns\\CookedPC\\" + _upk_number))
                            present--;
                        if (present > 0 && present < expect) 
                            _MetroToggle[ClassName][TypeInt].Checked = true;
                        else if ((present + expect) <= 0)
                        {
                            _MetroToggle[ClassName][TypeInt].Checked = false;
                            _MetroToggle[ClassName][TypeInt].Enabled = false;
                        }
                        else if (present > 0)
                            _MetroToggle[ClassName][TypeInt].Checked = false;
                    }
                }
                TypeInt++;
            }
        }
    }
}
