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
            if (region == "Europe" || region == "North America")
                region = "NA/EU";
            if (region == "Korean" && Form1.CurrentForm.materialComboBox4.SelectedItem.ToString() == "Test")
                region = "Korean Test";
            return region;
        }
    }
}
