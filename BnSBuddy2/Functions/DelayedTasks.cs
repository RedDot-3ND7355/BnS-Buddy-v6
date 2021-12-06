using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMI_ProcessorInformation;
using BnSBuddy2.Functions;

namespace BnSBuddy2.Functions
{
    public static class DelayedTasks
    {
        // Routine
        public static void Routine()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            RunDelay.Method(UpdateChecker);
            LoaderChecker();
            FasterToggle.CheckState(Form1.CurrentForm.materialComboBox2.SelectedItem.ToString(), Form1.CurrentForm.materialComboBox3.SelectedItem.ToString());
            RunDelay.Method(CPUCount);
            ApplyActivatedLicense();
            FixSizes();
        }

        // Fix size for 2 controls
        private static void FixSizes()
        {
            Form1.CurrentForm.treeView1.Size = new System.Drawing.Size(234, 486);
            Form1.CurrentForm.fastColoredTextBox1.Size = new System.Drawing.Size(710, 556);
        }

        // Wait for License
        private static void ApplyActivatedLicense()
        {
            Form1.CurrentForm.waitLicense.WaitOne();
            if (!Form1.CurrentForm.IniLicense.Validated)
                Form1.CurrentForm.materialTabControl1.SelectTab("Ads");
        }

        // Get CPU Core Count
        private static void CPUCount()
        {
            if (Form1.CurrentForm.cpuCount == 0)
                Form1.CurrentForm.cpuCount = WMI_Processor_Information.GetCpuNumberOfLogicalProcessors();
        }

        // Verify Binary
        private static int BinaryMatch(byte[] input, byte[] pattern)
        {
            int num = input.Length - pattern.Length + 1;
            for (int i = 0; i < num; i++)
            {
                bool flag = true;
                for (int j = 0; j < pattern.Length; j++)
                {
                    if (input[i + j] != pattern[j])
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    return i;
                }
            }
            return -1;
        }

        // Check for BnSBuddy Updates
        private static void UpdateChecker()
        {
            if (Form1.CurrentForm.materialSwitch57.Checked)
            {
                string s = "";
                string online = "";
                string offline = "";
                try
                {
                    if (Form1.CurrentForm._Validate.ValidateDomain())
                    {
                        try
                        {
                            try
                            {
                                X509Certificate.CreateFromSignedFile(Application.ExecutablePath).GetCertHashString();
                            }
                            catch
                            {
                                Prompt.Popup("BnS Buddy is not signed! Please Delete and get a fresh copy.");
                                Form1.CurrentForm.KillApp();
                            }
                            string text = "updates.bnsbuddy.com";
                            TcpClient tcpClient = new TcpClient(text, 443);
                            RemoteCertificateValidationCallback userCertificateValidationCallback = (object snd, X509Certificate certificate, X509Chain chainLocal, SslPolicyErrors sslPolicyErrors) => true;
                            using (SslStream sslStream = new SslStream(tcpClient.GetStream(), false, userCertificateValidationCallback))
                            {
                                string text2 = "";
                                sslStream.AuthenticateAsClient(text, null, SslProtocols.Tls12, checkCertificateRevocation: true);
                                tcpClient.SendTimeout = 500;
                                tcpClient.ReceiveTimeout = 1000;
                                StringBuilder stringBuilder = new StringBuilder();
                                stringBuilder.AppendLine("GET /BuddyVersion.txt HTTP/1.1");
                                stringBuilder.AppendLine("Host: updates.bnsbuddy.com");
                                stringBuilder.AppendLine("User-Agent: BnSBuddy/" + Application.ProductVersion + " (compatible;)");
                                stringBuilder.AppendLine("Connection: close");
                                stringBuilder.AppendLine();
                                byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
                                sslStream.WriteAsync(bytes, 0, bytes.Length);
                                using (MemoryStream memoryStream = new MemoryStream())
                                {
                                    sslStream.CopyTo(memoryStream);
                                    memoryStream.Position = 0L;
                                    byte[] array = memoryStream.ToArray();
                                    int num = BinaryMatch(array, Encoding.ASCII.GetBytes("\r\n\r\n")) + 4;
                                    string @string = Encoding.ASCII.GetString(array, 0, num);
                                    memoryStream.Position = num;
                                    if (@string.IndexOf("Content-Encoding: gzip") > 0)
                                    {
                                        using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                                        {
                                            using (MemoryStream memoryStream2 = new MemoryStream())
                                            {
                                                gZipStream.CopyTo(memoryStream2);
                                                memoryStream2.Position = 0L;
                                                text2 = Encoding.UTF8.GetString(memoryStream2.ToArray());
                                            }
                                        }
                                    }
                                    else
                                    {
                                        text2 = Encoding.UTF8.GetString(array, num, array.Length - num);
                                    }
                                }
                                s = text2;
                            }
                            tcpClient.Close();
                        }
                        catch (Exception)
                        {
                            Form1.CurrentForm.AddLauncherLog("Could not connect");
                            s = "Error";
                        }
                        Form1.CurrentForm.AddLauncherLog("Grabbed: " + s);
                    }
                    else
                    {
                        Form1.CurrentForm.AddLauncherLog("Domain Invalid");
                        s = "Error";
                    }
                }
                catch (Exception)
                {
                    Form1.CurrentForm.AddLauncherLog("Could not connect.");
                    s = "Error";
                }
                Form1.CurrentForm.materialLabel6.Text = s;
                Form1.CurrentForm.materialLabel5.Text = Application.ProductVersion;
                online = s.Replace(".", "");
                offline = Application.ProductVersion.ToString().Replace(".", "");
                if (online != "Error")
                {
                    if (Convert.ToInt32(online) > Convert.ToInt32(offline))
                    {
                        if (s != "Error")
                        {
                            Form1.CurrentForm.AddLauncherLog("Update available.");
                            if (Form1.CurrentForm.materialSwitch42.Checked)
                                Form1.CurrentForm.UpdateHandler.StartUpdate();
                            Form1.CurrentForm.materialButton5.Enabled = true;
                        }
                        else if (online == "Error")
                        {
                            Form1.CurrentForm.AddLauncherLog("Error checking update.");
                        }
                    }
                    else
                    {
                        Form1.CurrentForm.AddLauncherLog("Up to date.");
                    }
                }
                else
                {
                    Form1.CurrentForm.AddLauncherLog("Error checking update.");
                }
            }
            else
            {
                Form1.CurrentForm.materialLabel6.Text = "Offline";
                Form1.CurrentForm.materialLabel5.Text = Application.ProductVersion;
            }
        }

        // Check for WINMM.DLL (loader3)
        private static void LoaderChecker()
        {
            if (File.Exists(Form1.CurrentForm.ExecuteableFile.ExecFolder + "winmm.dll"))
                Form1.CurrentForm.materialLabel33.Text = "Active";
        }
    }
}
