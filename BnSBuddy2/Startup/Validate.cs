using BnSBuddy2.Functions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Forms;

namespace BnSBuddy2.Startup
{
    public class Validate
    {
        // Globals
        string error = "";
        private static Mutex mutex;
        // End Globals

        // Imports 
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string lclassName, string windowTitle);
        // End Imports

        public void AdminChecker()
        {
            if (Form1.CurrentForm.materialSwitch38.Checked && !IsAdministrator())
            {
                ProcessStartInfo startInfo = new ProcessStartInfo("cmd", "/c timeout 1 && \"" + Application.ExecutablePath + "\"")
                {
                    Verb = "runas",
                    RedirectStandardError = false,
                    RedirectStandardOutput = false,
                    UseShellExecute = true,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };
                using (Process process = new Process())
                {
                    process.StartInfo = startInfo;
                    process.Start();
                }
                Form1.CurrentForm.KillApp();
            }
        }

        // Handling Principal
        public bool IsAdministrator()
        {
            return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        }

        // Find Active window
        private static IntPtr FindWindow()
        {
            IntPtr thisPtr = FindWindow(null, "BnS Buddy");
            IntPtr parentPtr = GetParent(thisPtr);
            thisPtr = FindWindowEx(parentPtr, IntPtr.Zero, null, "BnS Buddy");
            thisPtr = FindWindowEx(parentPtr, thisPtr, null, "BnS Buddy");
            return thisPtr;
        }

        // Check Mutex
        public void CheckMutex()
        {
            mutex = new Mutex(true, "BnSBuddy", out bool createdNew);
            if (!createdNew)
            {
                Process[] pl = Process.GetProcessesByName("BnS Buddy");
                Process p = null;
                int current = Process.GetCurrentProcess().Id;
                foreach (Process ps in pl)
                {
                    if (ps.Id != current)
                        p = ps;
                }
                if (p != null)
                {
                    if (p.MainWindowHandle != null && p.MainWindowHandle != IntPtr.Zero)
                    {
                        var element = AutomationElement.FromHandle(p.MainWindowHandle);
                        if (element != null)
                        {
                            try
                            {
                                var pattern = element.GetCurrentPattern(WindowPattern.Pattern) as WindowPattern;
                                if (pattern != null)
                                    pattern.SetWindowVisualState(WindowVisualState.Normal);
                            }
                            catch { }
                        }
                    }
                    else
                    {
                        IntPtr t = FindWindow();
                        if (t != null && t != IntPtr.Zero)
                        {
                            var element = AutomationElement.FromHandle(t);
                            if (element != null)
                            {
                                try
                                {
                                    var pattern = element.GetCurrentPattern(WindowPattern.Pattern) as WindowPattern;
                                    if (pattern != null)
                                        pattern.SetWindowVisualState(WindowVisualState.Normal);
                                }
                                catch { }
                            }
                        }
                        else
                            Prompt.Popup("BnS Buddy is already running! Closing...");
                    }
                }
                else
                    Prompt.Popup("BnS Buddy is already running! Closing...");
                Form1.CurrentForm.KillApp();
            }
        }

        // Verify Buddy Signature
        public void Verify()
        {
            switch (ValidateBuddy())
            {
                case 0:
                    break;
                case 1:
                    Prompt.Popup("BnS Buddy signature does not match! Please Delete and get a fresh copy.");
                    Form1.CurrentForm.KillApp();
                    break;
                case 2:
                    Prompt.Popup("BnS Buddy has been altered! Please Delete and get a fresh copy");
                    Form1.CurrentForm.KillApp();
                    break;
                case 3:
                    Prompt.Popup("BnS Buddy is not signed! Please Delete and get a fresh copy.");
                    Form1.CurrentForm.KillApp();
                    break;
                case 4:
                    Prompt.Popup("Verification of BnS Buddy has failed. " + error);
                    Form1.CurrentForm.KillApp();
                    break;
            }
        }

        // Verify Function
        private int ValidateBuddy()
        {
            try
            {
                var thecert = X509Certificate2.CreateFromSignedFile(Application.ExecutablePath);
                var theSignedCert = new X509Certificate2(thecert);
                var theCertificateChain = new X509Chain();
                theCertificateChain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                theCertificateChain.ChainPolicy.RevocationMode = X509RevocationMode.Offline;
                theCertificateChain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                theCertificateChain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
                bool chainIsValid = theCertificateChain.Build(theSignedCert);
                if (chainIsValid)
                {
                    if (theSignedCert.Thumbprint.ToLower() != "341c5692cb780377c82154ab75fdd99ef1c4813f")
                        return 1;
                    return 0;
                }
                else
                    return 2;
            }
            catch (CryptographicException)
            {
                return 3;
            }
            catch (Exception ex)
            {
                error = ex.ToString();
                return 4;
            }
        }

        public bool ValidateDomain()
        {
            string text = "";
            bool text2 = false;
            try
            {
                var thecert = X509Certificate2.CreateFromSignedFile(Application.ExecutablePath);
                var x509Certificate = new X509Certificate2(thecert);
                var theCertificateChain = new X509Chain();
                theCertificateChain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                theCertificateChain.ChainPolicy.RevocationMode = X509RevocationMode.Offline;
                theCertificateChain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                theCertificateChain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
                bool chainIsValid = theCertificateChain.Build(x509Certificate);
                if (chainIsValid)
                {
                    text = x509Certificate.Thumbprint.ToLower();
                    if (text != "341c5692cb780377c82154ab75fdd99ef1c4813f")
                    {
                        Prompt.Popup("BnS Buddy signature does not match! Please Delete and get a fresh copy.");
                        Form1.CurrentForm.KillApp();
                    }
                }
                else
                {
                    Prompt.Popup("BnS Buddy has been altered! Please Delete and get a fresh copy.");
                    Form1.CurrentForm.KillApp();
                }
            }
            catch
            {
                Prompt.Popup("BnS Buddy is not signed! Please Delete and get a fresh copy.");
                Form1.CurrentForm.KillApp();
            }
            if (text.Length > 0)
            {
                X509Certificate2 x509Certificate = null;
                try
                {
                    using (SslStream sslStream = new SslStream(new TcpClient("updates.bnsbuddy.com", 443).GetStream(), true, (object snd, X509Certificate certificate, X509Chain chainLocal, SslPolicyErrors sslPolicyErrors) => true))
                    {
                        sslStream.AuthenticateAsClient("updates.bnsbuddy.com", null, SslProtocols.Tls12, checkCertificateRevocation: true);
                        x509Certificate = new X509Certificate2(sslStream.RemoteCertificate);
                    }
                }
                catch
                {
                    Form1.CurrentForm.AddLauncherLog("Could not connect securely to update server");
                    return false;
                }
                if (x509Certificate != null)
                    text2 = x509Certificate.Verify();
                if (text2 && text == "341c5692cb780377c82154ab75fdd99ef1c4813f")
                {
                    Form1.CurrentForm.AddLauncherLog("Domain validated");
                    return true;
                }
            }
            return false;
        }
    }
}
