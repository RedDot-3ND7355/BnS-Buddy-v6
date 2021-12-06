using BnSBuddy2.CoreHandlers;
using BnSBuddy2.Functions;
using BnSBuddy2.Packets;
using BnSBuddy2.Properties;
using BnSBuddy2.Settings;
using FastColoredTextBoxNS;
using MaterialSkin;
using Microsoft.Win32;
using Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using _Process = System.Diagnostics.Process;

namespace BnSBuddy2
{
    public partial class Form1 : MaterialSkin.Controls.MaterialForm
    {
        // Globals
        public void KillApp() => new KillApp();
        public readonly MaterialSkinManager materialSkinManager;
        public static Form1 CurrentForm;
        public string AppPath = Path.GetDirectoryName(Application.ExecutablePath);
        public bool AppStarted = false;
        public bool CalledKiller = false;
        public ProfileManager.ActiveSessions ActiveSessions = new ProfileManager.ActiveSessions();
        public ProfileManager.ManageSessions sessions;
        public Pathing.ClientPaths ClientPaths = new Pathing.ClientPaths();
        public Pathing.SplashPaths SplashPaths = new Pathing.SplashPaths();
        public Pathing.PakPaths PakPaths = new Pathing.PakPaths();
        public Pathing.ExecuteableFile ExecuteableFile = new Pathing.ExecuteableFile();
        public MemoryCleaner MemoryCleaner = new MemoryCleaner();
        public Startup.Validate _Validate = new Startup.Validate();
        public ClassToggles.Startup StartupToggles;
        private ClassToggles.ToggleAction ToggleAction;
        public Pinger Pinger = new Pinger();
        public Splash Splash = new Splash();
        public Cleaner Cleaner = new Cleaner();
        public Pathing.AddonsPaths AddonsPaths = new Pathing.AddonsPaths(Path.GetDirectoryName(Application.ExecutablePath));
        public OnlineCount OnlineCount = new OnlineCount();
        public BuddyUpdater.UpdateHandler UpdateHandler = new BuddyUpdater.UpdateHandler();
        public AutoResetEvent waitLicense = new AutoResetEvent(initialState: false);
        public AutoResetEvent waitbw = new AutoResetEvent(initialState: false);
        public Pages.Profiles Profiles;
        public FTH FTH = new FTH();
        public Game Game = new Game();
        public ULPS ULPS = new ULPS();
        public Mods Mods = new Mods();
        public Licensing.IniLicense IniLicense;
        public CheckMD5 CheckMD5 = new CheckMD5();
        public Pathing.LauncherPaths LauncherPaths;
        public Themeing themeing = new Themeing();
        public string langremembered = "";
        public string GarenaDefPath = "";
        public string SALT = "";
        public int cpuCount = 0;
        public NCHandler NCHandler = new NCHandler();
        private Style BlueStyle = new TextStyle(Brushes.DarkCyan, null, FontStyle.Bold);
        private Style RedStyle = new TextStyle(Brushes.OrangeRed, null, FontStyle.Regular);
        private Style MaroonStyle = new TextStyle(Brushes.Maroon, null, FontStyle.Regular);
        private Style GreenStyle = new TextStyle(Brushes.Green, null, FontStyle.Italic);
        // End Globals

        // Ini App
        public Form1()
        {
            InitializeComponent();
            // Set Static
            CurrentForm = this;
            // Initialize MaterialSkinManager
            materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.EnforceBackcolorOnAllComponents = true;
            materialSkinManager.AddFormToManage(this);
            // Start Loading Form
            StartLoading(true);
            // Set Universal Culture
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US", false);
            // Read and Apply Settings
            routine._Routine();
            // Check for Mutex
            FileCheck();
            // Check if requirements are met
            new OSCheck();
            // Check if App is ran as admin
            AdminChecker();
            // Error Handling
            Unhandler();
            // Check Buddy Validity
            ValidateBuddy();
            // First time use questions
            FirstTimeUse();
            // Kill All Active Game sessions
            KillGame();
            // Auto Find Custom Client Path OR Default Path
            FindGameDir();
            // Auto Find Custom Launcher Path OR Default Path
            FindLauncherDir();
            // Find Splash Directory from language
            FindSplashDir();
            // Find Pak Directory for Modding
            FindPakDir();
            // Find Game Executeable
            FindExeFile();
            // Start Pinger
            Pinger.StartPinger();
            // Start Online User Counter
            OnlineCount.StartCount();
            // Clean Mess up
            Cleaner.MessCleaner();
            // Handle Addons
            PopulateAddons();
            // Class Toggles
            // CheckClassToggles(); // Needs rework for PAK
            // Check AMD ULPS
            ULPS.CheckULPS();
            // Check FTH
            FTH.CheckFTH(ExecuteableFile.ExecPath);
            // Check License
            RunDelay.Method(CheckLicense);
            DelayedTasks.Routine();
            // Kill Loading Form
            StartLoading(false);
        }

        //
        // Buddy Pathing
        //

        // Reset Paths
        private void RedoPaths(string region) =>
            Pathing.RedoPathing.ResetPaths(AppStarted, region);

        // Buddy Addons
        private void PopulateAddons() =>
            AddonsPaths.GenerateAddons(AppPath);

        // Client Executeable location
        private void FindExeFile() =>
            ExecuteableFile.ReturnExecFileLocation(materialComboBox2.SelectedItem.ToString());

        // Client Pathing
        private void FindGameDir() =>
           ClientPaths.FindGameInstalls();

        // Splash Pathing per language
        private void FindSplashDir() =>
            SplashPaths.ReturnSplashPath(materialComboBox2.SelectedItem.ToString(), materialComboBox3.SelectedItem.ToString());

        // Pak Pathing
        private void FindPakDir() =>
            PakPaths.ReturnPakPath(materialComboBox2.SelectedItem.ToString());

        // Launcher Pathing
        private void FindLauncherDir()
        {
            LauncherPaths = new Pathing.LauncherPaths();
            LauncherPaths.FindLauncherDirs();
        }

        //
        // End Buddy Pathing
        //


        //
        // Buddy Loading Behaviour
        //

        // Loading Globals
        Loading loading;
        BackgroundWorker loadingform;

        // Show & Hide Loading Form
        private void StartLoading(bool _loading)
        {
            if (_loading)
            {
                this.Hide();
                loadingform = new BackgroundWorker();
                loadingform.WorkerSupportsCancellation = true;
                loadingform.WorkerReportsProgress = false;
                loadingform.DoWork += bw3_DoWork;
                loadingform.RunWorkerAsync();
            }
            else
            {
                if (loadingform != null)
                    loadingform.CancelAsync();
                Size = new Size(985, 670);
                AppStarted = true;
                TopMost = true;
            }
        }

        // Do Work
        public void bw3_DoWork(object Sender, DoWorkEventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            Loading loading = new Loading();
            materialSkinManager.AddFormToManage(loading);
            System.Threading.Tasks.Task.Delay(0).ContinueWith(delegate
            {
                Control.CheckForIllegalCrossThreadCalls = false;
                loading.ShowDialog();
            });
            System.Threading.Tasks.Task.Delay(0).ContinueWith(delegate
            {
                Control.CheckForIllegalCrossThreadCalls = false;
                while (!AppStarted)
                {
                    Application.DoEvents();
                    if (AppStarted && loading != null)
                        loading.Close();
                }
            });
        }

        //
        // End Buddy Loading Behaviour
        //


        // 
        // Buddy Verification
        //

        private void ValidateBuddy() =>
            _Validate.Verify();

        //
        // End Buddy Verification
        //

        //
        // Buddy Behaviour
        //

        // Force close buddy on close
        private void Form1_FormClosing(object sender, FormClosingEventArgs e) =>
            KillApp();

        // Check Live Process and Capture, reset UI if all dead
        private void CheckProcesses_Tick(object sender, EventArgs e) =>
            LiveCheck.Check();

        // Restore UI Button (Tray Context)
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = materialSwitch43.Checked;
        }

        // Close App Button (Tray Context)
        private void toolStripMenuItem2_Click(object sender, EventArgs e) =>
            KillApp();

        // Tray Icon
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                if (materialSwitch39.Checked)
                {
                    notifyIcon1.Visible = materialSwitch39.Checked;
                    Hide();
                }
                else if (WindowState == FormWindowState.Normal)
                    Show();
            }
        }

        // Behaviour Globals
        public string t_user = Path.GetFileName(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));

        // Kill Active Sessions
        public void KillGame()
        {
            if (materialSwitch40.Checked)
            {
                ActiveSessions.KillActiveProcesses();
                ActiveSessions.ClearInactiveClients();
            }
        }

        // First Time Use
        public void FirstTimeUse()
        {
            if (routine.CurrentSettings["firsttime"] == "true")
            {
                switch (Prompt.FirstTimeUse())
                {
                    case DialogResult.Yes:
                        {
                            routineChanger("firsttime", "false", true);
                            routineChanger("autoupdate", "true", true);
                            break;
                        }
                    case DialogResult.No:
                        {
                            routineChanger("firsttime", "false", true);
                            routineChanger("autoupdate", "false", true);
                            break;
                        }
                }
            }
        }

        // Admin Checker
        private void AdminChecker() =>
            _Validate.AdminChecker();

        // Unhandling errors
        private void Unhandler()
        {
            if (!AppDomain.CurrentDomain.FriendlyName.Contains("vshost.exe"))
            {
                Application.ThreadException += Form1_UIThreadException;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            }
        }

        // Unhandling Domain errors
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception x = (Exception)e.ExceptionObject;
                string str = "empty";
                if (x.InnerException != null)
                    str = x.InnerException.Message.Replace(t_user, "[censored]");
                string str2 = x.ToString().Replace(t_user, "[censored]");
                string str3 = x.StackTrace.ToString().Replace(t_user, "[censored]");
                Prompt.Popup("An application error occurred. Please contact Endless and report this error!" + Environment.NewLine + "Error: " + str + Environment.NewLine + "InnerException: " + str2 + Environment.NewLine + "StackTrace: " + str3);
            }
            catch (Exception ex)
            {
                try
                {
                    string str2 = "empty";
                    if (ex.InnerException != null)
                        str2 = ex.InnerException.Message.Replace(t_user, "[censored]");
                    string str3 = ex.ToString().Replace(t_user, "[censored]");
                    string str4 = ex.StackTrace.ToString().Replace(t_user, "[censored]");
                    Prompt.Popup("Fatal Non-UI Error, can't proceed." + Environment.NewLine + "Reason: " + str2 + Environment.NewLine + "InncerException: " + str3 + Environment.NewLine + "StackTrace: " + str4);
                }
                finally
                {
                    Application.Exit();
                }
            }
        }

        // Unhandled UIThread errors
        private static void Form1_UIThreadException(object sender, ThreadExceptionEventArgs t)
        {
            try
            {
                string str = t.Exception.ToString().Replace(Path.GetFileName(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)), "[censored]");
                Prompt.Popup("Report this error to Endless along with a screenshot, thank you!" + Environment.NewLine + str);
            }
            catch
            {
                try
                {
                    Prompt.Popup("Fatal Windows Forms Error, can't proceed.");
                }
                finally
                {
                    Application.Exit();
                }
            }
        }

        // Mutex Check
        public void FileCheck() =>
            _Validate.CheckMutex();

        //
        // End Buddy Behaviour
        //

        //
        // General UI
        //

        // Profiles Button
        private void materialButton1_Click(object sender, EventArgs e) =>
            ProfileManager.ProfileHandler.OpenProfileManager();

        //
        // End General UI
        //

        //
        // Launcher tab page
        //

        // Fix autoscroll
        private void materialMultiLineTextBox1_TextChanged(object sender, EventArgs e)
        {
            materialMultiLineTextBox1.SelectionStart = materialMultiLineTextBox1.Text.Length;
            materialMultiLineTextBox1.ScrollToCaret();
            materialMultiLineTextBox1.Refresh();
        }

        // Play Button
        private void PlayButton_Click(object sender, EventArgs e) =>
            Game.PlayGame(true, "", "", materialComboBox2.SelectedItem.ToString());

        // Manage Sessions Button
        private void materialButton2_Click(object sender, EventArgs e)
        {
            sessions = new ProfileManager.ManageSessions();
            sessions.ShowDialog();
        }

        // Faster Toggle
        private void materialSwitch1_CheckedChanged(object sender, EventArgs e) =>
            FasterToggle.Toggle(materialSwitch1.Checked, materialComboBox2.SelectedItem.ToString(), materialComboBox3.SelectedItem.ToString());

        // Update Button
        private void materialButton5_Click(object sender, EventArgs e) =>
            UpdateHandler.UpdateTransition();

        // Refresh Online User Count Button
        private void materialButton4_Click(object sender, EventArgs e) =>
            OnlineCount.StartCount(true);

        // Clean Memory Button
        private void materialButton3_Click(object sender, EventArgs e) =>
            MemoryCleaner.CleanMem();

        // New Ping status updater
        private void materialLabel8_TextChanged(object sender, EventArgs e) =>
            Pinger.PingStatus(materialLabel8.Text);

        // Save and set new server
        private void materialComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string region = materialComboBox2.SelectedItem.ToString();
            Pinger.PingRegionChange(region);
            materialComboBox4.Visible = (region == "Korean");
            if (materialComboBox4.Visible)
                materialComboBox4.SelectedIndex = 0;
            NCHandler.LauncherInfo(region);
            string _region = RegionConvert.Convert(region);
            RedoPaths(region); // Reset paths and items
            if (ClientPaths.Installs.ContainsKey(_region))
                routineChanger("lastserver", materialComboBox2.SelectedItem.ToString());
        }

        // Add logs to launcher textbox
        public void AddLauncherLog(string text, bool logger = false)
        {
            string tmp = text;
            if (materialSwitch44.Checked)
                tmp = DateTime.Now.ToString("[dd:HH:mm:ss:fff] ") + text;
            if (materialSwitch37.Checked && !logger)
                materialMultiLineTextBox1.AppendText(tmp + Environment.NewLine);
            else if (logger)
                materialMultiLineTextBox4.AppendText(tmp + Environment.NewLine);
            if (materialSwitch36.Checked && materialSwitch17.Checked)
                File.WriteAllText(AppPath + "\\Launcher_logs.txt", materialMultiLineTextBox1.Text);
        }

        // Use All Cores Toggle
        private void materialCheckbox1_CheckedChanged(object sender, EventArgs e) =>
            routineChanger("useallcores", materialCheckbox1.Checked.ToString().ToLower());

        // No Texture Streaming Toggle
        private void materialCheckbox3_CheckedChanged_1(object sender, EventArgs e) =>
            routineChanger("notexturestreaming", materialCheckbox3.Checked.ToString().ToLower());

        // Unattended Toggle
        private void materialCheckbox2_CheckedChanged(object sender, EventArgs e) =>
            routineChanger("unattended", materialCheckbox2.Checked.ToString().ToLower());

        //
        // End Launcher tab page
        //

        //
        // ModManager tab page
        //

        // Fix Autoscroll
        private void materialMultiLineTextBox3_TextChanged(object sender, EventArgs e)
        {
            materialMultiLineTextBox3.SelectionStart = materialMultiLineTextBox3.Text.Length;
            materialMultiLineTextBox3.ScrollToCaret();
            materialMultiLineTextBox3.Refresh();
        }

        // After Checking Handler
        private void treeView2_AfterCheck(object sender, TreeViewEventArgs e) =>
            Mods.CheckerHandling(e.Node);

        // After Selecting Handler
        private void treeView2_AfterSelect(object sender, TreeViewEventArgs e) =>
            Mods.AfterSelect(e.Node.Text);

        // Extend Mod Info
        private void materialButton26_Click(object sender, EventArgs e) =>
            Mods.ReadModInfo(treeView2.SelectedNode.FullPath.Replace(" (Installed)", ""));

        // Install Selected Mods
        private void materialButton25_Click(object sender, EventArgs e) =>
            Mods.DoFileSwap(true);

        // Uninstall Selected Mods
        private void materialButton27_Click(object sender, EventArgs e) =>
            Mods.DoFileSwap(false);

        // Preview Selected Mod
        private void materialButton32_Click(object sender, EventArgs e) =>
            Mods.PreviewMod(treeView2.SelectedNode.FullPath.Replace(" (Installed)", ""));

        // Refresh Mods
        private void materialButton30_Click(object sender, EventArgs e) =>
            PakPaths.GenerateTree();

        // Open Mods Folder
        private void materialButton29_Click(object sender, EventArgs e) =>
            Mods.OpenModsFolder();

        // Add logs to modmanager textbox
        public void AddModManLogs(string text)
        {
            string tmp = text;
            if (materialSwitch44.Checked)
                tmp = DateTime.Now.ToString("[dd:HH:mm:ss:fff] ") + text;
            if (materialSwitch37.Checked)
                materialMultiLineTextBox3.AppendText(tmp + Environment.NewLine);
            if (materialSwitch36.Checked && materialSwitch35.Checked)
                File.WriteAllText(AppPath + "\\ModManager_logs.txt", materialMultiLineTextBox3.Text);
        }

        //
        // End ModManager tab page
        //

        //
        // Menu Buttons behaviour
        //
        private void materialTabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            switch (e.TabPage.Text)
            {
                case "Donators":
                    _Process.Start("https://www.bnsbuddy.com/donate/");
                    e.Cancel = true;
                    break;
                case "Buy NCoin":
                    _Process.Start("https://www.bnsbuddy.com/pages/Merch_Store/");
                    e.Cancel = true;
                    break;
                case "Extras":
                    materialComboBox1.SelectedIndex = 0;
                    ResetSize();
                    break;
                default:
                    ResetSize();
                    break;
                case "Dat Editor":
                    MaximizeBox = true;
                    break;
            }
        }

        // Reset Size to default 
        private void ResetSize()
        {
            MaximizeBox = false;
            WindowState = FormWindowState.Normal;
            Size = new Size(985, 670);
        }

        //
        // End Menu Buttons behaviour
        //


        //
        // Color controls
        //
        private void GetColor(int key = 0)
        {
            if (key == 0)
                key = materialSlider1.Value << 16 | materialSlider2.Value << 8 | materialSlider3.Value;
            TextShade textShade = materialSkinManager.Theme == MaterialSkinManager.Themes.DARK ? TextShade.WHITE : TextShade.BLACK;
            switch (materialComboBox1.SelectedItem.ToString())
            {
                case "Primary":
                    materialSkinManager.ColorScheme = new ColorScheme((Primary)key, (Primary)materialSkinManager.ColorScheme.DarkPrimaryColor.ToArgb(), (Primary)materialSkinManager.ColorScheme.LightPrimaryColor.ToArgb(), (Accent)materialSkinManager.ColorScheme.AccentColor.ToArgb(), textShade);
                    break;
                case "Dark Primary":
                    materialSkinManager.ColorScheme = new ColorScheme((Primary)materialSkinManager.ColorScheme.PrimaryColor.ToArgb(), (Primary)key, (Primary)materialSkinManager.ColorScheme.LightPrimaryColor.ToArgb(), (Accent)materialSkinManager.ColorScheme.AccentColor.ToArgb(), textShade);
                    break;
                case "Light Primary":
                    materialSkinManager.ColorScheme = new ColorScheme((Primary)materialSkinManager.ColorScheme.PrimaryColor.ToArgb(), (Primary)materialSkinManager.ColorScheme.DarkPrimaryColor.ToArgb(), (Primary)key, (Accent)materialSkinManager.ColorScheme.AccentColor.ToArgb(), textShade);
                    break;
                case "Accent":
                    materialSkinManager.ColorScheme = new ColorScheme((Primary)materialSkinManager.ColorScheme.PrimaryColor.ToArgb(), (Primary)materialSkinManager.ColorScheme.DarkPrimaryColor.ToArgb(), (Primary)materialSkinManager.ColorScheme.LightPrimaryColor.ToArgb(), (Accent)key, textShade);
                    break;
            }
            Refresh();
        }

        private void materialSwitch2_Click(object sender, EventArgs e)
        {
            materialSkinManager.Theme = materialSwitch2.Checked ? MaterialSkinManager.Themes.DARK : MaterialSkinManager.Themes.LIGHT;
            TextShade textShade = materialSkinManager.Theme == MaterialSkinManager.Themes.DARK ? TextShade.WHITE : TextShade.BLACK;
            materialSkinManager.ColorScheme = new ColorScheme((Primary)materialSkinManager.ColorScheme.PrimaryColor.ToArgb(), (Primary)materialSkinManager.ColorScheme.DarkPrimaryColor.ToArgb(), (Primary)materialSkinManager.ColorScheme.LightPrimaryColor.ToArgb(), (Accent)materialSkinManager.ColorScheme.AccentColor.ToArgb(), textShade);
            routineChanger("theme", materialSkinManager.Theme.ToString());
            Refresh();
        }

        private void materialSlider1_Click(object sender, EventArgs e)
        {
            if (ignorechange)
                return;
            GetColor();
        }

        private void materialButton8_Click(object sender, EventArgs e)
        {
            ColorDialog dialog = new ColorDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                GetColor(dialog.Color.ToArgb());
                materialSlider1.Value = dialog.Color.R;
                materialSlider2.Value = dialog.Color.G;
                materialSlider3.Value = dialog.Color.B;
            }
        }

        bool ignorechange = false;
        private void materialComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ignorechange = true;
            if (materialComboBox1.SelectedItem != null)
            {
                if (materialComboBox1.SelectedItem.ToString() == "Primary")
                {
                    materialSlider1.Value = materialSkinManager.ColorScheme.PrimaryColor.R;
                    materialSlider2.Value = materialSkinManager.ColorScheme.PrimaryColor.G;
                    materialSlider3.Value = materialSkinManager.ColorScheme.PrimaryColor.B;
                }
                else if (materialComboBox1.SelectedItem.ToString() == "Dark Primary")
                {
                    materialSlider1.Value = materialSkinManager.ColorScheme.DarkPrimaryColor.R;
                    materialSlider2.Value = materialSkinManager.ColorScheme.DarkPrimaryColor.G;
                    materialSlider3.Value = materialSkinManager.ColorScheme.DarkPrimaryColor.B;
                }
                else if (materialComboBox1.SelectedItem.ToString() == "Light Primary")
                {
                    materialSlider1.Value = materialSkinManager.ColorScheme.LightPrimaryColor.R;
                    materialSlider2.Value = materialSkinManager.ColorScheme.LightPrimaryColor.G;
                    materialSlider3.Value = materialSkinManager.ColorScheme.LightPrimaryColor.B;
                }
                else if (materialComboBox1.SelectedItem.ToString() == "Accent")
                {
                    materialSlider1.Value = materialSkinManager.ColorScheme.AccentColor.R;
                    materialSlider2.Value = materialSkinManager.ColorScheme.AccentColor.G;
                    materialSlider3.Value = materialSkinManager.ColorScheme.AccentColor.B;
                }
            }
            ignorechange = false;
        }

        // Reset Colors
        private void materialButton6_Click(object sender, EventArgs e)
        {
            themeing.ResetColors();
        }

        // Save colors
        private void materialButton7_Click(object sender, EventArgs e)
        {
            themeing.SaveColors();
        }

        //
        // End Color controls
        //

        //
        // Dat Editor Page
        //

        private void materialButton21_Click(object sender, EventArgs e)
        {
            materialContextMenuStrip1.Show(fastColoredTextBox1, new System.Drawing.Point(0, 0));
        }

        //
        // End Dat Editor Page
        //

        //
        // Ads Page
        //

        private void pictureBox1_Click(object sender, EventArgs e) =>
            _Process.Start(urltobrowseto);

        private void materialButton37_Click(object sender, EventArgs e) =>
            materialTabControl1.SelectTab(0);

        public string urltobrowseto = "https://www.exitlag.com/refer/443658";
        public PictureBox exitlag = new PictureBox();
        int adscount = 1;
        int currentad = 2;
        private void AdTimer_Tick(object sender, EventArgs e)
        {
            if (exitlag.Image == null)
            {
                exitlag.Image = pictureBox1.Image;
            }

            if (materialProgressBar1.Maximum == materialProgressBar1.Value)
            {

                if (currentad == 1)
                {
                    pictureBox1.Image = exitlag.Image;
                    exitlag.Image = null;
                    urltobrowseto = "https://www.exitlag.com/refer/443658";
                }
                if (currentad == 2)
                {
                    pictureBox1.Image = Image.FromStream(new MemoryStream(ExtractRessources.ExtractRessource(Resources.ad2)));
                    urltobrowseto = "http://nopi.ng/bnsbuddy-gunerx";
                }
                if (currentad >= adscount)
                {
                    currentad = 0;
                }
                currentad++;
                materialProgressBar1.Value = 0;
            }

            if (adscount > 1)
            {
                materialProgressBar1.Visible = true;
                if (AppStarted)
                    materialProgressBar1.PerformStep();
            }
            else
            {
                materialProgressBar1.Visible = false;
            }
        }

        //
        // End Ads Page
        //

        //
        // Settings Page
        //

        // Globals for settings
        public Routine routine = new Routine();

        // Function to change settings
        public void routineChanger(string name, string value, bool bypass = false)
        {
            if (routine.CurrentSettings[name].ToString() != value)
            {
                if (!bypass && !AppStarted)
                    return;
                routine.CurrentSettings[name] = value;
                routine.SaveSettings();
            }
        }

        // Custom Client Path Toggle
        private void materialSwitch15_CheckedChanged(object sender, EventArgs e)
        {
            materialButton51.Enabled = materialSwitch15.Checked;
            materialButton48.Enabled = materialSwitch15.Checked;
            materialButton45.Enabled = materialSwitch15.Checked;
            materialTextBox1.Enabled = materialSwitch15.Checked;
            if (!materialSwitch15.Checked)
                routineChanger("customgame", "false");
        }

        // Save Custom Client Path
        private void materialButton51_Click(object sender, EventArgs e)
        {
            routineChanger("customgamepath", materialTextBox1.Text);
            routineChanger("customgame", "true");
            Prompt.Popup("Please Restart App If Done Configurating");
        }

        // Browse Custom Client Path
        private void materialButton48_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog().ToString() == "OK")
                if (Directory.Exists(folderBrowserDialog.SelectedPath + "\\Content"))
                    materialTextBox1.Text = folderBrowserDialog.SelectedPath;
                else
                    Prompt.Popup("Error: Invalid Path! Browse for BNSR Folder. | Path: " + folderBrowserDialog.SelectedPath);
        }

        // Reset Custom Client Path
        private void materialButton45_Click(object sender, EventArgs e)
        {
            materialTextBox1.Text = "";
            routineChanger("customgamepath", "");
            materialSwitch15.Checked = false;
        }

        // Custom Launcher Path Toggle
        private void materialSwitch16_CheckedChanged(object sender, EventArgs e)
        {
            materialButton53.Enabled = materialSwitch16.Checked;
            materialButton49.Enabled = materialSwitch16.Checked;
            materialButton46.Enabled = materialSwitch16.Checked;
            materialTextBox2.Enabled = materialSwitch16.Checked;
            if (!materialSwitch16.Checked)
                routineChanger("customclient", "false");
        }

        // Save Custom Launcher Path
        private void materialButton53_Click(object sender, EventArgs e)
        {
            routineChanger("customclientpath", materialTextBox2.Text);
            routineChanger("customclient", "true");
            Prompt.Popup("Please Restart App If Done Configurating");
        }

        // Browse Custom Launcher Path
        private void materialButton49_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog().ToString() == "OK")
                if (Directory.Exists(folderBrowserDialog.SelectedPath + "\\Download") || File.Exists(folderBrowserDialog.SelectedPath + "\\Matryoshka.exe"))
                    materialTextBox2.Text = folderBrowserDialog.SelectedPath;
                else
                    Prompt.Popup("Error: Invalid Path! Browse for NCLauncher Folder. | Path: " + folderBrowserDialog.SelectedPath);
        }

        // Reset Custom Launcher Path
        private void materialButton46_Click(object sender, EventArgs e)
        {
            materialTextBox2.Text = "";
            routineChanger("customclientpath", "");
            materialSwitch16.Checked = false;
        }

        // Reset Default Path
        private void materialButton47_Click(object sender, EventArgs e)
        {
            routineChanger("defaultset", "false");
            routineChanger("_default", "");
            StartupBuddy();
        }

        // Restart BnSBuddy
        public void StartupBuddy()
        {
            _Process process = new _Process();
            process.StartInfo.FileName = Application.ExecutablePath;
            process.Start();
            KillApp();
        }

        // Show Launcher Logs Toggle
        private void materialSwitch17_CheckedChanged(object sender, EventArgs e) =>
            routineChanger("launcherlogs", materialSwitch17.Checked.ToString().ToLower());

        // Show ModMan Logs Toggle
        private void materialSwitch35_CheckedChanged(object sender, EventArgs e) =>
            routineChanger("modmanlogs", materialSwitch17.Checked.ToString().ToLower());

        // Reset Settings to Default
        private void materialButton50_Click(object sender, EventArgs e)
        {
            routine.ResetSettings();
            StartupBuddy();
        }

        // Custom Mods Path Toggle
        private void materialSwitch51_CheckedChanged(object sender, EventArgs e)
        {
            materialButton52.Enabled = materialSwitch51.Checked;
            materialButton54.Enabled = materialSwitch51.Checked;
            materialButton55.Enabled = materialSwitch51.Checked;
            materialTextBox6.Enabled = materialSwitch51.Checked;
            if (!materialSwitch51.Checked)
                routineChanger("modfolderset", "false");
        }

        // Save Custom Mods Path
        private void materialButton54_Click(object sender, EventArgs e)
        {
            routineChanger("modfolder", materialTextBox6.Text);
            routineChanger("modfolderset", "true");
            Prompt.Popup("Please Restart App If Done Configurating");
        }

        // Browse Custom Mods Path
        private void materialButton52_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog().ToString() == "OK")
                if (Directory.Exists(folderBrowserDialog.SelectedPath + "\\"))
                    materialTextBox6.Text = folderBrowserDialog.SelectedPath;
                else
                    Prompt.Popup("Error: Invalid Path! Browse for a custom Mods folder. | Path: " + folderBrowserDialog.SelectedPath);
        }

        // Reset Custom Mods Path
        private void materialButton55_Click(object sender, EventArgs e)
        {
            routineChanger("modfolder", "");
            materialTextBox6.Text = "";
            materialSwitch51.Checked = false;
        }

        public ProcessPriorityClass Priority = ProcessPriorityClass.Normal;
        // Adjust Client's process priority
        private void materialComboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (materialComboBox8.SelectedItem.ToString() == "RealTime")
                Priority = ProcessPriorityClass.RealTime;
            else if (materialComboBox8.SelectedItem.ToString() == "High")
                Priority = ProcessPriorityClass.High;
            else if (materialComboBox8.SelectedItem.ToString() == "AboveNormal")
                Priority = ProcessPriorityClass.AboveNormal;
            else if (materialComboBox8.SelectedItem.ToString() == "Normal")
                Priority = ProcessPriorityClass.Normal;
            else if (materialComboBox8.SelectedItem.ToString() == "BelowNormal")
                Priority = ProcessPriorityClass.BelowNormal;
            else if (materialComboBox8.SelectedItem.ToString() == "Idle")
                Priority = ProcessPriorityClass.Idle;
            else
                Priority = ProcessPriorityClass.Normal;
            routineChanger("priority", Priority.ToString());
            SetPriority();
        }

        // Apply Priority Settings to Clients
        public void SetPriority()
        {
            if (ActiveSessions.GetActiveClientCount() != 0)
            {
                List<_Process> processes = new List<_Process>();
                processes.Concat(_Process.GetProcessesByName("BNSR").ToList());
                processes.Concat(_Process.GetProcessesByName("Client64").ToList());
                processes.Concat(_Process.GetProcessesByName(materialTextBox5.Text.Replace(".exe", "")).ToList());
                // Continue
                foreach (_Process process in processes)
                {
                    if (process.PriorityClass != Priority)
                    {
                        process.PriorityClass = Priority;
                        AddLauncherLog("Changed Priority.");
                    }
                }
            }
        }

        // Save custom client name
        private void materialTextBox5_TextChanged(object sender, EventArgs e) =>
            routineChanger("customclientname", materialTextBox5.Text);

        // Save Logs toggle
        private void materialSwitch36_CheckedChanged(object sender, EventArgs e) =>
            routineChanger("savelogs", materialSwitch36.Checked.ToString().ToLower());

        // Show Logs Toggle
        private void materialSwitch37_CheckedChanged(object sender, EventArgs e) =>
            routineChanger("showlogs", materialSwitch37.Checked.ToString().ToLower());

        // Show TimeStamps Toggle
        private void materialSwitch44_CheckedChanged(object sender, EventArgs e) =>
            routineChanger("timestamplogs", materialSwitch44.Checked.ToString().ToLower());

        // Save Client Arguments
        private void materialTextBox4_TextChanged(object sender, EventArgs e) =>
            routineChanger("arguments", materialTextBox4.Text);

        // Select new default lang
        private void materialComboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (materialComboBox6.SelectedIndex == -1) return;
            materialComboBox3.SelectedIndex = materialComboBox3.FindStringExact(materialComboBox6.SelectedItem.ToString());
            SetDefaultLanguage(materialComboBox6.SelectedItem.ToString());
        }

        // Save new default lang to settings
        private void SetDefaultLanguage(string promptValue) =>
            routineChanger("langpath", promptValue);

        // Auto Login Toggle
        private void materialSwitch49_CheckedChanged(object sender, EventArgs e) =>
            routineChanger("autologin", materialSwitch49.Checked.ToString().ToLower());

        // Remember me Toggle
        private void materialSwitch50_CheckedChanged(object sender, EventArgs e) =>
            routineChanger("rememberme", materialSwitch50.Checked.ToString().ToLower());

        // Auto clean memory on game startup
        private void materialSwitch52_CheckedChanged(object sender, EventArgs e) =>
            routineChanger("automemorycleanup", materialSwitch52.Checked.ToString().ToLower());

        // Save Interval for memory cleaner
        private void materialComboBox7_SelectedIndexChanged(object sender, EventArgs e) =>
            routineChanger("cleanint", materialComboBox7.SelectedItem.ToString());

        // Boost Process Priority Toggle
        private void materialSwitch53_CheckedChanged(object sender, EventArgs e) =>
            routineChanger("boostprocess", materialSwitch53.Checked.ToString().ToLower());

        // Admin Startup Check Toggle
        private void materialSwitch38_CheckedChanged(object sender, EventArgs e) =>
            routineChanger("admincheck", materialSwitch38.Checked.ToString().ToLower());

        // Minimize to tray Toggle
        private void materialSwitch39_CheckedChanged(object sender, EventArgs e) =>
            routineChanger("minimize", materialSwitch39.Checked.ToString().ToLower());

        // Auto Game Killer Toggle
        private void materialSwitch40_CheckedChanged(object sender, EventArgs e) =>
            routineChanger("gamekiller", materialSwitch40.Checked.ToString().ToLower());

        // Ping Checker Toggle
        private void materialSwitch41_CheckedChanged(object sender, EventArgs e)
        {
            routineChanger("pingchecker", materialSwitch41.Checked.ToString().ToLower());
            if (materialSwitch41.Checked)
                Pinger.StartPinger();
            else
                Pinger.StopPinger();
        }

        // Show Online User Count Toggle
        private void materialSwitch59_CheckedChanged(object sender, EventArgs e)
        {
            bool _hide = !materialSwitch59.Checked;
            materialButton4.Visible = _hide;
            materialLabel13.Visible = _hide;
            routineChanger("showcount", _hide.ToString().ToLower());
        }

        // Save new refersh value for ping checker
        private void materialScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            materialLabel64.Text = materialScrollBar1.Value.ToString();
            routineChanger("prtime", materialScrollBar1.Value.ToString());
        }

        // BnSBuddy Auto Update Toggle
        private void materialSwitch42_CheckedChanged(object sender, EventArgs e) =>
            routineChanger("autoupdate", materialSwitch42.Checked.ToString().ToLower());

        // Keep in Tray Toggle
        private void materialSwitch43_CheckedChanged(object sender, EventArgs e)
        {
            routineChanger("keepintray", materialSwitch43.Checked.ToString().ToLower());
            notifyIcon1.Visible = materialSwitch43.Checked;
        }

        // Show GCD Estimation on Launcher Tab Toggle
        private void materialSwitch55_CheckedChanged(object sender, EventArgs e)
        {
            routineChanger("gcdshow", materialSwitch55.Checked.ToString().ToLower());
            materialLabel91.Visible = materialSwitch55.Checked;
            materialLabel92.Visible = materialSwitch55.Checked;
        }

        // Show Affinity window
        private void materialButton56_Click(object sender, EventArgs e)
        {
            string backup = materialLabel88.Text;
            Pages.Affinity Affinity = new Pages.Affinity();
            Affinity.Enabled = true;
            if (Affinity.ShowDialog() != DialogResult.OK)
                materialLabel88.Text = backup;
        }

        // Start Buddy with Windows Toggle
        private void materialSwitch45_CheckedChanged(object sender, EventArgs e)
        {
            if (!BuddyLoaded) return;
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (materialSwitch45.Checked)
                rk.SetValue(_Process.GetCurrentProcess().ProcessName, Application.ExecutablePath);
            else
                rk.DeleteValue(_Process.GetCurrentProcess().ProcessName, false);
            routineChanger("startwWindows", materialSwitch45.Checked.ToString().ToLower());
        }

        bool BuddyLoaded = false;
        // Trigger to ready changes when form shown
        private void Form1_Shown(object sender, EventArgs e) { BuddyLoaded = true; TopMost = false; Refresh(); }

        // Auto Game Catcher Toggle
        private void materialSwitch46_CheckedChanged(object sender, EventArgs e) =>
            routineChanger("autogamecatcher", materialSwitch46.Checked.ToString().ToLower());

        // Game Version Check Toggle
        private void materialSwitch47_CheckedChanged(object sender, EventArgs e) =>
            routineChanger("gameversioncheck", materialSwitch47.Checked.ToString().ToLower());

        // Maintenance Check Toggle
        private void materialSwitch48_CheckedChanged(object sender, EventArgs e) =>
            routineChanger("maintenancecheck", materialSwitch48.Checked.ToString().ToLower());

        public int customValue = 0;
        // Use Affinity Toggle
        private void materialSwitch54_CheckedChanged(object sender, EventArgs e) =>
            routineChanger("affinityman", materialSwitch54.Checked.ToString().ToLower());

        private void materialSwitch57_CheckedChanged(object sender, EventArgs e) =>
            routineChanger("updatechecker", materialSwitch57.Checked.ToString().ToLower());

        //
        // End Settings Page
        //

        //
        // Home Page
        //

        // Home Globals
        public string readids = "";
        public msghandler.Inbox Inboxer = new msghandler.Inbox();
        // End Home Globals

        // Flash Inbox Button
        private void FlashInbox_Tick(object sender, EventArgs e)
        {
            if (!materialButton57.Text.StartsWith("0"))
                materialButton57.HighEmphasis = !materialButton57.HighEmphasis;
            else
                materialButton57.HighEmphasis = false;
        }

        //
        // End Home Page
        //

        //
        // Splash Page
        //

        // Refresh Button
        private void materialButton35_Click(object sender, EventArgs e) =>
            Splash.GenerateTree();

        // Preview Button
        private void materialButton36_Click(object sender, EventArgs e) =>
            Splash.PreviewSelected(materialListBox1.SelectedItem.Text);

        // Selection Handling
        private void materialListBox1_SelectedIndexChanged(object sender, MaterialListBoxItem selectedItem) =>
            Splash.SelectedChanged(selectedItem.Text);

        // Splash Folder Button
        private void materialButton34_Click(object sender, EventArgs e) =>
            Splash.OpenSplashFolder();

        // Restore Button
        private void materialButton33_Click(object sender, EventArgs e) =>
            Splash.RestoreSplash();

        // Change Button
        private void materialButton31_Click(object sender, EventArgs e) =>
            Splash.InstallSplash(materialListBox1.SelectedItem.Text);

        //
        // End Splash Page
        //

        //
        // Extras Page
        //

        // Check Toggle Status
        private void CheckClassToggles()
        {
            StartupToggles = new ClassToggles.Startup(materialComboBox2.SelectedItem.ToString());
            ToggleAction = new ClassToggles.ToggleAction(materialComboBox2.SelectedItem.ToString());
        }

        // Compatibility Warning
        private void pictureBox4_Click(object sender, EventArgs e) =>
            FTH.Warning();

        // Clear Compatiblity Entries
        private void materialButton17_Click(object sender, EventArgs e) =>
            FTH.ClearCompat();

        // Clear FTH Entries
        private void materialButton16_Click(object sender, EventArgs e) =>
            FTH.ClearFTH();

        // Exclude from FTH
        private void materialButton59_Click(object sender, EventArgs e) =>
            FTH.Exclude(ExecuteableFile.ExecPath);

        // FTH Toggle
        private void materialSwitch5_CheckedChanged(object sender, EventArgs e) =>
            FTH.FTHToggle(materialSwitch5.Checked);

        // ULPS Toggle
        private void materialSwitch3_CheckedChanged(object sender, EventArgs e) =>
            ULPS.ULPSHandler(materialSwitch3.Checked);

        // Manage License Button
        private void materialButton14_Click(object sender, EventArgs e)
        {
            Hide();
            Licensing.LicenseUI license = new Licensing.LicenseUI();
            license.ShowDialog();
            Show();
        }

        // Check License Routine
        private void CheckLicense()
        {
            // Set SALT
            SALT = FingerPrint.Value();
            // Check License
            IniLicense = new Licensing.IniLicense();
            // Set Completed
            CurrentForm.waitLicense.Set();
        }

        //
        // End Extras Page
        //

        //
        // Addons Page
        //

        // After Select TreeView
        private void treeView3_AfterSelect(object sender, TreeViewEventArgs e) =>
            CoreHandlers.Addons.AfterSelect(AddonsPaths.AddonsPath + "\\" + e.Node.FullPath.Replace(" (.patch)", ".patch").Replace(" (.xml)", ".xml"));

        // After Check TreeView
        private void treeView3_AfterCheck(object sender, TreeViewEventArgs e) =>
            CoreHandlers.Addons.AfterCheck();

        // Restore Button
        private void materialButton41_Click(object sender, EventArgs e) =>
            CoreHandlers.Addons.ManualRestoreGameAddons();

        // Start Patching Button
        private void materialButton42_Click(object sender, EventArgs e) =>
            CoreHandlers.Addons.ManualStartGameAddons();

        // Addon Folder Button
        private void materialButton38_Click(object sender, EventArgs e) =>
            CoreHandlers.Addons.OpenAddonsFolder(AddonsPaths.AddonsPath);

        // Refresh Button
        private void materialButton39_Click(object sender, EventArgs e) =>
            AddonsPaths.GenerateAddons(AppPath);

        // Help Button
        private void materialButton44_Click(object sender, EventArgs e) =>
            Prompt.Popup("How to use: \r\n 1: Add xml/patch in \"addons\" folder \r\n 2: Press refresh, then tick the addon you want to use \r\n 3: Patch / Play to automatically apply addons");

        // Select all checkbox
        private void materialCheckbox4_CheckedChanged(object sender, EventArgs e) =>
            CoreHandlers.Addons.SelectAllCheckBox(materialCheckbox4.CheckState);

        // Fix autoscroll
        private void materialMultiLineTextBox4_TextChanged(object sender, EventArgs e)
        {
            materialMultiLineTextBox4.SelectionStart = materialMultiLineTextBox4.Text.Length;
            materialMultiLineTextBox4.ScrollToCaret();
            materialMultiLineTextBox4.Refresh();
        }

        // Manage BNSPATCH Addons
        private void materialButton43_Click(object sender, EventArgs e)
        {
            PluginLoader.PLAddonManager PLAddonManager = new PluginLoader.PLAddonManager();
            Hide();
            PLAddonManager.ShowDialog();
            Show();
        }

        // Addon creator Button
        private void materialButton40_Click(object sender, EventArgs e)
        {
            Hide();
            Pages.AddonCreator form = new Pages.AddonCreator();
            form.ShowDialog();
            Show();
        }

        //
        // End Addons Page
        //

        //
        // Extras Page
        //

        // Manage Plugins
        private void materialButton15_Click(object sender, EventArgs e)
        {
            PluginLoader.PLGui pLGui = new PluginLoader.PLGui();
            Hide();
            pLGui.ShowDialog();
            Show();
        }

        //
        // End Extras Page
        //

        //
        // Dat Editor Page
        //

        // Unpack Pak Button
        private void materialButton58_Click(object sender, EventArgs e) =>
            PAK.PakHandler.PakUnpack(PakPaths.PakPath + "Pak0-Local.pak");

        // Repack Pak Button
        private void materialButton28_Click(object sender, EventArgs e) =>
            PAK.PakHandler.PakRepack(PAK.PakHandler.ExtractedDir);

        // Report Progress
        public void SortOutputHandler(string text)
        {
            materialLabel42.Text = "Status: " + text;
            materialLabel42.Refresh();
        }

        // After select dat
        private void materialComboBox5_SelectedIndexChanged(object sender, EventArgs e) =>
            Pilao.ComboBoxHandler.PopulateTree(materialComboBox5.SelectedItem.ToString());

        // After select xml/x16
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e) =>
            Pilao.TreeViewHandler.ReadSelectedFile(e.Node, materialComboBox5.SelectedItem.ToString());

        // Backup Button
        private void materialButton18_Click(object sender, EventArgs e) =>
            Pilao.DatFilesMan.Backup();

        // Restore Button
        private void materialButton19_Click(object sender, EventArgs e) =>
            Pilao.DatFilesMan.Restore();

        // Save Button
        private void materialButton20_Click(object sender, EventArgs e) =>
            Pilao.FastColoredHandler.SaveFile();

        // Save As Button
        private void materialButton22_Click(object sender, EventArgs e) =>
            Pilao.FastColoredHandler.SaveAs();

        // Beautify Button
        private void materialButton24_Click(object sender, EventArgs e) =>
            Pilao.FastColoredHandler.Beautify();

        // Help Button
        private void materialButton23_Click(object sender, EventArgs e) =>
            Prompt.Popup("1: Backup your pak files by pressing \"Backup\"" + Environment.NewLine + "2: Unpack then select your file to work on from the drop down" + Environment.NewLine + "3: Select the file to edit in the tree then modify it with the TextBox on the right" + Environment.NewLine + "4: Save the modifications made by pressing \"Save\" then hit Repack" + Environment.NewLine + "Tip: Restoring your pak files can be done by pressing \"Restore\"");

        // CTRL+F - Find
        private void findToolStripMenuItem_Click(object sender, EventArgs e) =>
            fastColoredTextBox1.ShowFindDialog();

        // CTRL+R - Find & Replace
        private void findReplaceToolStripMenuItem_Click(object sender, EventArgs e) =>
            fastColoredTextBox1.ShowReplaceDialog();

        // CTRL+D - Reload
        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;
            treeView1.SelectedNode = null;
            treeView1.SelectedNode = node;
        }

        // Fix Textcolor
        private void HTMLSyntaxHighlight(Range range)
        {
            range.ClearStyle(BlueStyle, MaroonStyle, RedStyle, GreenStyle);
            range.SetStyle(MaroonStyle, "<|/>|</|>");
            range.SetStyle(MaroonStyle, "<(?<range>[!\\w]+)");
            range.SetStyle(MaroonStyle, "</(?<range>\\w+)>");
            range.SetStyle(RedStyle, "(?<range>\\S+?)='[^']*'|(?<range>\\S+)=\"[^\"]*\"|(?<range>\\S+)=\\S+");
            range.SetStyle(BlueStyle, "\\S+?=(?<range>'[^']*')|\\S+=(?<range>\"[^\"]*\")|\\S+=(?<range>\\S+)");
            range.SetStyle(GreenStyle, "<!--(?<range>\\S+?)-->");
        }

        // Fix Textcolor
        private void fctb_TextChanged(object sender, TextChangedEventArgs e) =>
            HTMLSyntaxHighlight(fastColoredTextBox1.VisibleRange);

        // Fix Textcolor
        private void fastColoredTextBox1_VisibleRangeChangedDelayed(object sender, EventArgs e) =>
            HTMLSyntaxHighlight(fastColoredTextBox1.VisibleRange);

        // License Context Menu
        private void treeView1_MouseClick(object sender, TreeNodeMouseClickEventArgs e) =>
            Pilao.FastColoredHandler.DatHandler.LicenseContextMenuPop(e);

        //
        // End Dat Editor Page
        //

    }
}
