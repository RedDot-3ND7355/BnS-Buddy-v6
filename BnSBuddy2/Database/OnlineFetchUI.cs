using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Linq;

namespace BnSBuddy2.Database
{
    public partial class OnlineFetchUI : MaterialForm
    {
        private readonly MaterialSkinManager materialSkinManager;
        public static OnlineFetchUI CurrentForm;

        public OnlineFetchUI()
        {
            CurrentForm = this;
            InitializeComponent();
            materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.EnforceBackcolorOnAllComponents = true;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = Form1.CurrentForm.materialSkinManager.Theme;
            materialSkinManager.ColorScheme = Form1.CurrentForm.materialSkinManager.ColorScheme;
            Populate();
            CheckAuthorAcc();
            DownloadAuthors();
        }

        private void ResetCard()
        {
            materialCard1.Enabled = false;
            authorLabel.Text = "Author: -";
            nameLabel.Text = "Name: -";
            descriptionTextbox.Text = "";
            previewButton.Enabled = false;
            downloadButton.Enabled = false;
            downloadButton.Text = "Download";
        }

        private void Populate()
        {
            materialComboBox1.Items.Clear();
            materialComboBox1.Items.Add("Addons");
            materialComboBox1.Items.Add("Splashes");
            materialComboBox1.Items.Add("Mods");
        }

        private void DownloadAuthors()
        {
            materialComboBox2.Items.Clear();
            materialComboBox2.Items.Add(""); // empty = no filter
            foreach (string author in AccountHandler.ReturnAuthors())
                materialComboBox2.Items.Add(author);
        }

        private void CheckAuthorAcc()
        {
            if (AccountHandler.loggedin)
            {
                materialLabel1.Text = "Status: Connected as " + AccountHandler.Author;
                materialButton1.Enabled = true;
            }
            else
            {
                materialLabel1.Text = "Status: Visitor";
                materialButton1.Enabled = false;
            }
        }

        private void materialButton2_Click(object sender, EventArgs e)
        {
            Hide();
            using (var loginUI = new LoginUI())
                loginUI.ShowDialog();
            Show();
            CheckAuthorAcc();
            DownloadAuthors();
        }

        private object AuthorFilter()
        {
            if (materialComboBox2.SelectedItem == null) return null;
            string a = materialComboBox2.SelectedItem.ToString();
            return string.IsNullOrWhiteSpace(a) ? null : a;
        }

        private void materialComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (materialComboBox1.SelectedItem == null) return;
            ComboboxHandler.FilterPerCategory(materialComboBox1.SelectedItem.ToString(), AuthorFilter());
            PopulateListView();
            ResetCard();
        }

        private bool ignore = false;

        private void materialButton3_Click(object sender, EventArgs e)
        {
            materialButton3.Enabled = false;
            ignore = true;
            materialComboBox2.SelectedIndex = -1;
            if (materialComboBox1.SelectedItem != null)
            {
                ComboboxHandler.FilterPerCategory(materialComboBox1.SelectedItem.ToString(), null);
                PopulateListView();
                ResetCard();
            }
        }

        private void materialComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (materialComboBox1.SelectedItem != null)
                ComboboxHandler.FilterPerCategory(materialComboBox1.SelectedItem.ToString(), AuthorFilter());
            if (!ignore)
                materialButton3.Enabled = materialComboBox1.SelectedItem != null && AuthorFilter() != null;
            else
                ignore = false;
            PopulateListView();
            ResetCard();
        }

        private void PopulateListView()
        {
            materialListBox1.Items.Clear();
            foreach (string key in ComboboxHandler.GeneratedPairs.Keys.OrderBy(k => k, StringComparer.OrdinalIgnoreCase))
                materialListBox1.Items.Add(new MaterialListBoxItem(key));

            if (ComboboxHandler.GeneratedPairs.Count == 0 &&
                !string.IsNullOrEmpty(ComboboxHandler.LastCatalogStatus))
            {
                // Status line: empty catalog / filter / error
                materialLabel1.Text = ComboboxHandler.LastCatalogStatus.Split('\n')[0].Trim();
            }
        }

        private CatalogEntry SelectedEntry()
        {
            if (materialListBox1.SelectedItem == null) return null;
            string key = materialListBox1.SelectedItem.Text;
            return ComboboxHandler.GeneratedPairs.TryGetValue(key, out var e) ? e : null;
        }

        private void materialListBox1_SelectedIndexChanged(object sender, MaterialListBoxItem selectedItem)
        {
            var entry = SelectedEntry();
            if (entry == null)
            {
                ResetCard();
                return;
            }

            materialCard1.Enabled = true;
            downloadButton.Enabled = !string.IsNullOrEmpty(entry.Url);
            downloadButton.Text = "Download";
            string cat = materialComboBox1.SelectedItem?.ToString() ?? "";
            bool canPreview = (cat == "Mods" || cat == "Splashes") && !string.IsNullOrEmpty(entry.Preview);
            previewButton.Enabled = canPreview;
            nameLabel.Text = "Name: " + entry.Filename;
            authorLabel.Text = "Author: " + entry.Author;
            descriptionTextbox.Text = entry.Description ?? "";
        }

        private void downloadButton_Click(object sender, EventArgs e)
        {
            var entry = SelectedEntry();
            if (entry == null || materialComboBox1.SelectedItem == null) return;
            string type = ComboboxHandler.TypeFolderFromUi(materialComboBox1.SelectedItem.ToString());
            FileManager.Download(entry.Url, type);
        }

        private void previewButton_Click(object sender, EventArgs e)
        {
            var entry = SelectedEntry();
            if (entry == null || string.IsNullOrEmpty(entry.Preview)) return;
            try
            {
                new PreviewWindow(entry.Preview).ShowDialog();
            }
            catch (Exception ex)
            {
                MaterialMessageBox.Show("Preview failed: " + ex.Message);
            }
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            if (!AccountHandler.loggedin)
            {
                MaterialMessageBox.Show("Log in first to manage your uploads.");
                return;
            }
            Hide();
            using (var myManager = new MyManager())
                myManager.ShowDialog();
            Show();
            // Refresh list after manage
            if (materialComboBox1.SelectedItem != null)
            {
                ComboboxHandler.FilterPerCategory(materialComboBox1.SelectedItem.ToString(), AuthorFilter());
                PopulateListView();
                ResetCard();
            }
        }
    }
}
