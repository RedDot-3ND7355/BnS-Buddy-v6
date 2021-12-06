using BnSBuddy2.Functions;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace BnSBuddy2.Pathing
{
    public class PakPaths
    {
        // Globals
        public string ModsPath = "";
        public string YourModsPath = "";
        public string PakPath = "";
        public string LocalDatPath = "";
        // End Globals

        // Return Pak Folder Path & Set globals
        public void ReturnPakPath(string region)
        {
            region = RegionConvert.Convert(region);
            YourModsPath = Form1.CurrentForm.AppPath + "\\Mods\\";
            if (!Directory.Exists(YourModsPath))
                Directory.CreateDirectory(YourModsPath);
            ModsPath = Form1.CurrentForm.ClientPaths.Installs[region] + "BNSR\\Content\\Paks\\Mods\\";
            if (!Directory.Exists(ModsPath))
                Directory.CreateDirectory(ModsPath);
            PakPath = Form1.CurrentForm.ClientPaths.Installs[region] + "BNSR\\Content\\Paks\\";
            FindLocalPath(region);
            GenerateTree();
            VerifyInstalledMods();
        }

        // Find Local Dat Path 
        private void FindLocalPath(string region)
        {
            foreach (DirectoryInfo directory in new DirectoryInfo(Form1.CurrentForm.ClientPaths.Installs[region] + "BNSR\\Content\\local").GetDirectories())
                if (Directory.Exists(directory.FullName + "\\" + Form1.CurrentForm.materialComboBox3.SelectedItem.ToString() + "\\data"))
                    LocalDatPath = directory.FullName + "\\" + Form1.CurrentForm.materialComboBox3.SelectedItem.ToString() + "\\data";
        }

        // Generate Mod Tree
        public void GenerateTree()
        {
            // Reset
            Form1.CurrentForm.materialButton26.Visible = false;
            Form1.CurrentForm.materialButton32.Enabled = false;
            // Clear
            Form1.CurrentForm.treeView2.BeginUpdate();
            if (Form1.CurrentForm.treeView2.SelectedNode != null && Form1.CurrentForm.treeView2.SelectedNode.Parent == null)
                Form1.CurrentForm.treeView2.Nodes.Remove(Form1.CurrentForm.treeView2.SelectedNode);
            else if (Form1.CurrentForm.treeView2.SelectedNode != null && Form1.CurrentForm.treeView2.SelectedNode.Parent != null)
                Form1.CurrentForm.treeView2.Nodes[Form1.CurrentForm.treeView2.SelectedNode.Parent.Index].Nodes.Remove(Form1.CurrentForm.treeView2.SelectedNode);
            if (Form1.CurrentForm.treeView2.Nodes.Count > 0)
                Form1.CurrentForm.treeView2.Nodes.Clear();
            // Parent Dirs
            DirectoryInfo[] directories = new DirectoryInfo(YourModsPath).GetDirectories();
            foreach (DirectoryInfo directory in directories)
            {
                TreeNode node = new TreeNode(directory.Name);
                // Child Dirs
                foreach (DirectoryInfo subdirectory in new DirectoryInfo(directory.FullName).GetDirectories())
                    node.Nodes.Add(new TreeNode(subdirectory.Name));
                Form1.CurrentForm.treeView2.Nodes.Add(node);
            }
            Form1.CurrentForm.treeView2.EndUpdate();
        }

        // Verify Installed Mods
        private void VerifyInstalledMods()
        {
            // Parent Node
            foreach (TreeNode node in Form1.CurrentForm.treeView2.Nodes)
            {
                if (Directory.Exists(ModsPath + node.Text))
                {
                    node.Text += " (Installed)";
                    node.ForeColor = Form1.CurrentForm.materialSkinManager.ColorScheme.AccentColor;
                }
                // Child Node
                foreach (TreeNode subnode in Form1.CurrentForm.treeView2.Nodes[node.Index].Nodes)
                    if (Directory.Exists(ModsPath + subnode.FullPath.Replace(" (Installed)", "")))
                    {
                        subnode.Text += " (Installed)";
                        subnode.ForeColor = Form1.CurrentForm.materialSkinManager.ColorScheme.AccentColor;
                    }
            }
        }
    }
}
