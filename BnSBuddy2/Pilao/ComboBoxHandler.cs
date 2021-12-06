using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BnSBuddy2.Pilao
{
    public static class ComboBoxHandler
    {
        // Globals
        static string TempPath = Path.GetTempPath();
        // End Globals

        private static TreeNode PopulateTreeNode(List<string> paths)
        {
            TreeNode thisnode = new TreeNode();
            TreeNode currentnode = null;
            char[] cachedpathseparator = "\\".ToCharArray();
            for (int i = 0; i < paths.Count; i++)
            {
                currentnode = thisnode;
                foreach (string subPath in paths[i].Split(cachedpathseparator))
                {
                    if (null == currentnode.Nodes[subPath])
                        currentnode = currentnode.Nodes.Add(subPath, subPath);
                    else
                        currentnode = currentnode.Nodes[subPath];
                }
            }
            return thisnode;
        }

        public static string[] GetFileList(string datfile)
        {
            string filepath = PAK.PakHandler.FileList[datfile];
            bnscompression.ParseResult _result = bnscompression.GetCreateParams(filepath, out var _, out var _, out var encryptionKey, out var encryptionKeySize, out var _, out var _, out var _);
            switch (_result)
            {
                case bnscompression.ParseResult.InvalidIdentifier:
                    //MessageBox.Show("Invalid identifier.");
                    break;
                case bnscompression.ParseResult.UnsupportedVersion:
                    //MessageBox.Show("Unsupported version.");
                    break;
                case bnscompression.ParseResult.InvalidCentralDirectoryHeader:
                    //MessageBox.Show("Invalid central directory header.");
                    break;
                case bnscompression.ParseResult.UnsupportedAesEncryptionKey:
                    //MessageBox.Show("Unknown AES encryption key.");
                    break;
                case bnscompression.ParseResult.UnsupportedRsaPrivateKeyOrInvalidSignature:
                    //MessageBox.Show("Unsupported RSA private key or invalid signature.");
                    break;
            }
            byte[] array = new byte[16];
            Marshal.Copy(encryptionKey, array, 0, array.Length);
            int count = 0;
            bnscompression.DelegateResult result = bnscompression.DelegateResult.Continue;
            List<string> paths = new List<string>();
            double value = bnscompression.ExtractToDirectory(filepath, TempPath + Path.GetFileName(filepath), array, encryptionKeySize, delegate (string entryName, ulong entrySize)
            {
                paths.Add(entryName);
                if (result == bnscompression.DelegateResult.Continue)
                {
                    count++;
                }
                return result;
            });
            if (result != bnscompression.DelegateResult.Cancel)
            {
                return paths.ToArray();
            }
            return paths.ToArray();
        }

        public static void PopulateTree(string filename)
        {
            Form1.CurrentForm.SortOutputHandler("Loading Dat...");
            Form1.CurrentForm.treeView1.Nodes.Clear();
            string filepath = PAK.PakHandler.FileList[filename];
            bnscompression.ParseResult _result = bnscompression.GetCreateParams(filepath, out var _, out var _, out var encryptionKey, out var encryptionKeySize, out var _, out var _, out var _);
            switch (_result)
            {
                case bnscompression.ParseResult.InvalidIdentifier:
                    //MessageBox.Show("Invalid identifier.");
                    break;
                case bnscompression.ParseResult.UnsupportedVersion:
                    //MessageBox.Show("Unsupported version.");
                    break;
                case bnscompression.ParseResult.InvalidCentralDirectoryHeader:
                    //MessageBox.Show("Invalid central directory header.");
                    break;
                case bnscompression.ParseResult.UnsupportedAesEncryptionKey:
                    //MessageBox.Show("Unknown AES encryption key.");
                    break;
                case bnscompression.ParseResult.UnsupportedRsaPrivateKeyOrInvalidSignature:
                    //MessageBox.Show("Unsupported RSA private key or invalid signature.");
                    break;
            }
            byte[] array = new byte[16];
            Marshal.Copy(encryptionKey, array, 0, array.Length);
            int count = 0;
            bnscompression.DelegateResult result = bnscompression.DelegateResult.Continue;
            List<string> paths = new List<string>();
            double value = bnscompression.ExtractToDirectory(filepath, TempPath + Path.GetFileName(filepath), array, encryptionKeySize, delegate (string entryName, ulong entrySize)
            {
                paths.Add(entryName);
                if (result == bnscompression.DelegateResult.Continue)
                {
                    count++;
                }
                return result;
            });
            if (result != bnscompression.DelegateResult.Cancel)
            {
                // report progress here
                Form1.CurrentForm.treeView1.Nodes.Add(PopulateTreeNode(paths));
                Form1.CurrentForm.treeView1.Nodes[0].Text = Path.GetFileName(Path.GetFileName(filepath));
                Form1.CurrentForm.treeView1.Nodes[0].Expand();
                Form1.CurrentForm.SortOutputHandler("Loaded.");
            }
        }
    }
}
