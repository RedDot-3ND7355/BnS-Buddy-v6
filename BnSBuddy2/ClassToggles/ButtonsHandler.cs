using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialSkin.Controls;

namespace BnSBuddy2.ClassToggles
{
    public class ButtonsHandler
    {
        public Dictionary<string, List<MaterialButton>> MetroButton = new Dictionary<string, List<MaterialButton>>();
        private List<MaterialButton> ButtonNum = new List<MaterialButton>();

        public ButtonsHandler()
        {
            if (MetroButton.Count == 0)
            {
                // Animations
                ButtonNum = new List<MaterialButton>();
                ButtonNum.Add(Form1.CurrentForm.materialButton10); // Disable
                ButtonNum.Add(Form1.CurrentForm.materialButton9); // Enable
                MetroButton.Add("Animations", ButtonNum);
                // Effects
                ButtonNum = new List<MaterialButton>();
                ButtonNum.Add(Form1.CurrentForm.materialButton12);
                ButtonNum.Add(Form1.CurrentForm.materialButton11);
                MetroButton.Add("Effects", ButtonNum);
            }
        }
    }
}
