using MaterialSkin.Controls;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BnSBuddy2.Database
{
    public static class ManagerHandler
    {
        private static readonly HttpClient Http = CreateClient();

        private static HttpClient CreateClient()
        {
            // TLS 1.2 for modern hosts
            try
            {
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            }
            catch { }

            var c = new HttpClient();
            c.Timeout = TimeSpan.FromMinutes(3);
            c.DefaultRequestHeaders.UserAgent.ParseAdd("BnSBuddy2-RemoteDB/1.0");
            return c;
        }

        public static void Delete(string author, string type, string entry)
        {
            if (string.IsNullOrEmpty(author) || string.IsNullOrEmpty(type) || string.IsNullOrEmpty(entry))
            {
                MaterialMessageBox.Show("Missing author, type, or entry.");
                return;
            }

            int index = entry.LastIndexOf('.');
            if (index >= 0)
                entry = entry.Substring(0, index);

            string typeFolder = ComboboxHandler.TypeFolderFromUi(type);
            string url = AccountHandler.BaseUrl + "Delete.php"
                + "?author=" + Uri.EscapeDataString(author)
                + "&type=" + Uri.EscapeDataString(typeFolder)
                + "&entry=" + Uri.EscapeDataString(entry);

            // PHP Verify.php needs the WebBrowser session cookie from login
            AccountHandler.browser.ScriptErrorsSuppressed = true;
            AccountHandler.browser.Navigate(new Uri(url), null, null,
                "User-Agent: " + AccountHandler.UserAgent + "\r\n");
            AccountHandler.WaitComplete();
            string body = AccountHandler.NormalizeApiBody(AccountHandler.browser.DocumentText ?? "");
            MaterialMessageBox.Show(
                $"Delete {entry} ({typeFolder}) as {author}." + Environment.NewLine + body);
        }

        public static string Upload(
            string path2file,
            string description,
            string type,
            string author,
            string action = "upload",
            string path2preview = "")
        {
            if (!AccountHandler.loggedin || string.IsNullOrEmpty(AccountHandler.SessID))
                return "Not logged in (missing sessid). Log out and log in again before uploading.";

            if (string.IsNullOrEmpty(path2file) || !File.Exists(path2file))
                return "Package file not found.";

            if (string.IsNullOrEmpty(author))
                author = AccountHandler.Author;

            string typeFolder = ComboboxHandler.TypeFolderFromUi(type);
            if (string.IsNullOrEmpty(typeFolder))
                return "Invalid type.";

            action = string.Equals(action, "update", StringComparison.OrdinalIgnoreCase) ? "update" : "upload";

            string tempPackage = null;
            var report = new StringBuilder();

            try
            {
                tempPackage = NormalizePackagePath(path2file, typeFolder);
                string uploadPath = tempPackage ?? path2file;
                string remoteName = Path.GetFileName(uploadPath);
                string baseName = Path.GetFileNameWithoutExtension(uploadPath);

                report.AppendLine("=== Upload debug ===");
                report.AppendLine("Author: " + author);
                report.AppendLine("SessID: " + AccountHandler.SessID);
                report.AppendLine("Type: " + typeFolder);
                report.AppendLine("Action: " + action);
                report.AppendLine("Local file: " + path2file);
                report.AppendLine("Remote name: " + remoteName);
                report.AppendLine("Expect path: database/" + typeFolder + "/" + author + "/" + remoteName);
                report.AppendLine();

                // 1) Main package
                string result = UploadOne(uploadPath, description ?? "", typeFolder, author, action);
                report.AppendLine("[package] " + FormatServerReply(result));

                if (!IsUploadSuccess(result))
                {
                    report.AppendLine();
                    report.AppendLine("STOPPED: package did not upload. Nothing was written on the server.");
                    if (string.IsNullOrWhiteSpace(result))
                    {
                        report.AppendLine("Empty response usually means:");
                        report.AppendLine(" • sessid file missing/mismatch (re-login)");
                        report.AppendLine(" • Upload.php fopen(.sessid) failed (path/permissions)");
                        report.AppendLine(" • PHP silent exit() on auth failure");
                    }
                    return report.ToString();
                }

                // 2) .dcn description (listing depends on this)
                string dcnPath = Path.Combine(Path.GetTempPath(), baseName + ".dcn");
                File.WriteAllText(dcnPath, description ?? "", new UTF8Encoding(false));
                try
                {
                    string dcnResult = UploadOne(dcnPath, description ?? "", typeFolder, author, "update");
                    report.AppendLine("[.dcn] " + FormatServerReply(dcnResult));
                }
                finally
                {
                    try { File.Delete(dcnPath); } catch { }
                }

                // 3) Mods preview png
                if (typeFolder == "mods" && !string.IsNullOrEmpty(path2preview) && File.Exists(path2preview))
                {
                    string pngPath = Path.Combine(Path.GetTempPath(), baseName + ".png");
                    bool tempPng = false;
                    try
                    {
                        if (!Path.GetExtension(path2preview).Equals(".png", StringComparison.OrdinalIgnoreCase) ||
                            !Path.GetFileNameWithoutExtension(path2preview)
                                .Equals(baseName, StringComparison.OrdinalIgnoreCase))
                        {
                            File.Copy(path2preview, pngPath, true);
                            tempPng = true;
                        }
                        else pngPath = path2preview;

                        string pngResult = UploadOne(pngPath, description ?? "", typeFolder, author, "update");
                        report.AppendLine("[.png] " + FormatServerReply(pngResult));
                    }
                    finally
                    {
                        if (tempPng) try { File.Delete(pngPath); } catch { }
                    }
                }

                // 4) Verify public URL is reachable
                string publicUrl = AccountHandler.BaseUrl + typeFolder + "/" +
                                   Uri.EscapeDataString(author).Replace("%20", " ") + "/" +
                                   Uri.EscapeDataString(remoteName).Replace("%20", " ");
                // simpler path encode
                publicUrl = AccountHandler.BaseUrl + typeFolder + "/" + author + "/" + remoteName;
                report.AppendLine();
                report.AppendLine("Checking: " + publicUrl);
                try
                {
                    var head = Http.SendAsync(new HttpRequestMessage(HttpMethod.Head, publicUrl))
                        .ConfigureAwait(false).GetAwaiter().GetResult();
                    report.AppendLine("HEAD status: " + (int)head.StatusCode + " " + head.StatusCode);
                    if (!head.IsSuccessStatusCode)
                    {
                        var get = Http.GetAsync(publicUrl).ConfigureAwait(false).GetAwaiter().GetResult();
                        report.AppendLine("GET status: " + (int)get.StatusCode + " " + get.StatusCode);
                        if (!get.IsSuccessStatusCode)
                            report.AppendLine("WARNING: file not reachable at expected URL after upload.");
                    }
                    else
                    {
                        report.AppendLine("OK: file is publicly reachable.");
                    }
                }
                catch (Exception ex)
                {
                    report.AppendLine("URL check failed: " + ex.Message);
                }

                return report.ToString();
            }
            catch (Exception ex)
            {
                report.AppendLine("Exception: " + ex);
                return report.ToString();
            }
            finally
            {
                if (tempPackage != null)
                    try { File.Delete(tempPackage); } catch { }
            }
        }

        public static string Update(
            string path2file,
            string description,
            string type,
            string author,
            string path2preview = "")
        {
            return Upload(path2file, description, type, author, "update", path2preview);
        }

        private static bool IsUploadSuccess(string result)
        {
            if (string.IsNullOrWhiteSpace(result)) return false;
            // Real Upload.php success string
            if (result.IndexOf("has been uploaded", StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
            // Treat "already exists" as not a successful new write
            return false;
        }

        private static string FormatServerReply(string result)
        {
            if (string.IsNullOrWhiteSpace(result))
                return "(empty body)";
            string t = result.Replace("\r", " ").Replace("\n", " ").Trim();
            if (t.Length > 300) t = t.Substring(0, 300) + "…";
            return t;
        }

        private static string NormalizePackagePath(string path2file, string typeFolder)
        {
            string want;
            switch (typeFolder)
            {
                case "addons": want = ".xml"; break;
                case "mods": want = ".zip"; break;
                case "splashes": want = ".bmp"; break;
                default: want = Path.GetExtension(path2file).ToLowerInvariant(); break;
            }

            string baseName = Path.GetFileNameWithoutExtension(path2file);
            string destName = baseName + want; // e.g. Splash.bmp (lowercase ext only)
            if (string.Equals(Path.GetFileName(path2file), destName, StringComparison.Ordinal))
                return null;

            // Keep original base name; only normalize extension for Linux globs
            string dest = Path.Combine(Path.GetTempPath(), destName);
            File.Copy(path2file, dest, true);
            return dest;
        }

        /// <summary>Single multipart POST matching Upload.php field names.</summary>
        private static string UploadOne(
            string filePath,
            string description,
            string typeFolder,
            string author,
            string action)
        {
            string url = AccountHandler.BaseUrl + "Upload.php"
                + "?author=" + Uri.EscapeDataString(AccountHandler.Author)
                + "&sessid=" + Uri.EscapeDataString(AccountHandler.SessID);

            using (var form = new MultipartFormDataContent())
            {
                // Field names must match Upload.php exactly
                form.Add(new StringContent(author ?? "", Encoding.UTF8), "author");
                form.Add(new StringContent(description ?? "", Encoding.UTF8), "description");
                form.Add(new StringContent(action ?? "upload", Encoding.UTF8), "action");
                form.Add(new StringContent(typeFolder ?? "", Encoding.UTF8), "type");

                byte[] bytes = File.ReadAllBytes(filePath);
                var fileContent = new ByteArrayContent(bytes);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                string fileName = Path.GetFileName(filePath);
                // name="file" is required by $_FILES["file"]
                form.Add(fileContent, "file", fileName);

                HttpResponseMessage response;
                try
                {
                    response = Http.PostAsync(url, form).ConfigureAwait(false).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    return "HTTP transport error: " + ex.Message;
                }

                string body = response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult() ?? "";
                if (!response.IsSuccessStatusCode)
                    return "HTTP " + (int)response.StatusCode + " " + response.StatusCode + " — " + body;

                return body;
            }
        }
    }
}
