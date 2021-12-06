using BnSBuddy2.Functions;
using BnSBuddy2.NCServer;
using Mono.Math;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace BnSBuddy2.Packets
{
    public class NCHandler
    {
        // Globals
        ComboBox RegionCB = new ComboBox();
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

        public string StringBuilder(string region)
        {
            if (region == "North America")
                regionID = "0";
            if (region == "Europe")
                regionID = "1";
            if (region == "Korean" && Form1.CurrentForm.materialComboBox4.SelectedItem.ToString() == "Test")
                region = "Korean Test";
            string Args = "";
            switch (region)
            {
                case "Japanese":
                    Args = "/LaunchByLauncher /Sesskey /SessKey:\"\" /CompanyID:\"14\" /ChannelGroupIndex:\"-1\"";
                    break;
                case "Korean":
                    Args = "/LaunchByLauncher /AcctGUID: \"\" /UidHash:\"\" /UserAge:21 /AuthnToken:" + FinalToken + " /SessKey:" + FinalToken + " /ServiceRegion:" + LoginId + " /AuthProviderCode:np /ServiceNetwork:live /NPServerAddr:\"https://api.ncsoft.com:443\" -lite:8 /PresenceId:BNS_LIVE" + (Form1.CurrentForm.materialCheckbox1.Checked ? " -USEALLAVAILABLECORES" : "") + (Form1.CurrentForm.materialCheckbox2.Checked ? " -UNATTENDED" : "") + (Form1.CurrentForm.materialCheckbox3.Checked ? " -NOTEXTURESTREAMING " : "") + Form1.CurrentForm.materialTextBox4.Text;
                    break;
                case "Korean Test":
                    Args = "/LaunchByLauncher /AcctGUID: \"\" /UidHash:\"\" /UserAge:21 /AuthnToken:" + FinalToken + " /SessKey:" + FinalToken + " /ServiceRegion:" + LoginId + " /AuthProviderCode:np /ServiceNetwork:live /NPServerAddr:\"https://api.ncsoft.com:443\" -lite:8 /PresenceId:BNS_TEST" + (Form1.CurrentForm.materialCheckbox1.Checked ? " -USEALLAVAILABLECORES" : "") + (Form1.CurrentForm.materialCheckbox2.Checked ? " -UNATTENDED" : "") + (Form1.CurrentForm.materialCheckbox3.Checked ? " -NOTEXTURESTREAMING " : "") + Form1.CurrentForm.materialTextBox4.Text;
                    break;
                case "North America":
                case "Europe":
                    Args = "-lang:" + Form1.CurrentForm.materialComboBox3.SelectedItem.ToString() + " -lite:2 -region:" + regionID + " /AuthnToken:\"" + FinalToken + "\" /AuthProviderCode:\"np\" /sesskey /launchbylauncher /CompanyID:12 /ChannelGroupIndex:-1" + (Form1.CurrentForm.materialCheckbox1.Checked ? " -USEALLAVAILABLECORES" : "") + (Form1.CurrentForm.materialCheckbox2.Checked ? " -UNATTENDED" : "") + (Form1.CurrentForm.materialCheckbox3.Checked ? " -NOTEXTURESTREAMING " : "") + Form1.CurrentForm.materialTextBox4.Text;
                    break;
                case "Taiwan":
                    Args = "/LaunchByLauncher /SessKey:" + FinalToken + " /ServiceRegion:15 /ChannelGroupIndex:-1 /PresenceId:TWBNSUE4" + (Form1.CurrentForm.materialCheckbox1.Checked ? " -USEALLAVAILABLECORES" : "") + (Form1.CurrentForm.materialCheckbox2.Checked ? " -UNATTENDED" : "") + (Form1.CurrentForm.materialCheckbox3.Checked ? " -NOTEXTURESTREAMING " : "") + Form1.CurrentForm.materialTextBox4.Text;
                    break;
                default:
                    Args = "-lang:" + Form1.CurrentForm.materialComboBox3.SelectedItem.ToString() + " -lite:2 /sesskey /launchbylauncher /CompanyID:12 /ChannelGroupIndex:-1" + (Form1.CurrentForm.materialCheckbox1.Checked ? " -USEALLAVAILABLECORES" : "") + (Form1.CurrentForm.materialCheckbox2.Checked ? " -UNATTENDED" : "") + (Form1.CurrentForm.materialCheckbox3.Checked ? " -NOTEXTURESTREAMING " : "") + Form1.CurrentForm.materialTextBox4.Text;
                    break;
            }
            return Args;
        }

        public string GetLogin(string _username = "", string _password = "", string origreg = "")
        {
            if (!LauncherInfo(origreg, true))
                return "Maintenance";
            Form1.CurrentForm.Hide();
            if (_username.Length == 0 && _password.Length == 0)
            {
                Pages.Login splash = new Pages.Login();
                splash.ShowDialog();
                _username = splash.username.ToString().ToLower();
                _password = splash.password.ToString();
            }
            if (_username.Length > 0 && _password.Length > 0)
            {
                username = _username;
                password = _password;
                Form1.CurrentForm.Show();
                Form1.CurrentForm.WindowState = FormWindowState.Normal;
                GrabToken(origreg);
                return "Logged";
            }
            Form1.CurrentForm.Show();
            Form1.CurrentForm.WindowState = FormWindowState.Normal;
            return "Cancelled";
        }

        public bool LauncherInfo(string _region, bool login = false)
        {
            regions = new List<region>();
            Form1.CurrentForm.PlayButton.Enabled = false;
            if (_region == "North America" || _region == "Europe" || _region == "Taiwan" || _region == "Korean" || _region == "Japanese")
            {
                if (CheckMaintenance(_region))
                    return false;
                if (!VersionCheck(_region))
                    return false;
                if (login)
                    AvailableRegions(_region);
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

        private void AvailableRegions(string _region)
        {
            #region Available regions
            string text = ServerValues.GetServerValues(_region == "North America" || _region == "Europe" ? "NA/EU" : _region, "Login")[0];
            string text2 = ServerValues.GetServerValues(_region == "North America" || _region == "Europe" ? "NA/EU" : _region, "Login")[1];
            if (_region == "Japanese")
            {
                Form1.CurrentForm.PlayButton.Enabled = true;
                return;
            }
            else if (_region == "Russia" || _region == "Garena" || _region == "Chinese")
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
                @string = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<Settings>" + @string.Replace("<?xml version=\"1.0\" encoding=\"UTF-8\"?>", "").Replace("  ", "\r\n") + "\r\n</Settings>";
                LoginId = Regex.Match(@string, "id=\"([^\"]*)\"", RegexOptions.IgnoreCase).Groups[1].Value;
                LoginIp = Regex.Match(@string, "ip=\"([^\"]*)\"", RegexOptions.IgnoreCase).Groups[1].Value;
                LoginPort = int.Parse(Regex.Match(@string, "port=\"([^\"]*)\"", RegexOptions.IgnoreCase).Groups[1].Value);
                LoginProgramid = Regex.Match(@string, "programid=\"([^\"]*)\"", RegexOptions.IgnoreCase).Groups[1].Value;
                if (_region != "North America" && _region != "Europe")
                    LoginAppid = Regex.Match(@string, "appid=\"([^\"]*)\"", RegexOptions.IgnoreCase).Groups[1].Value;
                else if (LoginProgramid == "1400")
                    LoginProgramid = "1404";
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(@string);
                if (_region == "North America" || _region == "Europe")
                    foreach (XmlElement item in xmlDocument.SelectNodes("//region"))
                        regions.Add(new region(item.Attributes["name"].Value, item.Attributes["value"].Value, item.Attributes["appid"].Value));
            }
            catch
            {
                Prompt.Popup("There was an error connecting to the Login Server, please make sure there isn't a maintenance.");
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
                    binaryReader.ReadBytes(binaryReader.ReadByte() + 1);
                    bool flag = binaryReader.ReadBoolean();
                    stream.Close();
                    binaryReader.Close();
                    memoryStream.Close();
                    if (!flag)
                    {
                        Prompt.Popup("The Game Server is currently in maintenance, please try again later.");
                        Form1.CurrentForm.PlayButton.Enabled = true;
                        return true;
                    }
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
                    string text3 = Form1.CurrentForm.ClientPaths.Installs[_region] + "VersionInfo_" + text2 + ".ini";
                    string texttest = Form1.CurrentForm.ClientPaths.Installs[_region] + "VersionInfo_" + text2.Replace("BNS", "BnS") + ".ini";
                    if (File.Exists(text3))
                    {
                        foreach (string s in File.ReadAllLines(text3))
                        {
                            if (s.Contains("="))
                            {
                                string[] vp = s.Split('=');
                                if (vp[0].ToLower().Trim() == "globalversion")
                                    localVersion = vp[1].Trim();
                            }
                        }
                    }
                    else if (File.Exists(texttest))
                    {
                        foreach (string s in File.ReadAllLines(texttest))
                        {
                            if (s.Contains("="))
                            {
                                string[] vp = s.Split('=');
                                if (s.Split('=')[0].ToLower().Trim() == "globalversion")
                                    localVersion = vp[1].Trim();
                            }
                        }
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
                            Prompt.Popup("Game client update available! Please update the game via nclauncher or via in extras.");
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
                catch
                {
                    Prompt.Popup("Error: Could not compare online version and local one!");
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
                        if (key != null && encStart)
                        {
                            array = xor.Decrypt(array, 0, array.Length);
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
                                if (key != null && encStart)
                                {
                                    array = xor.Decrypt(array, 0, array.Length);
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
                    Prompt.Popup("Unknown function: " + function);
                }
                else
                {
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
                        string text7 = "<Request>\n<Language>1</Language>\n</Request>\n";
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
                    Prompt.Popup("Unknown function: " + function);
                }
            }
            else
            {
                if (function == "Connect")
                {
                    // ProductType was 0
                    // Had <Epoch>{epoch}</Epoch>\n
                    // Prompt.Popup(LoginProgramid);
                    string text11 = $"<Connect>\n<ConnType>400</ConnType>\n<Process>{pid}</Process>\n<ProductType>1000</ProductType>\n<AppIndex>1</AppIndex>\n<Address>{localIP}</Address>\n<Program>{LoginProgramid}</Program>\n<Build>1001</Build>\n</Connect>\n";
                    return $"POST /Sts/Connect STS/1.0\r\nl:{text11.Length}\r\n\r\n{text11}";
                }
                if (function == "Ping")
                {
                    return "POST /Sts/Ping STS/1.0\r\nl:0\r\n\r\n";
                }
                Prompt.Popup("Unknown function: " + function);
            }
            return null;
        }

        private void sendNext(string[] previous, string reply, string replyData)
        {
            //Prompt.Popup("prev 0: " + previous[0] + "\n" + "prev 1: " + previous[1] + "\n" + "replyData: " + replyData);
            switch (previous[0])
            {
                case "Auth":
                    switch (previous[1])
                    {
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
                                byte[] bytes3 = Encoding.ASCII.GetBytes(text5);
                                bytes3 = xor.Encrypt(bytes3, 0, bytes3.Length);
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
                                    bytes4 = Encoding.ASCII.GetBytes(text6);
                                    bytes4 = xor.Encrypt(bytes4, 0, bytes4.Length);
                                    ns.Write(bytes4, 0, bytes4.Length);
                                }
                                text6 = Builder("Auth", "RequestToken");
                                bytes4 = Encoding.ASCII.GetBytes(text6);
                                bytes4 = xor.Encrypt(bytes4, 0, bytes4.Length);
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
                            Prompt.Popup("Unknown packet received: " + previous[1]);
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
                    Prompt.Popup("Unknown packet received: " + previous[0]);
                    break;
            }
        }

        private void GetEnterCodePrompt()
        {
            enterCodePrompt = new Pages.enterCode(this);
            Form1.CurrentForm.Hide();
            enterCodePrompt.ShowDialog();
        }

        private void login_enable() //(bool yes)
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
            if (_region == "North America" || _region == "Europe")
            {
                if (regions.Count > 1)
                {
                    RegionCB = new ComboBox();
                    if (RegionCB.Items.Count == 0)
                    {
                        foreach (region reg in regions)
                            RegionCB.Items.Add(reg);
                        RegionCB.DataSource = regions;
                    }
                    if (_region == "North America")
                        RegionCB.SelectedIndex = (RegionCB.Items.Contains("North America") ? RegionCB.FindStringExact("North America") : RegionCB.FindStringExact("na"));
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
                    case "North America":
                    case "Europe":
                        currentAppId = ((region)RegionCB.SelectedItem).appId;
                        currentValue = ((region)RegionCB.SelectedItem).value;
                        break;
                    case "Korean":
                        if (Form1.CurrentForm.materialComboBox4.SelectedItem.ToString() == "Live")
                            currentAppId = "B0D42105-0CB6-BC9F-3CB2-BE28A0662340";
                        else if (Form1.CurrentForm.materialComboBox4.SelectedItem.ToString() == "Test")
                            currentAppId = "18A2B067-7A7E-DA99-CDF1-3BBE3BE93F68";
                        break;
                    case "Taiwan":
                        currentAppId = "33BC338F-2651-8ECD-9E2A-444843707997";
                        break;
                }

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
                    text = Builder("Auth", "LoginStart");
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