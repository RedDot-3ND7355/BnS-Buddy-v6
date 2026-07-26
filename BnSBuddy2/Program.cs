using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Windows.Forms;

namespace BnSBuddy2
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Check for BnS Buddy rename
            CheckProcessName();
            // .NET Framework requirement
            CheckForNetFramework();
            // Check for WebView2 in registry
            CheckForWebView2();
            // Extract WebView2 Requirements
            ExtractWebView2();
            // Load Assembly via tmp
            EmbeddedAssembly.Load("BnSBuddy2.Resources.DotNetZip.zip", "DotNetZip.dll");
            EmbeddedAssembly.Load("BnSBuddy2.Resources.BigInteger.zip", "BigInteger.dll");
            EmbeddedAssembly.Load("BnSBuddy2.Resources.bnscompression.zip", "bnscompression.dll");
            EmbeddedAssembly.Load("BnSBuddy2.Resources.BnS Buddy.XmlSerializers.zip", "BnS Buddy.XmlSerializers.dll");
            EmbeddedAssembly.Load("BnSBuddy2.Resources.BouncyCastle.Crypto.zip", "BouncyCastle.Crypto.dll");
            EmbeddedAssembly.Load("BnSBuddy2.Resources.DnsClient.zip", "DnsClient.dll");
            EmbeddedAssembly.Load("BnSBuddy2.Resources.FastColoredTextBox.zip", "FastColoredTextBox.dll");
            EmbeddedAssembly.Load("BnSBuddy2.Resources.MaterialSkin.zip", "MaterialSkin.dll");
            EmbeddedAssembly.Load("BnSBuddy2.Resources.Microsoft.Bcl.HashCode.zip", "Microsoft.Bcl.HashCode.dll");
            EmbeddedAssembly.Load("BnSBuddy2.Resources.Newtonsoft.Json.zip", "Newtonsoft.Json.dll");
            EmbeddedAssembly.Load("BnSBuddy2.Resources.System.Buffers.zip", "System.Buffers.dll");
            EmbeddedAssembly.Load("BnSBuddy2.Resources.System.Formats.Asn1.zip", "System.Formats.Asn1.dll");
            EmbeddedAssembly.Load("BnSBuddy2.Resources.System.Memory.zip", "System.Memory.dll");
            EmbeddedAssembly.Load("BnSBuddy2.Resources.System.Numerics.Vectors.zip", "System.Numerics.Vectors.dll");
            EmbeddedAssembly.Load("BnSBuddy2.Resources.System.Runtime.CompilerServices.Unsafe.zip", "System.Runtime.CompilerServices.Unsafe.dll");
            EmbeddedAssembly.Load("BnSBuddy2.Resources.System.ValueTuple.zip", "System.ValueTuple.dll");
            EmbeddedAssembly.Load("BnSBuddy2.Resources.Microsoft.Web.WebView2.Core.zip", "Microsoft.Web.WebView2.Core.dll");
            EmbeddedAssembly.Load("BnSBuddy2.Resources.Microsoft.Web.WebView2.WinForms.zip", "Microsoft.Web.WebView2.WinForms.dll");
            // Resolve Loaded Assembly
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(Program.CurrentDomain_AssemblyResolve);
            // Continue loading form
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Preload());
        }

        private static void CheckForWebView2()
        {
            // Check if WebView2 is installed in registry
            try
            {
                RegistryKey Attempt1Loc1 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\EdgeUpdate\Clients\{56EB18F8-B008-4CBD-B6D2-8C97FE7E9062}");
                RegistryKey Attempt2Loc1 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\EdgeUpdate\Clients\{56EB18F8-B008-4CBD-B6D2-8C97FE7E9062}");
                RegistryKey Attempt1Loc2 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}");
                RegistryKey Attempt2Loc2 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}");
                if (Attempt1Loc1 == null && Attempt2Loc1 == null && Attempt1Loc2 == null && Attempt2Loc2 == null)
                {
                    MessageBox.Show("This application requires WebView2 to function.");
                    Process.GetCurrentProcess().Kill();
                }
            }
            catch { }
        }

        private static void ExtractWebView2()
        {
            var AppPath = Path.GetDirectoryName(Application.ExecutablePath);
            // Clean old
            if (Directory.Exists(AppPath + "\\WebView2Env"))
                Directory.Delete(AppPath + "\\WebView2Env", true);
            if (Directory.Exists(AppPath + "\\runtimes"))
                Directory.Delete(AppPath + "\\runtimes", true);
            // Extract new
            using (Stream stream = new MemoryStream(Properties.Resources.WebView2Requirements))
            {
                using (ZipArchive z = new ZipArchive(stream, ZipArchiveMode.Read))
                {
                    foreach (ZipArchiveEntry entry in z.Entries)
                    {
                        // Skip if entry is a directory
                        if (string.IsNullOrEmpty(entry.Name))
                            continue;

                        string output = Path.Combine(AppPath, entry.FullName);
                        string parentDirectory = Path.GetDirectoryName(output);

                        // Create directory if it doesn't exist
                        if (!string.IsNullOrEmpty(parentDirectory) && !Directory.Exists(parentDirectory))
                        {
                            Directory.CreateDirectory(parentDirectory);
                        }

                        // Extract the file
                        using (Stream entryStream = entry.Open())
                        using (FileStream fileStream = File.Create(output))
                        {
                            entryStream.CopyTo(fileStream);
                        }
                    }
                }
            }
        }

        private static void CheckProcessName()
        {
            if (!Application.ExecutablePath.EndsWith("BnS Buddy.exe"))
            {
                MessageBox.Show("Please do not rename the process name or crypto errors will occur! \r\n Rename back to BnS Buddy.exe to use this app!");
                Process.GetCurrentProcess().Kill();
            }
        }

        private static void CheckForNetFramework()
        {
            const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";
            using (var ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey))
            {
                if (ndpKey != null && ndpKey.GetValue("Release") != null)
                {
                    int value = (int)ndpKey.GetValue("Release");
                    if (value < 461808)
                    {
                        var AppPath = Path.GetDirectoryName(Application.ExecutablePath);
                        MessageBox.Show("The required .NET Framework is not installed." + Environment.NewLine + $"You have {CheckFor45PlusVersion(value)}" + Environment.NewLine + "The installer for 4.7.2 will start after pressing OK");
                        File.WriteAllBytes(AppPath + "\\Installer.exe", Properties.Resources.NDP472_KB4054531_Web);
                        Process.Start(AppPath + "\\Installer.exe");
                        Process.GetCurrentProcess().Kill();
                    }
                }
                else
                {
                    MessageBox.Show("This application requires .NET Framework 4.7.2 to function.");
                    Process.GetCurrentProcess().Kill();
                }
            }
        }

        static string CheckFor45PlusVersion(int releaseKey)
        {
            if (releaseKey >= 528040)
                return "4.8+";
            if (releaseKey >= 461808)
                return "4.7.2";
            if (releaseKey >= 461308)
                return "4.7.1";
            if (releaseKey >= 460798)
                return "4.7";
            if (releaseKey >= 394802)
                return "4.6.2";
            if (releaseKey >= 394254)
                return "4.6.1";
            if (releaseKey >= 393295)
                return "4.6";
            if (releaseKey >= 379893)
                return "4.5.2";
            if (releaseKey >= 378675)
                return "4.5.1";
            if (releaseKey >= 378389)
                return "4.5";
            // This code should never execute. A non-null release key should mean
            // that 4.5 or later is installed.
            return "no 4.5 or later version detected";
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return EmbeddedAssembly.Get(args.Name);
        }
    }
}
