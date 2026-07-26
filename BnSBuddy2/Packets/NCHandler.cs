using BnSBuddy2.Functions;
using BnSBuddy2.NCServer;
using BnSBuddy2.Pages;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Mono.Math;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using HashAlgorithm = System.Security.Cryptography.HashAlgorithm;

namespace BnSBuddy2.Packets
{
    public class NCHandler
    {
        // Globals
        System.Windows.Forms.ComboBox RegionCB = new System.Windows.Forms.ComboBox();
        string localIP = "";
        string outsideIP = "";
        int LoginPort = 0;
        string LoginIp = "";
        string LoginId = "";
        string LoginProgramid = "";
        string LoginAppid = "";
        List<region> regions = new List<region>();
        BackgroundWorker worker;
        BackgroundWorker keepAliveBw;
        string username = "";
        string password = "";
        string epoch;
        string authEmailUsername;
        string authEmailIP;
        string authEmailCountry;
        string authEmailCode;
        string pid;
        string args = "{0}";
        string token;
        int counter;
        BNSXorEncryption xor;
        string currentAppId;
        string currentValue;
        static Mono.Math.BigInteger N = new Mono.Math.BigInteger("E306EBC02F1DC69F5B437683FE3851FD9AAA6E97F4CBD42FC06C72053CBCED68EC570E6666F529C58518CF7B299B5582495DB169ADF48ECEB6D65461B4D7C75DD1DA89601D5C498EE48BB950E2D8D5E0E0C692D613483B38D381EA9674DF74D67665259C4C31A29E0B3CFF7587617260E8C58FFA0AF8339CD68DB3ADB90AAFEE");
        static Mono.Math.BigInteger P = new Mono.Math.BigInteger("7A39FF57BCBFAA521DCE9C7DEFAB520640AC493E1B6024B95A28390E8F05787D");
        static byte[] staticKey = Conversions.HexStr2Bytes("AC34F3070DC0E52302C2E8DA0E3F7B3E63223697555DF54E7122A14DBC99A3E8");
        static Mono.Math.BigInteger Two = new Mono.Math.BigInteger(2u);
        Mono.Math.BigInteger privateKey;
        Mono.Math.BigInteger exchangeKey = Two;
        Mono.Math.BigInteger exchangeKeyServer;
        Mono.Math.BigInteger session;
        Mono.Math.BigInteger validate;
        SHA256 sha = SHA256.Create();
        byte[] key;
        bool encStart;
        Dictionary<int, string[]> responseHandler;
        TcpClient LoginServer;
        NetworkStream ns;
        string FinalToken = "";
        string regionID = "";
        Pages.enterCode enterCodePrompt;
        public bool rememberip = false;
        public string d6pin = "";
        // End Globals

        //public NCHandler() =>
        //    enterCodePrompt = new Pages.enterCode(this);

        public string LanguageConverter(string lang)
        {
            string _lang = "";
            switch (lang)
            {
                // NA/EU
                case "English":
                    _lang = "en-US";
                    break;
                case "German":
                    _lang = "de-DE";
                    break;
                case "French":
                    _lang = "fr-FR";
                    break;
                case "Spanish":
                    _lang = "es-ES";
                    break;
                // RU
                case "Russian":
                    _lang = "ru-RU";
                    break;
                // KR
                case "Korean":
                    _lang = "ko-KR";
                    break;
                // JP
                case "Japanese":
                    _lang = "ja-JP";
                    break;
                // CH
                case "Chinese":
                    _lang = "zh-CN";
                    break;
                case "ChineseT":
                    _lang = "zh-TW";
                    break;
                // TW
                case "Taiwanese":
                    _lang = "zh-TW";
                    break;
                // TW
                case "ChineseTaiwan":
                    _lang = "zh-TW";
                    break;
                // Other 
                case "Thai":
                    _lang = "th-TH";
                    break;
                default:
                    _lang = lang;
                    break;
            }
            return _lang;
        }


        public string StringBuilder(string region) // todo finish this shit
        {
            string _presence = "";
            string _gamecode = "";
            string _gameid = "";
            if (region == "Steam EU")
                regionID = "eu";
            if (region == "Steam NA")
                regionID = "na";
            if (region == "Korean")
            {
                if (Form1.CurrentForm.materialComboBox4.SelectedItem.ToString() == "Live")
                {
                    _presence = "BNS_LIVE:B0D42105-0CB6-BC9F-3CB2-BE28A0662340";
                    _gamecode = "bnsncs";
                    _gameid = "BNS_LIVE";
                }
                else if (Form1.CurrentForm.materialComboBox4.SelectedItem.ToString() == "Neo")
                {
                    _presence = "BNSNEO_LIVE:2B447C4E-122B-4B82-943E-3BA9990162B9";
                    _gamecode = "bnsneo";
                    _gameid = "BNSNEO_LIVE";
                }
                else if (Form1.CurrentForm.materialComboBox4.SelectedItem.ToString() == "Test")
                {
                    _presence = "BNS_TEST:18A2B067-7A7E-DA99-CDF1-3BBE3BE93F68";
                    _gamecode = "bnsneo";
                    _gameid = "BNS_TEST";
                }
            }
            if (region == "Japanese")
            {
                if (Form1.CurrentForm.materialComboBox4.SelectedItem.ToString() == "Live")
                {
                    _presence = "BNS_JPN_UE4:C2889F54-F81F-90F0-6A51-7A24012DA745";
                    _gamecode = "bnsncj";
                    _gameid = "BNS_JPN_UE4";
                }
                else if (Form1.CurrentForm.materialComboBox4.SelectedItem.ToString() == "Neo")
                {
                    _presence = "BNS_NEO_JP:1A080A25-91A9-40F3-83B6-A4156C82A4BBD";
                    _gamecode = "bnsneoncj";
                    _gameid = "BNS_NEO_JP";
                }
            }
            if (region == "Taiwan")
            {
                if (Form1.CurrentForm.materialComboBox4.SelectedItem.ToString() == "Live")
                {
                    _presence = "TWBNSUE4:33BC338F-2651-8ECD-9E2A-444843707997";
                    _gamecode = "bnsnct";
                    _gameid = "TWBNSUE4";
                }
                else if (Form1.CurrentForm.materialComboBox4.SelectedItem.ToString() == "Neo")
                {
                    _presence = "TWBNSNEO:BC03FF5B-800F-418E-8CA2-5653FD06B2CD";
                    _gamecode = "bnsneonct";
                    _gameid = "TWBNSNEO";
                }
            }
            if (region == "North America")
            {
                if (Form1.CurrentForm.materialComboBox4.SelectedItem.ToString() == "Live")
                {
                    _presence = "BnS_UE4:95684EE8-121E-0275-3258-4339DAD72236";
                    _gamecode = "bnsncwna";
                    _gameid = "BnS_UE4";
                    regionID = "na";
                }
                else if (Form1.CurrentForm.materialComboBox4.SelectedItem.ToString() == "Neo")
                {
                    _presence = "BNSNEO:32A8F729-55F2-40A8-BE1D-7C907E9F6764";
                    _gamecode = "bnsneoncana";
                    _gameid = "BNSNEO";
                    regionID = "na";
                }
            }
            if (region == "Europe")
            {
                if (Form1.CurrentForm.materialComboBox4.SelectedItem.ToString() == "Live")
                {
                    _presence = "BnS_UE4:FBB46F0C-2E6F-FC3C-31E2-6AA5ACDE5DA0";
                    _gamecode = "bnsncweu";
                    _gameid = "BnS_UE4";
                    regionID = "eu";
                }
                else if (Form1.CurrentForm.materialComboBox4.SelectedItem.ToString() == "Neo")
                {
                    _presence = "BNSNEO:ADA46A8F-65C2-46F2-BF84-2B09428B4538";
                    _gamecode = "bnsneoncaeu";
                    _gameid = "BNSNEO";
                    regionID = "eu";
                }
            }
            if ((region == "Purple" || region == "Korean") && Form1.CurrentForm.materialComboBox4.SelectedItem.ToString() == "Test")
                region = "Korean Test";
            string Args = "";
            switch (region)
            {
                case "Japanese":
                    Args = $"/LaunchByLauncher /AuthnToken:\"{FinalToken}\" /Sesskey:\"{FinalToken}\" -pzToken:\"{FinalToken}\" /ServiceRegion:\"{LoginId}\" /AuthProviderCode:\"np\" /CompanyID:\"14\" /ChannelGroupIndex:\"-1\" -gameId:\"BNS_JPN_UE4\" /PresenceId:\"BNS_JPN_UE4:C2889F54-F81F-90F0-6A51-7A24012DA745\" -gameCode:\"bnsncj\" -serviceNetwork:\"Live\" -serviceRegion:\"NCJ\" -appDataPath:\"Purple\"" + (Form1.CurrentForm.materialCheckbox1.Checked ? " -USEALLAVAILABLECORES" : "") + (Form1.CurrentForm.materialCheckbox2.Checked ? " -UNATTENDED" : "") + (Form1.CurrentForm.materialCheckbox3.Checked ? " -NOTEXTURESTREAMING " : "") + " " + Form1.CurrentForm.materialTextBox4.Text;
                    break;
                case "Korean":
                    Args = $"/LaunchByLauncher /AuthnToken:\"{FinalToken}\" /SessKey:\"{FinalToken}\" -pzToken:\"{FinalToken}\" /ServiceRegion:\"{LoginId}\" /AuthProviderCode:\"np\" /CompanyID:\"0\" -gameCode:\"{_gamecode}\" /ServiceNetwork:\"Live\" /NPServerAddr:\"https://api.ncsoft.com:443\" -gameId:\"{_gameid}\" /PresenceId:\"{_presence}\" -serviceRegion:\"NCS\" -appDataPath:\"Purple\"" + (Form1.CurrentForm.materialCheckbox1.Checked ? " -USEALLAVAILABLECORES" : "") + (Form1.CurrentForm.materialCheckbox2.Checked ? " -UNATTENDED" : "") + (Form1.CurrentForm.materialCheckbox3.Checked ? " -NOTEXTURESTREAMING " : "") + " " + Form1.CurrentForm.materialTextBox4.Text;
                    break;
                case "North America":
                case "Europe":
                    Args = $"/LaunchByLauncher /AuthnToken:\"{FinalToken}\" /SessKey:\"{FinalToken}\" -pzToken:\"{FinalToken}\" /ServiceRegion:\"{LoginId}\" /AuthProviderCode:\"np\" /CompanyID:\"12\" -gameCode:\"{_gamecode}\" /ServiceNetwork:\"Live\" -gameId:\"{_gameid}\" /PresenceId:\"{_presence}\" -serviceRegion:\"NCW\" -lang:\"{Form1.CurrentForm.materialComboBox3.SelectedItem.ToString()}\" -textLang:\"{LanguageConverter(Form1.CurrentForm.materialComboBox3.SelectedItem.ToString())}\" -voiceLang:\"{LanguageConverter(Form1.CurrentForm.materialComboBox3.SelectedItem.ToString())}\" -purpleLang:\"{LanguageConverter(Form1.CurrentForm.materialComboBox3.SelectedItem.ToString())}\" --additionalPublisher:\"ncwest\" -game-region:\"{regionID}\" -appDataPath:\"Purple\"" + (Form1.CurrentForm.materialCheckbox1.Checked ? " -USEALLAVAILABLECORES" : "") + (Form1.CurrentForm.materialCheckbox2.Checked ? " -UNATTENDED" : "") + (Form1.CurrentForm.materialCheckbox3.Checked ? " -NOTEXTURESTREAMING " : "") + " " + Form1.CurrentForm.materialTextBox4.Text;
                    break;
                case "Taiwan":
                    Args = $"/LaunchByLauncher /AuthnToken:\"{FinalToken}\" /SessKey:\"{FinalToken}\" -pzToken:\"{FinalToken}\" /ServiceRegion:\"{LoginId}\" /AuthProviderCode:\"np\" /CompanyID:\"15\" -gameCode:\"{_gamecode}\" /ServiceNetwork:\"Live\" -gameId:\"{_gameid}\" /PresenceId:\"{_presence}\" -serviceRegion:\"NCT\" -lang:\"{Form1.CurrentForm.materialComboBox3.SelectedItem.ToString()}\" -textLang:\"{LanguageConverter(Form1.CurrentForm.materialComboBox3.SelectedItem.ToString())}\" -voiceLang:\"{LanguageConverter(Form1.CurrentForm.materialComboBox3.SelectedItem.ToString())}\" -purpleLang:\"{LanguageConverter(Form1.CurrentForm.materialComboBox3.SelectedItem.ToString())}\" -appDataPath:\"Purple\"" + (Form1.CurrentForm.materialCheckbox1.Checked ? " -USEALLAVAILABLECORES" : "") + (Form1.CurrentForm.materialCheckbox2.Checked ? " -UNATTENDED" : "") + (Form1.CurrentForm.materialCheckbox3.Checked ? " -NOTEXTURESTREAMING " : "") + " " + Form1.CurrentForm.materialTextBox4.Text;
                    break;
                case "Steam NA":
                case "Steam EU":
                    Args = $"/GameRegion:{regionID} -textLang:{LanguageConverter(Form1.CurrentForm.materialComboBox3.SelectedItem.ToString())} /launchbylauncher /sesskey -authntoken";
                    break;
                default: // OLD
                    Args = "-lang:" + Form1.CurrentForm.materialComboBox3.SelectedItem.ToString() + " /sesskey /launchbylauncher /CompanyID:12" + (Form1.CurrentForm.materialCheckbox1.Checked ? " -USEALLAVAILABLECORES" : "") + (Form1.CurrentForm.materialCheckbox2.Checked ? " -UNATTENDED" : "") + (Form1.CurrentForm.materialCheckbox3.Checked ? " -NOTEXTURESTREAMING " : "") + Form1.CurrentForm.materialTextBox4.Text;
                    break;
            }
            return Args;
        }

        public string GetLogin(string _username = "", string _password = "", string origreg = "")
        {
            if (!LauncherInfo(origreg, true))
                return "Maintenance";
            Form1.CurrentForm.Hide();
            SetupBusyOverlay();
            if (_username.Length == 0 && _password.Length == 0)
            {
                if (!Form1.CurrentForm.materialSwitch62.Checked)
                {
                    Pages.Login splash = new Pages.Login();
                    splash.ShowDialog();
                    _username = splash.username.ToString().ToLower();
                    _password = splash.password.ToString();
                }
                else
                {
                    // Purple Login
                    webLogin = new Pages.WebLogin();
                    try
                    {
                        if (webLogin.ShowDialog() == DialogResult.OK)
                        {
                            WebToken = webLogin.token;
                            webLogin.Dispose();
                            Form1.CurrentForm.Show();
                            Form1.CurrentForm.WindowState = FormWindowState.Normal;
                            StartBusyOverlay();
                            GrabToken(origreg);
                            StopBusyOverlay();
                            return "Logged";
                        } 
                        else
                        {
                            webLogin.Dispose();
                            Form1.CurrentForm.Show();
                            Form1.CurrentForm.WindowState = FormWindowState.Normal;
                            return "Cancelled";
                        }
                    }
                    catch
                    {
                        webLogin.Dispose();
                        Form1.CurrentForm.Show();
                        Form1.CurrentForm.WindowState = FormWindowState.Normal;
                        return "Cancelled";
                    }
                }
            }
            if (_username.Length > 0 && _password.Length > 0 || Form1.CurrentForm.materialSwitch62.Checked)
            {
                username = _username;
                password = _password;
                Form1.CurrentForm.Show();
                Form1.CurrentForm.WindowState = FormWindowState.Normal;

                StartBusyOverlay();
                GrabToken(origreg);
                StopBusyOverlay();

                return "Logged";
            }
            Form1.CurrentForm.Show();
            Form1.CurrentForm.WindowState = FormWindowState.Normal;
            return "Cancelled";
        }

        // Busy overlay globals
        private Panel busyPanel;
        private System.Windows.Forms.Timer overlayTimer;
        private Label busyLabel;

        // Setup the busy overlay
        private void SetupBusyOverlay()
        {
            // Create the overlay panel
            busyPanel = new Panel
            {
                Size = Form1.CurrentForm.ClientSize, // Match the form's client area
                BackColor = Color.FromArgb(128, Color.Gray), // Semi-transparent gray
                Visible = false // Hidden by default
            };

            // Create a label to indicate "Busy"
            busyLabel = new Label
            {
                Text = "Busy, please wait...",
                ForeColor = Color.White,
                Font = new Font("Arial", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point((busyPanel.Width - 150) / 2, (busyPanel.Height - 20) / 2) // Approximate centering
            };
            busyPanel.Controls.Add(busyLabel);

            // Add the panel to the form and bring it to the front
            Form1.CurrentForm.Controls.Add(busyPanel);
            busyPanel.BringToFront();

            // Set up the timer to force repaints (optional, since the label is static)
            overlayTimer = new System.Windows.Forms.Timer
            {
                Interval = 100 // Refresh every 100ms
            };
            overlayTimer.Tick += (s, e) =>
            {
                busyPanel.Invalidate(); // Force repaint of the panel
                busyPanel.Update(); // Ensure it updates immediately
            };
        }

        private void StartBusyOverlay()
        {
            // Remove sh*t 
            Form1.CurrentForm.DrawerShowIconsWhenHidden = false;
            Form1.CurrentForm.Refresh();
            // Show the overlay
            busyPanel.Visible = true;
            busyPanel.BringToFront();
            overlayTimer.Start();
            busyPanel.Refresh();
            Thread.Sleep(100);
        }

        private void StopBusyOverlay()
        {
            overlayTimer.Stop();
            busyPanel.Visible = false;
            // Manually refresh the UI if needed
            Form1.CurrentForm.DrawerShowIconsWhenHidden = true;
            Form1.CurrentForm.Invalidate();
            Form1.CurrentForm.Update();
        }

        public bool LauncherInfo(string _region, bool login = false)
        {
            regions = new List<region>();
            Form1.CurrentForm.PlayButton.Enabled = false;
            if (_region == "North America" || _region == "Europe" || _region == "Taiwan" || _region.Contains("Korean") || _region == "Japanese" || _region == "Steam NA" || _region == "Steam EU")
            {
                if (!_region.Contains("Steam"))
                {
                    if (CheckMaintenance(_region))
                        return false;
                    if (!VersionCheck(_region))
                        return false;
                    if (login)
                        AvailableRegions(_region);
                }
                Form1.CurrentForm.PlayButton.Enabled = true;
                Form1.CurrentForm.AddLauncherLog("Added");
            }
            return true;
        }

        public void termConnection()
        {
            if (LoginServer != null && LoginServer.Connected)
                LoginServer.Close();
            if (worker != null && worker.IsBusy)
                worker.CancelAsync();
            if (keepAliveBw != null && keepAliveBw.IsBusy)
                keepAliveBw.CancelAsync();
        }

        //
        // New Purple Data Mining Job Done right! ;)
        //

        /*
         * === LauncherAppConfig Info Start ===
         * ProgramID = "1411"
         * 
         * cliGate Info:
         * Host = "cli.g.nc.com"
         * Port = 0x000019C8 (Convert to Int From Hex)
         * 
         * apiGate Info:
         * Host = "api.g.nc.com"
         * Port = 0x000001BB (Convert to Int From Hex)
         * 
         * LoginWebUri = "https://loginapi.plaync.com"
         * playNcAppId = "206A130F-BF96-667A-4E4B-FE1A62583117"
         * 
         * pbnInfo:
         * AppID = "75E8B140-50CD-4A05-BDCE-02248D3088F0"
         * LoginWebUri = "https://loginapi.plaync.com"
         * 
         * GameId = "Purple"
         * Update Server Address = "gs-purple.ncupdate.com"
         * Update Server Company ID = CompanyID.NCS (Formula.Data.Models.CompanyID)
         * AppId = "DE0112AE-63E3-4888-A235-14B01AD1691A"
         * 
         * === LauncherAppConfig Info End ===
         *
         * === GameConfig Info Start ===
         * App Groups: 
         * [NorthAmerica, "bnsncwna"]
         * [Europe, "bnsncweu"]
         * [Purple, "bnsncw"]
         * 
         * AppIds:
         * [NorthAmerica, "95684EE8-121E-0275-3258-4339DAD72236"]
         * [Europe, "FBB46F0C-2E6F-FC3C-31E2-6AA5ACDE5DA0"]
         * GameID = "BnS_UE4"
         * 
         * Regions:
         * [0, "NorthAmerica"]
         * [1, "Europe"]
         * 
         * Update Server Address = "updater.nclauncher.ncsoft.com"
         * Update Server Company ID = CompanyID.NCS (Formula.Data.Models.CompanyID)
         * === GameConfig Info End ===
         *
        */

        private void AvailableRegions(string _region)
        {
            #region Available regions
            string text = ServerValues.GetServerValues(_region == "North America" || _region == "Europe" ? "NA/EU" : _region, "Login")[0];
            string text2 = ServerValues.GetServerValues(_region == "North America" || _region == "Europe" ? "NA/EU" : _region, "Login")[1];
            if (_region == "Russia" || _region == "Garena" || _region == "Chinese")
            {
                Form1.CurrentForm.PlayButton.Enabled = false;
                return;
            }

            Form1.CurrentForm.AddLauncherLog("Adding available servers...");
            try
            {
                int port2 = 27500;
                MemoryStream memoryStream2 = new MemoryStream();
                BinaryWriter binaryWriter2 = new BinaryWriter(memoryStream2);
                binaryWriter2.Write((short)0);
                binaryWriter2.Write((short)8);
                binaryWriter2.Write((byte)10);
                binaryWriter2.Write((byte)text2.Length);
                binaryWriter2.Write(Encoding.ASCII.GetBytes(text2));
                binaryWriter2.BaseStream.Position = 0L;
                binaryWriter2.Write((short)memoryStream2.Length);
                TcpClient tcpClient = new TcpClient(text, port2);
                localIP = ((IPEndPoint)tcpClient.Client.LocalEndPoint).Address.ToString();
                try
                {
                    outsideIP = new WebClient().DownloadString("https://api.ipify.org");
                }
                catch
                {
                    WebClient webClient = new WebClient();
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    webClient.Headers.Add("User-Agent", "BnSBuddy");
                    outsideIP = webClient.DownloadString("https://bnsbuddy.com/count/remote.php");
                }
                NetworkStream stream2 = tcpClient.GetStream();
                stream2.Write(memoryStream2.ToArray(), 0, (int)memoryStream2.Length);
                binaryWriter2.Close();
                memoryStream2.Close();
                memoryStream2 = new MemoryStream();
                BinaryReader binaryReader2 = new BinaryReader(memoryStream2);
                byte[] array2 = new byte[1024];
                int num2 = 0;
                do
                {
                    num2 = stream2.Read(array2, 0, array2.Length);
                    if (num2 > 0)
                        memoryStream2.Write(array2, 0, num2);
                }
                while (num2 == array2.Length);
                memoryStream2.Position = 9L;
                binaryReader2.ReadBytes(binaryReader2.ReadByte() + 1);
                string @string = Encoding.UTF8.GetString(binaryReader2.ReadBytes(binaryReader2.ReadByte() + 128 * (binaryReader2.ReadByte() - 1)));
                stream2.Close();
                binaryReader2.Close();
                memoryStream2.Close();
                // Region Login Configs
                @string = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<Settings>" + @string.Replace("<?xml version=\"1.0\" encoding=\"UTF-8\"?>", "").Replace("  ", "\r\n") + "\r\n</Settings>";
                if ((_region == "North America" || _region == "Europe") && Form1.CurrentForm.materialComboBox4.SelectedItem.ToString() == "Neo")
                    @string = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<Settings>\r\n<game_64bit_execution use=\"on\" path=\"BNSR\\Binaries\\Win64\\BNSR.exe\" />\r\n<check_before_run use=\"on\" />\r\n<cligate_server ip=\"cli.g.nc.com\" port=\"6600\" programid=\"342\" appid=\"32A8F729-55F2-40A8-BE1D-7C907E9F6764\" />\r\n<game_region_argument format=\"-region:%s\">\r\n<region name=\"na\" value=\"0\" appid=\"32A8F729-55F2-40A8-BE1D-7C907E9F6764\" />\r\n<region name=\"eu\" value=\"1\" appid=\"ADA46A8F-65C2-46F2-BF84-2B09428B4538\" />\r\n</game_region_argument>\r\n<login_enable use=\"on\" />\r\n<installer_args>\r\n<![CDATA[/v\"INSTALLDIR=\\\"{{location}}\\\"\" /v\"GAMEID=\\\"{{game_id}}\\\"\" /clone_wait]]>\r\n</installer_args>\r\n<install_file>setup.exe</install_file>\r\n<hotfix version=\"0\" />\r\n<delete>\r\n<file>BNSR\\Binaries\\Win64\\GameGuard\\BnSUS.ini</file>\r\n</delete>\r\n<cleanup_target>\r\n<folders>BNSR/Content/local/NCWest</folders>\r\n</cleanup_target>\r\n</Settings>";
                else if ((_region == "Taiwan") && Form1.CurrentForm.materialComboBox4.SelectedItem.ToString() == "Neo")
                    @string = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<Settings>\r\n<cligate_server ip=\"cli.g.nc.com\" port=\"6600\" programid=\"3353\" appid=\"BC03FF5B-800F-418E-8CA2-5653FD06B2CD\" />\r\n<login_enable use=\"on\" />\r\n</Settings>";
                else if ((_region == "Japanese") && Form1.CurrentForm.materialComboBox4.SelectedItem.ToString() == "Neo")
                    @string = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<Settings>\r\n<cligate_server ip=\"cli.g.nc.com\" port=\"6600\" programid=\"3350\" appid=\"1A080A25-91A9-40F3-83B6-A4156C82A4BB\" />\r\n<login_enable use=\"on\" />\r\n</Settings>";
                else if ((_region == "Korean") && Form1.CurrentForm.materialComboBox4.SelectedItem.ToString() == "Neo")
                    @string = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<Settings>\r\n<cligate_server ip=\"cli.g.nc.com\" port=\"6600\" programid=\"3311\" appid=\"2B447C4E-122B-4B82-943E-3BA9990162B9\" />\r\n<login_enable use=\"on\" />\r\n</Settings>";
                // Continue 
                LoginId = Regex.Match(@string, "id=\"([^\"]*)\"", RegexOptions.IgnoreCase).Groups[1].Value;
                LoginIp = Regex.Match(@string, "ip=\"([^\"]*)\"", RegexOptions.IgnoreCase).Groups[1].Value;
                LoginPort = int.Parse(Regex.Match(@string, "port=\"([^\"]*)\"", RegexOptions.IgnoreCase).Groups[1].Value);
                LoginAppid = Regex.Match(@string, "appid=\"([^\"]*)\"", RegexOptions.IgnoreCase).Groups[1].Value;
                LoginProgramid = "1411";
                // ProgramID Switch for other regions
                if (_region == "Taiwan" || _region == "Japanese" || _region == "Korean")
                    LoginProgramid = Regex.Match(@string, "programid=\"([^\"]*)\"", RegexOptions.IgnoreCase).Groups[1].Value;
                //LoginProgramid = Regex.Match(@string, "programid=\"([^\"]*)\"", RegexOptions.IgnoreCase).Groups[1].Value;
                //if (_region != "North America" && _region != "Europe")
                //LoginAppid = Regex.Match(@string, "appid=\"([^\"]*)\"", RegexOptions.IgnoreCase).Groups[1].Value;
                //else if (LoginProgramid == "1400")
                //    LoginProgramid = "1404";
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(@string);
                if (_region == "North America" || _region == "Europe")
                    foreach (XmlElement item in xmlDocument.SelectNodes("//region"))
                        regions.Add(new region(item.Attributes["name"].Value, item.Attributes["value"].Value, item.Attributes["appid"].Value));
            }
            catch
            {
                Prompt.Popup("There was an error connecting to the Login Server, please make sure there isn't a maintenance and/or service is available.");
                Form1.CurrentForm.PlayButton.Enabled = true;
            }
            #endregion
        }

        public bool CheckMaintenance(string _region)
        {
            #region maintenace check
            string text = ServerValues.GetServerValues(_region == "North America" || _region == "Europe" ? "NA/EU" : _region, "Login")[0];
            string text2 = ServerValues.GetServerValues(_region == "North America" || _region == "Europe" ? "NA/EU" : _region, "Login")[1];
            if (Form1.CurrentForm.materialSwitch48.Checked)
            {
                try
                {
                    int port = 27500;
                    MemoryStream memoryStream = new MemoryStream();
                    BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
                    binaryWriter.Write((short)0);
                    binaryWriter.Write((short)4);
                    binaryWriter.Write((byte)10);
                    binaryWriter.Write((byte)text2.Length);
                    binaryWriter.Write(Encoding.ASCII.GetBytes(text2));
                    binaryWriter.BaseStream.Position = 0L;
                    binaryWriter.Write((short)memoryStream.Length);
                    NetworkStream stream = new TcpClient(text, port).GetStream();
                    stream.Write(memoryStream.ToArray(), 0, (int)memoryStream.Length);
                    binaryWriter.Close();
                    memoryStream.Close();
                    memoryStream = new MemoryStream();
                    BinaryReader binaryReader = new BinaryReader(memoryStream);
                    byte[] array = new byte[1024];
                    int num = 0;
                    do
                    {
                        num = stream.Read(array, 0, array.Length);
                        if (num > 0)
                            memoryStream.Write(array, 0, num);
                    }
                    while (num == array.Length);
                    memoryStream.Position = 9L;
                    // For debugging
                    //var tmmmm = binaryReader.ReadString(); // -> Read Fetched info from
                    binaryReader.ReadBytes(binaryReader.ReadByte() + 1);
                    bool flag = (_region == "Taiwan" || _region == "Japanese") ? !binaryReader.ReadBoolean() : binaryReader.ReadBoolean();
                    stream.Close();
                    binaryReader.Close();
                    memoryStream.Close();
                    if (!flag)
                    {
                        Prompt.Popup("The Game Server is currently in maintenance, please try again later.");
                        Form1.CurrentForm.AddLauncherLog("Game Server Offline!");
                        Form1.CurrentForm.PlayButton.Enabled = true;
                        return true;
                    }
                    Form1.CurrentForm.AddLauncherLog("Game Server Online!");
                    return false;
                }
                catch
                {
                    Prompt.Popup("There was an error connecting to the Login Server, please make sure your ip isn't blocked.");
                    Form1.CurrentForm.PlayButton.Enabled = true;
                    return true;
                }
            }
            else
                Form1.CurrentForm.AddLauncherLog("Maintenance Check Bypassed");
            return false;
            #endregion
        }

        private bool VersionCheck(string _region)
        {
            #region Version Check
            string text = ServerValues.GetServerValues(_region == "North America" || _region == "Europe" ? "NA/EU" : _region, "Version")[0];
            string text2 = ServerValues.GetServerValues(_region == "North America" || _region == "Europe" ? "NA/EU" : _region, "Version")[1];
            if (Form1.CurrentForm.materialSwitch47.Checked)
            {
                int Version = 0;
                try
                {
                    int port = 27500;
                    MemoryStream memoryStream = new MemoryStream();
                    BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
                    binaryWriter.Write((short)0);
                    binaryWriter.Write((short)6);
                    binaryWriter.Write((byte)10);
                    binaryWriter.Write((byte)text2.Length);
                    binaryWriter.Write(Encoding.ASCII.GetBytes(text2));
                    binaryWriter.BaseStream.Position = 0L;
                    binaryWriter.Write((short)memoryStream.Length);
                    NetworkStream stream = new TcpClient(text, port).GetStream();
                    stream.Write(memoryStream.ToArray(), 0, (int)memoryStream.Length);
                    binaryWriter.Close();
                    memoryStream.Close();
                    memoryStream = new MemoryStream();
                    BinaryReader binaryReader = new BinaryReader(memoryStream);
                    byte[] array = new byte[1024];
                    int num = 0;
                    do
                    {
                        num = stream.Read(array, 0, array.Length);
                        if (num > 0)
                        {
                            memoryStream.Write(array, 0, num);
                        }
                    }
                    while (num == array.Length);
                    memoryStream.Position = 9L;
                    binaryReader.ReadBytes(binaryReader.ReadByte() + 5);
                    Version = binaryReader.ReadByte();
                    if (binaryReader.ReadInt16() != 40)
                    {
                        memoryStream.Position -= 2;
                        Version += 128 * (binaryReader.ReadByte() - 1);
                    }
                    stream.Close();
                    binaryReader.Close();
                    memoryStream.Close();
                }
                catch
                {
                    Prompt.Popup("There was an error connecting to the Login Server, please make sure your ip isn't blocked.");
                    Form1.CurrentForm.PlayButton.Enabled = true;
                }
                try
                {
                    string localVersion = "0";
                    _region = RegionConvert.Convert(_region);
                    string text3 = Path.Combine(Form1.CurrentForm.ClientPaths.Installs[_region], "VersionInfo_" + text2 + ".xml");
                    if (File.Exists(text3))
                    {
                        // Read XML
                        XmlDocument xmlVersion = new XmlDocument();
                        xmlVersion.LoadXml(File.ReadAllText(text3));
                        localVersion = xmlVersion.GetElementsByTagName("Version")[0].InnerText;
                    }
                    if (Version != 0)
                    {
                        Form1.CurrentForm.AddLauncherLog("Online Version: " + Version);
                        //OnlineVersionLabel.Text = Version.ToString();
                    }
                    if (localVersion != "0")
                    {
                        Form1.CurrentForm.AddLauncherLog("Local Version: " + localVersion);
                        //LocalVersionLabel.Text = localVersion;
                    }
                    if (Version > int.Parse(localVersion))
                    {
                        if (localVersion != "0")
                        {
                            Prompt.Popup($"Game client update available for {_region} client! \r\nPlease update the game via nclauncher/Purple.");
                            Form1.CurrentForm.PlayButton.Enabled = true;
                            Form1.CurrentForm.AddLauncherLog("Client update available!");
                            return false;
                        }
                        else
                        {
                            Prompt.Popup("Game Client not installed/updated completely.");
                            Form1.CurrentForm.PlayButton.Enabled = true;
                            return false;
                        }
                        //GameUpdaterButton.Enabled = true;
                    }
                    else
                    {
                        Form1.CurrentForm.AddLauncherLog("Client up to date.");
                        Form1.CurrentForm.PlayButton.Enabled = true;
                        //GameUpdaterButton.Enabled = true;
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Prompt.Popup($"Error: Could not compare online version and local one!{Environment.NewLine}Error: {ex.Message}{Environment.NewLine}{ex.InnerException}");
                    Form1.CurrentForm.PlayButton.Enabled = true;
                    return false;
                }
            }
            else
                Form1.CurrentForm.AddLauncherLog("Game Version Check Bypassed");
            return true;
            #endregion
        }

        public void submitCode()
        {
            try
            {
                string text = Builder("Auth", "VerifySecondaryAuth");
                byte[] bytes = Encoding.ASCII.GetBytes(text);
                bytes = xor.Encrypt(bytes, 0, bytes.Length);
                ns.Write(bytes, 0, bytes.Length);
            }
            catch
            {
                Prompt.Popup("The session expired. Please try again.");
            }
        }

        private void receivePackets(object sender, DoWorkEventArgs e)
        {
            while (LoginServer.Connected)
            {
                if (ns.DataAvailable)
                {
                    try
                    {
                        MemoryStream memoryStream = new MemoryStream();
                        string text = "";
                        byte[] array = new byte[1024];
                        int num = 0;
                        do
                        {
                            num = ns.Read(array, 0, array.Length);
                            if (num > 0)
                            {
                                memoryStream.Write(array, 0, num);
                            }
                        }
                        while (num == array.Length);
                        array = memoryStream.ToArray();
                        if (encStart)
                        {
                            array = _recRC4.Decrypt(array);
                            //array = xor.Decrypt(array, 0, array.Length);
                        }
                        string text2 = Encoding.ASCII.GetString(array);
                        memoryStream.Close();
                        int result = 0;
                        int result2 = 0;
                        int.TryParse(Regex.Match(text2, "\r\nl:([0-9]*)\r\n", RegexOptions.IgnoreCase).Groups[1].Value, out result2);
                        int.TryParse(Regex.Match(text2, "\r\ns:([0-9]*)R\r\n", RegexOptions.IgnoreCase).Groups[1].Value, out result);
                        int num2 = Regex.Match(text2, "(\r\n\r\n)", RegexOptions.IgnoreCase).Groups[1].Index + 4;
                        if (num2 + result2 < text2.Length)
                        {
                            List<string> list = new List<string>();
                            while (text2 != "")
                            {
                                int.TryParse(Regex.Match(text2, "\r\nl:([0-9]*)\r\n", RegexOptions.IgnoreCase).Groups[1].Value, out result2);
                                num2 = Regex.Match(text2, "(\r\n\r\n)", RegexOptions.IgnoreCase).Groups[1].Index + 4;
                                string text3 = text2.Substring(0, result2 + num2);
                                list.Add(text3);
                                text2 = ((text2.Length == text3.Length) ? "" : text2.Substring(text3.Length));
                            }
                            foreach (string item in list)
                            {
                                int.TryParse(Regex.Match(item, "\r\ns:([0-9]*)R\r\n", RegexOptions.IgnoreCase).Groups[1].Value, out result);
                                if (responseHandler.ContainsKey(result))
                                {
                                    string[] previous = responseHandler[result];
                                    sendNext(previous, item, "");
                                }
                            }
                        }
                        else
                        {
                            if (num2 + result2 != text2.Length)
                            {
                                memoryStream = new MemoryStream();
                                array = new byte[1024];
                                do
                                {
                                    num = ns.Read(array, 0, array.Length);
                                    if (num > 0)
                                    {
                                        memoryStream.Write(array, 0, num);
                                    }
                                }
                                while (num == array.Length);
                                array = memoryStream.ToArray();
                                if (encStart)
                                {
                                    array = _recRC4.Decrypt(array);
                                    //array = xor.Decrypt(array, 0, array.Length);
                                }
                                text = Encoding.ASCII.GetString(array);
                                memoryStream.Close();
                            }
                            if (responseHandler.ContainsKey(result))
                            {
                                string[] previous2 = responseHandler[result];
                                sendNext(previous2, text2, text);
                            }
                        }
                    }
                    catch { }
                }
                Thread.Sleep(200);
            }
        }

        private byte[][] GenerateKeyClient(Mono.Math.BigInteger exchangeKey)
        {
            byte[] tmp = sha.ComputeHash(Encoding.ASCII.GetBytes(username + ":" + password));
            Mono.Math.BigInteger bi = SHA256Hash2ArrayInverse(GetKeyExchange().getBytes(), exchangeKey.getBytes());
            Mono.Math.BigInteger bigInteger = SHA256Hash2ArrayInverse(session.getBytes(), tmp);
            Mono.Math.BigInteger bi2 = new Mono.Math.BigInteger(exchangeKey.getBytes());
            Mono.Math.BigInteger bi3 = Two.modPow(bigInteger, N);
            bi3 = bi3 * P % N;
            while (bi2 < bi3)
            {
                bi2 += N;
            }
            bi2 -= bi3;
            Mono.Math.BigInteger exp = (bi * bigInteger + privateKey) % N;
            Mono.Math.BigInteger bigInteger2 = bi2.modPow(exp, N);
            key = GenerateEncryptionKeyRoot(bigInteger2.getBytes());
            byte[] array = sha.ComputeHash(CombineBuffers(staticKey, sha.ComputeHash(Encoding.ASCII.GetBytes(username)), session.getBytes(), GetKeyExchange().getBytes(), exchangeKey.getBytes(), key));
            byte[] array2 = sha.ComputeHash(CombineBuffers(GetKeyExchange().getBytes(), array, key));
            key = Generate256BytesKey(key);
            return new byte[2][]
            {
            array,
            array2
            };
        }

        private Mono.Math.BigInteger GetKeyExchange()
        {
            if (exchangeKey == Two)
            {
                exchangeKey = Two.modPow(privateKey, N);
            }
            return exchangeKey;
        }

        private Mono.Math.BigInteger SHA256Hash2ArrayInverse(byte[] tmp1, byte[] tmp2)
        {
            byte[] array = new byte[tmp1.Length + tmp2.Length];
            tmp1.CopyTo(array, 0);
            tmp2.CopyTo(array, tmp1.Length);
            byte[] buf = sha.ComputeHash(array);
            return new Mono.Math.BigInteger(IntegerReverse(buf));
        }

        private unsafe byte[] IntegerReverse(byte[] buf)
        {
            byte[] array = new byte[buf.Length];
            for (int i = 0; i < array.Length / 4; i++)
            {
                fixed (byte* ptr = buf)
                {
                    fixed (byte* ptr3 = array)
                    {
                        int* ptr2 = (int*)ptr;
                        int* ptr4 = (int*)ptr3;
                        ptr4[i] = ptr2[array.Length / 4 - 1 - i];
                    }
                }
            }
            return array;
        }

        private byte[] GenerateEncryptionKeyRoot(byte[] src)
        {
            int num = src.Length;
            int num2 = 0;
            byte[] array = new byte[64];
            if (src.Length > 4)
            {
                while (src[num2] != 0)
                {
                    num--;
                    num2++;
                    if (num <= 4)
                    {
                        break;
                    }
                }
            }
            int num3 = num >> 1;
            byte[] array2 = new byte[num3];
            if (num3 > 0)
            {
                int num4 = num2 + num - 1;
                for (int i = 0; i < num3; i++)
                {
                    array2[i] = src[num4];
                    num4 -= 2;
                }
            }
            byte[] array3 = sha.ComputeHash(array2, 0, num3);
            for (int j = 0; j < 32; j++)
            {
                array[2 * j] = array3[j];
            }
            if (num3 > 0)
            {
                int num5 = num2 + num - 2;
                for (int k = 0; k < num3; k++)
                {
                    array2[k] = src[num5];
                    num5 -= 2;
                }
            }
            array3 = sha.ComputeHash(array2, 0, num3);
            for (int l = 0; l < 32; l++)
            {
                array[2 * l + 1] = array3[l];
            }
            return array;
        }

        private byte[] CombineBuffers(params byte[][] buffers)
        {
            int num = 0;
            foreach (byte[] array in buffers)
            {
                num += array.Length;
            }
            byte[] array2 = new byte[num];
            int num2 = 0;
            foreach (byte[] array3 in buffers)
            {
                array3.CopyTo(array2, num2);
                num2 += array3.Length;
            }
            return array2;
        }

        private byte[] Generate256BytesKey(byte[] src)
        {
            int num = 1;
            byte[] array = new byte[256];
            for (int i = 0; i < 256; i++)
            {
                array[i] = (byte)i;
            }
            int num2 = 0;
            int num3 = 0;
            for (int num4 = 64; num4 > 0; num4--)
            {
                int num5 = (num2 + src[num3] + array[num - 1]) & 0xFF;
                int num6 = array[num - 1];
                array[num - 1] = array[num5];
                int num7 = num3 + 1;
                array[num5] = (byte)num6;
                if (num7 == src.Length)
                {
                    num7 = 0;
                }
                int num8 = num5 + src[num7];
                int num9 = num7 + 1;
                int num10 = num8 + array[num];
                num8 = array[num];
                int num11 = (byte)num10;
                array[num] = array[num11];
                array[num11] = (byte)num8;
                if (num9 == src.Length)
                {
                    num9 = 0;
                }
                int num12 = (num11 + src[num9] + array[num + 1]) & 0xFF;
                int num13 = array[num + 1];
                array[num + 1] = array[num12];
                int num14 = num9 + 1;
                array[num12] = (byte)num13;
                if (num14 == src.Length)
                {
                    num14 = 0;
                }
                int num15 = num12 + src[num14];
                int num16 = array[num + 2];
                num2 = ((num15 + array[num + 2]) & 0xFF);
                num3 = num14 + 1;
                array[num + 2] = array[num2];
                array[num2] = (byte)num16;
                if (num3 == src.Length)
                {
                    num3 = 0;
                }
                num += 4;
            }
            return array;
        }

        private long _incrementalStsTransactionId;

        private long StsTransactionId
        {
            get
            {
                long num = this._incrementalStsTransactionId + 1L;
                this._incrementalStsTransactionId = num;
                return num;
            }
        }

        // Vars for Purple Login
        byte[] _randClientSrp = new byte[32];

        public static byte[] EncryptDataByRSA(byte[] source, byte[] serverPublicKey)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            byte[] obj = (byte[])((global::System.Array)source).Clone();
            global::System.Array.Reverse((global::System.Array)obj);
            uint num = BitConverter.ToUInt32(serverPublicKey, 0);
            byte[] array = new byte[num];
            global::System.Array.Copy((global::System.Array)serverPublicKey, 4, (global::System.Array)array, 0, array.Length);
            global::System.Array.Reverse((global::System.Array)array);
            byte[] array2 = new byte[BitConverter.ToUInt32(serverPublicKey, (int)(4 + num))];
            global::System.Array.Copy((global::System.Array)serverPublicKey, (long)(8 + num), (global::System.Array)array2, 0L, (long)array2.Length);
            global::System.Array.Reverse((global::System.Array)array2);
            BigInteger bigInteger = new BigInteger(obj);
            BigInteger n = new BigInteger(array2);
            BigInteger exp = new BigInteger(array);
            byte[] bytes = bigInteger.modPow(exp, n).getBytes();
            global::System.Array.Reverse((global::System.Array)bytes);
            return bytes;
        }

        private static RijndaelManaged CreateAES256()
        {
            RijndaelManaged rijndaelManaged = new RijndaelManaged();
            rijndaelManaged.KeySize = 256;
            rijndaelManaged.BlockSize = 128;
            rijndaelManaged.Mode = CipherMode.CBC;
            rijndaelManaged.Padding = PaddingMode.PKCS7;
            rijndaelManaged.IV = new byte[16];
            rijndaelManaged.GenerateKey();
            return rijndaelManaged;
        }

        public static string AES256Encrypt(byte[] plainData, byte[] key)
        {
            RijndaelManaged val = new RijndaelManaged
            {
                KeySize = 256,
                BlockSize = 128,
                Mode = (CipherMode)1,
                Padding = (PaddingMode)2,
                Key = key,
                IV = new byte[16]
            };
            try
            {
                ICryptoTransform val2 = ((SymmetricAlgorithm)val).CreateEncryptor(((SymmetricAlgorithm)val).Key, ((SymmetricAlgorithm)val).IV);
                byte[] array = null;
                MemoryStream val3 = new MemoryStream();
                try
                {
                    CryptoStream val4 = new CryptoStream((Stream)(object)val3, val2, (CryptoStreamMode)1);
                    try
                    {
                        ((Stream)val4).Write(plainData, 0, plainData.Length);
                    }
                    finally
                    {
                        ((global::System.IDisposable)val4)?.Dispose();
                    }

                    array = val3.ToArray();
                }
                finally
                {
                    ((global::System.IDisposable)val3)?.Dispose();
                }

                return Convert.ToBase64String(array);
            }
            finally
            {
                ((global::System.IDisposable)val)?.Dispose();
            }
        }

        private string Builder(string nameSpace, string function)
        {
            //Prompt.Popup($"nameSpace: {nameSpace} \n function: {function}");
            if (!(nameSpace == "Sts"))
            {
                if (!(nameSpace == "Auth"))
                {
                    if (!(nameSpace == "Mail"))
                    {
                        if (!(nameSpace == "IpAuthz"))
                        {
                            if (nameSpace == "Gate" && function == "LookupGeolocation")
                            {
                                string text = $"<Request>\n<NetAddress>127.0.0.1</NetAddress>\n</Request>\n";
                                int num = ++counter;
                                responseHandler.Add(num, new string[2]
                                {
                                nameSpace,
                                function
                                });
                                return string.Format("POST /Gate/LookupGeolocation STS/1.0\r\ns:{4}\r\np:*{0} 0 1 0 {1}\r\nl:{2}\r\n\r\n{3}", localIP, epoch, text.Length, text, num);
                            }
                        }
                        else
                        {
                            if (function == "RequestIpToken")
                            {
                                string text2 = "<Request/>\n";
                                int num2 = ++counter;
                                responseHandler.Add(num2, new string[2]
                                {
                                nameSpace,
                                function
                                });
                                return $"POST /IpAuthz/SendIpAuthCode STS/1.0\r\nf:;client={outsideIP}:80;appid={currentAppId}\r\nt:@login:{username}\r\ns:{num2}\r\nl:{text2.Length}\r\n\r\n{text2}";
                            }
                            if (function == "AddIp")
                            {
                                string text3 = $"<Request>\n<NetAddress>{outsideIP}</NetAddress>\n<Description/>\n</Request>\n";
                                return $"POST /IpAuthz/AddIp STS/1.0\r\nt:${currentAppId}\r\nl:{text3.Length}\r\n\r\n{text3}";
                            }
                        }
                    }
                    else if (function == "Mail")
                    {
                        string text4 = $"<Request>\n<Subject>[NCSOFT] Security Notification</Subject>\n<Recipient>{username}</Recipient>\n<Sender>noreply@ncsoft.com</Sender>\n<Contents>{authEmailUsername}|{authEmailIP}|{authEmailCountry}|{authEmailCode}</Contents>\n<TemplateId>1081</TemplateId>\n</Request>\n";
                        int num3 = ++counter;
                        responseHandler.Add(num3, new string[2]
                        {
                        nameSpace,
                        function
                        });
                        return string.Format("POST /Mail/Mail STS/1.0\r\ns:{2}\r\nl:{0}\r\n\r\n{1}", text4.Length, text4, counter);
                    }
                    Prompt.Popup("[Please send screenshot to Endless] Builder Unknown function: " + function);
                }
                else
                {
                    if (function == "LoginTokenStart") // Step 2 [After STS Connect]   (Auth/LoginTokenStart Builder)
                    {
                        _randClientSrp = new byte[32];
                        ((RandomNumberGenerator)new RNGCryptoServiceProvider()).GetBytes(_randClientSrp);
                        string textlol = Convert.ToBase64String(_randClientSrp);
                        string newlol = $"<Request>\n<ClientRand>{textlol}</ClientRand>\n</Request>\n";
                        int num69 = ++counter;
                        responseHandler.Add(num69, new string[2]
                        {
                        nameSpace,
                        function
                        });
                        return string.Format("POST /Auth/LoginTokenStart STS/1.0\r\ns:{4}\r\np:*{0} 0 1 0 {1}\r\nl:{2}\r\n\r\n{3}", localIP, epoch, newlol.Length, newlol, num69);
                    }
                    if (function == "LoginStart")
                    {
                        string text5 = $"<Request>\n<LoginName>{username}</LoginName>\n<NetAddress>127.0.0.1</NetAddress>\n</Request>\n";
                        int num4 = ++counter;
                        responseHandler.Add(num4, new string[2]
                        {
                        nameSpace,
                        function
                        });
                        return string.Format("POST /Auth/LoginStart STS/1.0\r\ns:{4}\r\np:*{0} 0 1 0 {1}\r\nl:{2}\r\n\r\n{3}", localIP, epoch, text5.Length, text5, num4);
                    }
                    if (function == "TokenKeyData") // Step 2
                    {
                        // For Validate
                        //byte[][] array00 = GenerateKeyClient(exchangeKeyServer);
                        //MemoryStream memoryStream00 = new MemoryStream();
                        //BinaryWriter binaryWriter00 = new BinaryWriter(memoryStream00);
                        //binaryWriter00.Write(exchangeKey.getBytes().Length);
                        //binaryWriter00.Write(exchangeKey.getBytes());
                        //binaryWriter00.Write(array00[0].Length);
                        //binaryWriter00.Write(array00[0]);
                        //validate = new Mono.Math.BigInteger(array00[1]);
                        string texttoken;
                        using (RijndaelManaged val = CreateAES256())
                        {
                            string text00 = Convert.ToBase64String(EncryptDataByRSA(val.Key, exchange_public));
                            string text22 = AES256Encrypt(srpPremasterKey, val.Key);
                            string text33 = AES256Encrypt(Encoding.UTF8.GetBytes(WebToken), val.Key);
                            texttoken = $"<Request>\n<EncryptionAlgorithm>AES</EncryptionAlgorithm>\n<EncryptionKey>{text00}</EncryptionKey>\n<PremasterSecret>{text22}</PremasterSecret>\n<AuthnToken>{text33}</AuthnToken>\n<AuthProviderCode>{_3RDPartyProvider.ProviderCode}</AuthProviderCode>\n<AppId>{_loginJson.app_id}</AppId>\n</Request>\n";
                            _3RDPartyProvider.ProviderCode = "np";
                        }
                        int num5 = ++counter;
                        responseHandler.Add(num5, new string[2]
                        {
                        nameSpace,
                        function
                        });
                        return string.Format("POST /Auth/TokenKeyData STS/1.0\r\ns:{4}\r\np:*{0} 0 1 0 {1}\r\nl:{2}\r\n\r\n{3}", localIP, epoch, texttoken.Length, texttoken, num5);
                    }
                    if (function == "KeyData")
                    {
                        byte[][] array = GenerateKeyClient(exchangeKeyServer);
                        MemoryStream memoryStream = new MemoryStream();
                        BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
                        binaryWriter.Write(exchangeKey.getBytes().Length);
                        binaryWriter.Write(exchangeKey.getBytes());
                        binaryWriter.Write(array[0].Length);
                        binaryWriter.Write(array[0]);
                        validate = new Mono.Math.BigInteger(array[1]);
                        string text6 = $"<Request>\n<KeyData>{Convert.ToBase64String(memoryStream.ToArray())}</KeyData>\n</Request>\n";
                        binaryWriter.Close();
                        memoryStream.Close();
                        int num5 = ++counter;
                        responseHandler.Add(num5, new string[2]
                        {
                        nameSpace,
                        function
                        });
                        return string.Format("POST /Auth/KeyData STS/1.0\r\ns:{4}\r\np:*{0} 0 1 0 {1}\r\nl:{2}\r\n\r\n{3}", localIP, epoch, text6.Length, text6, num5);
                    }
                    if (function == "LoginFinish")
                    {
                        string text7 = "<Request></Request>\n"; // string text7 = "<Request>\n<Language>1</Language>\n</Request>\n";
                        int num6 = ++counter;
                        responseHandler.Add(num6, new string[2]
                        {
                        nameSpace,
                        function
                        });
                        return string.Format("POST /Auth/LoginFinish STS/1.0\r\ns:{2}\r\nl:{0}\r\n\r\n{1}", text7.Length, text7, num6);
                    }
                    if (function == "RequestToken")
                    {
                        string text8 = $"<Request>\n<AppId>{currentAppId}</AppId>\n</Request>\n";
                        int num7 = ++counter;
                        responseHandler.Add(num7, new string[2]
                        {
                        nameSpace,
                        function
                        });
                        return string.Format("POST /Auth/RequestToken STS/1.0\r\ns:{2}\r\nl:{0}\r\n\r\n{1}", text8.Length, text8, num7);
                    }
                    if (function == "GetUserInfo")
                    {
                        string text9 = "<Request/>\n";
                        int num8 = ++counter;
                        responseHandler.Add(num8, new string[2]
                        {
                        nameSpace,
                        function
                        });
                        return $"POST /Auth/GetUserInfo STS/1.0\r\nt:@login:{username}\r\ns:{num8}\r\np:*{localIP} 0 1 0 {epoch}\r\nl:{text9.Length}\r\n\r\n{text9}";
                    }
                    if (function == "VerifySecondaryAuth")
                    {
                        rememberip = enterCodePrompt.materialCheckbox1.Checked;
                        string text10 = $"<Request>\n<AuthType>8</AuthType>\n<Value>{enterCodePrompt.materialTextBox1.Text}</Value>\n</Request>\n";
                        int num9 = ++counter;
                        responseHandler.Add(num9, new string[2]
                        {
                        nameSpace,
                        function
                        });
                        return $"POST /Auth/VerifySecondaryAuth STS/1.0\r\ns:{num9}\r\np:*{localIP} 0 1 0 {epoch}\r\nl:{text10.Length}\r\n\r\n{text10}";
                    }
                    Prompt.Popup("[Please send screenshot to Endless] Builder Unknown function: " + function);
                }
            }
            else
            {
                if (function == "Connect") // Step 1 (Sts Connect)
                {
                    // ProductType was 0
                    // Had <Epoch>{epoch}</Epoch>\n
                    // Prompt.Popup(LoginProgramid); 
                    // Process was {pid}
                    string text11 = $"<Connect>\n<ConnType>400</ConnType>\n<Process>28848</Process>\n<ProductType>1000</ProductType>\n<AppIndex>1</AppIndex>\n<Address>{localIP}</Address>\n<Program>{LoginProgramid}</Program>\n<Build>1001</Build>\n</Connect>\n";
                    return $"POST /Sts/Connect STS/1.0\r\nl:{text11.Length}\r\n\r\n{text11}";
                }
                if (function == "Ping")
                {
                    return "POST /Sts/Ping STS/1.0\r\nl:0\r\n\r\n";
                }
                Prompt.Popup("[Please send screenshot to Endless] Builder Unknown function: " + function);
            }
            return null;
        }

        private class loginJson // todo may change for KR (Others will use QR signin)
        {
            public string app_id = "DE0112AE-63E3-4888-A235-14B01AD1691A";
            public string black_box = "blackbox";
            public string credential_result = "token";
            public string game_id = "13";
            public string login_name = @"";
            public string login_type = "np_email";
            public string password = "";
            public string persistent = "false";
            public string return_url = "purple://signin";
            public string target_app_id = "DE0112AE-63E3-4888-A235-14B01AD1691A";
        }

        private loginJson _loginJson = new loginJson();

        private class captcharequired
        {
            public bool captcha_use;
        }

        private class locationCode
        {
            public string location;
        }

        private class _2stepError
        {
            public string code = "";
            public string msg = "";
            public string redirect_url = "";
        }

        private class tokenReceived
        {
            public string authn_token;
        }

        private string WebToken = "";

        private string GDPRInfo = "";

        private string Jcookie = "";

        WebView2 webbrowser;

        Pages.WebLogin webLogin;

        // new weblogin
        public async void StartWebView2()
        {
            // Specify the custom folder containing WebView2Loader.dll
            var envOptions = new CoreWebView2EnvironmentOptions();
            var environment = await CoreWebView2Environment.CreateAsync(
                browserExecutableFolder: null,
                userDataFolder: Form1.CurrentForm.AppPath + "\\WebView2Env", // Optionally specify a user data folder
                options: envOptions
            );
            webbrowser = new WebView2();
            webbrowser.CoreWebView2InitializationCompleted += new EventHandler<CoreWebView2InitializationCompletedEventArgs>(WebView21_CoreWebView2InitializationCompleted);
            await webbrowser.EnsureCoreWebView2Async(environment);
        }

        // old login
        /*
        public async void StartWebView2()
        {
            webbrowser.CoreWebView2InitializationCompleted += new EventHandler<CoreWebView2InitializationCompletedEventArgs>(WebView21_CoreWebView2InitializationCompleted);
            await webbrowser.EnsureCoreWebView2Async();
        }
        */

        private void WebView21_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            // Browser Var
            var browser = sender as WebView2;
            var b_name = browser.Name;
            // Reset 

            if (!e.IsSuccess)
            {
                if (browser.CoreWebView2 == null)
                {
                    // Configure WebView2
                    var envOptions = new CoreWebView2EnvironmentOptions();
                    var environment = CoreWebView2Environment.CreateAsync(
                        browserExecutableFolder: null,
                        userDataFolder: Form1.CurrentForm.AppPath + "\\WebView2Env",
                        options: envOptions
                    );
                    browser.Dispose();
                    browser = new WebView2();
                    browser.CoreWebView2InitializationCompleted += new EventHandler<CoreWebView2InitializationCompletedEventArgs>(WebView21_CoreWebView2InitializationCompleted);
                }
                return;
            }

            webbrowser.CoreWebView2.NavigationCompleted += CoreWebView2_DOMContentLoaded;
            //webbrowser.CoreWebView2.CookieManager.CreateCookie();
            webbrowser.CoreWebView2.Navigate("https://login.plaync.com/nclogin/signin?app_id=DE0112AE-63E3-4888-A235-14B01AD1691A&target_app_id=DE0112AE-63E3-4888-A235-14B01AD1691A&credential_result=token&persistent=false&layout=content&return_url=purple://signin&login_type=np_email&hide_external_login_button=true");
        }

        private async void CoreWebView2_DOMContentLoaded(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            try
            {
                var cookies = await ((CoreWebView2)sender).CookieManager.GetCookiesAsync(null);
                var cookie = cookies.Where(x => x.Name == "geoLocationInfo").FirstOrDefault();
                if (cookie != null)
                    GDPRInfo = cookie.Value;
            }
            catch { }
            if (GDPRInfo != "")
                webbrowser.Dispose();
        }

        // new login
        private bool NewLogin(string login_name = "", string password = "")
        {
            // Create Json Login Object
            _loginJson = new loginJson()
            {
                login_name = login_name,
                password = @password
            };

            string NGPVersion = Form1.CurrentForm.LauncherPaths.PurpleVersionNGP;
            string JsonString = JsonConvert.SerializeObject(_loginJson);
            // Create virtual browser & send Json
            WebClient webClient = new WebClient();
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault;
            // Ini Login Page to save initial cookies
            webClient.Headers[HttpRequestHeader.UserAgent] = $"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.5414.87 Safari/537.36 NGPClient/{NGPVersion} (Windows)";
            webClient.DownloadString("https://login.plaync.com/nclogin/signin");
            // Read cookies to get JSESSIONID
            var vals = webClient.ResponseHeaders.AllKeys;
            string jcookie = "";
            foreach (string val in vals)
            {
                string[] _vals = webClient.ResponseHeaders.GetValues(val);
                foreach (string _val in _vals)
                    if (_val.Contains("JSESSIONID"))
                        jcookie = _val;
            }
            // Location Services for Auth
            webClient.Headers[HttpRequestHeader.UserAgent] = $"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.5414.87 Safari/537.36 NGPClient/{NGPVersion} (Windows)";
            webClient.Headers[HttpRequestHeader.Cookie] = "prelogin_country_code=AA";
            string _webregioncodeJson = webClient.DownloadString("https://www.plaync.com/user/location");
            string _webregioncode = JsonConvert.DeserializeObject<locationCode>(_webregioncodeJson).location;
            // Captcha Bypass
            webClient.Headers[HttpRequestHeader.UserAgent] = $"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.5414.87 Safari/537.36 NGPClient/{NGPVersion} (Windows)";
            string _webCaptchaJson = webClient.DownloadString($"https://loginapi.plaync.com/signin/captcha/use?login_name={login_name.Replace("@", "%40")}");
            bool _webCaptcharesult = JsonConvert.DeserializeObject<captcharequired>(_webCaptchaJson).captcha_use;
            // Continue Login in
            webClient.Headers.Add("Origin", "https://login.plaync.com/");
            webClient.Headers.Add("cookie", $"{jcookie}");
            webClient.Headers.Add("cookie", "prelogin_country_code=ZZ");
            webClient.Headers.Add("cookie", $"country={_webregioncode}");
            webClient.Headers.Add("cookie", "domain=plaync.com");
            webClient.Headers.Add("cookie", "gw_locale=en-US");
            webClient.Headers.Add("cookie", $"geoLocationInfo={GDPRInfo}");
            webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
            webClient.Headers[HttpRequestHeader.UserAgent] = $"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.5414.87 Safari/537.36 NGPClient/{NGPVersion} (Windows)";
            webClient.Headers[HttpRequestHeader.Accept] = "application/json, text/plain, */*";
            webClient.Headers[HttpRequestHeader.Referer] = "https://login.plaync.com/";
            webClient.Headers[HttpRequestHeader.AcceptEncoding] = "gzip, deflate, br, zstd";
            string response = "";
            try
            {
                // login.plaync.com/nclogin/signin
                switch (Form1.CurrentForm.materialComboBox2.SelectedItem.ToString())
                {
                    case "Europe":
                    case "North America":
                    case "Korean":
                        response = webClient.UploadString("https://loginapi.plaync.com/signin/credential", $"{JsonString}");
                        break;
                    case "Taiwan":
                        _loginJson.app_id = "2728FD09-6770-928E-E7AE-A7FE8B4E8ED5";
                        _loginJson.target_app_id = "1462E551-8E41-4DB8-A679-A5AF8A74A3AE";
                        JsonString = JsonConvert.SerializeObject(_loginJson);
                        response = webClient.UploadString("https://tw.ncsoft.com/signin/credential", $"{JsonString}");
                        break;
                    case "Japanese":
                        response = webClient.UploadString("https://login.ncsoft.jp/login/signin", $"{JsonString}");
                        break;
                }
            }
            catch (WebException ex)
            {
                var error_result = ex.Response.GetResponseStream();
                Encoding encode = Encoding.GetEncoding("utf-8");
                StreamReader readStream = new StreamReader(error_result, encode);
                var tmp_error = readStream.ReadToEnd();
                var __obj = JsonConvert.DeserializeObject<_2stepError>(tmp_error);
                if (__obj.msg == "web error - need next step")
                {
                    Process.Start($"{__obj.redirect_url}");
                    Prompt.Popup("[2STEP] Please proceed to the 2 step verification opened on your browser.\r\nOnce completed, press OK to sign in again!");
                    return NewLogin(login_name, password);
                }
                else if (__obj.msg == "web error - bad password or account not found")
                    Prompt.Popup("[CREDENTIALS] Wrong Email/Password");
                else if (__obj.msg == "server error - Captcha Parameter Error")
                    Prompt.Popup("[CAPTCHA] Please login via Purple in settings instead to clear the captcha verification.");
                else
                    Prompt.Popup("[Error] Connection Refused! Screenshot and send this to Endless" + Environment.NewLine + tmp_error);
                return false;
            }

            // Receive and set authntoken for gaming
            try
            {
                token = "";
                token = JsonConvert.DeserializeObject<tokenReceived>(response).authn_token;
            }
            catch
            {
                Prompt.Popup("Login Server returned invalid data, using Purple Login instead.");
                // FailSafe Method
                webLogin = new Pages.WebLogin();
                if (webLogin.ShowDialog() == DialogResult.OK)
                {
                    WebToken = webLogin.token;
                    return true;
                }
                else
                    return false;
            }
            WebToken = token;
            if (token.Length > 1)
                return true;
            return false;
        }

        public static void CreateRC4EncryptKey(byte[] serverRandomKey, byte[] clientRandomKey, byte[] premasterSecret, out byte[] serverEncKeyData, out byte[] clientEncKeyData)
        {
            byte[] array = new byte[48];
            int num = 0;
            int num2 = 0;
            do
            {
                byte[] array2 = new byte[premasterSecret.Length + serverRandomKey.Length + clientRandomKey.Length + num + 1];
                int num3 = num % 26 + 1;
                for (int i = 0; i < num3; i++)
                {
                    array2[i] = (byte)(65 + num3);
                }

                int num4 = 0;
                num4 += num3;
                global::System.Array.Copy((global::System.Array)premasterSecret, 0, (global::System.Array)array2, num4, premasterSecret.Length);
                num4 += premasterSecret.Length;
                global::System.Array.Copy((global::System.Array)clientRandomKey, 0, (global::System.Array)array2, num4, clientRandomKey.Length);
                num4 += clientRandomKey.Length;
                global::System.Array.Copy((global::System.Array)serverRandomKey, 0, (global::System.Array)array2, num4, serverRandomKey.Length);
                byte[] array3 = ((HashAlgorithm)new SHA256Managed()).ComputeHash(array2);
                byte[] array4 = new byte[premasterSecret.Length + array3.Length];
                global::System.Array.Copy((global::System.Array)premasterSecret, (global::System.Array)array4, premasterSecret.Length);
                global::System.Array.Copy((global::System.Array)array3, 0, (global::System.Array)array4, premasterSecret.Length, array3.Length);
                byte[] array5 = ((HashAlgorithm)new MD5CryptoServiceProvider()).ComputeHash(array4);
                int num5 = array5.Length * num;
                int num6 = array5.Length;
                if (array.Length - num2 < num6)
                {
                    num6 = array.Length - num2;
                }

                global::System.Array.Copy((global::System.Array)array5, 0, (global::System.Array)array, num5, num6);
                num++;
                num2 += num6;
            }
            while (num2 != array.Length);
            num = 0;
            num2 = 0;
            byte[] array6 = new byte[288];
            do
            {
                byte[] array7 = new byte[array.Length + clientRandomKey.Length + serverRandomKey.Length + num + 1];
                int num7 = num % 26 + 1;
                for (int j = 0; j < num7; j++)
                {
                    array7[j] = (byte)(65 + num7);
                }

                int num8 = 0;
                num8 += num7;
                global::System.Array.Copy((global::System.Array)array, 0, (global::System.Array)array7, num8, array.Length);
                num8 += array.Length;
                global::System.Array.Copy((global::System.Array)serverRandomKey, 0, (global::System.Array)array7, num8, serverRandomKey.Length);
                num8 += serverRandomKey.Length;
                global::System.Array.Copy((global::System.Array)clientRandomKey, 0, (global::System.Array)array7, num8, clientRandomKey.Length);
                byte[] array8 = ((HashAlgorithm)new SHA256Managed()).ComputeHash(array7);
                byte[] array9 = new byte[array.Length + array8.Length];
                global::System.Array.Copy((global::System.Array)array, (global::System.Array)array9, array.Length);
                global::System.Array.Copy((global::System.Array)array8, 0, (global::System.Array)array9, array.Length, array8.Length);
                byte[] array10 = ((HashAlgorithm)new MD5CryptoServiceProvider()).ComputeHash(array9);
                int num9 = array10.Length * num;
                int num10 = array10.Length;
                if (array6.Length - num2 < num10)
                {
                    num10 = array6.Length - num2;
                }

                global::System.Array.Copy((global::System.Array)array10, 0, (global::System.Array)array6, num9, num10);
                num++;
                num2 += num10;
            }
            while (num2 != array6.Length);
            clientEncKeyData = new byte[128];
            serverEncKeyData = new byte[128];
            int num11 = 0;
            num11 += 16;
            num11 += 16;
            global::System.Array.Copy((global::System.Array)array6, num11, (global::System.Array)clientEncKeyData, 0, clientEncKeyData.Length);
            num11 += clientEncKeyData.Length;
            global::System.Array.Copy((global::System.Array)array6, num11, (global::System.Array)serverEncKeyData, 0, serverEncKeyData.Length);
        }

        // EncryptInitialize(key)

        // new vars for purple token login
        byte[] exchange_rand = null;
        byte[] exchange_public = null;
        byte[] exchange_signature = null;
        byte[] srpPremasterKey = null;

        private RC4 _sendRC4;
        private RC4 _recRC4;

        private void sendNext(string[] previous, string reply, string replyData)
        {
            //Prompt.Popup($"prev 0: {previous[0]} {Environment.NewLine} prev 1: {previous[1]} {Environment.NewLine} replyData: {replyData} {Environment.NewLine} reply: {reply}");
            switch (previous[0])
            {
                case "Auth":
                    switch (previous[1])
                    {
                        case "LoginTokenStart": // Step 3 [Reply from LoginTokenStart]
                            if (replyData == "")
                            {
                                replyData = reply;
                            }
                            reply = reply.Split('\r')[0].Split(' ')[2];
                            if (!(reply == "OK"))
                            {
                                Prompt.Popup($"Unsupported Error: {reply}");
                                Form1.CurrentForm.PlayButton.Enabled = true;
                                termConnection();
                            }
                            else
                            {
                                exchange_signature = Convert.FromBase64String(Regex.Match(replyData, "<ServerSignature>([^<]*)</ServerSignature>", RegexOptions.IgnoreCase).Groups[1].Value);
                                exchange_public = Convert.FromBase64String(Regex.Match(replyData, "<ServerPublicKey>([^<]*)</ServerPublicKey>", RegexOptions.IgnoreCase).Groups[1].Value);
                                exchange_rand = Convert.FromBase64String(Regex.Match(replyData, "<ServerRand>([^<]*)</ServerRand>", RegexOptions.IgnoreCase).Groups[1].Value);
                                byte[] array00 = new byte[48];
                                ((RandomNumberGenerator)new RNGCryptoServiceProvider()).GetBytes(array00);
                                array00[0] = 3;
                                array00[1] = 3;
                                srpPremasterKey = array00;
                                //replyData = Regex.Match(replyData, "<ServerPublicKey>([^<]*)</ServerPublicKey>", RegexOptions.IgnoreCase).Groups[1].Value;
                                //MemoryStream memoryStream2 = new MemoryStream(Convert.FromBase64String(replyData));
                                //BinaryReader binaryReader2 = new BinaryReader(memoryStream2);
                                //session = new Mono.Math.BigInteger(binaryReader2.ReadBytes(binaryReader2.ReadInt32()));
                                //exchangeKeyServer = new Mono.Math.BigInteger(binaryReader2.ReadBytes(binaryReader2.ReadInt32()));
                                //binaryReader2.Close();
                                //memoryStream2.Close();
                                string text4 = Builder("Auth", "TokenKeyData");
                                ns.Write(Encoding.ASCII.GetBytes(text4), 0, text4.Length);
                            }
                            break;
                        case "LoginStart":
                            if (replyData == "")
                            {
                                replyData = reply;
                            }
                            reply = reply.Split('\r')[0].Split(' ')[2];
                            if (!(reply == "OK"))
                            {
                                if (reply == "ErrAccountNotFound")
                                {
                                    Prompt.Popup("The provided email address wasn't found");
                                    Form1.CurrentForm.PlayButton.Enabled = true;
                                    termConnection();
                                }
                                else if (reply == "ErrPermission")
                                {
                                    Prompt.Popup("ErrPermission | STS LoginStart Denied");
                                    Form1.CurrentForm.PlayButton.Enabled = true;
                                    termConnection();
                                }
                                else
                                {
                                    Prompt.Popup("Invalidly formated email");
                                    Form1.CurrentForm.PlayButton.Enabled = true;
                                    termConnection();
                                }
                            }
                            else
                            {
                                replyData = Regex.Match(replyData, "<KeyData>([^<]*)</KeyData>", RegexOptions.IgnoreCase).Groups[1].Value;
                                MemoryStream memoryStream2 = new MemoryStream(Convert.FromBase64String(replyData));
                                BinaryReader binaryReader2 = new BinaryReader(memoryStream2);
                                session = new Mono.Math.BigInteger(binaryReader2.ReadBytes(binaryReader2.ReadInt32()));
                                exchangeKeyServer = new Mono.Math.BigInteger(binaryReader2.ReadBytes(binaryReader2.ReadInt32()));
                                binaryReader2.Close();
                                memoryStream2.Close();
                                string text4 = Builder("Auth", "KeyData");
                                ns.Write(Encoding.ASCII.GetBytes(text4), 0, text4.Length);
                            }
                            break;
                        case "TokenKeyData": // Step 4 [Login Finish here]
                            if (replyData == "")
                            {
                                replyData = reply;
                            }
                            reply = reply.Split('\r')[0].Split(' ')[2];
                            if (!(reply == "OK"))
                            {
                                if (reply == "ErrKeyNotFound")
                                {
                                    Prompt.Popup("Bad AppID for STS Connect");
                                    Form1.CurrentForm.PlayButton.Enabled = true;
                                    termConnection();
                                }
                                else
                                {
                                    Prompt.Popup("[Unknown Error Code]: " + reply);
                                    Form1.CurrentForm.PlayButton.Enabled = true;
                                    termConnection();
                                }
                            }
                            else
                            {
                                encStart = true;
                                byte[] array;
                                byte[] array2;
                                CreateRC4EncryptKey(exchange_rand, _randClientSrp, srpPremasterKey, out array, out array2);
                                this._sendRC4 = new RC4(array2);
                                this._recRC4 = new RC4(array);
                                //xor = new BNSXorEncryption(key);
                                string text3 = Builder("Auth", "LoginFinish");
                                byte[] inData = Encoding.UTF8.GetBytes(text3);
                                inData = _sendRC4.Encrypt(inData);
                                //inData = xor.Encrypt(inData, 0, inData.Length);
                                ns.Write(inData, 0, inData.Length);
                                //string text6 = Builder("Auth", "RequestToken");
                                //byte[] bytes4 = Encoding.ASCII.GetBytes(text6);
                                //bytes4 = xor.Encrypt(bytes4, 0, bytes4.Length);
                                //ns.Write(bytes4, 0, bytes4.Length);
                            }
                            break;
                        case "KeyData":
                            if (replyData == "")
                            {
                                replyData = reply;
                            }
                            reply = reply.Split('\r')[0].Split(' ')[2];
                            if (!(reply == "OK"))
                            {
                                if (reply == "ErrBadPasswd")
                                {
                                    Prompt.Popup("Wrong Password");
                                    Form1.CurrentForm.PlayButton.Enabled = true;
                                    termConnection();
                                }
                                else if (reply == "ErrService")
                                {
                                    Prompt.Popup("NCLauncher login service error, try again.");
                                    Form1.CurrentForm.PlayButton.Enabled = true;
                                    termConnection();
                                }
                                else
                                {
                                    Prompt.Popup("Unknown Error: " + reply);
                                    Form1.CurrentForm.PlayButton.Enabled = true;
                                    termConnection();
                                }
                            }
                            else
                            {
                                replyData = Regex.Match(replyData, "<KeyData>([^<]*)</KeyData>", RegexOptions.IgnoreCase).Groups[1].Value;
                                MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(replyData));
                                BinaryReader binaryReader = new BinaryReader(memoryStream);
                                byte[] inData = binaryReader.ReadBytes(binaryReader.ReadInt32());
                                binaryReader.Close();
                                memoryStream.Close();
                                if (new Mono.Math.BigInteger(inData) == validate)
                                {
                                    xor = new BNSXorEncryption(key);
                                    string text3 = Builder("Auth", "LoginFinish");
                                    inData = Encoding.ASCII.GetBytes(text3);
                                    inData = xor.Encrypt(inData, 0, inData.Length);
                                    ns.Write(inData, 0, inData.Length);
                                    encStart = true;
                                }
                                else
                                {
                                    Prompt.Popup("Negotiation Failed, please try again.");
                                    key = null;
                                    Form1.CurrentForm.PlayButton.Enabled = true;
                                    termConnection();
                                }
                            }
                            break;
                        case "LoginFinish":
                            if ((reply + replyData).Contains("<AuthType>8</AuthType>"))
                            {
                                string text5 = Builder("IpAuthz", "RequestIpToken");
                                byte[] bytes3 = Encoding.UTF8.GetBytes(text5);
                                bytes3 = _sendRC4.Encrypt(bytes3);
                                //bytes3 = xor.Encrypt(bytes3, 0, bytes3.Length);
                                ns.Write(bytes3, 0, bytes3.Length);
                            }
                            else
                            {
                                bool flag = false;
                                foreach (string[] value in responseHandler.Values)
                                {
                                    if (value[0] == "IpAuthz" && value[1] == "RequestIpToken")
                                    {
                                        flag = true;
                                        break;
                                    }
                                }
                                bool @checked = rememberip; // was enterCodePrompt.materialCheckbox1.Checked
                                string text6;
                                byte[] bytes4;
                                if (flag && @checked)
                                {
                                    text6 = Builder("IpAuthz", "AddIp");
                                    bytes4 = Encoding.UTF8.GetBytes(text6);
                                    bytes4 = _sendRC4.Encrypt(bytes4);
                                    //bytes4 = xor.Encrypt(bytes4, 0, bytes4.Length);
                                    ns.Write(bytes4, 0, bytes4.Length);
                                }
                                text6 = Builder("Auth", "RequestToken");
                                bytes4 = Encoding.UTF8.GetBytes(text6);
                                bytes4 = _sendRC4.Encrypt(bytes4);
                                //bytes4 = xor.Encrypt(bytes4, 0, bytes4.Length);
                                ns.Write(bytes4, 0, bytes4.Length);
                            }
                            break;
                        case "RequestToken":
                            {
                                reply += replyData;
                                token = Regex.Match(reply, "<AuthnToken>([^<]*)</AuthnToken>", RegexOptions.IgnoreCase).Groups[1].Value;
                                Task.Delay(0).ContinueWith(delegate { Control.CheckForIllegalCrossThreadCalls = false; login_enable(); });
                                break;
                            }
                        case "GetUserInfo":
                            {
                                reply += replyData;
                                authEmailUsername = Regex.Match(reply, "<UserName>([^<]*)</UserName>", RegexOptions.IgnoreCase).Groups[1].Value;
                                string text7 = Builder("Mail", "Mail");
                                byte[] bytes5 = Encoding.ASCII.GetBytes(text7);
                                bytes5 = xor.Encrypt(bytes5, 0, bytes5.Length);
                                ns.Write(bytes5, 0, bytes5.Length);
                                break;
                            }
                        case "VerifySecondaryAuth":
                            reply = reply.Split('\r')[0].Split(' ')[2];
                            if (!(reply == "OK"))
                            {
                                if (reply == "ErrIpAuthzInvalidIpToken")
                                {
                                    Prompt.Popup("Wrong Code");
                                }
                                else if (reply == "ErrService")
                                {
                                    Prompt.Popup("NCSoft login server error. Please try again.");
                                    Form1.CurrentForm.PlayButton.Enabled = true;
                                    termConnection();
                                }
                                else
                                {
                                    Prompt.Popup("Unknown Error: " + reply);
                                    Form1.CurrentForm.PlayButton.Enabled = true;
                                    termConnection();
                                }
                            }
                            else
                            {
                                enterCodePrompt.called = true;
                                enterCodePrompt.materialTextBox1.Text = "";
                                enterCodePrompt.Invoke(new Action(enterCodePrompt.Close));
                                Form1.CurrentForm.Show();
                                Form1.CurrentForm.PlayButton.Enabled = true;
                                string text2 = Builder("Auth", "LoginFinish");
                                byte[] bytes2 = Encoding.ASCII.GetBytes(text2);
                                bytes2 = xor.Encrypt(bytes2, 0, bytes2.Length);
                                ns.Write(bytes2, 0, bytes2.Length);
                            }
                            break;
                        default:
                            Prompt.Popup("[send next 0] Unknown packet received: " + previous[1]);
                            break;
                    }
                    break;
                case "IpAuthz":
                    {
                        string a = previous[1];
                        if (a == "RequestIpToken")
                        {
                            reply += replyData;
                            authEmailCode = Regex.Match(reply, "<IpToken>([^<]*)</IpToken>", RegexOptions.IgnoreCase).Groups[1].Value;
                            string text = Builder("Gate", "LookupGeolocation");
                            byte[] bytes = Encoding.ASCII.GetBytes(text);
                            bytes = xor.Encrypt(bytes, 0, bytes.Length);
                            ns.Write(bytes, 0, bytes.Length);
                        }
                        else
                        {
                            Prompt.Popup("Unknown packet received");
                        }
                        break;
                    }
                case "Gate":
                    {
                        string a = previous[1];
                        if (a == "LookupGeolocation")
                        {
                            reply += replyData;

                            if (!Regex.Match(reply, "<Geolocation>([^<]*)</Geolocation>", RegexOptions.IgnoreCase).Success || !Regex.Match(reply, "<NetAddress>([^<]*)</NetAddress>", RegexOptions.IgnoreCase).Success || !reply.Contains("200"))
                            {
                                try
                                {
                                    reply = "";
                                    WebClient webClient = new WebClient();
                                    ServicePointManager.Expect100Continue = true;
                                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                                    webClient.Headers.Add("User-Agent", "BnSBuddy");
                                    string rando_ip = webClient.DownloadString("https://bnsbuddy.com/count/remote.php");
                                    reply += "<NetAddress>" + rando_ip + "</NetAddress>";
                                    string rando_cc = (string)JObject.Parse(webClient.DownloadString("http://www.geoplugin.net/json.gp?ip=" + Regex.Match(rando_ip, "<NetAddress>([^<]*)</NetAddress>", RegexOptions.IgnoreCase).Groups[1].Value)).SelectToken("geoplugin_countryCode");
                                    if (rando_cc == null || rando_cc.Length == 0)
                                    {
                                        rando_cc = "Error, not found.";
                                    }
                                    reply += "<Geolocation>" + rando_cc + "</Geolocation>";
                                }
                                catch
                                {
                                    Prompt.Popup("Could not grab required info for auth code.");
                                }
                            }

                            authEmailCountry = Regex.Match(reply, "<Geolocation>([^<]*)</Geolocation>", RegexOptions.IgnoreCase).Groups[1].Value;
                            authEmailIP = Regex.Match(reply, "<NetAddress>([^<]*)</NetAddress>", RegexOptions.IgnoreCase).Groups[1].Value;
                            string text8 = Builder("Auth", "GetUserInfo");
                            byte[] bytes6 = Encoding.ASCII.GetBytes(text8);
                            bytes6 = xor.Encrypt(bytes6, 0, bytes6.Length);
                            ns.Write(bytes6, 0, bytes6.Length);
                        }
                        else
                        {
                            Prompt.Popup("Unknown packet received");
                        }
                        break;
                    }
                case "Mail":
                    {
                        string a = previous[1];
                        if (a == "Mail")
                        {
                            BackgroundWorker backgroundWorker = new BackgroundWorker();
                            backgroundWorker.WorkerSupportsCancellation = true;
                            backgroundWorker.DoWork += delegate
                            {
                                enterCodePrompt = new Pages.enterCode(this);
                                Form1.CurrentForm.Hide();
                                enterCodePrompt.ShowDialog();
                            };
                            backgroundWorker.RunWorkerAsync();
                            /* For right click fix */
                            /*
                             * Using BackgroundWorker Like original code
                             * removed context on textbox to bypass sta threading issue
                             * and added a instant right click paste
                             */
                            //Thread task = new Thread(() => GetEnterCodePrompt());
                            //task.SetApartmentState(ApartmentState.STA);
                            //task.IsBackground = false;
                            //task.Start();
                        }
                        else
                        {
                            Prompt.Popup("Unknown packet received");
                        }
                        break;
                    }
                default:
                    Prompt.Popup("[send next 1] Unknown packet received: " + previous[0]);
                    break;
            }
        }

        private void GetEnterCodePrompt()
        {
            enterCodePrompt = new Pages.enterCode(this);
            Form1.CurrentForm.Hide();
            enterCodePrompt.ShowDialog();
        }

        public void login_enable() //(bool yes)
        {
            string _region = Form1.CurrentForm.materialComboBox2.SelectedItem.ToString();
            Form1.CurrentForm.AddLauncherLog("Login successful!");
            FinalToken = string.Format(args, token);
            Form1.CurrentForm.AddLauncherLog("Starting Client!");
            Process process = new Process();
            string PathToExe = Form1.CurrentForm.ExecuteableFile.ExecPath;
            if (Form1.CurrentForm.materialTextBox5.Text.Length > 0)
                PathToExe = PathToExe.Replace("BNSR.exe", Form1.CurrentForm.materialTextBox5.Text);
            process.StartInfo.FileName = PathToExe;
            process.StartInfo.Arguments = StringBuilder(_region);
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardError = true;
            try
            {
                if (process.Start())
                {
                    process.PriorityBoostEnabled = Form1.CurrentForm.materialSwitch53.Checked;
                    process.PriorityClass = Form1.CurrentForm.Priority;
                    if (Form1.CurrentForm.materialSwitch54.Checked)
                        process.ProcessorAffinity = GetAffinityClass.AffinityClass(Form1.CurrentForm.materialLabel88.Text.ToString().Replace("Affinity: ", ""));
                    if (Form1.CurrentForm.materialLabel33.Text == "Active")
                    {
                        if (!Form1.CurrentForm.ActiveSessions.Add(_region, username, process.Id))
                        {
                            Form1.CurrentForm.AddLauncherLog("Error: You can't play on a account that's already connected!");
                            process.Kill();
                            return;
                        }
                        else
                            Form1.CurrentForm.materialButton2.Enabled = true;
                    }
                    else
                        Form1.CurrentForm.ActiveSessions.Add(_region, username, process.Id);
                    Form1.CurrentForm.AddLauncherLog("Started " + Path.GetFileName(Form1.CurrentForm.ExecuteableFile.ExecPath) + "!");
                    Form1.CurrentForm.Game.gamesessionid = process.Id;
                    Form1.CurrentForm.WindowState = FormWindowState.Minimized;
                    if (Form1.CurrentForm.materialSwitch52.Checked)
                        Form1.CurrentForm.MemoryCleaner.CleanMem();
                    Form1.CurrentForm.PlayButton.Text = "Kill Game";
                    if (Form1.CurrentForm.materialLabel33.Text == "Active")
                        Form1.CurrentForm.materialButton2.Enabled = true;
                }
            }
            catch
            {
                Form1.CurrentForm.Show();
                Form1.CurrentForm.PlayButton.Enabled = true;
                Form1.CurrentForm.AddLauncherLog("Error: Could Not Start " + Path.GetFileName(PathToExe) + "!");
            }
        }

        private void keepAlive(object sender, DoWorkEventArgs e)
        {
            DateTime now = DateTime.Now;
            int num = 30;
            while (LoginServer.Connected)
            {
                if (DateTime.Now >= now.AddSeconds(num))
                {
                    byte[] array = new byte[1024];
                    string text = Builder("Sts", "Ping");
                    array = Encoding.ASCII.GetBytes(text);
                    if (key != null && encStart)
                    {
                        array = xor.Encrypt(array, 0, array.Length);
                    }
                    ns.Write(array, 0, array.Length);
                    now = DateTime.Now;
                }
                Thread.Sleep(200);
            }
        }

        public void GrabToken(string _region)
        {
            #region Grab Token
            bool flag = false;
            if (_region == "North America" || _region == "Europe" || _region == "Purple")
            {
                if (regions.Count > 1)
                {
                    RegionCB = new System.Windows.Forms.ComboBox();
                    if (RegionCB.Items.Count == 0)
                    {
                        foreach (region reg in regions)
                            RegionCB.Items.Add(reg);
                        RegionCB.DataSource = regions;
                    }
                    if (_region == "North America")
                        RegionCB.SelectedIndex = (RegionCB.Items.Contains("North America") ? RegionCB.FindStringExact("North America") : RegionCB.FindStringExact("na"));
                    else if (_region == "North America")
                        RegionCB.SelectedIndex = (RegionCB.Items.Contains("Europe") ? RegionCB.FindStringExact("Europe") : RegionCB.FindStringExact("eu"));
                    else
                        RegionCB.SelectedIndex = (RegionCB.Items.Contains("Europe") ? RegionCB.FindStringExact("Europe") : RegionCB.FindStringExact("eu"));
                }
                else if (regions.Count == 1)
                    RegionCB.SelectedIndex = 0;
                else
                {
                    Prompt.Popup("Error: No available servers were found for NA/EU!");
                    flag = true;
                }
            }
            termConnection();
            if (!flag)
            {
                switch (_region)
                {
                    // NA: 
                    // EU: 
                    case "Purple":
                    case "North America":
                    case "Europe":
                        currentAppId = ((region)RegionCB.SelectedItem).appId; // new appid 95684EE8-121E-0275-3258-4339DAD72236 (game?) OR  DE0112AE-63E3-4888-A235-14B01AD1691A (for login)
                        currentValue = ((region)RegionCB.SelectedItem).value;
                        break;
                    case "Korean":
                        if (Form1.CurrentForm.materialComboBox4.SelectedItem.ToString() == "Live")
                            currentAppId = "B0D42105-0CB6-BC9F-3CB2-BE28A0662340";
                        else if (Form1.CurrentForm.materialComboBox4.SelectedItem.ToString() == "Test")
                            currentAppId = "18A2B067-7A7E-DA99-CDF1-3BBE3BE93F68";
                        else if (Form1.CurrentForm.materialComboBox4.SelectedItem.ToString() == "Neo")
                            currentAppId = "2B447C4E-122B-4B82-943E-3BA9990162B9";
                        break;
                    case "Taiwan":
                        if (Form1.CurrentForm.materialComboBox4.SelectedItem.ToString() == "Live")
                            currentAppId = "33BC338F-2651-8ECD-9E2A-444843707997";
                        else if (Form1.CurrentForm.materialComboBox4.SelectedItem.ToString() == "Neo")
                            currentAppId = "BC03FF5B-800F-418E-8CA2-5653FD06B2CD";
                        break;
                    case "Japanese":
                        if (Form1.CurrentForm.materialComboBox4.SelectedItem.ToString() == "Live")
                            currentAppId = "C2889F54-F81F-90F0-6A51-7A24012DA745";
                        else if (Form1.CurrentForm.materialComboBox4.SelectedItem.ToString() == "Neo")
                            currentAppId = "1A080A25-91A9-40F3-83B6-A4156C82A4BBD";
                        break;
                }

                // New login method
                if (Form1.CurrentForm.materialSwitch62.Checked || NewLogin(username, password))
                {
                    Form1.CurrentForm.AddLauncherLog("Grabbed Web Token!");
                }
                else
                {
                    Form1.CurrentForm.AddLauncherLog("[Error] Failed to sign in!");
                    return;
                }
                // OLD CODE FOR NCLAUNCHER added LoginTokenStart
                try
                {
                    encStart = false;
                    key = null;
                    epoch = ((long)(DateTime.UtcNow - new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds).ToString();
                    pid = Process.GetCurrentProcess().Id.ToString();
                    privateKey = new Mono.Math.BigInteger(sha.ComputeHash(Mono.Math.BigInteger.genRandom(6).getBytes()));
                    exchangeKey = Two;
                    counter = 0;
                    responseHandler = new Dictionary<int, string[]>();
                    LoginServer = new TcpClient(LoginIp, LoginPort);
                    LoginServer.ReceiveBufferSize = 1024;
                    ns = LoginServer.GetStream();
                    ns.ReadTimeout = 60000;
                    keepAliveBw = new BackgroundWorker();
                    keepAliveBw.WorkerSupportsCancellation = true;
                    keepAliveBw.DoWork += keepAlive;
                    keepAliveBw.RunWorkerAsync();
                    worker = new BackgroundWorker();
                    worker.WorkerSupportsCancellation = true;
                    worker.DoWork += receivePackets;
                    worker.RunWorkerAsync();
                    string text = Builder("Sts", "Connect");
                    ns.Write(Encoding.ASCII.GetBytes(text), 0, text.Length);
                    text = Builder("Auth", "LoginTokenStart");
                    //text = Builder("Auth", "LoginStart");
                    ns.Write(Encoding.ASCII.GetBytes(text), 0, text.Length);
                }
                catch
                {
                    Prompt.Popup("Error: Could not create Session Key! Login server may be down.");
                }
            }
            else
            {
                Form1.CurrentForm.AddLauncherLog("Error: Login Failed, servers are not reachable!");
            }
            #endregion
        }
    }
}
#region My flow vs Purple flow
/*
 * === Purple Launcher ===
 * 
 * [Ini | Sts/Connect]
 * <Connect>\n<ConnType>400</ConnType>\n<Process>28848</Process>\n<ProductType>1000</ProductType>\n<AppIndex>1</AppIndex>\n<Address>192.168.50.26</Address>\n<Program>1411</Program>\n<Build>1001</Build>\n</Connect>\r\n
 * 
 * [Reply | Sts/Connect]
 * OK 200 <Reply/>
 * 
 * [Request | Auth/LoginTokenStart]
 * <Request>\n<ClientRand>Vp6tJQNZmzgk2Xh96eohcapd/DokLFg9VCYmmHz0pQg=</ClientRand>\n</Request>\r\n
 * 
 * [Reply | Auth/LoginTokenStart]
 * <Reply>\n<ServerRand>131aZ6asmr4TVg8P9zYY2h4pjcS98EPJOpyP8M/ZedA=</ServerRand>\n<ServerPublicKey>BAAAAAEAAQCAAAAAuUIfPlfUtHdBndhHkq2s1x+WJL8jPovgZo82I9pQrECB3KGXvGRXMFrIceBSidB+KAVBN9WpMtai2tqbrDKfYvfFvQ5RjpBECpPEBwQ6r3g48P67ZT2tJM7ZCqgcQLdUzpbJf8CDen5E2pTMzpMzjjeMmAkWo635sK5vjybfmJw=</ServerPublicKey>\n<ServerSignature>Z6E7Tihh3J1LdseSx4oKtiVPbgICpPxFhJI3rwEauVSST47+WPx2nPvegH1mmZo0eXsjrif2wVnFH5BS2VX5NmSb4Smb2tpGLN7hrE8HaiPXz+mFSGhxPy25ghT3rR6/f4LsoSfhRzwHvmTeEaeAAqpTCWsFDNohfeYMi9K249JFXA0J6FFBPcJdtjAyje6vUqLvbPFXTu6VJeukz5hh/f1UfNuh2FXpvPQaknPSOtHsd8f8moOUi+H7GSk7C427WxtaGmqw6YQihOc6tFJ6u/O5vRCjE2ifYqWHE8Ggkzr7Xl5Cz5FwAWRHVSkOshUPtpKK0vrMGPRX5JMHq+R4xA==</ServerSignature>\n</Reply>\n

 * [Request | Auth/TokenKeyData]
 * <Request>\n<EncryptionAlgorithm>AES</EncryptionAlgorithm>\n<EncryptionKey>45rL+3dFAPMZ9+7n+JT3PKEV3yUhajfhiAWwsudaYGQWGE9hTz4GmsQbggZ65cXncvb0YeAEwj57TBuyoDeRNjsV9wBQIAvzMohOvOGHrUYcT/tDLIsdTPUfXaL5eHv+P9azZLmQpYE4ugzJgCJZCpZwUVqFmdbrg8LhJXoduQU=</EncryptionKey>\n<PremasterSecret>nbwy51qtwBYKWfP0+5wYZ0WPOumeaFt1l5Z2aGW7pLkkmHEnCTFTXTyhHa44a5kf48DikmX7u322RwF7rue8Rw==</PremasterSecret>\n<AuthnToken>ZV08ro/q9ExR5pgB294UPJctiWchvXI7c4RgUMNQ8nnTzSuvx/AlVPWZb4w3QE5CN7Q6NJnG9rhRTJcwBFrgk3ZpnKZo9osRiJ+koSs4TyiPLVD/rK810FZAYn0FZ0nomLAChDMVMe5gWeEtLDRc8g==</AuthnToken>\n<AuthProviderCode>np</AuthProviderCode>\n<AppId>DE0112AE-63E3-4888-A235-14B01AD1691A</AppId>\n</Request>\r\n

 * [Reply | Auth/TokenKeyData]
 * OK 200 <Reply/>
 * 
 * [Request | Auth/LoginFinish]
 * <Request></Request>\r\n
 * 
 * [Reply | Auth/LoginFinish
 * <Reply>\n<UserId>D137E619-CFE3-488D-8266-9A5CB2897114</UserId>\n<UserStatus>1</UserStatus>\n<Created>2016-03-19T00:07:14Z</Created>\n<UserCenter>1</UserCenter>\n<LoginCenter>1</LoginCenter>\n<UserName>Endless</UserName>\n<LoginName>petardftwalavie@gmail.com</LoginName>\n<IsLoginNameValidated>1</IsLoginNameValidated>\n<PasswordUntil/>\n<Roles type=\"array\"/>\n<GameAccountId>EA93288B-61C8-4F17-BCCD-06200121F1A7</GameAccountId>\n<AccessMask>134217727</AccessMask>\n<Session>447C98D5-1357-4297-9983-60C3B9EF724E</Session>\n<LocationId>447C98D5-1357-4297-9983-60C3B9EF724E</LocationId>\n<UserCredentialCode>np_email</UserCredentialCode>\n</Reply>\n
 * [Request | Auth/RequestToken]
 * <Request><AppId>DE0112AE-63E3-4888-A235-14B01AD1691A</AppId></Request>\r\n
 * 
 * [Reply | Auth/RequestToken]
 * <Reply>\n<AuthnToken>RDEzN0U2MTktQ0ZFMy00ODhELTgyNjYtOUE1Q0IyODk3MTE0OkI1OTJDNTEyLTgxRTEtNENFMS05ODZCLTFBNEEwRDgzMTlFNwA=</AuthnToken>\n</Reply>\n
 * 
 * === BnS Buddy ===
 * 
 * [Ini | Sts/Connect]
 * <Connect>\n<ConnType>400</ConnType>\n<Process>28848</Process>\n<ProductType>1000</ProductType>\n<AppIndex>1</AppIndex>\n<Address>192.168.50.26</Address>\n<Program>1411</Program>\n<Build>1001</Build>\n</Connect>\r\n
 * 
 * [Reply | Sts/Connect]
 * OK 200 <Reply/>
 * 
 * [Request | Auth/LoginTokenStart]
 * <Request>\n<ClientRand>GOAV4RnTvNf7yWNZMFfVtmS/XjQr67wFEsih6K1jD2o=</ClientRand>\n</Request>\r\n
 * 
 * [Reply | Auth/LoginTokenStart]
 * <Reply>\n<ServerRand>2phaZ46qZ/bBXNFs9jQcYwaXI6QMOP7CoOJabWkcQz8=</ServerRand>\n<ServerPublicKey>BAAAAAEAAQCAAAAAuUIfPlfUtHdBndhHkq2s1x+WJL8jPovgZo82I9pQrECB3KGXvGRXMFrIceBSidB+KAVBN9WpMtai2tqbrDKfYvfFvQ5RjpBECpPEBwQ6r3g48P67ZT2tJM7ZCqgcQLdUzpbJf8CDen5E2pTMzpMzjjeMmAkWo635sK5vjybfmJw=</ServerPublicKey>\n<ServerSignature>Z6E7Tihh3J1LdseSx4oKtiVPbgICpPxFhJI3rwEauVSST47+WPx2nPvegH1mmZo0eXsjrif2wVnFH5BS2VX5NmSb4Smb2tpGLN7hrE8HaiPXz+mFSGhxPy25ghT3rR6/f4LsoSfhRzwHvmTeEaeAAqpTCWsFDNohfeYMi9K249JFXA0J6FFBPcJdtjAyje6vUqLvbPFXTu6VJeukz5hh/f1UfNuh2FXpvPQaknPSOtHsd8f8moOUi+H7GSk7C427WxtaGmqw6YQihOc6tFJ6u/O5vRCjE2ifYqWHE8Ggkzr7Xl5Cz5FwAWRHVSkOshUPtpKK0vrMGPRX5JMHq+R4xA==</ServerSignature>\n</Reply>\n
 * 
 * [Request | Auth/TokenKeyData]
 * <Request>\n<EncryptionAlgorithm>AES</EncryptionAlgorithm>\n<EncryptionKey>yEyEZ6iMSxO8IvDArkWiWavs3xfRbs9p6AaT0IHYsN5kVveOZqZ8CgA/SvyHH2ZB3t4ykWcJJMGWNwiX4Jg7j84BO2RtLLnvtK0r1gsSYZorQyHPBKcbT/1qQ//K2EAwUxaI4sS12YTbCbI1izRMKisTn1onyQpaqKjiZ9cRfzI=</EncryptionKey>\n<PremasterSecret>qxNvgf/kT97kvxFlzQo8ezEkvnEoCUPNjA77TzQd3JqN/crWgqpHGAu+80b/USxC9ODt5CMEn8GbKRTJFUkbSg==</PremasterSecret>\n<AuthnToken>0drCSTXx2+h4s58YXp79sLW1tqUQczWlHDj6dw5GOOGoVgiSTDbXQJMWi6y7KU46wFqaWr2HbDOvysDehB10LdZPy/O6Lo3YxAEQvji+ahSi02OnXu8y+27T7PAVNYCU/DHsZwuCf+LcAINS5igOLA==</AuthnToken>\n<AuthProviderCode>np</AuthProviderCode>\n<AppId>DE0112AE-63E3-4888-A235-14B01AD1691A</AppId>\n</Request>\r\n
 * 
 * [Reply | Auth/TokenKeyData]
 * OK 200 <Reply/>
 *
 * [Request | Auth/LoginFinish]
 * <Request></Request>\r\n
 *
 * [Reply | Auth/LoginFinish]
 * >>>DISCONNECTS FROM SENDING LoginFinish TO HERE <<<
 * 
 * [Request | Auth/RequestToken]
 *
 *
 * [Reply | Auth/RequestToken]
 * 
 * 
 */
#endregion