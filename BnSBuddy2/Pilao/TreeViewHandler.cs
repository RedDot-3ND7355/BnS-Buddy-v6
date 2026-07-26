using BnSBuddy2.Functions;
using bnstool;
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

        public static void ReadSelectedFile(TreeNode node, object DatFile)
        {
            if (DatFile == null || node == null)
                return;
            SelectedDat = PAK.PakHandler.FileList[DatFile.ToString()];
            if ((node.Text.EndsWith(".xml") || node.Text.EndsWith(".txt") || node.Text.EndsWith(".x16") || node.Text.EndsWith(".copy")) && !node.Parent.Text.EndsWith(".bin") && !node.Text.EndsWith(".dat"))
            {
                Form1.CurrentForm.SortOutputHandler($"Reading {node.Text.Substring(node.Text.LastIndexOf('.') + 1)}...");
                Form1.CurrentForm.fastColoredTextBox1.Text = GetFileText(node.FullPath.Replace(Path.GetFileName(SelectedDat) + "\\", ""));
                Form1.CurrentForm.SortOutputHandler("Read.");
            }
            else if (node.Parent != null && node.Parent.Text.EndsWith(".bin"))
            {
                Form1.CurrentForm.SortOutputHandler("Reading xml...");
                Form1.CurrentForm.fastColoredTextBox1.Text = ReadXmlFile(node.FullPath);
                Form1.CurrentForm.SortOutputHandler("Read.");
            }
        }

        private static string ReadXmlFile(string path)
        {
            return File.ReadAllText(Path.GetTempPath() + path, Encoding.UTF8);
        }

        public static string ReadStringFromFile(string filename, string DatFile)
        {
            return ReturnFileText(filename, DatFile);
        }

        public static string ReadStringFromFileZ(string pathtofile, string pathtodat)
        {
            string tmp = Path.GetTempPath() + Path.GetFileName(pathtodat) + "\\" + pathtofile;
            if (pathtofile.EndsWith(".xml") || pathtofile.EndsWith("x16"))
                return File.ReadAllText(Path.GetTempPath() + Path.GetFileName(pathtodat) + "\\" + pathtofile, Encoding.Unicode);
            else
                return File.ReadAllText(Path.GetTempPath() + Path.GetFileName(pathtodat) + "\\" + pathtofile);
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
            /*
            bnscompression.ParseResult _result = bnscompression.GetCreateParams2(pathtodat, out var _, out var _, out var encryptionKey, out var encryptionKeySize, out var _, out var _, out var _);
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
            */
            var array = ((AESKeyObject)FastColoredHandler.DatHandler._configurator.Encryption.SelectedItem).Key;
            var keysize = ((AESKeyObject)FastColoredHandler.DatHandler._configurator.Encryption.SelectedItem).Key.Length;
            int count = 0;
            bnscompression.DelegateResult result = bnscompression.DelegateResult.Continue;
            bool? reuseResult = null;
            string filetext = "";
            double value = bnscompression.ExtractToDirectory(pathtodat, Path.GetTempPath() + Path.GetFileName(pathtodat), array, (uint)keysize, true, delegate (string entryName, ulong entrySize)
            {
                if (entryName == pathtofile)
                {
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
            /*
            bnscompression.ParseResult _result = bnscompression.GetCreateParams2(SelectedDat, out var _, out var _, out var encryptionKey, out var encryptionKeySize, out var _, out var _, out var _);
            switch (_result)
            {
                case bnscompression.ParseResult.InvalidIdentifier:
                    //MessageBox.Show("Invalid identifier.");
                    break;
                case bnscompression.ParseResult.UnsupportedVersion:
                    Prompt.Popup("Unsupported version.");
                    break;
                case bnscompression.ParseResult.InvalidCentralDirectoryHeader:
                    //Prompt.Popup("Invalid central directory header.");
                    break;
                case bnscompression.ParseResult.UnsupportedAesEncryptionKey:
                    Prompt.Popup("Unknown AES encryption key.");
                    break;
                case bnscompression.ParseResult.UnsupportedRsaPrivateKeyOrInvalidSignature:
                    //Prompt.Popup("Unsupported RSA private key or invalid signature.");
                    break;
            }
            byte[] array = new byte[16];
            Marshal.Copy(encryptionKey, array, 0, array.Length);
            */
            var array = ((AESKeyObject)FastColoredHandler.DatHandler._configurator.Encryption.SelectedItem).Key;
            var keysize = ((AESKeyObject)FastColoredHandler.DatHandler._configurator.Encryption.SelectedItem).Key.Length;
            int count = 0;
            bnscompression.DelegateResult result = bnscompression.DelegateResult.Continue;
            bool? reuseResult = null;
            string filetext = "";
            double value = bnscompression.ExtractToDirectory(SelectedDat, Path.GetTempPath() + "\\" + Form1.CurrentForm.treeView1.Nodes[0].Text, array, (uint)keysize, true, delegate (string entryName, ulong entrySize)
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
