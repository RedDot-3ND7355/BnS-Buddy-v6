using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml;
using MaterialSkin;

namespace BnSBuddy2.msghandler
{
    public class Inbox
    {
        private string[] IDS;
        public Dictionary<int, string> IDMessagePairs = new Dictionary<int, string>();
        public List<XmlNode> ReadMessages = new List<XmlNode>();
        public List<int> ReadIDs = new List<int>();
        public Dictionary<string, int> TitleIDPairs = new Dictionary<string, int>();
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
            catch { xmltext = "<?xml version=\"1.0\" encoding=\"utf-8\"?><inbox><message title=\"Error\" name=\"1\">Error, could not fetch inbox messages.</message></inbox>"; }
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
            // Apply unread count
            ApplyCount();
            // Generate Inbox List
            PopulateInbox();
        }

        private void PopulateInbox()
        {
            foreach (XmlNode node in ReadMessages)
            {
                string readornot = "";
                if (!ReadIDs.Contains(int.Parse(node.Attributes["name"].Value)))
                    readornot = " (unread)";
                Form1.CurrentForm.materialListBox2.Items.Add(new MaterialListBoxItem(node.Attributes["title"].Value + readornot));
                TitleIDPairs.Add(node.Attributes["title"].Value, int.Parse(node.Attributes["name"].Value));
            }
        }

        private void ApplyCount() =>
            Form1.CurrentForm.materialButton57.Text = Form1.CurrentForm.materialButton57.Text.Replace("0", IDMessagePairs.Count.ToString());

        private void SortIDS()
        {
            foreach (XmlNode node in MsgList)
            {
                IDMessagePairs.Add(int.Parse(node.Attributes["name"].Value), node.InnerText);
                ReadMessages.Add(node);
            }
            MsgList = null;
        }

        public void ReadID(int id)
        {
            if (!ReadIDs.Contains(id))
                ReadIDs.Add(id);
        }

        private void GetReadIds() =>
            ReadIDs.AddRange(Array.ConvertAll(Form1.CurrentForm.routine.CurrentSettings["readmessagesid"].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries), s => int.Parse(s)));

        private Uri _UriBuilder(string url)
        {
            return new Uri(url, UriKind.Absolute);
        }
    }
}
