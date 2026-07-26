using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Windows.Forms;

namespace BnSBuddy2.Database
{
    public partial class LoginUI : MaterialForm
    {
        // Globals
        private readonly MaterialSkinManager materialSkinManager;
        // End Globals

        public LoginUI()
        {
            InitializeComponent();
            // Initialize MaterialSkinManager
            materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.EnforceBackcolorOnAllComponents = true;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = Form1.CurrentForm.materialSkinManager.Theme;
            materialSkinManager.ColorScheme = Form1.CurrentForm.materialSkinManager.ColorScheme;
            // Check if logged in
            CheckLogin();
        }

        // Verify if logged in
        private void CheckLogin()
        {
            materialButton1.Enabled = !AccountHandler.loggedin;
            materialButton3.Enabled = AccountHandler.loggedin;
            if (AccountHandler.loggedin)
                materialLabel1.Text = $"Status: Logged in as {AccountHandler.Author}";
        }

        // Register Button
        private void materialButton2_Click(object sender, EventArgs e) =>
            materialLabel1.Text = AccountHandler.RegisterAuthor(materialTextBox21.Text, materialTextBox22.Text);

        // Login Button
        private void materialButton1_Click(object sender, EventArgs e)
        {
            materialLabel1.Text = AccountHandler.LoginAuthor(materialTextBox21.Text, materialTextBox22.Text);
            CheckLogin();
        }

        // Logout Button
        private void materialButton3_Click(object sender, EventArgs e)
        {
            materialButton3.Enabled = false;
            AccountHandler.LogoutAuthor();
            materialButton1.Enabled = !AccountHandler.loggedin;
            materialLabel1.Text = "Status: Logged out!";
        }

        // Clear text & submit results
        private void LoginUI_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            materialTextBox21.Text = "";
            materialTextBox22.Text = "";
        }

        private void materialTextBox22_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                materialButton1_Click(sender, e);
        }
    }
}
