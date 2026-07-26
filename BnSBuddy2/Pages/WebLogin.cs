using BnSBuddy2.Packets;
using MaterialSkin;
using MaterialSkin.Controls;
using System.Drawing;
using System.Windows.Forms;

namespace BnSBuddy2.Pages
{
    public partial class WebLogin : MaterialForm
    {
        // Globals
        public static WebLogin CurrentForm;
        private readonly MaterialSkinManager materialSkinManager;
        public string token = "";
        public string method = "";
        public bool LoginWorked = false;
        // End Globals

        public WebLogin()
        {
            InitializeComponent();
            this.Enabled = false;
            // Globalize
            CurrentForm = this;
            // Initialize MaterialSkinManager
            materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.EnforceBackcolorOnAllComponents = true;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = Form1.CurrentForm.materialSkinManager.Theme;
            materialSkinManager.ColorScheme = Form1.CurrentForm.materialSkinManager.ColorScheme;
            // Set Default Result
            DialogResult = DialogResult.Cancel;
            // Init Selector
            method = "";
            LoginSelect loginSelect = new LoginSelect();
            if (loginSelect.ShowDialog() == DialogResult.OK)
                if (loginSelect.method != "")
                    WebViewsCollection.method = loginSelect.method;
                else
                {
                    this.Enabled = true;
                    this.Close();
                    return;
                }
            else
            {
                this.Enabled = true;
                this.Close();
                return;
            }
            // Set WebView2 controls
            WebViewsCollection.InitBrowsers();
            // Add WebView2 controls to the form
            Controls.Add(WebViewsCollection.LoginPage);
            // Set Spinning control
            AddCircProg();
            // Check if 3rd Party
            if (WebViewsCollection.method != "email" && WebViewsCollection.method != "phone" && WebViewsCollection.method != "qrcode")
                materialCard1.Visible = true;
            this.Enabled = true;
        }

        // Add Progress Circle
        private void AddCircProg()
        {
            var progressBar = new CircularProgressBar
            {
                Size = new Size(100, 100),
                ProgressColor = Color.Green,
                LineWidth = 10f
            };
            // Center it in the form
            int x = (materialCard1.ClientSize.Width - progressBar.Width) / 2;
            int y = (materialCard1.ClientSize.Height - progressBar.Height) / 2;
            progressBar.Location = new Point(x, y);
            materialCard1.Controls.Add(progressBar);
        }

        // Prevent WebView2 Issues
        private void WebLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            Controls.Remove(WebViewsCollection.LoginPage);
        }
    }
}
