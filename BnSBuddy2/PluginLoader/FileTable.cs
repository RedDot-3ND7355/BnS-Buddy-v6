using BnSBuddy2.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace BnSBuddy2.PluginLoader
{
    /// <summary>
    /// Online plugin listing from bnsbuddy.com (public, no account).
    /// </summary>
    public class FileTable
    {
        public const string PluginsBaseUrl = "https://www.bnsbuddy.com/database/plugins/";

        /// <summary>Display / install names (zip files only).</summary>
        public List<string> File_Table = new List<string>();

        /// <summary>filename → direct download URL (includes patches.xml when present).</summary>
        public Dictionary<string, string> Url_Table = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Fetch catalog. oldFiles=true loads plugins/old/ ("Old Builds" button).
        /// </summary>
        public void GrabOnlineFileTable(bool oldFiles = false)
        {
            File_Table = new List<string>();
            Url_Table = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            string catalogUrl = oldFiles
                ? PluginsBaseUrl + "old/files_n_hash.php"
                : PluginsBaseUrl + "files_n_hash.php";

            try
            {
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

                string xml;
                using (var client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    client.Headers[HttpRequestHeader.UserAgent] = "BnSBuddy2-PluginLoader";
                    client.CachePolicy = new System.Net.Cache.RequestCachePolicy(
                        System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                    xml = client.DownloadString(catalogUrl);
                }

                if (string.IsNullOrWhiteSpace(xml))
                {
                    Prompt.Popup("Plugin catalog was empty.\r\n" + catalogUrl);
                    return;
                }

                var doc = new XmlDocument();
                doc.LoadXml(xml);
                if (doc.DocumentElement == null)
                {
                    Prompt.Popup("Invalid plugin catalog XML.\r\n" + catalogUrl);
                    return;
                }

                foreach (XmlNode file in doc.DocumentElement.GetElementsByTagName("File"))
                {
                    string filename = "";
                    string url = "";
                    foreach (XmlNode child in file.ChildNodes)
                    {
                        if (child.NodeType != XmlNodeType.Element) continue;
                        switch (child.Name.ToLowerInvariant())
                        {
                            case "filename": filename = (child.InnerText ?? "").Trim(); break;
                            case "url": url = (child.InnerText ?? "").Trim().Replace("/./", "/"); break;
                        }
                    }

                    if (string.IsNullOrEmpty(filename))
                        continue;

                    // Build URL if PHP only sent filename
                    if (string.IsNullOrEmpty(url))
                    {
                        string folder = oldFiles ? PluginsBaseUrl + "old/" : PluginsBaseUrl;
                        url = folder + Uri.EscapeDataString(filename).Replace("%20", " ");
                        // Prefer raw path for simple names; encode spaces
                        url = folder + filename.Replace(" ", "%20");
                    }

                    if (!IsListablePluginFile(filename))
                        continue;

                    if (!Url_Table.ContainsKey(filename))
                        Url_Table.Add(filename, url);

                    // UI list: zips only (patches.xml kept for bnspatch install, not shown as a row)
                    if (filename.EndsWith(".zip", StringComparison.OrdinalIgnoreCase) &&
                        !File_Table.Contains(filename))
                    {
                        File_Table.Add(filename);
                    }
                }

                File_Table = File_Table.OrderBy(n => n, StringComparer.OrdinalIgnoreCase).ToList();

                if (File_Table.Count == 0)
                {
                    Prompt.Popup(
                        "No plugins listed online.\r\n" +
                        "Catalog: " + catalogUrl + "\r\n" +
                        "Put .zip files under database/plugins/ (or plugins/old/) and ensure files_n_hash.php is deployed.");
                }
            }
            catch (Exception ex)
            {
                Prompt.Popup(
                    "Failed to load plugin catalog (no login required).\r\n" +
                    catalogUrl + "\r\n\r\n" + ex.Message);
            }
        }

        private static bool IsListablePluginFile(string name)
        {
            if (name.Equals("patches.xml", StringComparison.OrdinalIgnoreCase))
                return true; // kept in Url_Table for bnspatch

            if (!name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                return false;

            if (name.IndexOf("nclauncher2", StringComparison.OrdinalIgnoreCase) >= 0)
                return false;
            if (name.IndexOf("bnstool", StringComparison.OrdinalIgnoreCase) >= 0)
                return false;
            if (name.IndexOf("Gaffeine", StringComparison.OrdinalIgnoreCase) >= 0)
                return false;

            return true;
        }

        public bool TryGetUrl(string fileName, out string url) =>
            Url_Table.TryGetValue(fileName, out url);
    }
}
