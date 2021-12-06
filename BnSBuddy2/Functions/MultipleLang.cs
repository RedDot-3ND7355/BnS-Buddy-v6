using System;
using MaterialSkin;
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
    public partial class MultipleLang : MaterialSkin.Controls.MaterialForm
    {
        public static MaterialSkinManager materialSkinManager;
        private List<string> languages = new List<string>();
        public MultipleLang(List<string> langs)
        {
            InitializeComponent();
            // Pass langs
            languages = langs;
            // Ini material
            materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.EnforceBackcolorOnAllComponents = true;
            materialSkinManager.AddFormToManage(this);
            // Get Color & Theme
            materialSkinManager.Theme = Form1.CurrentForm.routine.CurrentSettings["theme"] == "DARK" ? MaterialSkinManager.Themes.DARK : MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme((Primary)Convert.ToInt32(Form1.CurrentForm.routine.CurrentSettings["primary"]), (Primary)Convert.ToInt32(Form1.CurrentForm.routine.CurrentSettings["darkprimary"]), (Primary)Convert.ToInt32(Form1.CurrentForm.routine.CurrentSettings["lightprimary"]), (Accent)Convert.ToInt32(Form1.CurrentForm.routine.CurrentSettings["accent"]), (TextShade)Convert.ToInt32(Form1.CurrentForm.routine.CurrentSettings["textshade"]));
        }

        private void MultipleLang_Shown(object sender, EventArgs e)
        {
            // Generate tree & select default/first
            materialComboBox1.Items.AddRange(languages.ToArray());
            if (materialComboBox1.Items.Contains("NA/EU"))
            {
                materialComboBox1.Items.Remove("NA/EU");
                materialComboBox1.Items.Add("North America");
                materialComboBox1.Items.Add("Europe");
            }
            materialComboBox1.SelectedIndex = 0;
        }
    }
}
