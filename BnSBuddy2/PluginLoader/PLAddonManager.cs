using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin.Controls;
using MaterialSkin;
using System.IO;
using BnSBuddy2.Functions;

namespace BnSBuddy2.PluginLoader
{
    public partial class PLAddonManager : MaterialForm
    {
        // Globals
        private readonly MaterialSkinManager materialSkinManager;
        private bool IsWorking = false;
        private AddonHandler InstallHandler;
        private string UserProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        public string BnSBuddy_Addons_Path = Form1.CurrentForm.AddonsPaths.AddonsPath;
        public string BnSPatch_Addons_Path = "";
        public static PLAddonManager CurrentForm;
        // End Globals

        public PLAddonManager()
        {
            CurrentForm = this;
            InitializeComponent();
            // Initialize MaterialSkinManager
            materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.EnforceBackcolorOnAllComponents = true;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = Form1.CurrentForm.materialSkinManager.Theme;
            materialSkinManager.ColorScheme = Form1.CurrentForm.materialSkinManager.ColorScheme;
        }

        private void IniPLAM()
        {
            IsWorking = true;
            BnSPatch_Addons_Path = UserProfile + "\\Documents\\BnS\\addons";
            if (!Directory.Exists(BnSPatch_Addons_Path))
                Directory.CreateDirectory(BnSPatch_Addons_Path);
            else
                GenerateProperly(BnSPatch_Addons_Path, materialListBox2);
            // Generate bnsbuddy addons
            GenerateProperly(BnSBuddy_Addons_Path, materialListBox1);
            IsWorking = false;
        }

        private void GenerateProperly(string PathToAddons, MaterialListBox control)
        {
            string[] files = Directory.GetFiles(PathToAddons);
            foreach (string text in files)
            {
                string text2 = text.Split(Path.DirectorySeparatorChar).Last();
                if (text.Length != 0 && (text2.EndsWith(".patch") || text2.EndsWith(".xml")))
                {
                    string text3 = text2.Replace(".patch", " (.patch)").Replace(".xml", " (.xml)");
                    if (!control.Items.Contains(new MaterialListBoxItem(text3)))
                        control.Items.Add(new MaterialListBoxItem(text3));
                }
            }
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            if (IsWorking)
                return;
            IsWorking = true;
            Task.Delay(50).ContinueWith(delegate
            {
                //for (int i = 0; i < materialListBox1.SelectedItems.Count; i++)
                //    InstallHandler.Install(materialListBox1.SelectedItems[i].ToString().Replace(" (.patch)", ".patch").Replace(" (.xml)", ".xml"));
                InstallHandler.Install(materialListBox1.SelectedItem.Text.ToString().Replace(" (.patch)", ".patch").Replace(" (.xml)", ".xml"));
                GetNewAddons();
                IsWorking = false;
            });
        }
        private void GetNewAddons()
        {
            materialListBox2.Items.Clear();
            GenerateProperly(BnSPatch_Addons_Path, materialListBox2);
        }

        private void materialButton2_Click(object sender, EventArgs e)
        {
            if (IsWorking)
                return;
            IsWorking = true;
            Task.Delay(50).ContinueWith(delegate
            {
                //for (int i = 0; i < materialListBox2.SelectedItems.Count; i++)
                //    InstallHandler.Uninstall(materialListBox2.SelectedItems[i].ToString().Replace(" (.patch)", ".patch").Replace(" (.xml)", ".xml"));
                InstallHandler.Uninstall(materialListBox2.SelectedItem.Text.ToString().Replace(" (.patch)", ".patch").Replace(" (.xml)", ".xml"));
                GetNewAddons();
                IsWorking = false;
            });
        }

        private void materialListBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (sender == materialListBox2)
                if (e.Button.ToString() == "Right")
                    materialListBox2.SelectedIndex = -1;
            if (sender == materialListBox1)
                if (e.Button.ToString() == "Right")
                    materialListBox1.SelectedIndex = -1;
        }

        private void PLAddonManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsWorking)
            {
                Prompt.Popup("The manager is working, please wait.");
                e.Cancel = true;
            }
        }

        private void materialButton3_Click(object sender, EventArgs e)
        {
            Prompt.Popup(
                "Add: Adds the highlighted addons to BnSPatch addons folder." + Environment.NewLine +
                "Remove: Removes the highlighted addons from the BnSPatch addons folder." + Environment.NewLine +
                "Help: This popup" + Environment.NewLine +
                "How to use list boxes:" + Environment.NewLine +
                "1: Hold down and drag to select multiple or single press to select item/s. Hold ALT and press on the item to multi select." + Environment.NewLine +
                "2: Right click to deselect all items from the according list box." + Environment.NewLine +
                "3: Select Add to add highlighted BnSBuddy addons OR Remove to remove highlighted BnSPatch addons." + Environment.NewLine +
                "Take note: You REQUIRE having the BnSPatch plugin installed for all this to work."
                );
        }

        private void PLAddonManager_Shown(object sender, EventArgs e)
        {
            IniPLAM();
            InstallHandler = new AddonHandler();
        }
    }
}
