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
using BnSBuddy2.ProfileManager;
using BnSBuddy2.Functions;

namespace BnSBuddy2.Pages
{
    public partial class Profiles : MaterialSkin.Controls.MaterialForm
    {
        // Globals
        private readonly MaterialSkinManager materialSkinManager;
        // End Globals

        public Profiles()
        {
            InitializeComponent();
            // Initialize MaterialSkinManager
            materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.EnforceBackcolorOnAllComponents = true;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = Form1.CurrentForm.materialSkinManager.Theme;
            materialSkinManager.ColorScheme = Form1.CurrentForm.materialSkinManager.ColorScheme;
            // Routine
            RunDelay.Method(Routine);
        }

        // Routine
        private void Routine()
        {
            Application.DoEvents();
            CheckForIllegalCrossThreadCalls = false;
            // Ini Registry for users
            ProfileHandler.IniReg();
            // Populate Combobox
            materialComboBox1.Items.Add("NA/EU");
            materialComboBox1.Items.AddRange(ProfileHandler.PopulateAddGui());
            materialComboBox1.Items.Remove("North America");
            materialComboBox1.Items.Remove("Europe");
            materialComboBox1.SelectedIndex = 0;
        }

        // Add New Button
        private void materialButton1_Click(object sender, EventArgs e)
        {
            ProfileHandler.OpenAddGui();
            materialComboBox1_SelectedIndexChanged(sender, e);
        }

        // Convert Button
        private void materialButton2_Click(object sender, EventArgs e)
        {
            ProfileHandler.Convert(materialListBox1.SelectedItem.Text);
            foreach (string user in ProfileHandler.Populate(materialComboBox1.SelectedItem.ToString()))
                materialListBox1.Items.Add(new MaterialListBoxItem(user));
            materialComboBox1_SelectedIndexChanged(sender, e);
        }

        // Remove Selected Button
        private void materialButton3_Click(object sender, EventArgs e)
        {
            ProfileHandler.Remove(materialListBox1.SelectedItem.Text);
            materialButton3.Enabled = false;
            materialButton2.Enabled = false;
            materialComboBox1_SelectedIndexChanged(sender, e);
        }

        // After Select
        private void materialListBox1_SelectedIndexChanged(object sender, MaterialListBoxItem selectedItem)
        {
            materialButton3.Enabled = (materialListBox1.SelectedItem.ToString() != null);
            materialButton2.Enabled = (materialButton3.Enabled && selectedItem.Text.Contains("(UNMANAGED)"));
        }

        // After Region Change
        private void materialComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (materialListBox1.Items.Count > 0)
                materialListBox1.Items.Clear();
            foreach (string user in ProfileHandler.Populate(materialComboBox1.SelectedItem.ToString()))
                materialListBox1.Items.Add(new MaterialListBoxItem(user));
        }
    }
}
