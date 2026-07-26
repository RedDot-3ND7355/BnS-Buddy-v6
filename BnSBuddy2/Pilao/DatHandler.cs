using BnSBuddy2.Functions;
using bnstool;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;
using static bnscompression;

namespace BnSBuddy2.Pilao
{
    public class DatHandler
    {
        // Globals
        public configurator _configurator = new configurator();
        public TreeNode TouchedNode = null;
        string DatPath = "";
        // End Globals

        public void BrowseToButton()
        {
            if (TouchedNode.Text.EndsWith(".dat") && Directory.Exists((Path.GetTempPath() + Path.GetFileName(DatPath))))
                Process.Start("explorer.exe", Path.GetTempPath() + Path.GetFileName(DatPath));
            else if (!Directory.Exists((Path.GetTempPath() + Path.GetFileName(DatPath))))
                Prompt.Popup($"Please extract {TouchedNode.Text}!");
            else if (TouchedNode.Text.EndsWith(".bin.files") && Directory.Exists(Path.GetTempPath() + Path.GetFileName(DatPath) + "\\" + TouchedNode.Text))
                Process.Start("explorer.exe", Path.GetTempPath() + Path.GetFileName(DatPath) + "\\" + TouchedNode.Text);
            else if (!Directory.Exists(Path.GetTempPath() + Path.GetFileName(DatPath) + "\\" + TouchedNode.Text))
                Prompt.Popup($"Please extract {TouchedNode.Text}!");
        }

        public void FileUnpackButton()
        {
            if (TouchedNode.Text.EndsWith(".dat"))
                ExtractDat(PAK.PakHandler.FileList[TouchedNode.Text]);
            else if (TouchedNode.Text.EndsWith(".bin"))
                LegacyBin.BinHandler.DumpBin(Path.GetTempPath() + Path.GetFileName(DatPath) + "\\" + TouchedNode.Text, Form1.CurrentForm.toolStripComboBox1.SelectedItem.ToString() != "Hex", TouchedNode);
        }

        public void FileRepackButton()
        {
            if (TouchedNode.Text.EndsWith(".dat"))
                RepackDat(Path.GetTempPath() + TouchedNode.Text, TouchedNode.Text);
            else if (TouchedNode.Text.EndsWith(".bin.files"))
                LegacyBin.BinHandler.PackBin(Path.GetTempPath() + Path.GetFileName(DatPath) + "\\" + TouchedNode.Text, Form1.CurrentForm.toolStripComboBox1.SelectedItem.ToString() != "Hex", TouchedNode);
        }

        /// <summary>Open the in-app bin editor for the selected .bin (or bare editor if none).</summary>
        public void OpenBinEditorButton()
        {
            if (TouchedNode != null && TouchedNode.Text.EndsWith(".bin") && !TouchedNode.Text.EndsWith(".bin.files"))
            {
                string binPath = Path.GetTempPath() + Path.GetFileName(DatPath) + "\\" + TouchedNode.Text;
                if (File.Exists(binPath))
                {
                    LegacyBin.BinHandler.OpenBinEditor(binPath);
                    return;
                }
            }
            LegacyBin.BinHandler.OpenBinEditor();
        }

        public void LicenseContextMenuPop(TreeNodeMouseClickEventArgs e)
        {
            if (Form1.CurrentForm.treeView1.SelectedNode != null)
            {
                if ((e.Node.Text.EndsWith(".dat") || (e.Node.Text.EndsWith(".bin") || e.Node.Text.EndsWith(".bin.files"))) && e.Button == MouseButtons.Right)
                {
                    if (Form1.CurrentForm.IniLicense.Validated)
                    {
                        DatPath = PAK.PakHandler.FileList[Form1.CurrentForm.treeView1.Nodes[0].Text];
                        TouchedNode = e.Node;
                        if (TouchedNode.Text.EndsWith(".bin") || TouchedNode.Text.EndsWith(".bin.files"))
                        {
                            Form1.CurrentForm.toolStripMenuItem5.Visible = false;
                            Form1.CurrentForm.toolStripSeparator1.Visible = false;
                            Form1.CurrentForm.toolStripSeparator3.Visible = true;
                            Form1.CurrentForm.toolStripComboBox1.Visible = true;
                            Form1.CurrentForm.toolStripMenuItem7.Visible = true;
                            Form1.CurrentForm.toolStripMenuItem8.Visible = TouchedNode.Text.EndsWith(".bin") && !TouchedNode.Text.EndsWith(".bin.files");
                        }
                        else
                        {
                            Form1.CurrentForm.toolStripMenuItem5.Visible = true;
                            Form1.CurrentForm.toolStripSeparator1.Visible = true;
                            Form1.CurrentForm.toolStripSeparator3.Visible = false;
                            Form1.CurrentForm.toolStripComboBox1.Visible = false;
                            Form1.CurrentForm.toolStripMenuItem7.Visible = false;
                            Form1.CurrentForm.toolStripMenuItem8.Visible = false;
                        }
                        Form1.CurrentForm.LicenseContextMenu.Show(Form1.CurrentForm.treeView1, e.X, e.Y);
                    }
                    else
                    {
                        Prompt.Popup("License required for this feature.");
                    }
                }
            }
        }

        public void KeyInfo()
        {
            DetectSettings(DatPath);
            Prompt.Popup($"Dat File: {DatPath}" + Environment.NewLine +
               $"Use 64Bit? {_configurator.Use64Bit.Checked}" + Environment.NewLine +
               $"Use Compression? {_configurator.UseCompression.Checked} | Level: {_configurator.Compression.Value}" + Environment.NewLine +
               $"Use Encryption? {_configurator.UseEncryption.Checked} | Key: {_configurator.Encryption.SelectedItem.ToString()}" + Environment.NewLine +
               $"Use Signature? {_configurator.UseSignature.Checked} | Signature: {_configurator.Signature.SelectedItem.ToString()}" + Environment.NewLine +
               $"XML? {_configurator.BXML.Checked} | BXMLv3? {_configurator.BXMLv3.Checked} | BXMLv4? {_configurator.BXMLv4.Checked}");
        }

        // Extract new Dat
        public void ExtractDat(string filepath)
        {
            try
            {
                string tmp_pth = DatPath;
                if (DatPath == "")
                    tmp_pth = filepath;
                DetectSettings(tmp_pth);
                /*
                bnscompression.ParseResult _result = bnscompression.GetCreateParams2(filepath, out var _, out var _, out var encryptionKey, out var encryptionKeySize, out var _, out var _, out var _);
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
                */
                var array = ((AESKeyObject)_configurator.Encryption.SelectedItem).Key;
                var keysize = ((AESKeyObject)_configurator.Encryption.SelectedItem).Key.Length;
                int count = 0;
                bnscompression.DelegateResult result = bnscompression.DelegateResult.Continue;
                bool? reuseResult = null;
                double value = bnscompression.ExtractToDirectory(filepath, Path.GetTempPath() + Path.GetFileName(filepath), array, (uint)keysize, true, delegate (string entryName, ulong entrySize)
               {
                   DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetTempPath() + Path.GetFileName(filepath));
                   FileInfo fileInfo = new FileInfo(Path.GetTempPath() + Path.GetFileName(filepath) + "\\" + entryName);
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
               });
                if (result != bnscompression.DelegateResult.Cancel)
                {
                    // report progress here once completed
                    Form1.CurrentForm.SortOutputHandler($"Extracted in {TimeSpan.FromSeconds(value).Seconds} Seconds.");
                    Form1.CurrentForm.waitbw.Set();
                }
            }
            catch (Exception eee)
            {
                Prompt.Popup(eee.ToString());
            }
        }

        // Repack extracted Dat
        public void RepackDat(string folderpath, string datname)
        {
            try
            {
                DetectSettings(PAK.PakHandler.FileList[datname]);
                AESKeyObject aESKeyObject = (_configurator.UseEncryption.Checked ? ((AESKeyObject)_configurator.Encryption.SelectedItem) : null);
                RSAKeyObject rSAKeyObject = (_configurator.UseSignature.Checked ? ((RSAKeyObject)_configurator.Signature.SelectedItem) : null);
                bnscompression.BinaryXmlVersion binaryXmlVersion = bnscompression.BinaryXmlVersion.None;
                if (_configurator.BXMLv3.Checked)
                {
                    binaryXmlVersion = bnscompression.BinaryXmlVersion.Version3;
                }
                else if (_configurator.BXMLv4.Checked)
                {
                    binaryXmlVersion = bnscompression.BinaryXmlVersion.Version4;
                }
                int count = 0;
                double value = bnscompression.CreateFromDirectory(folderpath, PAK.PakHandler.FileList[datname], _configurator.Use64Bit.Checked, (bnscompression.CompressionLevel)_configurator.Compression.Value, aESKeyObject?.Key, (aESKeyObject != null) ? ((uint)aESKeyObject.Key.Length) : 0u, rSAKeyObject?.KeyBlob, (rSAKeyObject != null) ? ((uint)rSAKeyObject.KeyBlob.Length) : 0u, binaryXmlVersion, delegate
                {
                    count++;
                    return bnscompression.DelegateResult.Continue;
                });
                // report progress here once completed
                Form1.CurrentForm.SortOutputHandler($"Repacked in {TimeSpan.FromSeconds(value).Seconds} Seconds.");
                Form1.CurrentForm.waitbw.Set();
            }
            catch (Exception eee)
            {
                Prompt.Popup(eee.ToString());
            }
        }

        // Detect Dat Settings
        public void DetectSettings(string filepath)
        {
            bnscompression.ParseResult reso = bnscompression.GetCreateParams(filepath, _configurator.Encryption.Items.Cast<AESKeyObject>().Select(obj => obj.Key).ToArray(), _configurator.Signature.Items.Cast<RSAKeyObject>().Select(obj => obj.KeyBlob).ToArray(), out bool use64bit, out CompressionLevel compressionLevel, out uint encKeyIndex, out uint signKeyIndex, out BinaryXmlVersion _binaryXmlVersion);
            switch (reso)
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
            _configurator.Use64Bit.Checked = use64bit;
            if (_configurator.UseCompression.Checked = compressionLevel != bnscompression.CompressionLevel.None)
            {
                _configurator.Compression.Value = (int)((float)compressionLevel / 3f * (float)(_configurator.Compression.Maximum - _configurator.Compression.Minimum));
            }
            if (_configurator.UseEncryption.Checked = (int)encKeyIndex > -1 && (int)encKeyIndex <= _configurator.Encryption.Items.Count)
            {
                _configurator.Encryption.SelectedIndex = (int)encKeyIndex;
            }
            if (_configurator.UseSignature.Checked = (int)signKeyIndex > -1 && (int)signKeyIndex <= _configurator.Signature.Items.Count)
            {
                _configurator.Signature.SelectedIndex = (int)signKeyIndex;
            }
            switch (_binaryXmlVersion)
            {
                case bnscompression.BinaryXmlVersion.None:
                    _configurator.BXML.Checked = true;
                    _configurator.BXMLv3.Checked = false;
                    _configurator.BXMLv4.Checked = false;
                    break;
                case bnscompression.BinaryXmlVersion.Version3:
                    _configurator.BXML.Checked = false;
                    _configurator.BXMLv3.Checked = true;
                    _configurator.BXMLv4.Checked = false;
                    break;
                case bnscompression.BinaryXmlVersion.Version4:
                    _configurator.BXML.Checked = false;
                    _configurator.BXMLv3.Checked = false;
                    _configurator.BXMLv4.Checked = true;
                    break;
            }
            // OLD DLL CODE
            /*
            bnscompression.ParseResult _result = bnscompression.GetCreateParams(filepath, out var use64Bit, out var compressionLevel, out var encryptionKey2, out var encryptionKeySize, out var privateKeyBlob, out var privateKeyBlobSize, out var binaryXmlVersion);
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
                    //MessageBox.Show("Unsupported RSA private key or invalid signature."); // debug
                    break;
            }
            _configurator.Use64Bit.Checked = use64Bit;
            if (_configurator.UseCompression.Checked = compressionLevel != bnscompression.CompressionLevel.None)
            {
                int num = _configurator.Compression.Maximum - _configurator.Compression.Minimum;
                _configurator.Compression.Value = (int)((float)compressionLevel / 3f * (float)num);
            }
            if (_configurator.UseEncryption.Checked = encryptionKey2 != IntPtr.Zero)
            {
                byte[] encryptionKey = new byte[encryptionKeySize];
                Marshal.Copy(encryptionKey2, encryptionKey, 0, encryptionKey.Length);
                AESKeyObject aESKeyObject = (from AESKeyObject x in _configurator.Encryption.Items
                                             where x.Key.Length == encryptionKey.Length && x.Key.SequenceEqual(encryptionKey)
                                             select x).FirstOrDefault();
                if (aESKeyObject == null)
                {
                    aESKeyObject = new AESKeyObject(encryptionKey);
                    _configurator.Encryption.Items.Add(aESKeyObject);
                }
                _configurator.Encryption.SelectedItem = aESKeyObject;
            }
            if (_configurator.UseSignature.Checked = privateKeyBlob != IntPtr.Zero && privateKeyBlobSize != 0)
            {
                byte[] keyBlob = new byte[privateKeyBlobSize];
                Marshal.Copy(privateKeyBlob, keyBlob, 0, keyBlob.Length);
                RSAKeyObject rSAKeyObject = (from RSAKeyObject x in _configurator.Signature.Items
                                             where x.KeyBlob.Length == keyBlob.Length && x.KeyBlob.SequenceEqual(keyBlob)
                                             select x).FirstOrDefault();
                if (rSAKeyObject == null)
                {
                    rSAKeyObject = new RSAKeyObject(keyBlob);
                    _configurator.Signature.Items.Add(rSAKeyObject);
                }
                _configurator.Signature.SelectedItem = rSAKeyObject;
            }
            switch (binaryXmlVersion)
            {
                case bnscompression.BinaryXmlVersion.None:
                    _configurator.BXML.Checked = true;
                    _configurator.BXMLv3.Checked = false;
                    _configurator.BXMLv4.Checked = false;
                    break;
                case bnscompression.BinaryXmlVersion.Version3:
                    _configurator.BXML.Checked = false;
                    _configurator.BXMLv3.Checked = true;
                    _configurator.BXMLv4.Checked = false;
                    break;
                case bnscompression.BinaryXmlVersion.Version4:
                    _configurator.BXML.Checked = false;
                    _configurator.BXMLv3.Checked = false;
                    _configurator.BXMLv4.Checked = true;
                    break;
            }
            */
        }

        // Other needed stuff
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
    }
}
