using Microsoft.Win32;
using System;
using System.IO;
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
            // .NET Framework requirement
            CheckForNetFramework();
            // Load Assembly via tmp
            EmbeddedAssembly.Load("BnSBuddy2.Resources.DotNetZip.zip", "DotNetZip.dll");
            EmbeddedAssembly.Load("BnSBuddy2.Resources.BigInteger.zip", "BigInteger.dll");
            EmbeddedAssembly.Load("BnSBuddy2.Resources.bnscompression.zip", "bnscompression.dll");
            EmbeddedAssembly.Load("BnSBuddy2.Resources.BouncyCastle.Crypto.zip", "BouncyCastle.Crypto.dll");
            EmbeddedAssembly.Load("BnSBuddy2.Resources.DnsClient.zip", "DnsClient.dll");
            EmbeddedAssembly.Load("BnSBuddy2.Resources.FastColoredTextBox.zip", "FastColoredTextBox.dll");
            EmbeddedAssembly.Load("BnSBuddy2.Resources.MaterialSkin.zip", "MaterialSkin.dll");
            EmbeddedAssembly.Load("BnSBuddy2.Resources.MegaApiClient.zip", "MegaApiClient.dll");
            EmbeddedAssembly.Load("BnSBuddy2.Resources.Microsoft.Win32.Registry.zip", "Microsoft.Win32.Registry.dll");
            EmbeddedAssembly.Load("BnSBuddy2.Resources.Microsoft.Bcl.HashCode.zip", "Microsoft.Bcl.HashCode.dll");
            EmbeddedAssembly.Load("BnSBuddy2.Resources.Newtonsoft.Json.zip", "Newtonsoft.Json.dll");
            EmbeddedAssembly.Load("BnSBuddy2.Resources.System.Buffers.zip", "System.Buffers.dll");
            EmbeddedAssembly.Load("BnSBuddy2.Resources.System.CodeDom.zip", "System.CodeDom.dll");
            EmbeddedAssembly.Load("BnSBuddy2.Resources.System.Memory.zip", "System.Memory.dll");
            EmbeddedAssembly.Load("BnSBuddy2.Resources.System.Numerics.Vectors.zip", "System.Numerics.Vectors.dll");
            EmbeddedAssembly.Load("BnSBuddy2.Resources.System.Security.AccessControl.zip", "System.Security.AccessControl.dll");
            EmbeddedAssembly.Load("BnSBuddy2.Resources.System.Security.Principal.Windows.zip", "System.Security.Principal.Windows.dll");
            EmbeddedAssembly.Load("BnSBuddy2.Resources.System.Formats.Asn1.zip", "System.Formats.Asn1.dll");
            EmbeddedAssembly.Load("BnSBuddy2.Resources.System.Runtime.CompilerServices.Unsafe.zip", "System.Runtime.CompilerServices.Unsafe.dll");
            EmbeddedAssembly.Load("BnSBuddy2.Resources.System.ValueTuple.zip", "System.ValueTuple.dll");
            // Resolve Loaded Assembly
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(Program.CurrentDomain_AssemblyResolve);
            // Continue loading form
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Preload());
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
                        System.Diagnostics.Process.Start(AppPath + "\\Installer.exe");
                        System.Diagnostics.Process.GetCurrentProcess().Kill();
                    }
                }
                else
                {
                    MessageBox.Show("This application requires .NET Framework 4.7.2 to function.");
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
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
