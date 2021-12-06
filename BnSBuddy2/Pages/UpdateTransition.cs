using BnSBuddy2.Functions;
using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;

namespace BnSBuddy2.Pages
{
    public partial class UpdateTransition : MaterialForm
    {
        private readonly MaterialSkinManager materialSkinManager;
        public UpdateTransition()
        {
            InitializeComponent();
            // Initialize MaterialSkinManager
            materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.EnforceBackcolorOnAllComponents = true;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = Form1.CurrentForm.materialSkinManager.Theme;
            materialSkinManager.ColorScheme = Form1.CurrentForm.materialSkinManager.ColorScheme;
            // Validate Buddy
            Form1.CurrentForm._Validate.Verify();
            // Grab Fetched Versions from Form1
            materialLabel1.Text += Form1.CurrentForm.materialLabel5.Text; // Local
            materialLabel2.Text += Form1.CurrentForm.materialLabel6.Text; // Online
            // Grab Changelog
            RunDelay.Method(GrabChangelog);
        }

        // Binary Matching function
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

        // Read Changelog
        private void GrabChangelog()
        {
            if (Form1.CurrentForm._Validate.ValidateDomain())
            {
                try
                {
                    string text = "updates.bnsbuddy.com";
                    TcpClient tcpClient = new TcpClient(text, 443);
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    RemoteCertificateValidationCallback userCertificateValidationCallback = (object snd, X509Certificate certificate, X509Chain chainLocal, SslPolicyErrors sslPolicyErrors) => true;
                    using (SslStream sslStream = new SslStream(tcpClient.GetStream(), false, userCertificateValidationCallback))
                    {
                        string text2 = "";
                        sslStream.AuthenticateAsClient(text, null, SslProtocols.Tls12, true);
                        tcpClient.SendTimeout = 500;
                        tcpClient.ReceiveTimeout = 1000;
                        StringBuilder stringBuilder = new StringBuilder();
                        stringBuilder.AppendLine("GET /Changelog.txt HTTP/1.1");
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
                                        text2 = Encoding.UTF8.GetString(memoryStream2.ToArray()).Replace("\r\n", Environment.NewLine).Replace("\n", Environment.NewLine);
                                    }
                                }
                            }
                            else
                            {
                                text2 = Encoding.UTF8.GetString(array, num, array.Length - num).Replace("\r\n", Environment.NewLine).Replace("\n", Environment.NewLine);
                            }
                        }
                        materialMultiLineTextBox1.Text = text2;
                    }
                    tcpClient.Close();
                }
                catch
                {
                    materialLabel2.Text = "Online Version: Offline";
                }
            }
        }
    }
}
