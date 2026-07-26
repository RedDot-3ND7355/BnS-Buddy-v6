using BnSBuddy2.Functions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace BnSBuddy2.CoreHandlers
{
    public class Mods
    {
        // Globals
        List<TreeNode> Queue = new List<TreeNode>();
        Thread DoSwapThreads;
        // End Globals

        bool isbusy = false;
        // Parent Ticking & Child
        public void CheckerHandling(TreeNode node)
        {
            if (isbusy)
                return;
            isbusy = true;
            // Parent
            if (node.Parent == null && node.Nodes.Count > 0)
                foreach (TreeNode childnode in node.Nodes)
                    Form1.CurrentForm.treeView2.Nodes[node.Index].Nodes[childnode.Index].Checked = node.Checked;
            // Child
            if (node.Parent != null)
            {
                int _checked = 0;
                foreach (TreeNode childnode in node.Parent.Nodes)
                    if (childnode.Checked)
                        _checked++;
                if (_checked == 0 || _checked > 0 && _checked < node.Parent.Nodes.Count)
                    Form1.CurrentForm.treeView2.Nodes[node.Parent.Index].Checked = false;
                else
                    Form1.CurrentForm.treeView2.Nodes[node.Parent.Index].Checked = true;
            }
            isbusy = false;
        }

        // After Selecting
        public void AfterSelect(string FolderName)
        {
            Form1.CurrentForm.materialButton32.Visible = true;
            Form1.CurrentForm.materialButton32.Enabled = true;
            string PathToFolderName = Form1.CurrentForm.PakPaths.YourModsPath + FolderName;
            if (File.Exists(PathToFolderName + "\\description.txt"))
                Form1.CurrentForm.materialLabel43.Text = $"Description: {File.ReadAllText(PathToFolderName + "\\description.txt")}";
            else
                Form1.CurrentForm.materialLabel43.Text = "Description:";
            Form1.CurrentForm.materialButton26.Visible = Form1.CurrentForm.materialLabel43.Text.Length > 40;
        }

        // Preview Mod
        public void PreviewMod(string FolderName)
        {
            string PathToFolderName = Form1.CurrentForm.PakPaths.YourModsPath + FolderName;
            if (File.Exists(PathToFolderName + "\\preview.png"))
                Process.Start(PathToFolderName + "\\preview.png");
            else if (File.Exists(PathToFolderName + "\\preview.jpg"))
                Process.Start(PathToFolderName + "\\preview.jpg");
            else if (File.Exists(PathToFolderName + "\\preview.gif"))
                Process.Start(PathToFolderName + "\\preview.gif");
            else
                Prompt.Popup("No Preview found!");
        }

        // Open Mod Folder
        public void OpenModsFolder() =>
            Process.Start("explorer.exe", Form1.CurrentForm.PakPaths.YourModsPath);

        // Read Mod Info
        public void ReadModInfo(string FolderName)
        {
            string PathToFolderName = Form1.CurrentForm.PakPaths.YourModsPath + FolderName;
            if (File.Exists(PathToFolderName + "\\description.txt"))
                Prompt.Popup(File.ReadAllText(PathToFolderName + "\\description.txt"));
            else
                Prompt.Popup("No Description found!");
        }

        // Start File Swapping and prep
        public void DoFileSwap(bool _Install = false)
        {
            DisableButtons();
            CreateQueue(_Install);
            foreach (TreeNode node in Queue)
                if (_Install)
                {
                    DoSwapThreads = new Thread(() => Install(node.FullPath.Replace(" (Installed)", "")));
                    DoSwapThreads.Priority = ThreadPriority.Normal;
                    DoSwapThreads.Start();
                    DoSwapThreads.Join();
                }
                else
                {
                    DoSwapThreads = new Thread(() => Uninstall(node.FullPath.Replace(" (Installed)", "")));
                    DoSwapThreads.Priority = ThreadPriority.Normal;
                    DoSwapThreads.Start();
                    DoSwapThreads.Join();
                }
            RenameCompletedNodes(_Install);
            DisableButtons(false);
        }

        // Rename completed nodes
        private void RenameCompletedNodes(bool _Install)
        {
            foreach (TreeNode Node in Form1.CurrentForm.treeView2.Nodes)
            {
                // Parent
                if (Node.Checked && !Node.Text.Contains(" (Installed)") && _Install)
                {
                    Node.Text += " (Installed)";
                    Node.ForeColor = Form1.CurrentForm.materialSkinManager.ColorScheme.AccentColor;
                }
                else if (Node.Checked && Node.Text.Contains(" (Installed)") && !_Install)
                {
                    Node.Text = Node.Text.Replace(" (Installed)", "");
                    Node.ForeColor = default(Color);
                }
                // Child
                if (Node.Nodes.Count > 0)
                    foreach (TreeNode ChildNode in Node.Nodes)
                    {
                        if (ChildNode.Checked && !ChildNode.Text.Contains(" (Installed)") && _Install)
                        {
                            ChildNode.Text += " (Installed)";
                            ChildNode.ForeColor = Form1.CurrentForm.materialSkinManager.ColorScheme.AccentColor;
                        }
                        else if (ChildNode.Checked && ChildNode.Text.Contains(" (Installed)") && !_Install)
                        {
                            ChildNode.Text = ChildNode.Text.Replace(" (Installed)", "");
                            ChildNode.ForeColor = default(Color);
                        }
                    }
            }
        }

        // Create Queue for installing & uninstalling
        private void CreateQueue(bool _Install)
        {
            Queue = new List<TreeNode>();
            foreach (TreeNode Node in Form1.CurrentForm.treeView2.Nodes)
            {
                // Parent
                if (Node.Checked && Node.Text.Contains(" (Installed)") && !_Install && Node.Nodes.Count == 0)
                    Queue.Add(Node);
                else if (Node.Checked && !Node.Text.Contains(" (Installed)") && _Install && Node.Nodes.Count == 0)
                    Queue.Add(Node);
                // Child
                if (Node.Nodes.Count > 0)
                    foreach (TreeNode ChildNode in Node.Nodes)
                    {
                        if (ChildNode.Checked && ChildNode.Text.Contains(" (Installed)") && !_Install)
                            Queue.Add(ChildNode);
                        else if (ChildNode.Checked && !ChildNode.Text.Contains(" (Installed)") && _Install)
                            Queue.Add(ChildNode);
                    }
            }
        }

        [DllImport("kernel32.dll")]
        static extern bool CreateSymbolicLink(
        string lpSymlinkFileName, string lpTargetFileName, SymbolicLink dwFlags);

        enum SymbolicLink
        {
            File = 0,
            Directory = 1
        }

        // Disable & Enabled Buttons when busy
        private void DisableButtons(bool disabled = true)
        {
            Form1.CurrentForm.materialProgressBar2.Visible = disabled;
            Form1.CurrentForm.materialButton30.Enabled = !disabled;
            Form1.CurrentForm.materialButton27.Enabled = !disabled;
            Form1.CurrentForm.materialButton25.Enabled = !disabled;
        }

        private void bw_ProgressChangedV2()
        {
            var maxint = Form1.CurrentForm.materialProgressBar2.Maximum;
            var curint = Form1.CurrentForm.materialProgressBar2.Value;
            if (curint == maxint)
                Form1.CurrentForm.materialProgressBar2.Value = 0;
        }

        private void CopyFilesRecursively(string sourcePath, string targetPath)
        {
            Directory.CreateDirectory(targetPath);
            //Copy all the files & Replaces any files with the same name
            DirectoryInfo directoryInfo = new DirectoryInfo(sourcePath);
            FileInfo[] pak = directoryInfo.GetFiles("*.pak");
            FileInfo[] sig = directoryInfo.GetFiles("*.sig");
            FileInfo[] files = pak.Concat(sig).ToArray();
            foreach (FileInfo newPath in files)
                File.Copy(newPath.FullName, targetPath + "\\" + newPath.Name, true);
        }

        // Install Mod by Folder
        private void Install(string FolderName)
        {
            string DestinationFolder = Form1.CurrentForm.PakPaths.ModsPath;
            string OriginalLocation = Form1.CurrentForm.PakPaths.YourModsPath;
            if (!Directory.Exists(OriginalLocation + FolderName))
            {
                Form1.CurrentForm.AddModManLogs($"[Error] {FolderName} [modded] does not exist!");
                return;
            }    
            DirectoryInfo directoryInfo = new DirectoryInfo(OriginalLocation + FolderName);
            FileInfo[] pak = directoryInfo.GetFiles("*.pak");
            FileInfo[] sig = directoryInfo.GetFiles("*.sig");
            FileInfo[] files = pak.Concat(sig).ToArray();
            int num = 0;
            if (files.Length != 0)
            {
                Form1.CurrentForm.materialProgressBar2.Value = 0;
                Control.CheckForIllegalCrossThreadCalls = false;
                Form1.CurrentForm.AddModManLogs("");
                try
                {
                    bool isparent = CheckIfParent(FolderName);
                    if (isparent)
                        Directory.CreateDirectory(DestinationFolder + Directory.GetParent(FolderName).Name);
                    Form1.CurrentForm.AddModManLogs($"[Linking] {FolderName} [folder] at Pak/Mods");
                    if (!Directory.Exists(DestinationFolder + FolderName))
                    {
                        if (!CreateSymbolicLink(DestinationFolder + FolderName, OriginalLocation + FolderName, SymbolicLink.Directory))
                        {
                            Form1.CurrentForm.AddModManLogs($"[Failed Linking] {FolderName} [modded] copying instead");
                            Form1.CurrentForm.AddModManLogs($"[Copying] {FolderName} [modded] to Pak/Mods");
                            CopyFilesRecursively(OriginalLocation + FolderName, DestinationFolder + FolderName);
                            Form1.CurrentForm.AddModManLogs($"[Copied] {FolderName} [modded] to Pak/Mods");
                        }
                        else
                            Form1.CurrentForm.AddModManLogs($"[Linked] {FolderName} [modded] to Pak/Mods");
                        num++;
                        bw_ProgressChangedV2();
                    }
                    else
                        Form1.CurrentForm.AddModManLogs($"[Info] {FolderName} already installed!");
                }
                catch
                {
                    Form1.CurrentForm.AddModManLogs($"[Error] {FolderName} could not be touched!");
                }
                if (num > 0)
                    Form1.CurrentForm.AddModManLogs($"[Log] Done! {num} directory were linked.");
                else
                    Form1.CurrentForm.AddModManLogs("[Log] Done! No directory were linked.");
            }
            else
                Form1.CurrentForm.AddModManLogs("[Notice] Can't install an empty mod folder");
        }

        private bool CheckIfParent(string path)
        {
            if (Path.GetDirectoryName(path) != path)
                return true;
            else
                return false;
        }

        // Uninstall Mod by Folder
        private void Uninstall(string FolderName)
        {
            string PakModsFolder = Form1.CurrentForm.PakPaths.ModsPath + FolderName;
            Control.CheckForIllegalCrossThreadCalls = false;
            Form1.CurrentForm.AddModManLogs("");
            try
            {
                if (Directory.Exists(PakModsFolder))
                {
                    Form1.CurrentForm.AddModManLogs($"[Removing] {FolderName} [modded] from Pak/Mods");
                    Directory.Delete(PakModsFolder, true);
                    if (Directory.GetParent(PakModsFolder).GetFiles("*.*", SearchOption.AllDirectories).Length == 0)
                        Directory.GetParent(PakModsFolder).Delete();
                    Form1.CurrentForm.AddModManLogs($"[Removed] {FolderName} [modded] from Pak/Mods");
                }
            }
            catch
            {
                Form1.CurrentForm.AddModManLogs($"[Error] {FolderName} could not be touched!");
            }
        }
    }
}
