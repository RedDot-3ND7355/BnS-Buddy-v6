using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Linq;
using System.Windows.Forms;

namespace BnSBuddy2.Database
{
    public partial class MyManager : MaterialForm
    {
        private readonly MaterialSkinManager materialSkinManager;

        public MyManager()
        {
            InitializeComponent();
            materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.EnforceBackcolorOnAllComponents = true;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = Form1.CurrentForm.materialSkinManager.Theme;
            materialSkinManager.ColorScheme = Form1.CurrentForm.materialSkinManager.ColorScheme;
            Populate();
        }

        private void Populate()
        {
            materialComboBox1.Items.Clear();
            materialComboBox1.Items.Add("Addons");
            materialComboBox1.Items.Add("Splashes");
            materialComboBox1.Items.Add("Mods");
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            if (materialListBox1.SelectedItem == null || materialComboBox1.SelectedItem == null)
                return;

            if (MaterialMessageBox.Show("Are you sure you want to delete this entry?") != DialogResult.OK)
                return;

            var entry = SelectedEntry();
            string name = entry != null ? entry.EntryName : materialListBox1.SelectedItem.Text;
            ManagerHandler.Delete(AccountHandler.Author, materialComboBox1.SelectedItem.ToString(), name);
            RefreshList();
        }

        private void materialComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshList();
        }

        private void RefreshList()
        {
            materialListBox1.Items.Clear();
            materialButton1.Enabled = false;
            materialButton2.Enabled = false;
            if (materialComboBox1.SelectedItem == null || !AccountHandler.loggedin)
                return;

            ComboboxHandler.FilterPerCategory(materialComboBox1.SelectedItem.ToString(), AccountHandler.Author);
            foreach (string key in ComboboxHandler.GeneratedPairs.Keys.OrderBy(k => k, StringComparer.OrdinalIgnoreCase))
                materialListBox1.Items.Add(new MaterialListBoxItem(key));
        }

        private void PopulateListView() => RefreshList();

        private CatalogEntry SelectedEntry()
        {
            if (materialListBox1.SelectedItem == null) return null;
            return ComboboxHandler.GeneratedPairs.TryGetValue(materialListBox1.SelectedItem.Text, out var e) ? e : null;
        }

        private void materialListBox1_SelectedIndexChanged(object sender, MaterialListBoxItem selectedItem)
        {
            bool has = materialListBox1.SelectedItem != null;
            materialButton1.Enabled = has;
            materialButton2.Enabled = has;
        }

        // Update existing entry
        private void materialButton2_Click(object sender, EventArgs e)
        {
            if (!AccountHandler.loggedin)
            {
                MaterialMessageBox.Show("Not logged in.");
                return;
            }
            if (materialComboBox1.SelectedItem == null)
            {
                MaterialMessageBox.Show("Select a category first.");
                return;
            }

            using (var upload = new UploadUI("update", materialComboBox1.SelectedItem.ToString()))
            {
                if (upload.ShowDialog() == DialogResult.OK)
                    RefreshList();
            }
        }

        // New upload
        private void materialButton3_Click(object sender, EventArgs e)
        {
            if (!AccountHandler.loggedin)
            {
                MaterialMessageBox.Show("Not logged in.");
                return;
            }

            string preset = materialComboBox1.SelectedItem?.ToString();
            using (var upload = new UploadUI("upload", preset))
            {
                if (upload.ShowDialog() == DialogResult.OK)
                    RefreshList();
            }
        }
    }
}
