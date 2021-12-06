using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace BnSBuddy2.ProfileManager
{
    public partial class AddGui : MaterialForm
    {
        // Globals
        private readonly MaterialSkinManager materialSkinManager;
        // End Globals

        public AddGui()
        {
            InitializeComponent();
            // Initialize MaterialSkinManager
            materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.EnforceBackcolorOnAllComponents = true;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = Form1.CurrentForm.materialSkinManager.Theme;
            materialSkinManager.ColorScheme = Form1.CurrentForm.materialSkinManager.ColorScheme;
            // Populate Region ComboBox
            PopulateTree();
        }

        private void PopulateTree()
        {
            materialComboBox1.Items.Add("NA/EU");
            materialComboBox1.Items.AddRange(ProfileHandler.PopulateAddGui());
            materialComboBox1.Items.Remove("North America");
            materialComboBox1.Items.Remove("Europe");
            materialComboBox1.SelectedIndex = 0;
        }

        private void materialButton1_Click(object sender, EventArgs e) =>
            SaveAction();

        private void SaveAction()
        {
            if (ProfileHandler.SaveUser(materialTextBox1.Text, materialTextBox2.Text, materialComboBox1.SelectedItem.ToString()))
            {
                materialButton1.Text = "Saved";
                materialTextBox1.Text = "";
                materialTextBox2.Text = "";
            }
            else
                materialButton1.Text = "Cancelled";
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            materialButton1.Text = "Save User";
            timer1.Enabled = false;
        }

        private void materialTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                SaveAction();
                e.SuppressKeyPress = true;
            }
        }
    }
}
