using MaterialSkin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BnSBuddy2.Functions
{
    public partial class MultipleInstallation : MaterialSkin.Controls.MaterialForm
    {
        public static MaterialSkinManager materialSkinManager;
        public Dictionary<string, string> installs = new Dictionary<string, string>();
        public MultipleInstallation()
        {
            InitializeComponent();
            // Ini material
            materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.EnforceBackcolorOnAllComponents = true;
            materialSkinManager.AddFormToManage(this);
            // Get Color & Theme
            materialSkinManager.Theme = Form1.CurrentForm.routine.CurrentSettings["theme"] == "DARK" ? MaterialSkinManager.Themes.DARK : MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme((Primary)Convert.ToInt32(Form1.CurrentForm.routine.CurrentSettings["primary"]), (Primary)Convert.ToInt32(Form1.CurrentForm.routine.CurrentSettings["darkprimary"]), (Primary)Convert.ToInt32(Form1.CurrentForm.routine.CurrentSettings["lightprimary"]), (Accent)Convert.ToInt32(Form1.CurrentForm.routine.CurrentSettings["accent"]), (TextShade)Convert.ToInt32(Form1.CurrentForm.routine.CurrentSettings["textshade"]));
        }

        private void MultipleInstallation_Shown(object sender, EventArgs e)
        {
            materialComboBox1.Items.AddRange(installs.Keys.ToArray());
            materialComboBox1.SelectedIndex = 0;
        }
    }
}
