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
using BnSBuddy2.Functions;
using System.IO;

namespace BnSBuddy2.Pages
{
    public partial class AddonCreator : MaterialSkin.Controls.MaterialForm
    {
        private readonly MaterialSkinManager materialSkinManager;
        public AddonCreator()
        {
            InitializeComponent();
            // Initialize MaterialSkinManager
            materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.EnforceBackcolorOnAllComponents = true;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = Form1.CurrentForm.materialSkinManager.Theme;
            materialSkinManager.ColorScheme = Form1.CurrentForm.materialSkinManager.ColorScheme;
            //
        }

        // tips button
        private void materialButton1_Click(object sender, EventArgs e) =>
            Prompt.Popup("You can add as many Search and Replace in the addon." + Environment.NewLine + Environment.NewLine + "Don't include .xml in addon name." + Environment.NewLine + Environment.NewLine + "Manually get your search pattern through dat editor.");

        // Save Button
        private void materialButton4_Click(object sender, EventArgs e)
        {
            try
            {
                File.WriteAllText(Form1.CurrentForm.AddonsPaths.ToString() + materialTextBox1.Text + ".xml", ((Control)fastColoredTextBox1).Text);
                materialTextBox1.ReadOnly = false;
            }
            catch (Exception ex)
            {
                Prompt.Popup("Error: " + ex.ToString());
            }
        }

        // Edit Button
        private void materialButton2_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = Form1.CurrentForm.AddonsPaths.ToString();
                openFileDialog.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.Title = "Select Xml Addon to Edit";
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string fileName = Path.GetFileName(openFileDialog.FileName);
                        fileName = fileName.Replace(".xml", "");
                        ((Control)fastColoredTextBox1).Text = File.ReadAllText(openFileDialog.FileName);
                        materialTextBox1.Text = fileName;
                        materialTextBox1.ReadOnly = true;
                    }
                    catch (Exception ex)
                    {
                        Prompt.Popup("Error: " + ex.ToString());
                    }
                }
            }
            catch (Exception ex2)
            {
                Prompt.Popup("Error: " + ex2.ToString());
            }
        }

        // Save as Button
        private void materialButton3_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.FileName = materialTextBox1.Text + ".xml";
                saveFileDialog.InitialDirectory = Form1.CurrentForm.AddonsPaths.ToString();
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.Title = "Select Where to Save Patch";
                saveFileDialog.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(saveFileDialog.FileName, ((Control)fastColoredTextBox1).Text);
                    materialTextBox1.ReadOnly = false;
                }
            }
            catch (Exception ex)
            {
                Prompt.Popup("Error: " + ex.ToString());
            }
        }
    }
}
