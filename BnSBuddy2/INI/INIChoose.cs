using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.IO;

namespace BnSBuddy2.INI
{
    public partial class INIChoose : MaterialForm
    {
        // Globals
        private readonly MaterialSkinManager materialSkinManager;
        private Dictionary<string, string> INICouples = new Dictionary<string, string>();
        private string LocalPath2INI = "";
        // End Globals

        public INIChoose()
        {
            InitializeComponent();
            // Initialize MaterialSkinManager
            materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.EnforceBackcolorOnAllComponents = true;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = Form1.CurrentForm.materialSkinManager.Theme;
            materialSkinManager.ColorScheme = Form1.CurrentForm.materialSkinManager.ColorScheme;
        }

        private void INIChoose_Shown(object sender, EventArgs e) =>
            PopulateComboBox();

        // On start combobox populating
        private void PopulateComboBox()
        {
            LocalPath2INI = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\BNSR\\Saved\\Config\\WindowsNoEditor\\";
            if (Directory.Exists(LocalPath2INI))
            {
                foreach (string filepath in Directory.GetFiles(LocalPath2INI, "*.ini"))
                    INICouples.Add(Path.GetFileName(filepath), filepath);
                foreach (string filenames in INICouples.Keys)
                    materialComboBox1.Items.Add(filenames);
            }
        }

        // Combobox INI Chooser
        private void materialComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                materialButton1.Enabled = false;
                fastColoredTextBox1.Text =
                    INIHandler.ReadINIFile(INICouples[materialComboBox1.SelectedItem.ToString()]);
                materialButton1.Enabled = true;
            }
            catch
            {
                materialButton1.Text = "Error";
            }
        }

        // Save Button
        private void materialButton1_Click(object sender, EventArgs e)
        {
            try
            {
                materialButton1.Enabled = false;
                INIHandler.SaveINIFile(INICouples[materialComboBox1.SelectedItem.ToString()], fastColoredTextBox1.Text);
                materialButton1.Text = "Saved!";
                CheckResult.Enabled = true;
            }
            catch
            {
                materialButton1.Text = "Error";
                CheckResult.Enabled = true;
            }
        }

        private void CheckResult_Tick(object sender, EventArgs e)
        {
            materialButton1.Text = "Save";
            materialButton1.Enabled = true;
        }
    }
}
