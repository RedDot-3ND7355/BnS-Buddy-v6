using System;
using MaterialSkin;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BnSBuddy2.Functions
{
    public static class Prompt
    {
        public static void Popup(string Message)
        {
            Popup popup = new Popup();
            popup.materialMultiLineTextBox1.Text = Message;
            Size size = TextRenderer.MeasureText(popup.materialMultiLineTextBox1.Text, popup.materialMultiLineTextBox1.Font);
            popup.Width = size.Width;
            popup.materialMultiLineTextBox1.Height = size.Height;
            popup.ShowDialog();
        }

        public static string MultipleLang(List<string> languages)
        {
            MultipleLang multiplelang = new MultipleLang(languages);
            multiplelang.ShowDialog();
            return multiplelang.materialComboBox1.SelectedItem.ToString();
        }

        public static string MultipleInstalltion(Dictionary<string, string> installs)
        {
            Dictionary<string, string> _installs = new Dictionary<string, string>(installs);
            MultipleInstallation multipleinstallation = new MultipleInstallation();
            if (_installs.ContainsKey("NA/EU"))
            {
                _installs.Add("North America", installs["NA/EU"]);
                _installs.Add("Europe", installs["NA/EU"]);
                _installs.Remove("NA/EU");
            }
            multipleinstallation.installs = _installs;
            multipleinstallation.ShowDialog();
            return multipleinstallation.materialComboBox1.SelectedItem.ToString();
        }

        public static DialogResult FirstTimeUse()
        {
            FirstTimeUse firsttimeuse = new FirstTimeUse();
            return firsttimeuse.ShowDialog();
        }
    }
}
