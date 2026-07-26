using BnSBuddy2.Pages;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BnSBuddy2.Packets
{
    public static class WebViewsCollection
    {
        public static WebView2 LoginPage = new WebView2();
        public static WebView2 ConfirmationPage = new WebView2();
        private static bool IsInitiated = false;
        public static string method = "";
        public static string token = "";
        private static string _rec_ticketid = "";
        private static string _rec_state = "";

        public static async void InitBrowsers()
        {
            if (IsInitiated)
            {
                try
                {
                    LoginPage.CoreWebView2.CookieManager.DeleteAllCookies();
                    ConfirmationPage.CoreWebView2.CookieManager.DeleteAllCookies();
                    WebView21_CoreWebView2InitializationCompleted(ConfirmationPage, new CoreWebView2InitializationCompletedEventArgs(null));
                    WebView21_CoreWebView2InitializationCompleted(LoginPage, new CoreWebView2InitializationCompletedEventArgs(null));
                    return;
                }
                catch
                {
                    LoginPage.Dispose();
                    ConfirmationPage.Dispose();
                    LoginPage = new WebView2();
                    ConfirmationPage = new WebView2();
                }
            }
            // Set LoginPage UI
            LoginPage.Dock = DockStyle.Fill;
            // Configure WebView2
            var envOptions = new CoreWebView2EnvironmentOptions();
            var environment = await CoreWebView2Environment.CreateAsync(
                browserExecutableFolder: null,
                userDataFolder: Form1.CurrentForm.AppPath + "\\WebView2Env",
                options: envOptions
            );
            // Set WebView2 Envs
            InternetProxyInfo proxyInformation = _InternetProxyInfo.GetProxyInformation();
            string proxsettings = (proxyInformation.AccessType != InternetOpenType.Direct ? "--no-proxy-server=1" : "");
            Environment.SetEnvironmentVariable("WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS", $"--autoplay-policy=no-user-gesture-required --enable-media-stream=1 --disable-site-isolation-trials=1 {proxsettings}");
            // Init Browsers
            LoginPage.Name = "NormalPage";
            ConfirmationPage.Name = "ConfirmationPage";
            ConfirmationPage.CoreWebView2InitializationCompleted += new EventHandler<CoreWebView2InitializationCompletedEventArgs>(WebView21_CoreWebView2InitializationCompleted);
            LoginPage.CoreWebView2InitializationCompleted += new EventHandler<CoreWebView2InitializationCompletedEventArgs>(WebView21_CoreWebView2InitializationCompleted);
            // Ensure
            await ConfirmationPage.EnsureCoreWebView2Async(environment);
            await LoginPage.EnsureCoreWebView2Async(environment);
            IsInitiated = true;
        }

        private static void WebView21_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            // Browser Var
            var browser = sender as WebView2;
            var b_name = browser.Name;
            // Reset 

            if (!e.IsSuccess)
            {
                if (browser.CoreWebView2 == null)
                {
                    // Configure WebView2
                    var envOptions = new CoreWebView2EnvironmentOptions();
                    var environment = CoreWebView2Environment.CreateAsync(
                        browserExecutableFolder: null,
                        userDataFolder: Form1.CurrentForm.AppPath + "\\WebView2Env",
                        options: envOptions
                    );
                    browser.Dispose();
                    browser = new WebView2();
                    browser.Name = b_name;
                    browser.CoreWebView2InitializationCompleted += new EventHandler<CoreWebView2InitializationCompletedEventArgs>(WebView21_CoreWebView2InitializationCompleted);
                }
                return;
            }

            if (browser.Name == "NormalPage" && LoginPage.CoreWebView2 != null)
            {
                LoginPage.CoreWebView2.WebResourceResponseReceived += CoreWebView2_WebResourceResponseReceived;
                LoginPage.CoreWebView2.Settings.UserAgent = $"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.5414.87 Safari/537.36 NGPClient/{Form1.CurrentForm.LauncherPaths.PurpleVersionNGP} (Windows)";
                if (method == "email" || method == "phone" || method == "qrcode")
                    LoginPage.CoreWebView2.Navigate(MethodURL(method));
                else
                    _3rdPartyBuilder(method);
            }
            else if (browser.Name == "ConfirmationPage" && ConfirmationPage.CoreWebView2 != null)
            {
                ConfirmationPage.CoreWebView2.WebResourceResponseReceived += CoreWebView2_WebResourceResponseReceived;
                ConfirmationPage.CoreWebView2.Settings.UserAgent = $"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.5414.87 Safari/537.36 NGPClient/{Form1.CurrentForm.LauncherPaths.PurpleVersionNGP} (Windows)";
            }
            // Email login
            // https://login.plaync.com/nclogin/signin?app_id=DE0112AE-63E3-4888-A235-14B01AD1691A&target_app_id=DE0112AE-63E3-4888-A235-14B01AD1691A&credential_result=token&persistent=false&layout=content&return_url=purple://signin&login_type=np_email&hide_external_login_button=true
            // Phone number login
            // https://phoneauth.plaync.com/signin?app_id=DE0112AE-63E3-4888-A235-14B01AD1691A&target_app_id=DE0112AE-63E3-4888-A235-14B01AD1691A&credential_result=token&persistent=false&layout=content&return_url=purple%3A%2F%2Fsignin&login_type=np_phone
            // QR Code login
            // https://login.plaync.com/nclogin/signin?app_id=DE0112AE-63E3-4888-A235-14B01AD1691A&target_app_id=DE0112AE-63E3-4888-A235-14B01AD1691A&credential_result=token&persistent=false&layout=content&return_url=purple://signin&login_type=qr
        }

        private static void _3rdPartyBuilder(string kind)
        {
            string url = _3RDPartyProvider.Init(kind);
            byte[] PostData = Encoding.ASCII.GetBytes(_3RDPartyProvider.PostData);
            Stream StreamData = new MemoryStream();
            StreamData.Write(PostData, 0, PostData.Length);
            var request = LoginPage.CoreWebView2.Environment.CreateWebResourceRequest(url,
              "POST", StreamData, "Content-Type: application/x-www-form-urlencoded");
            LoginPage.CoreWebView2.Settings.UserAgent = "RestSharp/111.4.1.0";
            LoginPage.CoreWebView2.NavigateWithWebResourceRequest(request);
        }

        private static string MethodURL(string method)
        {
            switch (method)
            {
                case "email":
                    return "https://login.plaync.com/nclogin/signin?app_id=DE0112AE-63E3-4888-A235-14B01AD1691A&target_app_id=DE0112AE-63E3-4888-A235-14B01AD1691A&credential_result=token&persistent=false&layout=content&return_url=purple://signin&login_type=np_email&hide_external_login_button=true";
                case "phone":
                    return "https://phoneauth.plaync.com/signin?app_id=DE0112AE-63E3-4888-A235-14B01AD1691A&target_app_id=DE0112AE-63E3-4888-A235-14B01AD1691A&credential_result=token&persistent=false&layout=content&return_url=purple%3A%2F%2Fsignin&login_type=np_phone";
                case "qrcode":
                    return "https://login.plaync.com/nclogin/signin?app_id=DE0112AE-63E3-4888-A235-14B01AD1691A&target_app_id=DE0112AE-63E3-4888-A235-14B01AD1691A&credential_result=token&persistent=false&layout=content&return_url=purple://signin&login_type=qr";
                default:
                    return "";
            }
            ;
        }

        private static bool LoginWorked = false;

        private class _3rdResults
        {
            public List<string> EncryptedData;
            public string authProviderCode;
            public bool success;
        }

        private static void CoreWebView2_WebResourceResponseReceived(object sender, CoreWebView2WebResourceResponseReceivedEventArgs e)
        {
            // Catch 3RD Party Detour Results
            if (e.Response != null && e.Response.StatusCode == 200 && e.Request.Uri.Contains($"thirdparty.plaync.com/{method}/login/authntoken/result") && !LoginWorked)
            {
                try
                {
                    Task<Stream> ContentResult = e.Response.GetContentAsync();
                    if (ContentResult != null)
                    {
                        var result = ContentResult.GetAwaiter();
                        result.OnCompleted(async () =>
                        {
                            try
                            {
                                var res = result.GetResult();
                                StreamReader reader = new StreamReader(res);
                                string JsonText = reader.ReadToEnd();
                                while (JsonText == "") { Thread.Sleep(100); }
                                var success = JsonConvert.DeserializeObject<_3rdResults>(JsonText);
                                if (success != null && success.success && success.EncryptedData.Count == 1)
                                {
                                    WebLogin.CurrentForm.token = _3RDPartyProvider.rsaProvider.GetDecryptedToken(success.EncryptedData);
                                    WebLogin.CurrentForm.DialogResult = DialogResult.OK;
                                    WebLogin.CurrentForm.LoginWorked = true;
                                    WebLogin.CurrentForm.Close();
                                }
                                else
                                {
                                    // Recheck
                                    await Task.Delay(1000);
                                    var request = LoginPage.CoreWebView2.Environment.CreateWebResourceRequest(e.Request.Uri,
                                        "POST", new MemoryStream(), "Content-Type: application/x-www-form-urlencoded");
                                    ConfirmationPage.CoreWebView2.Settings.UserAgent = "RestSharp/111.4.1.0";
                                    ConfirmationPage.CoreWebView2.NavigateWithWebResourceRequest(request);
                                }
                            }
                            catch { }
                        });
                    }
                }
                catch { }
            }
            // 3RD Party Login
            if (e.Response != null && e.Response.StatusCode == 200 && e.Request.Uri.EndsWith($"thirdparty.plaync.com/{method}/login/authntoken") && !LoginWorked)
            {
                try
                {
                    Task<Stream> ContentResult = e.Response.GetContentAsync();
                    if (ContentResult != null)
                    {
                        var result = ContentResult.GetAwaiter();
                        result.OnCompleted(() =>
                        {
                            try
                            {
                                var res = result.GetResult();
                                StreamReader reader = new StreamReader(res);
                                string JsonText = reader.ReadToEnd();
                                while (JsonText == "") { Thread.Sleep(100); }
                                _rec_ticketid = JsonConvert.DeserializeObject<ticketReceived>(JsonText).ticket_id;
                                _rec_state = JsonConvert.DeserializeObject<ticketReceived>(JsonText).state;
                                var request = LoginPage.CoreWebView2.Environment.CreateWebResourceRequest($"https://thirdparty.plaync.com/{method}/login/authntoken/result?ticket_id={_rec_ticketid}",
                                  "POST", new MemoryStream(), "Content-Type: application/x-www-form-urlencoded");
                                LoginPage.CoreWebView2.Settings.UserAgent = "RestSharp/111.4.1.0";
                                LoginPage.CoreWebView2.NavigateWithWebResourceRequest(request);
                                // Login Request
                                Process.Start($"https://thirdparty.plaync.com/{method}/login/authntoken?ticket_id={_rec_ticketid}&callback_type=callback_unavailable");
                                // Hide Form
                                //this.Hide();
                            }
                            catch { }
                        });
                    }
                }
                catch { }
            }
            // Normal Login
            if (e.Response != null && e.Response.StatusCode == 200 && e.Request.Uri.Contains("loginapi.plaync.com/signin/credential") && !LoginWorked)
            {
                try
                {
                    Task<Stream> ContentResult = e.Response.GetContentAsync();
                    if (ContentResult != null)
                    {
                        var result = ContentResult.GetAwaiter();
                        result.OnCompleted(() =>
                        {
                            try
                            {
                                var res = result.GetResult();
                                StreamReader reader = new StreamReader(res);
                                string JsonText = reader.ReadToEnd();
                                while (JsonText == "") { Thread.Sleep(100); }
                                WebLogin.CurrentForm.token = JsonConvert.DeserializeObject<tokenReceived>(JsonText).authn_token;
                                WebLogin.CurrentForm.DialogResult = DialogResult.OK;
                                WebLogin.CurrentForm.LoginWorked = true;
                                WebLogin.CurrentForm.Close();
                            }
                            catch { }
                        });
                    }
                }
                catch { }
            }
        }

        private class tokenReceived
        {
            public string authn_token;
        }

        private class ticketReceived
        {
            public string state;
            public string ticket_id;
        }
    }

    internal enum InternetOpenType
    {
        PreConfig,
        Direct,
        Proxy = 3,
        PreConfigWithNoAutoProxy
    }

    internal struct InternetProxyInfo
    {
        public InternetOpenType AccessType;

        public string ProxyAddress;

        public string ProxyBypass;
    }

    internal class _InternetProxyInfo
    {
        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool InternetQueryOption(IntPtr hInternet, uint dwOption, IntPtr lpBuffer, ref int lpdwBufferLength);
        private const uint InternetOptionProxy = 38U;

        public static InternetProxyInfo GetProxyInformation()
        {
            int num = 0;
            InternetQueryOption(IntPtr.Zero, 38U, IntPtr.Zero, ref num);
            IntPtr intPtr = IntPtr.Zero;
            InternetProxyInfo internetProxyInfo;
            try
            {
                intPtr = Marshal.AllocHGlobal(num);
                if (!InternetQueryOption(IntPtr.Zero, 38U, intPtr, ref num))
                {
                    throw new Win32Exception();
                }
                internetProxyInfo = (InternetProxyInfo)Marshal.PtrToStructure(intPtr, typeof(InternetProxyInfo));
            }
            catch
            {
                internetProxyInfo = new InternetProxyInfo
                {
                    AccessType = InternetOpenType.Direct
                };
            }
            finally
            {
                if (intPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(intPtr);
                }
            }
            return internetProxyInfo;
        }
    }
}
