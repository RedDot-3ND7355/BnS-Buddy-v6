using BnSBuddy2.Functions;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace BnSBuddy2.Pilao
{
    public static class TreeViewHandler
    {
        // Globals
        static string SelectedDat = "";
        // End Globals

        public static void ReadSelectedFile(TreeNode node, string DatFile)
        {
            Form1.CurrentForm.SortOutputHandler("Reading xml...");
            SelectedDat = PAK.PakHandler.FileList[DatFile];
            if ((node.Name.EndsWith(".xml") || node.Name.EndsWith(".txt") || node.Name.EndsWith(".x16") || node.Name.EndsWith(".copy")) && !node.Parent.Name.EndsWith(".bin"))
                Form1.CurrentForm.fastColoredTextBox1.Text = GetFileText(node.FullPath.Replace(Path.GetFileName(SelectedDat) + "\\", ""));
            Form1.CurrentForm.SortOutputHandler("Read.");
        }

        public static string ReadStringFromFile(string filename, string DatFile)
        {
            return ReturnFileText(filename, DatFile);
        }

        [DllImport("Shlwapi.dll", CharSet = CharSet.Unicode)]
        private unsafe static extern char* StrFormatByteSize(long qdw, char* pszBuf, uint cchBuf);

        private unsafe static string FormatByteSize(long size)
        {
            char[] array = new char[32];
            fixed (char* ptr = array)
            {
                if (StrFormatByteSize(size, ptr, (uint)array.Length) != null)
                {
                    return new string(ptr);
                }
            }
            return string.Empty;
        }

        private static string ReturnFileText(string pathtofile, string pathtodat)
        {
            bnscompression.ParseResult _result = bnscompression.GetCreateParams(pathtodat, out var _, out var _, out var encryptionKey, out var encryptionKeySize, out var _, out var _, out var _);
            switch (_result)
            {
                case bnscompression.ParseResult.InvalidIdentifier:
                    //MessageBox.Show("Invalid identifier.");
                    break;
                case bnscompression.ParseResult.UnsupportedVersion:
                    Prompt.Popup("Unsupported version.");
                    break;
                case bnscompression.ParseResult.InvalidCentralDirectoryHeader:
                    Prompt.Popup("Invalid central directory header.");
                    break;
                case bnscompression.ParseResult.UnsupportedAesEncryptionKey:
                    Prompt.Popup("Unknown AES encryption key.");
                    break;
                case bnscompression.ParseResult.UnsupportedRsaPrivateKeyOrInvalidSignature:
                    Prompt.Popup("Unsupported RSA private key or invalid signature.");
                    break;
            }
            byte[] array = new byte[16];
            Marshal.Copy(encryptionKey, array, 0, array.Length);
            int count = 0;
            bnscompression.DelegateResult result = bnscompression.DelegateResult.Continue;
            bool? reuseResult = null;
            string filetext = "";
            double value = bnscompression.ExtractToDirectory(pathtodat, Path.GetTempPath() + Path.GetFileName(pathtodat), array, encryptionKeySize, delegate (string entryName, ulong entrySize)
            {
                if (entryName == pathtofile)
                {
                    //DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetTempPath() + "\\" + Form1.CurrentForm.treeView1.Nodes[0].Text);
                    FileInfo fileInfo = new FileInfo(Path.GetTempPath() + Path.GetFileName(pathtodat) + "\\" + entryName);
                    if (fileInfo.Exists && !reuseResult.GetValueOrDefault(false))
                    {
                        string text = FormatByteSize((long)entrySize);
                        bool flag = !Path.GetExtension(entryName).Equals(".xml") || !Path.GetExtension(entryName).Equals(".x16", StringComparison.OrdinalIgnoreCase);
                    }
                    if (result == bnscompression.DelegateResult.Continue)
                    {
                        count++;
                    }
                    return result;
                }
                else
                    return bnscompression.DelegateResult.Skip;
            });
            if (result != bnscompression.DelegateResult.Cancel)
            {
                if (pathtofile.EndsWith(".xml") || pathtofile.EndsWith("x16"))
                    filetext = File.ReadAllText(Path.GetTempPath() + Path.GetFileName(pathtodat) + "\\" + pathtofile, Encoding.Unicode);
                else
                    filetext = File.ReadAllText(Path.GetTempPath() + Path.GetFileName(pathtodat) + "\\" + pathtofile);
            }
            return filetext;
        }

        private static string GetFileText(string pathtofile)
        {
            bnscompression.ParseResult _result = bnscompression.GetCreateParams(SelectedDat, out var _, out var _, out var encryptionKey, out var encryptionKeySize, out var _, out var _, out var _);
            switch (_result)
            {
                case bnscompression.ParseResult.InvalidIdentifier:
                    //MessageBox.Show("Invalid identifier.");
                    break;
                case bnscompression.ParseResult.UnsupportedVersion:
                    Prompt.Popup("Unsupported version.");
                    break;
                case bnscompression.ParseResult.InvalidCentralDirectoryHeader:
                    Prompt.Popup("Invalid central directory header.");
                    break;
                case bnscompression.ParseResult.UnsupportedAesEncryptionKey:
                    Prompt.Popup("Unknown AES encryption key.");
                    break;
                case bnscompression.ParseResult.UnsupportedRsaPrivateKeyOrInvalidSignature:
                    Prompt.Popup("Unsupported RSA private key or invalid signature.");
                    break;
            }
            byte[] array = new byte[16];
            Marshal.Copy(encryptionKey, array, 0, array.Length);
            int count = 0;
            bnscompression.DelegateResult result = bnscompression.DelegateResult.Continue;
            bool? reuseResult = null;
            string filetext = "";
            double value = bnscompression.ExtractToDirectory(SelectedDat, Path.GetTempPath() + "\\" + Form1.CurrentForm.treeView1.Nodes[0].Text, array, encryptionKeySize, delegate (string entryName, ulong entrySize)
            {
                if (entryName == pathtofile)
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetTempPath() + "\\" + Form1.CurrentForm.treeView1.Nodes[0].Text);
                    FileInfo fileInfo = new FileInfo(Path.GetTempPath() + "\\" + Form1.CurrentForm.treeView1.Nodes[0].Text + "\\" + entryName);
                    if (fileInfo.Exists && !reuseResult.GetValueOrDefault(false))
                    {
                        string text = FormatByteSize((long)entrySize);
                        bool flag = !Path.GetExtension(entryName).Equals(".xml") || !Path.GetExtension(entryName).Equals(".x16", StringComparison.OrdinalIgnoreCase);
                    }
                    if (result == bnscompression.DelegateResult.Continue)
                    {
                        count++;
                    }
                    return result;
                }
                else
                    return bnscompression.DelegateResult.Skip;
            });
            if (result != bnscompression.DelegateResult.Cancel)
            {
                if (pathtofile.EndsWith(".xml") || pathtofile.EndsWith("x16"))
                    filetext = File.ReadAllText(Path.GetTempPath() + "\\" + Form1.CurrentForm.treeView1.Nodes[0].Text + "\\" + pathtofile, FastColoredTextBoxNS.EncodingDetector.DetectTextFileEncoding(Path.GetTempPath() + "\\" + Form1.CurrentForm.treeView1.Nodes[0].Text + "\\" + pathtofile));
                else
                    filetext = File.ReadAllText(Path.GetTempPath() + "\\" + Form1.CurrentForm.treeView1.Nodes[0].Text + "\\" + pathtofile);
            }
            return filetext;
        }
    }
}
