using Ionic.Zip;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace BnSBuddy2.Database
{
    public static class FileManager
    {
        private static SaveFileDialog _saveDialog;
        private static string _type;

        public static void Download(string url, string type)
        {
            if (string.IsNullOrEmpty(url))
            {
                MaterialSkin.Controls.MaterialMessageBox.Show("No download URL.");
                return;
            }

            if (OnlineFetchUI.CurrentForm != null)
            {
                OnlineFetchUI.CurrentForm.downloadButton.Text = "Downloading...";
                OnlineFetchUI.CurrentForm.downloadButton.Enabled = false;
            }

            _type = (type ?? "").ToLowerInvariant();
            _saveDialog = new SaveFileDialog();
            string suggested = url.Contains("/") ? url.Substring(url.LastIndexOf('/') + 1) : "download.bin";
            try { suggested = Uri.UnescapeDataString(suggested); } catch { }

            switch (_type)
            {
                case "mods":
                    // Default save near game mods; zip is also extracted into YourModsPath after download
                    _saveDialog.InitialDirectory = Form1.CurrentForm.PakPaths.ModsPath;
                    _saveDialog.Filter = "Zip packages (*.zip)|*.zip|All files (*.*)|*.*";
                    break;
                case "splashes":
                    _saveDialog.InitialDirectory = Form1.CurrentForm.SplashPaths.ModdedSplashPath;
                    _saveDialog.Filter = "Bitmap (*.bmp;*.BMP)|*.bmp;*.BMP|All files (*.*)|*.*";
                    break;
                case "addons":
                    _saveDialog.InitialDirectory = Form1.CurrentForm.AddonsPaths.AddonsPath;
                    _saveDialog.Filter = "Addon XML (*.xml)|*.xml|All files (*.*)|*.*";
                    break;
                default:
                    _saveDialog.Filter = "All files (*.*)|*.*";
                    break;
            }

            _saveDialog.FileName = suggested;
            if (_saveDialog.ShowDialog() != DialogResult.OK)
            {
                SetDownloadUi("Cancelled!", true);
                return;
            }

            try
            {
                using (var client = new WebClient())
                {
                    client.DownloadFileCompleted += Client_DownloadFileCompleted;
                    client.DownloadFileAsync(new Uri(url), _saveDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                SetDownloadUi("Failed!", true);
                MaterialSkin.Controls.MaterialMessageBox.Show("Download failed: " + ex.Message);
            }
        }

        private static void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                SetDownloadUi("Failed!", true);
                MaterialSkin.Controls.MaterialMessageBox.Show("Download failed: " + e.Error.Message);
                return;
            }
            if (e.Cancelled)
            {
                SetDownloadUi("Cancelled!", true);
                return;
            }

            // Mods: extract zip into the game YourMods folder for the user
            if (_type == "mods" &&
                _saveDialog != null &&
                _saveDialog.FileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    string extractTo = Form1.CurrentForm.PakPaths.YourModsPath;
                    if (!Directory.Exists(extractTo))
                        Directory.CreateDirectory(extractTo);
                    using (ZipFile zipfile = ZipFile.Read(_saveDialog.FileName))
                        zipfile.ExtractAll(extractTo, ExtractExistingFileAction.OverwriteSilently);
                }
                catch (Exception ex)
                {
                    SetDownloadUi("Downloaded!", true);
                    MaterialSkin.Controls.MaterialMessageBox.Show(
                        "Downloaded the zip, but extract into the mods folder failed:" +
                        Environment.NewLine + ex.Message);
                    return;
                }
            }

            SetDownloadUi("Downloaded!", true);
        }

        private static void SetDownloadUi(string text, bool enable)
        {
            if (OnlineFetchUI.CurrentForm == null) return;
            try
            {
                if (OnlineFetchUI.CurrentForm.InvokeRequired)
                {
                    OnlineFetchUI.CurrentForm.BeginInvoke(new Action(() =>
                    {
                        OnlineFetchUI.CurrentForm.downloadButton.Text = text;
                        OnlineFetchUI.CurrentForm.downloadButton.Enabled = enable;
                    }));
                }
                else
                {
                    OnlineFetchUI.CurrentForm.downloadButton.Text = text;
                    OnlineFetchUI.CurrentForm.downloadButton.Enabled = enable;
                }
            }
            catch { }
        }
    }
}
