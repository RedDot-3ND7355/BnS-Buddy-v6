using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BnSBuddy2.Functions
{
    public static class RegionConvert
    {
        public static string Convert(string region)
        {
            // Fix missing region part
            if (Form1.CurrentForm.materialComboBox4.Items.Count >= 1 && Form1.CurrentForm.materialComboBox4.SelectedItem == null)
            {
                Form1.CurrentForm._noise = true;
                Form1.CurrentForm.materialComboBox4.SelectedIndex = 0;
            }
            // Continue
            if ((region == "Europe" || region == "North America") && Form1.CurrentForm.materialComboBox4.SelectedItem.ToString() == "Live")
                region = "NA/EU Live";
            if ((region == "Europe" || region == "North America") && Form1.CurrentForm.materialComboBox4.SelectedItem.ToString() == "Neo")
                region = "NA/EU Neo";
            if (region == "Korean" && Form1.CurrentForm.materialComboBox4.SelectedItem.ToString() == "Test")
                region = "Korean Test";
            if (region == "Korean" && Form1.CurrentForm.materialComboBox4.SelectedItem.ToString() == "Live")
                region = "Korean Live";
            if (region == "Korean" && Form1.CurrentForm.materialComboBox4.SelectedItem.ToString() == "Neo")
                region = "Korean Neo";
            if (region == "Taiwan" && Form1.CurrentForm.materialComboBox4.SelectedItem.ToString() == "Live")
                region = "Taiwan Live";
            if (region == "Taiwan" && Form1.CurrentForm.materialComboBox4.SelectedItem.ToString() == "Neo")
                region = "Taiwan Neo";
            if (region == "Japanese" && Form1.CurrentForm.materialComboBox4.SelectedItem.ToString() == "Live")
                region = "Japanese Live";
            if (region == "Japanese" && Form1.CurrentForm.materialComboBox4.SelectedItem.ToString() == "Neo")
                region = "Japanese Neo";
            if ((region == "Steam NA" || region == "Steam EU") && Form1.CurrentForm.materialComboBox4.SelectedItem.ToString() == "Neo")
                region = "Steam Neo";
            return region;
        }
    }
}
