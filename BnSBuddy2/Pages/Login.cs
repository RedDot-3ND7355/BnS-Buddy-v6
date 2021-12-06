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
using Microsoft.Win32;
using BnSBuddy2.ProfileManager;
using BnSBuddy2.Functions;
using System.Text.RegularExpressions;

namespace BnSBuddy2.Pages
{
    public partial class Login : MaterialForm
    {
        // Globals
        private readonly MaterialSkinManager materialSkinManager;
        public static Login CurrentForm;
        public string username = "";
        public string password = "";
        public string Protect;
        public string result;
        public string SALT = Form1.CurrentForm.SALT;
        public bool remembered;
        public bool INTRUDER;
        public bool autologinapproved;
        private bool ready = false;
        private Dictionary<string, string> CurrentList = new Dictionary<string, string>();
        RegistryKey defaultKey = null;
        // End Globals

        public Login()
        {
            CurrentForm = this;
            InitializeComponent();
            // Initialize MaterialSkinManager
            materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.EnforceBackcolorOnAllComponents = true;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = Form1.CurrentForm.materialSkinManager.Theme;
            materialSkinManager.ColorScheme = Form1.CurrentForm.materialSkinManager.ColorScheme;
            //
            IniRegistry();
            PopulateRegions();
            GetLastUsedIfAny(CheckRememberMe());
            SortComboBox(defaultKey);
            CheckLock();
            ready = true;
        }

        private bool CheckIfAny()
        {
            string[] subKeyNames = defaultKey.GetSubKeyNames();
            if (subKeyNames.Length > 0)
                return true;
            else
                return false;
        }

        private void Perform()
        {
            if (materialTextBox1.Text != "" && materialTextBox2.Text != "")
            {
                materialButton2.Visible = false;
                try
                {
                    username = materialTextBox1.Text.ToLower().Replace(" ", "");
                    password = materialTextBox2.Text;
                    if (!materialCheckbox1.Checked)
                    {
                        try
                        {
                            string text = materialComboBox2.SelectedItem.ToString();
                            defaultKey.DeleteSubKeyTree(text);
                            if (defaultKey.GetValue("lastused") != null)
                            {
                                defaultKey.DeleteValue("lastused");
                            }
                            if (!CheckIfAny() && Form1.CurrentForm.routine.CurrentSettings["rememberme"] == "true")
                            {
                                Form1.CurrentForm.routineChanger("rememberme", "false");
                            }
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        try
                        {
                            Regex regex = new Regex(@"^[\w\.\-]+@[\w\-]+(?:\.\w{2,5})+$");
                            Match match = regex.Match(materialTextBox1.Text);
                            if (match.Success)
                            {
                                if (!materialComboBox2.Items.Contains(materialTextBox1.Text))
                                    new functions().Add(materialTextBox1.Text, materialTextBox2.Text, materialComboBox1.SelectedItem.ToString());

                                if (materialComboBox2.SelectedItem != null && materialComboBox2.Items.Contains(materialTextBox1.Text))
                                {
                                    string username = materialComboBox2.SelectedItem.ToString();
                                    string _password = new functions().GetInfo(CurrentList[username])[CurrentList[username]][1];
                                    if (_password != materialTextBox2.Text)
                                        new functions().Add(materialTextBox1.Text, materialTextBox2.Text, materialComboBox1.SelectedItem.ToString());
                                }
                                if (Form1.CurrentForm.routine.CurrentSettings["rememberme"] == "false")
                                    Form1.CurrentForm.routineChanger("rememberme", "true");
                                string md5hash = new functions().GetMD5(materialTextBox1.Text, materialComboBox1.SelectedItem.ToString());
                                defaultKey.SetValue("lastused", md5hash);
                            }
                            else { Prompt.Popup("Invalid character detected, please try again."); }
                        }
                        catch (Exception ex)
                        {
                            Prompt.Popup(ex.ToString());
                        }
                    }
                    Hide();
                    Close();
                }
                catch
                {
                    materialButton2.Visible = true;
                }
            }
            else if (materialTextBox1.Text == "" || materialTextBox2.Text == "")
            {
                if (materialTextBox1.Text == "" && materialTextBox2.Text == "")
                {
                    Prompt.Popup("Fields are empty!");
                    materialButton2.Visible = true;
                }
                else
                {
                    Prompt.Popup("One of the fields are empty!");
                    materialButton2.Visible = true;
                }
            }
            else
            {
                Prompt.Popup("Error!");
                materialButton2.Visible = true;
            }
        }

        private void CheckLock()
        {
            if (Control.IsKeyLocked(Keys.Capital))
                materialLabel3.Visible = true;
            else
                materialLabel3.Visible = false;
        }

        private void PopulateTree(RegistryKey regKey)
        {
            var user = new functions().GetUserListByRegion(materialComboBox1.SelectedItem.ToString());
            foreach (string md5 in user.Keys)
            {
                materialComboBox2.Items.Add(user[md5][0]);
                CurrentList.Add(user[md5][0], md5);
            }
        }

        private void PopulateRegions()
        {
            // Add Default region (universal)
            materialComboBox1.Items.Add("NA/EU");
            // Add all
            materialComboBox1.Items.AddRange(Form1.CurrentForm.materialComboBox2.Items.Cast<Object>().ToArray());
            // Remove incompatible
            materialComboBox1.Items.Remove("Japanese");
            materialComboBox1.Items.Remove("Russia");
            materialComboBox1.Items.Remove("Garena");
            materialComboBox1.Items.Remove("Chinese");
            // Remove NA and EU for new universal entry
            materialComboBox1.Items.Remove("North America");
            materialComboBox1.Items.Remove("Europe");
            // Select default region
            if ((Form1.CurrentForm.materialComboBox2.SelectedItem.ToString() == "North America" || Form1.CurrentForm.materialComboBox2.SelectedItem.ToString() == "Europe"))
                materialComboBox1.SelectedIndex = materialComboBox1.FindString("NA/EU");
            else
                materialComboBox1.SelectedIndex = materialComboBox1.FindString(Form1.CurrentForm.materialComboBox2.SelectedItem.ToString());
        }

        private void SortComboBox(RegistryKey localMachine)
        {
            if (materialComboBox2.Items.Count >= 1)
            {
                materialComboBox2.SelectedIndex = 0;
                if (localMachine.GetValue("lastused") != null)
                {
                    string md5 = localMachine.GetValue("lastused").ToString();
                    if (md5.Length > 1)
                    {
                        var tmp_user = new functions().GetInfo(md5);
                        if (tmp_user.ContainsKey(md5))
                            materialComboBox2.SelectedIndex = materialComboBox2.FindString(tmp_user[md5][0]);
                        else
                            localMachine.DeleteValue("lastused");
                        materialButton3.Visible = true;
                    }
                }
                string email = materialComboBox2.SelectedItem.ToString();
                var user = new functions().GetInfo(CurrentList[email]);
                materialTextBox1.Text = user[CurrentList[email]][0];
                materialTextBox2.Text = user[CurrentList[email]][1];
                materialCheckbox1.Checked = true;
                materialButton3.Visible = true;
                autologinapproved = true;
            }
            else
            {
                if (Form1.CurrentForm.routine.CurrentSettings["rememberme"] == "false")
                    materialCheckbox1.Checked = false;
                materialButton3.Visible = false;
            }
        }


        private void GetLastUsedIfAny(bool isremembered)
        {
            if (isremembered)
                PopulateTree(defaultKey);
        }

        private bool CheckRememberMe()
        {
            if (Form1.CurrentForm.routine.CurrentSettings["rememberme"] == "true")
            {
                materialComboBox2.Enabled = true;
                materialCheckbox1.Checked = true;
                return true;
            }
            return false;
        }

        private void IniRegistry()
        {
            RegistryKey localMachine = Registry.LocalMachine;
            if (localMachine.OpenSubKey("SOFTWARE\\BnS Buddy\\") == null)
            {
                Registry.LocalMachine.CreateSubKey("SOFTWARE\\BnS Buddy");
            }
            defaultKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\BnS Buddy\\", writable: true);
        }

        // Help Button
        private void materialButton1_Click(object sender, EventArgs e) =>
            Prompt.Popup("Getting 'Wrong Password...' error?" + Environment.NewLine + "Password has to be the following." + Environment.NewLine + "Must be 8 - 16 characters long." + Environment.NewLine + "Must not be similar to your email address or date of birth." + Environment.NewLine + "Must contain at least one number." + Environment.NewLine + "Must contain at least one alphabetic character(A - Z)." + Environment.NewLine + "No more than 4 of the continuous number or letter in a row." + Environment.NewLine + "No more than 4 of the same number or letter in a row." + Environment.NewLine + Environment.NewLine + "If your password does not respect the following," + Environment.NewLine + "please change it.");


        // X Button to delete saved user
        private void materialButton3_Click(object sender, EventArgs e)
        {
            try
            {
                string name = materialComboBox2.SelectedItem.ToString();
                new functions().Remove(CurrentList[name]);
                materialComboBox2.Items.Remove(name);
                materialButton3.Visible = false;
                materialTextBox1.Text = "";
                materialTextBox2.Text = "";
            }
            catch
            {
                Prompt.Popup("Error: Could not Forget user!");
            }
        }

        // Remember me checkbox
        private void materialCheckbox1_CheckedChanged(object sender, EventArgs e)
        {
            if (materialCheckbox1.Checked)
                Form1.CurrentForm.materialSwitch50.Checked = true;
            else
                Form1.CurrentForm.materialSwitch50.Checked = false;
        }

        // Key Down TextBox1 & TextBox2
        private void materialTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            CheckLock();
            if (e.KeyCode == Keys.Return)
                Perform();
        }

        // Change Selected Region
        private void materialComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ready)
            {
                CurrentList = new Dictionary<string, string>();
                materialComboBox2.Items.Clear();
                if (CheckRememberMe())
                    PopulateTree(defaultKey);
                if (materialComboBox2.Items.Count > 0)
                    materialComboBox2.SelectedIndex = 0;
            }
        }

        // Change Selected User
        private void materialComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string name = materialComboBox2.SelectedItem.ToString();
                var user = new functions().GetInfo(CurrentList[name]);
                materialTextBox1.Text = user[CurrentList[name]][0];
                materialTextBox2.Text = user[CurrentList[name]][1];
                materialButton3.Visible = true;
            }
            catch
            {
                materialButton3.Visible = false;
            }
        }

        // Sign in Button
        private void materialButton2_Click(object sender, EventArgs e) =>
            Perform();
        
    }
}
