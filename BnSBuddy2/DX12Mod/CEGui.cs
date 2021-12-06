using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FastColoredTextBoxNS;
using MetroFramework;
using MetroFramework.Forms;
using Revamped_BnS_Buddy.Functions;

namespace Revamped_BnS_Buddy.DX12Mod
{
    public partial class CEGui : MetroForm
    {
        private string RegPath = Form1.CurrentForm.RegPath;
        private string LauncherPath64 = Form1.CurrentForm.LauncherPath64;

        public CEGui()
        {
            InitializeComponent();
            GetColor();
            if (CheckIfExists())
            {
                try
                {
                    fastColoredTextBox2.Text = File.ReadAllText(RegPath + LauncherPath64 + "\\d912pxy\\config.ini");
                } 
                catch
                {
                    Prompt.Popup("Error: Could not load ini file!");
                }
            }
            else
            {
                Prompt.Popup("No config.ini found for D912PXY. Please install properly or reinstall via BnS Buddy");
                this.Close();
            }
        }

        private void GetColor()
        {
            base.Style = Prompt.ColorSet;
            Themer.Style = Prompt.ColorSet;
            Refresh();
        }

        private bool CheckIfExists()
        {
            return File.Exists(RegPath + LauncherPath64 + "\\d912pxy\\config.ini");
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            try
            {
                File.WriteAllText(RegPath + LauncherPath64 + "\\d912pxy\\config.ini", fastColoredTextBox2.Text);
            }
            catch
            {
                Prompt.Popup("Error: Could not save config.ini!");
            }
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
