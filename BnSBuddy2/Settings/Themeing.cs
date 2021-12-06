using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BnSBuddy2.Settings
{
    public class Themeing
    {
        public void SaveColors()
        {
            // Save changes
            Form1.CurrentForm.routineChanger("primary", Form1.CurrentForm.materialSkinManager.ColorScheme.PrimaryColor.ToArgb().ToString());
            Form1.CurrentForm.routineChanger("darkprimary", Form1.CurrentForm.materialSkinManager.ColorScheme.DarkPrimaryColor.ToArgb().ToString());
            Form1.CurrentForm.routineChanger("lightprimary", Form1.CurrentForm.materialSkinManager.ColorScheme.LightPrimaryColor.ToArgb().ToString());
            Form1.CurrentForm.routineChanger("accent", Form1.CurrentForm.materialSkinManager.ColorScheme.AccentColor.ToArgb().ToString());
            Form1.CurrentForm.routineChanger("textshade", Form1.CurrentForm.materialSkinManager.ColorScheme.TextColor.ToArgb().ToString());
            Form1.CurrentForm.routineChanger("theme", Form1.CurrentForm.materialSkinManager.Theme.ToString());
        }

        public void ResetColors() => new Default().colors();
    }
}
