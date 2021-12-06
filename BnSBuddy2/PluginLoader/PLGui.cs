using BnSBuddy2.Functions;
using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BnSBuddy2.PluginLoader
{
    public partial class PLGui : MaterialForm
    {
        // Globals
        public string PluginPath = Form1.CurrentForm.ExecuteableFile.ExecFolder + "plugins";
        private readonly MaterialSkinManager materialSkinManager;
        public static PLGui CurrentForm;
        public static List<string> File_Table;
        private Dictionary<string, string> File_Local = new Dictionary<string, string>();
        private PluginHandler InstallHandler = new PluginHandler();
        private bool IsOnline = false;
        public FileTable OnlineFile = new FileTable();
        private string CurrentPlugin = "";
        private bool IsWorking = false;
        private bool BootCheck = false;
        // End Globals

        public PLGui()
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

        private void GetLocalPlugins()
        {
            materialListBox1.Items.Clear();
            File_Local = new Dictionary<string, string>();
            // create folders if missing
            if (!Directory.Exists(PluginPath))
                Directory.CreateDirectory(PluginPath);
            // this generates the table of current plugins installed/local
            FileInfo[] files = new DirectoryInfo(PluginPath).GetFiles("*.dll");
            // Reset multi until found
            for (int i = 0; i < files.Length; i++)
            {
                if (!materialListBox1.Items.Contains(new MaterialListBoxItem(files[i].Name)))
                {
                    string FileName = files[i].Name;
                    FileName = FileName.Substring(FileName.LastIndexOf(")") + 1);
                    FileName = FileName.Replace(".dll", "");
                    FileName = FileName + "_" + DateTime.ParseExact(GetDateOfFile(files[i].FullName).Replace("-", "/").Replace(".", "/").Replace(" ", ""), "MM/dd/yyyy", CultureInfo.InvariantCulture).ToString("yyyy.MM.dd");
                    materialListBox1.Items.Add(new MaterialListBoxItem(FileName));
                    File_Local.Add(FileName, files[i].FullName); // For handling of uninstalling (Please don't forget me!! <3)
                }
            }
            materialListBox1.Items.Add(new MaterialListBoxItem(""));
        }

        private void CheckPlugins()
        {
            if (IsOnline)
                foreach (string plugin in File_Table)
                    materialListBox2.Items.Add(new MaterialListBoxItem(plugin));
        }

        private void CheckOldFile()
        {
            if (IsOnline)
            {
                //Check if PluginLoader is up-to-date/outdated
                if (materialLabel1.Text != "00-00-0000" && materialLabel1.Text != "00/00/0000" && materialLabel1.Text != "00.00.0000" && materialLabel1.Text != "00. 00. 0000")
                {
                    string tmp_date = "";
                    int CurrInt = -1;
                    foreach (MaterialListBoxItem Item in materialListBox2.Items)
                    {
                        CurrInt++;
                        if (Item.Text.Contains("loader3_"))
                            break;
                    }
                    if (CurrInt == -1)
                        tmp_date = "00000000";
                    else
                    {
                        tmp_date = materialListBox2.Items[CurrInt].Text;
                        tmp_date = tmp_date.Replace("loader3_", "").Replace(".zip", "").Replace(".", "").Replace("-", "");
                        int indexOfSteam = tmp_date.IndexOf("_rc");
                        if (indexOfSteam >= 0)
                            tmp_date = tmp_date.Remove(indexOfSteam);
                        indexOfSteam = tmp_date.IndexOf("_hot");
                        if (indexOfSteam >= 0)
                            tmp_date = tmp_date.Remove(indexOfSteam);
                        tmp_date = tmp_date.Replace("UE4_", "");
                    }
                    // Get installed build
                    string tmp_curr = materialLabel1.Text;
                    tmp_curr = tmp_curr.Replace("/", "-").Replace(".", "-").Replace(" ", "");
                    //Prompt.Popup(tmp_curr); // here debug
                    tmp_curr = DateTime.ParseExact(tmp_curr, "MM-dd-yyyy", CultureInfo.InvariantCulture).ToString("yyyyMMdd");
                    int _Online = int.Parse(tmp_date);
                    int _Local = int.Parse(tmp_curr);
                    // Compare
                    if (_Online > _Local)
                        materialButton5.Enabled = true;
                }
            }
        }

        private void CheckIfOnline()
        {
            try
            {
                // this generates the table of current plugins installed/local
                GetLocalPlugins();
                // Use online file listing
                OnlineFile.GrabMegaFileTable();
                File_Table = OnlineFile.File_Table;
                // Set online if both listings were successful
                if (File_Table.Count > 0)
                    IsOnline = true;
            }
            catch
            {
                Prompt.Popup("Error: Could not grab online list of plugins.");
            }
        }

        private void CheckIfExists()
        {
            if (File.Exists(Form1.CurrentForm.ExecuteableFile.ExecFolder + "winmm.dll"))
                GetCurrPlugin();
        }

        private void GetCurrPlugin()
        {
            CurrentPlugin = GetDateOfFile(Form1.CurrentForm.ExecuteableFile.ExecFolder + "winmm.dll");
            materialLabel1.Text = CurrentPlugin;
            InfoHandler(true);
        }

        private string GetDateOfFile(string FilePath)
        {
            DateTime stamp;
            string tmp;
            if (File.Exists(FilePath))
            {
                var file = new FileInfo(FilePath);
                stamp = file.LastWriteTime;
                tmp = stamp.ToString("MM/dd/yyyy");
            }
            else
                tmp = "00-00-0000";
            return tmp;
        }

        private void InfoHandler(bool hh = false)
        {
            materialLabel1.Visible = hh;
            materialLabel1.Refresh();
            this.Refresh();
        }

        // Help Button
        private void materialButton2_Click(object sender, EventArgs e) =>
            Prompt.Popup(
                "Install: Installs the highlighted areas of the 'Online' list." + Environment.NewLine +
                "Uninstall: Uninstalls the highlighted areas of the 'Installed' list." + Environment.NewLine +
                "Old Builds: Loads more items from previous outdated builds" + Environment.NewLine +
                "Help: This popup" + Environment.NewLine +
                "Select install to install highlighted Online item OR uninstall to uninstall highlighted Installed item." + Environment.NewLine +
                "Take note: Some plugins require manual edits and/or manipulations. Uninstalling some plugins might leave leftovers of files added manually and/or more."
                );

        // Old Builds Button
        private void materialButton1_Click(object sender, EventArgs e)
        {
            if (IsWorking || BootCheck)
                return;
            Task.Delay(50).ContinueWith(delegate
            {
                IsWorking = true;
                materialLabel2.Visible = true;
                materialLabel2.Refresh();
                OnlineFile.GrabMegaFileTable(true);
                materialListBox2.Items.Clear();
                if (IsOnline)
                    foreach (string plugin in File_Table)
                        materialListBox2.Items.Add(new MaterialListBoxItem(plugin));
                materialLabel2.Visible = false;
                materialLabel2.Refresh();
                IsWorking = false;
            });
        }

        // Uninstall
        private void materialButton3_Click(object sender, EventArgs e)
        {
            if (IsWorking || materialListBox1.SelectedItem == null)
                return;
            IsWorking = true;
            Task.Delay(50).ContinueWith(delegate
            {
                //for (int i = 0; i < materialListBox1.SelectedItems.Count; i++)
                //    InstallHandler.Uninstall(File_Local[materialListBox1.SelectedItems[i].ToString()]);
                if (materialListBox1.SelectedItem.Text.Length > 0)
                    InstallHandler.Uninstall(File_Local[materialListBox1.SelectedItem.Text]);
                GetLocalPlugins();
                IsWorking = false;
            });
        }

        // Install
        private void materialButton4_Click(object sender, EventArgs e)
        {
            if (IsWorking || BootCheck || materialListBox2.SelectedItem == null)
                return;
            IsWorking = true;
            materialProgressBar1.Visible = true;
            Task.Delay(50).ContinueWith(delegate
            {
                CheckForIllegalCrossThreadCalls = false;
                //for (int i = 0; i < materialListBox2.SelectedItems.Count; i++)
                //    InstallHandler.Install(materialListBox2.SelectedItems[i].ToString());
                if (materialListBox2.SelectedItem.Text.Length > 0)
                    InstallHandler.Install(materialListBox2.SelectedItem.Text);
                GetLocalPlugins();
                materialProgressBar1.Visible = false;
                IsWorking = false;
            });
        }

        // Toggle Loader
        private void materialSwitch1_CheckedChanged(object sender, EventArgs e)
        {
            if (IsWorking || BootCheck)
            {
                materialSwitch1.Checked = !materialSwitch1.Checked;
                return;
            }
            IsWorking = true;
            Task.Delay(50).ContinueWith(delegate
            {
                if (materialSwitch1.Checked)
                {
                    InstallHandler.InstallPL("loader3_");
                    Form1.CurrentForm.materialLabel33.Text = "Active";
                    GetCurrPlugin();
                    InfoHandler(true);
                }
                else
                {
                    InstallHandler.Uninstall(Form1.CurrentForm.ExecuteableFile.ExecFolder + "winmm.dll");
                    InfoHandler();
                    materialLabel1.Text = "00-00-0000";
                    Form1.CurrentForm.materialLabel33.Text = "Inactive";
                }
                IsWorking = false;
            });
        }

        private void PLGui_Shown(object sender, EventArgs e)
        {
            BootCheck = true;
            // Routine
            Task.Delay(0).ContinueWith(delegate
            {
                CheckForIllegalCrossThreadCalls = false;
                try
                {
                    CheckIfOnline();
                    CheckIfExists();
                    CheckPlugins();
                    CheckOldFile();
                    materialLabel2.Visible = false;
                    materialLabel2.Refresh();
                    BootCheck = false;
                    Refresh();
                }
                catch (Exception ex)
                {
                    Prompt.Popup(ex.ToString());
                }
            });
            while (BootCheck) { Application.DoEvents();  materialSwitch1.Checked = (materialLabel1.Text != "00-00-0000"); Task.Delay(25); }
        }
    }
}
