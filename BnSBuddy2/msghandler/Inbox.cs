using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;

namespace BnSBuddy2.msghandler
{
    public class Inbox
    {
        private string[] IDS;
        public Dictionary<int, string> IDMessagePairs = new Dictionary<int, string>();
        public Dictionary<int, string> ReadMessages = new Dictionary<int, string>();
        private XmlNodeList MsgList;

        private void GetInbox()
        {
            WebClient browser = new WebClient();
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            browser.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3865.90 Safari/537.36 BnSBuddy");
            string xmltext = "";
            try
            {
                xmltext = browser.DownloadString(_UriBuilder("https://bnsbuddy.com/count/inbox.xml"));
            }
            catch { xmltext = "<?xml version=\"1.0\" encoding=\"utf-8\"?><inbox><message name=\"1\">Error, could not fetch inbox messages.</message></inbox>"; }
            XmlDocument objects = new XmlDocument();
            objects.LoadXml(xmltext);
            MsgList = objects.DocumentElement.GetElementsByTagName("message");
        }

        public void Routine()
        {
            // Fetch msgs
            GetInbox();
            // Get read ids
            GetReadIds();
            // Sort msgs by ids
            SortIDS();
            // Remove read ids
            RemoveIDS();
            // Apply unread count
            ApplyCount();
        }

        private void ApplyCount() =>
            Form1.CurrentForm.materialButton57.Text = Form1.CurrentForm.materialButton57.Text.Replace("0", IDMessagePairs.Count.ToString());

        private void RemoveIDS()
        {
            foreach (string id in IDS)
            {
                if (IDMessagePairs.ContainsKey(int.Parse(id)))
                {
                    IDMessagePairs.Remove(int.Parse(id));
                }
            }
        }

        private void SortIDS()
        {
            foreach (XmlNode node in MsgList)
            {
                IDMessagePairs.Add(int.Parse(node.Attributes["name"].Value), node.InnerText);
                ReadMessages.Add(int.Parse(node.Attributes["name"].Value), node.InnerText);
            }
            MsgList = null;
        }

        private void GetReadIds()
        {
            IDS = Form1.CurrentForm.routine.CurrentSettings["readmessagesid"].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
        }

        private Uri _UriBuilder(string url)
        {
            Uri returl = new Uri(url, UriKind.Absolute);
            return returl;
        }
    }
}
