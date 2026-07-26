using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Windows.Forms;

namespace BnSBuddy2.Pages
{
    public partial class LoginSelect : MaterialForm
    {
        // Globals
        private readonly MaterialSkinManager materialSkinManager;
        public string method = "";
        // End Globals

        public LoginSelect()
        {
            InitializeComponent();
            // Initialize MaterialSkinManager
            materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.EnforceBackcolorOnAllComponents = true;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = Form1.CurrentForm.materialSkinManager.Theme;
            materialSkinManager.ColorScheme = Form1.CurrentForm.materialSkinManager.ColorScheme;
        }

        // Email
        private void materialButton1_Click(object sender, EventArgs e)
        {
            method = "email";
            DialogResult = DialogResult.OK;
            this.Close();
        }

        // QR Code
        private void materialButton2_Click(object sender, EventArgs e)
        {
            method = "qrcode";
            DialogResult = DialogResult.OK;
            this.Close();
        }

        // Phone
        private void materialButton3_Click(object sender, EventArgs e)
        {
            method = "phone";
            DialogResult = DialogResult.OK;
            this.Close();
        }

        // Google
        private void materialButton5_Click(object sender, EventArgs e)
        {
            method = "google";
            DialogResult = DialogResult.OK;
            this.Close();
        }

        // Facebook
        private void materialButton4_Click(object sender, EventArgs e)
        {
            method = "facebook";
            DialogResult = DialogResult.OK;
            this.Close();
        }

        // Apple
        private void materialButton6_Click(object sender, EventArgs e)
        {
            method = "appleid";
            DialogResult = DialogResult.OK;
            this.Close();
        }

        // X
        private void materialButton7_Click(object sender, EventArgs e)
        {
            method = "twitter";
            DialogResult = DialogResult.OK;
            this.Close();
        }

        // Line
        private void materialButton8_Click(object sender, EventArgs e)
        {
            method = "line";
            DialogResult = DialogResult.OK;
            this.Close();
        }

        // VK
        private void materialButton9_Click(object sender, EventArgs e)
        {
            method = "vk";
            DialogResult = DialogResult.OK;
            this.Close();
        }

        // Steam
        private void materialButton10_Click(object sender, EventArgs e)
        {
            method = "steam";
            DialogResult = DialogResult.OK;
            this.Close();
        }

        // Nintendo
        private void materialButton11_Click(object sender, EventArgs e)
        {
            method = "nintendo";
            DialogResult = DialogResult.OK;
            this.Close();
        }

        // Taiwan Web
        private void materialButton12_Click(object sender, EventArgs e)
        {
            method = "nct";
            DialogResult = DialogResult.OK;
            this.Close();
        }

        // Taiwan QR Code
        private void materialButton13_Click(object sender, EventArgs e)
        {
            method = "qrcode";
            DialogResult = DialogResult.OK;
            this.Close();
        }

        // Japan Web
        private void materialButton15_Click(object sender, EventArgs e)
        {
            method = "ncj";
            DialogResult = DialogResult.OK;
            this.Close();
        }

        // Japan QR Code
        private void materialButton14_Click(object sender, EventArgs e)
        {
            method = "qrcode";
            DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
