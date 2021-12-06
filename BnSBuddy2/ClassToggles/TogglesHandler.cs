using System.Collections.Generic;
using MaterialSkin;
using MaterialSkin.Controls;

namespace BnSBuddy2.ClassToggles
{
    public class TogglesHandler
    {
        public Dictionary<string, List<MaterialSwitch>> MetroToggle = new Dictionary<string, List<MaterialSwitch>>();
        private List<MaterialSwitch> ToggleNum = new List<MaterialSwitch>();
        public TogglesHandler()
        {
            if (MetroToggle.Count == 0)
            {
                // Sum
                ToggleNum = new List<MaterialSwitch>();
                ToggleNum.Add(Form1.CurrentForm.materialSwitch23); // Anims
                ToggleNum.Add(Form1.CurrentForm.materialSwitch12); // Effects
                MetroToggle.Add("Summoner", ToggleNum);
                // BM
                ToggleNum = new List<MaterialSwitch>();
                ToggleNum.Add(Form1.CurrentForm.materialSwitch25);
                ToggleNum.Add(Form1.CurrentForm.materialSwitch26);
                MetroToggle.Add("Blade Master", ToggleNum);
                // KFM
                ToggleNum = new List<MaterialSwitch>();
                ToggleNum.Add(Form1.CurrentForm.materialSwitch22);
                ToggleNum.Add(Form1.CurrentForm.materialSwitch11);
                MetroToggle.Add("Kung-Fu Master", ToggleNum);
                // FM
                ToggleNum = new List<MaterialSwitch>();
                ToggleNum.Add(Form1.CurrentForm.materialSwitch14);
                ToggleNum.Add(Form1.CurrentForm.materialSwitch24);
                MetroToggle.Add("Force Master", ToggleNum);
                // DES
                ToggleNum = new List<MaterialSwitch>();
                ToggleNum.Add(Form1.CurrentForm.materialSwitch20);
                ToggleNum.Add(Form1.CurrentForm.materialSwitch9);
                MetroToggle.Add("Destroyer", ToggleNum);
                // GS
                ToggleNum = new List<MaterialSwitch>();
                ToggleNum.Add(Form1.CurrentForm.materialSwitch21);
                ToggleNum.Add(Form1.CurrentForm.materialSwitch10);
                MetroToggle.Add("Gunslinger", ToggleNum);
                // ASS
                ToggleNum = new List<MaterialSwitch>();
                ToggleNum.Add(Form1.CurrentForm.materialSwitch4);
                ToggleNum.Add(Form1.CurrentForm.materialSwitch13);
                MetroToggle.Add("Assassin", ToggleNum);
                // BD
                ToggleNum = new List<MaterialSwitch>();
                ToggleNum.Add(Form1.CurrentForm.materialSwitch27);
                ToggleNum.Add(Form1.CurrentForm.materialSwitch28);
                MetroToggle.Add("Blade Dancer", ToggleNum);
                // WL
                ToggleNum = new List<MaterialSwitch>();
                ToggleNum.Add(Form1.CurrentForm.materialSwitch29);
                ToggleNum.Add(Form1.CurrentForm.materialSwitch30);
                MetroToggle.Add("Warlock", ToggleNum);
                // SF
                ToggleNum = new List<MaterialSwitch>();
                ToggleNum.Add(Form1.CurrentForm.materialSwitch31);
                ToggleNum.Add(Form1.CurrentForm.materialSwitch32);
                MetroToggle.Add("Soul Fighter", ToggleNum);
                // WD
                ToggleNum = new List<MaterialSwitch>();
                ToggleNum.Add(Form1.CurrentForm.materialSwitch19);
                ToggleNum.Add(Form1.CurrentForm.materialSwitch8);
                MetroToggle.Add("Warden", ToggleNum);
                // AR
                ToggleNum = new List<MaterialSwitch>();
                ToggleNum.Add(Form1.CurrentForm.materialSwitch33);
                ToggleNum.Add(Form1.CurrentForm.materialSwitch34);
                MetroToggle.Add("Archer", ToggleNum);
                // ASTRO
                ToggleNum = new List<MaterialSwitch>();
                ToggleNum.Add(Form1.CurrentForm.materialSwitch18);
                ToggleNum.Add(Form1.CurrentForm.materialSwitch7);
                MetroToggle.Add("Astromancer", ToggleNum);
                // EXTRAS
                ToggleNum = new List<MaterialSwitch>();
                ToggleNum.Add(null);
                ToggleNum.Add(Form1.CurrentForm.materialSwitch6);
                MetroToggle.Add("Extras", ToggleNum);
            }
        }
    }
}
