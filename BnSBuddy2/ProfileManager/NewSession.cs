using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BnSBuddy2.Functions;
using MaterialSkin;
using MaterialSkin.Controls;

namespace BnSBuddy2.ProfileManager
{
    public partial class NewSession : MaterialForm
    {
        // Globals
        private readonly MaterialSkinManager materialSkinManager;
        private Dictionary<string, string> Identify = new Dictionary<string, string>();
        // End Globals

        public NewSession()
        {
            InitializeComponent();
            // Initialize MaterialSkinManager
            materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.EnforceBackcolorOnAllComponents = true;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = Form1.CurrentForm.materialSkinManager.Theme;
            materialSkinManager.ColorScheme = Form1.CurrentForm.materialSkinManager.ColorScheme;
            // Populate combobox
            Populate();
        }

        // Populate combobox
        private void Populate()
        {
            if (Form1.CurrentForm.materialComboBox2.SelectedItem != null)
            {
                string region = Form1.CurrentForm.materialComboBox2.SelectedItem.ToString();
                if (region == "North America" || region == "Europe")
                    region = "NA/EU";
                materialLabel2.Text = region;
                var users = new functions().GetUserListByRegion(region);
                foreach (string md5 in users.Keys)
                {
                    string tmp = "";
                    if (Form1.CurrentForm.ActiveSessions.ActiveClients.ContainsKey(region) && Form1.CurrentForm.ActiveSessions.ActiveClients[region].ContainsKey(users[md5][0]))
                        tmp = " (Active)";
                    materialComboBox2.Items.Add(users[md5][0] + tmp);
                    Identify.Add(users[md5][0], md5);
                }
                if (materialComboBox2.Items.Count > 0)
                    materialComboBox2.SelectedIndex = 0;
                else
                    materialButton1.Enabled = false;
            }
            else
            {
                Prompt.Popup("Please select a region on launcher tab.");
                Close();
            }
        }

        // Start session
        private void materialButton1_Click(object sender, EventArgs e)
        {
            if (materialComboBox2.SelectedItem.ToString().Contains(" (Active)"))
                Prompt.Popup("Please select another user that is not active.");
            else
            {
                var user = new functions().GetInfo(Identify[materialComboBox2.SelectedItem.ToString()]);
                Form1.CurrentForm.Game.PlayGame(false, user[Identify[materialComboBox2.SelectedItem.ToString()]][0], user[Identify[materialComboBox2.SelectedItem.ToString()]][1]);
                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}
