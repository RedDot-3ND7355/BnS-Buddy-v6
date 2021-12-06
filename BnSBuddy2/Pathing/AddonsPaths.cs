using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BnSBuddy2.Pathing
{
    public class AddonsPaths
    {
        // Globals
        public string AddonsPath = "";
        // End Globals

        // Initialize Addons
        public AddonsPaths(string AppPath)
        {
            if (!Directory.Exists(AppPath + "\\Addons"))
                Directory.CreateDirectory(AppPath + "\\Addons");
            AddonsPath = AppPath + "\\Addons";
        }

        // Generate TreeView
        public void GenerateAddons(string AppPath)
        {
            // Clear
            Form1.CurrentForm.treeView3.BeginUpdate();
            if (Form1.CurrentForm.treeView3.SelectedNode != null)
                Form1.CurrentForm.treeView3.Nodes.Remove(Form1.CurrentForm.treeView3.SelectedNode);
            if (Form1.CurrentForm.treeView3.Nodes.Count > 0)
                Form1.CurrentForm.treeView3.Nodes.Clear();
            // Generate TreeNodes
            var files = new DirectoryInfo(AppPath + "\\Addons").GetFiles("*.*").Where(s => s.Name.EndsWith(".xml") || s.Name.EndsWith(".patch")); ;
            foreach (FileInfo file in files)
                Form1.CurrentForm.treeView3.Nodes.Add(file.Name.Replace(".xml", " (.xml)").Replace(".patch", " (.patch)"));
            Form1.CurrentForm.treeView3.Sort();
            Form1.CurrentForm.treeView3.EndUpdate();
        }
    }
}
