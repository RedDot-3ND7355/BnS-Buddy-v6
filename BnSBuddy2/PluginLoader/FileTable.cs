using CG.Web.MegaApiClient;
using BnSBuddy2.Functions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BnSBuddy2.PluginLoader
{
    public class FileTable
    {
        public List<string> File_Table = new List<string>();
        public Dictionary<string, INode> Node_Table = new Dictionary<string, INode>();

        public void GrabMegaFileTable(bool oldFiles = false)
        {
            try
            {
                var client = new MegaApiClient(new WebClient(20000));
                client.LoginAnonymous();

                Dictionary<string, Uri> Web2Nodes = new Dictionary<string, Uri>();
                if (!oldFiles)
                {
                    Web2Nodes.Add("qaZnWQyD", new Uri("https://mega.nz/folder/WXhzUZ7Y#XzlqkPa8DU4X8xrILQDdZA")); // Pilao
                    Web2Nodes.Add("ocUW3SSR", new Uri("https://mega.nz/folder/4EUF2IhL#Ci1Y-sbbyw7nwwMGvHV2_w")); // Hora
                    Web2Nodes.Add("psoExbCa", new Uri("https://mega.nz/folder/olxggBJR#292EvTBI81Vwbo2WvOSVNA")); // Endless
                }
                else
                {
                    Web2Nodes.Add("OG4B2D6S", new Uri("https://mega.nz/folder/WXhzUZ7Y#XzlqkPa8DU4X8xrILQDdZA")); // Pilao old
                    Web2Nodes.Add("1EcmySDQ", new Uri("https://mega.nz/folder/4EUF2IhL#Ci1Y-sbbyw7nwwMGvHV2_w")); // Hora old
                    Web2Nodes.Add("gtgyDTQB", new Uri("https://mega.nz/folder/olxggBJR#292EvTBI81Vwbo2WvOSVNA")); // Endless old
                }

                IEnumerable<INode> nodes;
                foreach (string folder in Web2Nodes.Keys)
                {
                    try
                    {
                        nodes = client.GetNodesFromLink(Web2Nodes[folder]);
                        foreach (INode node in nodes.Where(x => x.Type == NodeType.File).Where(x => x.ParentId == folder))
                        {
                            if (node.Name.EndsWith(".zip") || node.Name == "patches.xml")
                            {
                                if (!File_Table.Contains(node.Name))
                                {
                                    if (node.Name != "patches.xml" && !node.Name.Contains("nclauncher2") && !node.Name.Contains("bnstool") && !node.Name.Contains("Gaffeine"))
                                    {
                                        File_Table.Add(node.Name);
                                    }
                                    Node_Table.Add(node.Name, node);
                                }
                            }
                        }
                    }
                    catch { Prompt.Popup("Failed to grab nodes from link: " + Web2Nodes[folder]); }
                }
                client.Logout();
            }
            catch (Exception ex)
            {
                Prompt.Popup("Error: " + ex.ToString());
            }
        }
    }
}
