using BnSBuddy2.Properties;
using BnSBuddy2.Functions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BnSBuddy2.Pathing
{
    public class SplashPaths
    {
        // Globals
        public string BackupSplashPath = "";
        public string SplashPath = "";
        public string ModdedSplashPath = "";
        // End Globals

        // Return Splash path based on language and region & set globals
        public void ReturnSplashPath(string region, string lang)
        {
            region = RegionConvert.Convert(region);
            RecursiveFind(Form1.CurrentForm.ClientPaths.Installs[region], lang);
            VerifyIntegrety();
            ExtractBnSBuddySplashes();
            Form1.CurrentForm.Splash.GenerateTree();
        }

        // Add BnSBuddy Splashes to Modded Splashes
        private void ExtractBnSBuddySplashes()
        {
            if (!File.Exists(ModdedSplashPath + "BuddySplash-Small.bmp") && !File.Exists(ModdedSplashPath + "BuddySplash-Big.bmp"))
            {
                File.WriteAllBytes(ModdedSplashPath + "BuddySplash-Small.bmp", ExtractRessource(Resources.BuddySplash_Small));
                File.WriteAllBytes(ModdedSplashPath + "BuddySplash-Big.bmp", ExtractRessource(Resources.BuddySplash_Big));
            }
        }

        // Extract Resource
        private byte[] ExtractRessource(byte[] zippedBuffer)
        {
            MemoryStream stream = new MemoryStream(zippedBuffer);
            using (Ionic.Zip.ZipFile z = Ionic.Zip.ZipFile.Read(stream))
            {
                MemoryStream TempArrray = new MemoryStream();
                z[0].Extract(TempArrray);
                return TempArrray.ToArray();
            }
        }

        // Verify if Original or Modded
        private void VerifyIntegrety()
        {
            if (File.Exists(BackupSplashPath + "\\Splash.BMP"))
                if (Form1.CurrentForm.CheckMD5.ReturnMD5(BackupSplashPath + "\\Splash.BMP") != Form1.CurrentForm.CheckMD5.ReturnMD5(SplashPath + "\\Splash.BMP"))
                {
                    Form1.CurrentForm.materialLabel44.Text = "Status: Modded";
                    Form1.CurrentForm.materialButton33.Enabled = true;
                }
                else
                {
                    Form1.CurrentForm.materialLabel44.Text = "Status: Original";
                    Form1.CurrentForm.materialButton33.Enabled = false;
                }
            else
            {
                Form1.CurrentForm.materialLabel44.Text = "Status: Original";
                Form1.CurrentForm.materialButton33.Enabled = false;
            }
        }

        // Find recursively for path
        private void RecursiveFind(string ClientPath, string Language)
        {
            if (Directory.Exists(ClientPath + "BNSR\\Content\\Splash")) // KR support
            {
                BackupSplashPath = ClientPath + "BNSR\\Content\\Splash\\Backup\\";
                if (!Directory.Exists(BackupSplashPath))
                    Directory.CreateDirectory(BackupSplashPath);
                ModdedSplashPath = Form1.CurrentForm.AppPath + "\\Splashes\\Mods\\";
                if (!Directory.Exists(ModdedSplashPath))
                    Directory.CreateDirectory(ModdedSplashPath);
                SplashPath = ClientPath + "BNSR\\Content\\Splash\\";
            }
            else if (Directory.Exists(ClientPath + "BNSR\\Content\\local")) // Universal
                foreach (DirectoryInfo dir in new DirectoryInfo(ClientPath + "BNSR\\Content\\local").GetDirectories())
                    if (Directory.Exists(ClientPath + "BNSR\\Content\\local\\" + Path.GetFileName(dir.ToString()) + "\\data"))
                        if (Directory.Exists(ClientPath + "BNSR\\Content\\local\\" + Path.GetFileName(dir.ToString()) + "\\" + Language))
                        {
                            BackupSplashPath = ClientPath + "BNSR\\Content\\local\\" + Path.GetFileName(dir.ToString()) + "\\" + Language + "\\Splash\\Backup\\";
                            if (!Directory.Exists(BackupSplashPath))
                                Directory.CreateDirectory(BackupSplashPath);
                            ModdedSplashPath = Form1.CurrentForm.AppPath + "\\Splashes\\Mods\\";
                            if (!Directory.Exists(ModdedSplashPath))
                                Directory.CreateDirectory(ModdedSplashPath);
                            SplashPath = ClientPath + "BNSR\\Content\\local\\" + Path.GetFileName(dir.ToString()) + "\\" + Language + "\\Splash\\";
                        }
        }
    }
}
