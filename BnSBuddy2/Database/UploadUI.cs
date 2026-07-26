using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.IO;
using System.Windows.Forms;

namespace BnSBuddy2.Database
{
    public partial class UploadUI : MaterialForm
    {
        private readonly MaterialSkinManager materialSkinManager;
        private readonly string _forcedAction; // "upload" | "update" | null = auto
        private readonly string _presetType;   // addons/mods/splashes optional

        private string _selectedFile = "";
        private string _selectedPreview = "";

        bool Condition1 = false; // type
        bool Condition2 = false; // description
        bool Condition3 = false; // file
        // Preview required only for mods

        /// <param name="action">upload, update, or null for upload default</param>
        /// <param name="presetType">optional category to preselect</param>
        public UploadUI(string action = "upload", string presetType = null)
        {
            _forcedAction = string.IsNullOrEmpty(action) ? "upload" : action.ToLowerInvariant();
            _presetType = presetType;
            InitializeComponent();
            materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.EnforceBackcolorOnAllComponents = true;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = Form1.CurrentForm.materialSkinManager.Theme;
            materialSkinManager.ColorScheme = Form1.CurrentForm.materialSkinManager.ColorScheme;

            materialLabel1.Text = "Uploading as: " + (AccountHandler.Author ?? "?");
            Text = _forcedAction == "update" ? "Update Form" : "Upload Form";

            materialButton1.Click += materialButton1_Click; // preview
            materialButton2.Click += materialButton2_Click; // submit
            materialButton3.Click += materialButton3_Click; // select file
            materialButton4.Click += materialButton4_Click; // help

            if (!string.IsNullOrEmpty(_presetType))
                PreselectType(_presetType);

            RefreshSubmitEnabled();
        }

        private void PreselectType(string type)
        {
            string t = ComboboxHandler.TypeFolderFromUi(type);
            string ui = t == "addons" ? "Addon" : t == "mods" ? "Mod" : t == "splashes" ? "Splash" : null;
            if (ui == null) return;
            for (int i = 0; i < materialComboBox1.Items.Count; i++)
            {
                if (materialComboBox1.Items[i].ToString() == ui)
                {
                    materialComboBox1.SelectedIndex = i;
                    break;
                }
            }
        }

        private void materialComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (materialComboBox1.SelectedItem == null)
            {
                Condition1 = false;
                materialButton1.Enabled = false;
                RefreshSubmitEnabled();
                return;
            }

            Condition1 = true;
            string t = materialComboBox1.SelectedItem.ToString();
            // Preview image only for Mod (and optional for Splash — splash uses bmp as preview)
            materialButton1.Enabled = t == "Mod";
            if (t != "Mod")
                _selectedPreview = "";

            // File filter hints
            switch (t)
            {
                case "Addon":
                    selectFileDlg.Filter = "Addon XML (*.xml)|*.xml|All files (*.*)|*.*";
                    break;
                case "Mod":
                    selectFileDlg.Filter = "Mod zip (*.zip)|*.zip|All files (*.*)|*.*";
                    selectPreviewImageDlg.Filter = "PNG preview (*.png)|*.png|Images|*.png;*.jpg;*.jpeg|All files (*.*)|*.*";
                    break;
                case "Splash":
                    selectFileDlg.Filter = "Splash BMP (*.bmp)|*.bmp|All files (*.*)|*.*";
                    break;
            }
            RefreshSubmitEnabled();
        }

        private void materialMultiLineTextBox21_TextChanged(object sender, EventArgs e)
        {
            Condition2 = materialMultiLineTextBox21.Text != null && materialMultiLineTextBox21.Text.Trim().Length > 0;
            RefreshSubmitEnabled();
        }

        private void materialButton3_Click(object sender, EventArgs e)
        {
            if (selectFileDlg.ShowDialog() != DialogResult.OK) return;
            _selectedFile = selectFileDlg.FileName;
            Condition3 = File.Exists(_selectedFile);
            materialButton3.Text = Condition3 ? Path.GetFileName(_selectedFile) : "Select File";
            RefreshSubmitEnabled();
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            if (selectPreviewImageDlg.ShowDialog() != DialogResult.OK) return;
            _selectedPreview = selectPreviewImageDlg.FileName;
            materialButton1.Text = File.Exists(_selectedPreview) ? Path.GetFileName(_selectedPreview) : "Preview Image";
            RefreshSubmitEnabled();
        }

        private void RefreshSubmitEnabled()
        {
            bool previewOk = true;
            if (materialComboBox1.SelectedItem != null &&
                materialComboBox1.SelectedItem.ToString() == "Mod")
            {
                // Preview strongly recommended for mods catalog; require it for a complete listing
                previewOk = !string.IsNullOrEmpty(_selectedPreview) && File.Exists(_selectedPreview);
            }
            materialButton2.Enabled = Condition1 && Condition2 && Condition3 && previewOk && AccountHandler.loggedin;
        }

        private void materialButton2_Click(object sender, EventArgs e)
        {
            if (!AccountHandler.loggedin)
            {
                MaterialMessageBox.Show("You must be logged in.");
                return;
            }

            string uiType = materialComboBox1.SelectedItem?.ToString() ?? "";
            string typeFolder = ComboboxHandler.TypeFolderFromUi(uiType);
            string action = _forcedAction == "update" ? "update" : "upload";

            // Extension sanity
            string ext = Path.GetExtension(_selectedFile).ToLowerInvariant();
            if (typeFolder == "addons" && ext != ".xml")
            {
                MaterialMessageBox.Show("Addons must be a .xml file.");
                return;
            }
            if (typeFolder == "mods" && ext != ".zip")
            {
                MaterialMessageBox.Show("Mods must be a .zip file.");
                return;
            }
            if (typeFolder == "splashes" && ext != ".bmp")
            {
                MaterialMessageBox.Show("Splashes must be a .bmp file.");
                return;
            }

            materialButton2.Enabled = false;
            materialButton2.Text = "Working...";
            Application.DoEvents();

            string result = ManagerHandler.Upload(
                _selectedFile,
                materialMultiLineTextBox21.Text.Trim(),
                typeFolder,
                AccountHandler.Author,
                action,
                _selectedPreview);

            materialButton2.Text = "Submit";
            RefreshSubmitEnabled();
            MaterialMessageBox.Show(result);

            if (result.IndexOf("has been uploaded", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void materialButton4_Click(object sender, EventArgs e)
        {
            MaterialMessageBox.Show(
                "Upload help:" + Environment.NewLine + Environment.NewLine +
                "• Addon: .xml package + description" + Environment.NewLine +
                "• Mod: .zip package + description + .png preview" + Environment.NewLine +
                "• Splash: .bmp + description (bmp is also the preview)" + Environment.NewLine + Environment.NewLine +
                "Description is stored as a .dcn next to the file on the server." + Environment.NewLine +
                "Use Update if the package name already exists." + Environment.NewLine + Environment.NewLine +
                "Logged in as: " + AccountHandler.Author + Environment.NewLine +
                "Session: " + (string.IsNullOrEmpty(AccountHandler.SessID) ? "(missing — re-login)" : "ok"));
        }
    }
}
