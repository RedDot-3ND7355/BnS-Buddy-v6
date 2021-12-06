using MaterialSkin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BnSBuddy2.CoreHandlers
{
    public class Splash
    {
        // Install Splash
        public void InstallSplash(string selected)
        {
            if (!File.Exists(Form1.CurrentForm.SplashPaths.BackupSplashPath + "Splash.BMP"))
                File.Copy(Form1.CurrentForm.SplashPaths.SplashPath + "Splash.BMP", Form1.CurrentForm.SplashPaths.BackupSplashPath + "Splash.BMP");
            File.Copy(Form1.CurrentForm.SplashPaths.ModdedSplashPath + selected, Form1.CurrentForm.SplashPaths.SplashPath + "Splash.BMP", true);
            // Set Status
            Form1.CurrentForm.materialLabel44.Text = "Status: Modded";
            Form1.CurrentForm.materialButton33.Enabled = true;
        }

        // Restore Splash
        public void RestoreSplash()
        {
            File.Copy(Form1.CurrentForm.SplashPaths.BackupSplashPath + "Splash.BMP", Form1.CurrentForm.SplashPaths.SplashPath + "Splash.BMP", true);
            File.Delete(Form1.CurrentForm.SplashPaths.BackupSplashPath + "Splash.BMP");
            // Set Status
            Form1.CurrentForm.materialLabel44.Text = "Status: Original";
            Form1.CurrentForm.materialButton33.Enabled = false;
        }

        // Generate Tree of Modded Splash
        public void GenerateTree()
        {
            if (Form1.CurrentForm.materialListBox1.Items.Count > 0)
                Form1.CurrentForm.materialListBox1.Items.Clear();
            FileInfo[] files = new DirectoryInfo(Form1.CurrentForm.SplashPaths.ModdedSplashPath).GetFiles("*.BMP");
            foreach (FileInfo fileInfo in files)
                Form1.CurrentForm.materialListBox1.Items.Add(new MaterialListBoxItem(fileInfo.Name));
            Form1.CurrentForm.materialButton31.Enabled = false;
            Form1.CurrentForm.materialButton36.Enabled = false;
        }

        // Preview Mod
        public void PreviewSelected(string selected)
        {
            if (File.Exists(Form1.CurrentForm.SplashPaths.ModdedSplashPath + selected))
                Process.Start(Form1.CurrentForm.SplashPaths.ModdedSplashPath + selected);
        }

        // Selected Changed
        public void SelectedChanged(string selected)
        {
            if (File.Exists(Form1.CurrentForm.SplashPaths.ModdedSplashPath + selected))
            {
                Form1.CurrentForm.pictureBox3.Image = Image.FromFile(Form1.CurrentForm.SplashPaths.ModdedSplashPath + selected);
                Form1.CurrentForm.materialLabel45.Text = $"Width: {Form1.CurrentForm.pictureBox3.Image.Width} | Height: {Form1.CurrentForm.pictureBox3.Image.Height} ({Form1.CurrentForm.pictureBox3.Image.Width}x{Form1.CurrentForm.pictureBox3.Image.Height})";
                Form1.CurrentForm.materialButton36.Enabled = true;
                Form1.CurrentForm.materialButton31.Enabled = true;
            }
        }

        // Open Splash Folder
        public void OpenSplashFolder()
        {
            Process.Start("explorer.exe", Form1.CurrentForm.SplashPaths.ModdedSplashPath.Remove(Form1.CurrentForm.SplashPaths.ModdedSplashPath.Length - 1));
        }
    }
}
