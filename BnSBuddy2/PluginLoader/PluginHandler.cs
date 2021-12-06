using BnSBuddy2.Functions;
using CG.Web.MegaApiClient;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BnSBuddy2.PluginLoader
{
    class PluginHandler
    {
        private Dictionary<string, INode> PluginNodes = new Dictionary<string, INode>();
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
            PluginNodes = PLGui.CurrentForm.OnlineFile.Node_Table;
            var client = new MegaApiClient(new WebClient(20000));
            client.LoginAnonymous();
            if (!Directory.Exists(AppPath + "\\Plugins Downloads"))
                Directory.CreateDirectory(AppPath + "\\Plugins Downloads");
            if (File.Exists(AppPath + "\\Plugins Downloads\\" + PluginNodes[PluginName].Name))
                File.Delete(AppPath + "\\Plugins Downloads\\" + PluginNodes[PluginName].Name);
            client.DownloadFile(PluginNodes[PluginName], AppPath + "\\Plugins Downloads\\" + PluginNodes[PluginName].Name);
            // Check for patches.xml
            try
            {
                if (!Directory.Exists(UserProfile))
                    Directory.CreateDirectory(UserProfile);
                if (!File.Exists(UserProfile.Replace("patches", "") + "patches.xml") && PluginName.Contains("bnspatch"))
                    client.DownloadFile(PluginNodes["patches.xml"], UserProfile.Replace("patches", "") + "patches.xml");
            }
            catch (Exception ex)
            {
                Prompt.Popup("Failed to download patches.xml to user profile documents." + Environment.NewLine + "SavePath: " + UserProfile + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
            }
            // Extract the zip file accordingly over the RegPath (only bin and bin64 folders!)
            List<string> FileInfos = new List<string>();
            bool error = false;
            if (PluginNodes[PluginName].Name.EndsWith(".zip"))
            {
                try
                {
                    using (ZipFile zip = ZipFile.Read(AppPath + "\\Plugins Downloads\\" + PluginNodes[PluginName].Name))
                    {
                        foreach (ZipEntry e in zip.Where(x => x.FileName.StartsWith("BNSR")))
                        {
                            e.Extract(RegPath, ExtractExistingFileAction.OverwriteSilently);
                            FileInfos.Add(e.FileName); // Added successfully
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
                    // Rollback
                    foreach (string FilePath in FileInfos)
                    {
                        if (File.Exists(FilePath))
                            File.Delete(FilePath);
                    }
                }

                // Dispose zip file and folder if empty
                File.Delete(AppPath + "\\Plugins Downloads\\" + PluginNodes[PluginName].Name);
                if (Directory.EnumerateFiles(AppPath + "\\Plugins Downloads").Count() == 0)
                {
                    Directory.Delete(AppPath + "\\Plugins Downloads");
                }
            }
        }

        public void InstallPL(string PluginName)
        {
            PluginNodes = PLGui.CurrentForm.OnlineFile.Node_Table;
            foreach (string plname in PluginNodes.Keys)
                if (plname.Contains(PluginName))
                {
                    PluginName = plname;
                    break;
                }
            var client = new MegaApiClient(new WebClient(20000));
            client.LoginAnonymous();
            if (!Directory.Exists(AppPath + "\\Plugins Downloads"))
                Directory.CreateDirectory(AppPath + "\\Plugins Downloads");
            if (File.Exists(AppPath + "\\Plugins Downloads\\" + PluginName))
                File.Delete(AppPath + "\\Plugins Downloads\\" + PluginName);
            client.DownloadFile(PluginNodes[PluginName], AppPath + "\\Plugins Downloads\\" + PluginName);
            //
            using (ZipFile zip = ZipFile.Read(AppPath + "\\Plugins Downloads\\" + PluginName))
                foreach (ZipEntry e in zip.Where(x => x.FileName.StartsWith("BNSR")))
                    e.Extract(RegPath, ExtractExistingFileAction.OverwriteSilently);
            // Dispose folder
            Directory.Delete(AppPath + "\\Plugins Downloads", true);
        }
    }
}
