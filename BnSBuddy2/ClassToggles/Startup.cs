using MaterialSkin.Controls;
using System.Collections.Generic;
using System.IO;

namespace BnSBuddy2.ClassToggles
{
    public class Startup
    {
        public TogglesHandler _TogglesHandler = new TogglesHandler();
        public UpkNumbs _UpkNumbs = new UpkNumbs();
        public ButtonsHandler _ButtonsHandler = new ButtonsHandler();

        public void _Startup()
        {
            string ModdedPakPath = Form1.CurrentForm.PakPaths.ModsPath;

            Dictionary<string, List<MaterialSwitch>> _MetroToggle = _TogglesHandler.MetroToggle;
            Dictionary<string, Dictionary<string, List<string>>> _ClassNumber = _UpkNumbs.Classes_Toggles;

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
                        string _pak_number = upk_number + ".pak";
                        if (File.Exists(ModdedPakPath + _pak_number))
                            present++;
                        else
                            present--;
                        if (present > 0 && present <= expect)
                            _MetroToggle[ClassName][TypeInt].Checked = false;
                        else if ((present + expect) <= 0)
                        {
                            _MetroToggle[ClassName][TypeInt].Checked = true;
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
