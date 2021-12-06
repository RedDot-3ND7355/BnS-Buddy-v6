using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Revamped_BnS_Buddy.QoLToggles
{
    public class FetchPatches
    {
        public FetchPatches()
        {
            try
            {
                List<string> KeyGot = new List<string>();
                WebClient browser = new WebClient();
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                browser.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3865.90 Safari/537.36 BnSBuddy");
                string xmltext = browser.DownloadString(_UriBuilder("https://bnsbuddy.com/count/Patches.xml"));
                XmlDocument objects = new XmlDocument();
                objects.LoadXml(xmltext);
                PatchSet = objects;
                OnlinePatches = true;
            }
            catch
            {
                PatchSet = DefaultXml();
            }
        }

        public XmlDocument PatchSet
        {
            get;
            internal set;
        }

        public bool OnlinePatches = false;

        private XmlDocument DefaultXml() 
        {
            XmlDocument _defxml = new XmlDocument();
            _defxml.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\"?><patches><patch name=\"f5inf8\"><UsesBit>False</UsesBit><on>EB</on><off>74</off><pattern>4D 85 D2 [bool] ?? 41 80 BA 80 ?? ?? ?? 01</pattern><length>13</length></patch><patch name=\"infwallc\"><UsesBit></UsesBit><on></on><off></off><pattern></pattern><length></length></patch><patch name=\"no2au\"><UsesBit>True</UsesBit><bit32><on>B3 01 90</on><off>8A 5E 1C</off><pattern>[bool] 8D 74 24 10</pattern><length>7</length></bit32><bit64><on>C6 06 01 90</on><off>0F B6 76 20</off><pattern>[bool] 48 85 D2</pattern><length>7</length></bit64></patch></patches>");
            return _defxml;
        }

        private Uri _UriBuilder(string url)
        {
            Uri returl = new Uri(url, UriKind.Absolute);
            return returl;
        }
    }
}
