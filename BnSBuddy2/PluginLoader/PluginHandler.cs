using BnSBuddy2.Functions;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace BnSBuddy2.PluginLoader
{
    class PluginHandler
    {
        private string UserProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Documents\\BnS\\patches\\";
        private string AppPath = Form1.CurrentForm.AppPath;
        private string RegPath = Form1.CurrentForm.ClientPaths.Installs[RegionConvert.Convert(Form1.CurrentForm.materialComboBox2.SelectedItem.ToString())];

        public void Uninstall(string PluginPath)
        {
            File.Delete(PluginPath);
            File.Delete(PluginPath.Replace("bin64", "bin"));
            if (PluginPath.Contains("bnspatch") && File.Exists(UserProfile.Replace("patches", "") + "patches.xml"))
            {
                File.Delete(UserProfile.Replace("patches", "") + "patches.xml");
            }
        }

        public void Install(string PluginName)
        {
            var table = PLGui.CurrentForm.OnlineFile;
            if (!table.TryGetUrl(PluginName, out string downloadUrl))
            {
                Prompt.Popup("Plugin not found in online catalog: " + PluginName);
                return;
            }

            string downloadsDir = AppPath + "\\Plugins Downloads";
            if (!Directory.Exists(downloadsDir))
                Directory.CreateDirectory(downloadsDir);

            string localZip = Path.Combine(downloadsDir, Path.GetFileName(PluginName));
            if (File.Exists(localZip))
                File.Delete(localZip);

            try
            {
                DownloadFile(downloadUrl, localZip);
            }
            catch (Exception ex)
            {
                Prompt.Popup("Download failed: " + PluginName + Environment.NewLine + ex.Message);
                return;
            }

            // patches.xml for bnspatch
            try
            {
                if (!Directory.Exists(UserProfile))
                    Directory.CreateDirectory(UserProfile);
                string patchesDest = UserProfile.Replace("patches\\", "") + "patches.xml";
                if (!File.Exists(patchesDest) && PluginName.IndexOf("bnspatch", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    if (table.TryGetUrl("patches.xml", out string patchesUrl))
                        DownloadFile(patchesUrl, patchesDest);
                }
            }
            catch (Exception ex)
            {
                Prompt.Popup("Failed to download patches.xml to user profile documents." + Environment.NewLine +
                             "SavePath: " + UserProfile + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
            }

            List<string> FileInfos = new List<string>();
            bool error = false;
            if (localZip.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    using (ZipFile zip = ZipFile.Read(localZip))
                    {
                        foreach (ZipEntry e in zip.Where(x => x.FileName.StartsWith("BNSR")))
                        {
                            e.Extract(RegPath, ExtractExistingFileAction.OverwriteSilently);
                            FileInfos.Add(e.FileName);
                        }
                    }
                }
                catch (IOException)
                {
                    Prompt.Popup("Error: File not found. Probably anti-virus.");
                    error = true;
                }
                catch (AccessViolationException)
                {
                    Prompt.Popup("Error: File is being blocked. Probably anti-virus.");
                    error = true;
                }
                if (error)
                {
                    foreach (string FilePath in FileInfos)
                    {
                        if (File.Exists(FilePath))
                            File.Delete(FilePath);
                    }
                }

                try { File.Delete(localZip); } catch { }
                try
                {
                    if (Directory.Exists(downloadsDir) &&
                        !Directory.EnumerateFiles(downloadsDir).Any())
                        Directory.Delete(downloadsDir);
                }
                catch { }
            }
        }

        public void InstallPL(string PluginName)
        {
            var table = PLGui.CurrentForm.OnlineFile;
            string resolved = PluginName;
            foreach (string plname in table.Url_Table.Keys)
            {
                if (plname.IndexOf(PluginName, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    resolved = plname;
                    break;
                }
            }

            if (!table.TryGetUrl(resolved, out string downloadUrl))
            {
                Prompt.Popup("Plugin loader package not found online (looking for names containing: " + PluginName + ").");
                return;
            }

            string downloadsDir = AppPath + "\\Plugins Downloads";
            if (!Directory.Exists(downloadsDir))
                Directory.CreateDirectory(downloadsDir);

            string localZip = Path.Combine(downloadsDir, Path.GetFileName(resolved));
            if (File.Exists(localZip))
                File.Delete(localZip);

            try
            {
                DownloadFile(downloadUrl, localZip);
            }
            catch (Exception ex)
            {
                Prompt.Popup("Download failed: " + resolved + Environment.NewLine + ex.Message);
                return;
            }

            using (ZipFile zip = ZipFile.Read(localZip))
                foreach (ZipEntry e in zip.Where(x => x.FileName.StartsWith("BNSR")))
                    e.Extract(RegPath, ExtractExistingFileAction.OverwriteSilently);

            try { Directory.Delete(downloadsDir, true); } catch { }
        }

        private static void DownloadFile(string url, string destPath)
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            using (var client = new WebClient())
            {
                client.Headers[HttpRequestHeader.UserAgent] = "BnSBuddy2-PluginLoader";
                client.DownloadFile(url, destPath);
            }
        }
    }
}
