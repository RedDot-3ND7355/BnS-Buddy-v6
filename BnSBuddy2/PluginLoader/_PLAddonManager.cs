using MetroFramework.Forms;
using Revamped_BnS_Buddy.Functions;
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

namespace Revamped_BnS_Buddy.PluginLoader
{
    public partial class PLAddonManager : MetroForm
    {
        private bool IsWorking = false;
        private AddonHandler InstallHandler;
        private string UserProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        public string BnSBuddy_Addons_Path = Form1.CurrentForm.FullAddonPath;
        public string BnSPatch_Addons_Path = "";
        public static PLAddonManager CurrentForm;

        public PLAddonManager()
        {
            CurrentForm = this;
            InitializeComponent();
            GetColor();
        }

        private void IniPLAM()
        {
            IsWorking = true;
            BnSPatch_Addons_Path = UserProfile + "\\Documents\\BnS\\addons";
            if (!Directory.Exists(BnSPatch_Addons_Path))
                Directory.CreateDirectory(BnSPatch_Addons_Path);
            else
                GenerateProperly(BnSPatch_Addons_Path, metroListBox1);
            // Generate bnsbuddy addons
            GenerateProperly(BnSBuddy_Addons_Path, metroListBox2);
            IsWorking = false;
        }

        private void GenerateProperly(string PathToAddons, MetroFramework.CustomMetroControls.MetroListBox control)
        {
            string[] files = Directory.GetFiles(PathToAddons);
            foreach (string text in files)
            {
                string text2 = text.Split(Path.DirectorySeparatorChar).Last();
                if (text.Length != 0 && (text2.EndsWith(".patch") || text2.EndsWith(".xml")))
                {
                    string text3 = text2.Replace(".patch", " (.patch)").Replace(".xml", " (.xml)");
                    if (!control.Items.Contains(text3))
                        control.Items.Add(text3);
                }
            }
        }

        private void GetColor()
        {
            base.Style = Prompt.ColorSet;
            Themer.Style = Prompt.ColorSet;
            Refresh();
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            if (IsWorking)
                return;
            IsWorking = true;
            Task.Delay(50).ContinueWith(delegate
            {
                for (int i = 0; i < metroListBox2.SelectedItems.Count; i++)
                    InstallHandler.Install(metroListBox2.SelectedItems[i].ToString().Replace(" (.patch)", ".patch").Replace(" (.xml)", ".xml"));
                GetNewAddons();
                IsWorking = false;
            });
        }

        private void GetNewAddons()
        {
            metroListBox1.Items.Clear();
            GenerateProperly(BnSPatch_Addons_Path, metroListBox1);
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            if (IsWorking)
                return;
            IsWorking = true;
            Task.Delay(50).ContinueWith(delegate
            {
                for (int i = 0; i < metroListBox1.SelectedItems.Count; i++)
                {
                    InstallHandler.Uninstall(metroListBox1.SelectedItems[i].ToString().Replace(" (.patch)", ".patch").Replace(" (.xml)", ".xml"));
                }
                GetNewAddons();
                IsWorking = false;
            });
        }

        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (sender == metroListBox1)
            {
                if (e.Button.ToString() == "Right")
                {
                    metroListBox1.SelectedIndex = -1;
                }
            }
            if (sender == metroListBox2)
            {
                if (e.Button.ToString() == "Right")
                {
                    metroListBox2.SelectedIndex = -1;
                }
            }
        }

        private void PLAddonManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsWorking)
            {
                Prompt.Popup("The manager is working, please wait.");
                e.Cancel = true;
            }
        }

        private void metroButton3_Click(object sender, EventArgs e)
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
