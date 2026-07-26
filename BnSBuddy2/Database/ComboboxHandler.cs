using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;

namespace BnSBuddy2.Database
{
    public static class ComboboxHandler
    {
        public static Dictionary<string, CatalogEntry> GeneratedPairs =
            new Dictionary<string, CatalogEntry>(StringComparer.OrdinalIgnoreCase);

        /// <summary>Last catalog fetch diagnostics (shown in UI when empty/fail).</summary>
        public static string LastCatalogStatus { get; private set; } = "";

        public static void FilterPerCategory(string category, object filter)
        {
            GeneratedPairs = new Dictionary<string, CatalogEntry>(StringComparer.OrdinalIgnoreCase);
            LastCatalogStatus = "";
            if (string.IsNullOrWhiteSpace(category))
            {
                LastCatalogStatus = "No category selected.";
                return;
            }

            string type = TypeFolderFromUi(category);
            string url = AccountHandler.BaseUrl + type + "/files_n_hash.php";
            string authorFilter = filter?.ToString();
            if (string.IsNullOrWhiteSpace(authorFilter))
                authorFilter = null;

            try
            {
                string xml;
                using (var client = new WebClient())
                {
                    client.Encoding = System.Text.Encoding.UTF8;
                    client.Headers[HttpRequestHeader.UserAgent] = "BnSBuddy2-RemoteDB";
                    xml = client.DownloadString(url);
                }

                if (string.IsNullOrWhiteSpace(xml))
                {
                    LastCatalogStatus = "Empty response from " + url;
                    return;
                }

                var doc = new XmlDocument();
                doc.LoadXml(xml);

                if (doc.DocumentElement == null)
                {
                    LastCatalogStatus = "Invalid catalog XML.";
                    return;
                }

                int rawCount = 0;
                foreach (XmlNode file in doc.DocumentElement.GetElementsByTagName("File"))
                {
                    rawCount++;
                    var entry = new CatalogEntry();
                    foreach (XmlNode info in file.ChildNodes)
                    {
                        if (info.NodeType != XmlNodeType.Element) continue;
                        switch (info.Name.ToLowerInvariant())
                        {
                            case "filename": entry.Filename = info.InnerText ?? ""; break;
                            case "filehash": entry.FileHash = info.InnerText ?? ""; break;
                            case "author": entry.Author = info.InnerText ?? ""; break;
                            case "description": entry.Description = info.InnerText ?? ""; break;
                            case "preview": entry.Preview = info.InnerText ?? ""; break;
                            case "url": entry.Url = info.InnerText ?? ""; break;
                        }
                    }

                    // Fix catalog URLs that still contain "./Author" from PHP glob
                    if (!string.IsNullOrEmpty(entry.Url))
                        entry.Url = entry.Url.Replace("/./", "/");
                    if (!string.IsNullOrEmpty(entry.Preview))
                        entry.Preview = entry.Preview.Replace("/./", "/");

                    if (string.IsNullOrEmpty(entry.Filename))
                        continue;

                    if (authorFilter != null &&
                        !string.Equals(entry.Author, authorFilter, StringComparison.OrdinalIgnoreCase))
                        continue;

                    string key = entry.Filename;
                    if (GeneratedPairs.ContainsKey(key))
                        key = entry.Author + "/" + entry.Filename;

                    GeneratedPairs[key] = entry;
                }

                if (rawCount == 0)
                {
                    LastCatalogStatus =
                        "Catalog is empty on server (" + type + ")." + Environment.NewLine +
                        "URL: " + url + Environment.NewLine +
                        "Upload must land in database/" + type + "/{author}/*." + Environment.NewLine +
                        "Splashes need lowercase .bmp (+ .dcn). Linux is case-sensitive.";
                }
                else if (GeneratedPairs.Count == 0)
                {
                    LastCatalogStatus =
                        "Server returned " + rawCount + " file(s) but none matched author filter \"" +
                        authorFilter + "\". Clear the author filter.";
                }
                else
                {
                    LastCatalogStatus = "Loaded " + GeneratedPairs.Count + " item(s) from " + type + ".";
                }
            }
            catch (Exception ex)
            {
                LastCatalogStatus = "Catalog load failed: " + ex.Message + Environment.NewLine + url;
            }
        }

        public static string TypeFolderFromUi(string uiType)
        {
            if (string.IsNullOrEmpty(uiType)) return "";
            switch (uiType.Trim().ToLowerInvariant())
            {
                case "addon":
                case "addons": return "addons";
                case "mod":
                case "mods": return "mods";
                case "splash":
                case "splashes": return "splashes";
                default: return uiType.Trim().ToLowerInvariant();
            }
        }
    }
}
