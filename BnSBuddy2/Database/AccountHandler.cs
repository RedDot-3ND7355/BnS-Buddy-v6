using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace BnSBuddy2.Database
{
    public static class AccountHandler
    {
        public const string BaseUrl = "https://www.bnsbuddy.com/database/";

        /// <summary>Used only for Delete.php PHP session cookie (Verify.php).</summary>
        public static WebBrowser browser = new WebBrowser();

        public static bool loggedin = false;
        public static string Author = "";
        /// <summary>Written to server {author}.sessid — required by Upload.php.</summary>
        public static string SessID = "";

        public static string UserAgent =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";

        public static void WaitComplete()
        {
            while (browser.ReadyState != WebBrowserReadyState.Complete)
                Application.DoEvents();
        }

        /// <summary>Raw HTTP GET (no IE HTML wrapper). Preferred for API status codes.</summary>
        public static string HttpGet(string url)
        {
            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                client.Headers[HttpRequestHeader.UserAgent] = UserAgent;
                // Avoid cached login/catalog responses
                client.CachePolicy = new System.Net.Cache.RequestCachePolicy(
                    System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                return client.DownloadString(url) ?? "";
            }
        }

        /// <summary>Strip IE WebBrowser HTML wrapping if present; return plain body.</summary>
        public static string NormalizeApiBody(string body)
        {
            if (string.IsNullOrEmpty(body)) return "";
            body = body.Trim();
            // Plain status
            if (body.Length <= 4 && body.All(char.IsDigit))
                return body;
            // IE often wraps plain text in <html><body>...</body></html>
            if (body.IndexOf('<') >= 0)
            {
                var m = Regex.Match(body, @"<body[^>]*>\s*(.*?)\s*</body>",
                    RegexOptions.IgnoreCase | RegexOptions.Singleline);
                if (m.Success)
                    body = m.Groups[1].Value.Trim();
                body = Regex.Replace(body, "<[^>]+>", "").Trim();
            }
            return body;
        }

        private static void BrowserNavigate(string url)
        {
            try
            {
                browser.ScriptErrorsSuppressed = true;
                browser.Navigate(new Uri(url), null, null, "User-Agent: " + UserAgent + "\r\n");
                WaitComplete();
            }
            catch { /* cookie warm-up best-effort */ }
        }

        public static string RegisterAuthor(string user, string pass)
        {
            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
                return "Status: One or more field is empty!";

            string url = BaseUrl + "RegisterUser.php?author=" + Uri.EscapeDataString(user.Trim()) +
                         "&password=" + Uri.EscapeDataString(pass);
            string body = NormalizeApiBody(HttpGet(url));

            switch (body)
            {
                case "1": return "Status: Registered Successfully — now log in.";
                case "2": return "Status: User already exists!";
                case "3": return "Status: One or more field is empty!";
            }
            return "Status: Error (" + (body.Length > 80 ? body.Substring(0, 80) + "…" : body) + ")";
        }

        public static string LoginAuthor(string user, string pass)
        {
            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
                return "Status: One or more field is empty!";

            user = user.Trim();
            // Must match byte-for-byte what Upload.php reads from {author}.sessid
            SessID = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

            string url = BaseUrl + "LoginUser.php?author=" + Uri.EscapeDataString(user) +
                         "&password=" + Uri.EscapeDataString(pass) +
                         "&sessid=" + Uri.EscapeDataString(SessID);

            string body;
            try
            {
                body = NormalizeApiBody(HttpGet(url));
            }
            catch (Exception ex)
            {
                SessID = "";
                return "Status: Network error — " + ex.Message;
            }

            // Accept plain "1" or body that is only that code
            if (body == "1" || body.Trim() == "1")
            {
                loggedin = true;
                Author = user;
                // Warm IE cookie jar for Delete.php / Verify.php (PHP $_SESSION)
                BrowserNavigate(url);
                return "Status: Logged in as " + Author + " (sessid ok)";
            }

            SessID = "";
            loggedin = false;
            Author = "";

            switch (body)
            {
                case "2": return "Status: User does not exist!";
                case "3": return "Status: Password mismatch!";
                case "4": return "Status: One or more field is empty!";
            }
            return "Status: Login failed. Server returned: " +
                   (string.IsNullOrEmpty(body) ? "(empty)" : body.Substring(0, Math.Min(120, body.Length)));
        }

        public static void LogoutAuthor()
        {
            if (!string.IsNullOrEmpty(Author))
            {
                string url = BaseUrl + "LogoutUser.php?author=" + Uri.EscapeDataString(Author);
                try { HttpGet(url); } catch { }
                try { BrowserNavigate(url); } catch { }
            }
            loggedin = false;
            Author = "";
            SessID = "";
        }

        public static List<string> ReturnAuthors()
        {
            try
            {
                string body = HttpGet(BaseUrl + "GetUserList.php");
                return body.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(a => a.Trim())
                    .Where(a => a.Length > 0)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(a => a, StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }
            catch
            {
                return new List<string>();
            }
        }
    }
}
