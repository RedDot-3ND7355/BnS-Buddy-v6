using BnSBuddy2.Functions;
using bnstool;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BnSBuddy2.Pilao
{
    public class DatHandler
    {
        public configurator _configurator = new configurator();

        public void LicenseContextMenuPop(TreeNodeMouseClickEventArgs e)
        {
            if (Form1.CurrentForm.treeView1.SelectedNode != null)
            {
                if ((e.Node.Text.EndsWith(".dat") || e.Node.Text.EndsWith(".bin")) && e.Button == MouseButtons.Right)
                {
                    if (Form1.CurrentForm.IniLicense.Validated)
                    { /*
                        string tmp = metroComboBox3.Items[metroComboBox3.SelectedIndex].ToString();
                        string file = myDictionary[tmp];
                        PathOnly = file;
                        TouchedNode = e.Node.Text;
                        if (TouchedNode.EndsWith(".bin"))
                        {
                            ParentNode = e.Node.Index;
                            if (!ParentNodeIndex.ContainsKey(SelItem))
                                ParentNodeIndex.Add(SelItem, ParentNode);
                            toolStripComboBox1.Visible = true;
                            toolStripMenuItem1.Text = "Unpack (via memory)";
                            toolStripMenuItem2.Visible = false;
                            toolStripMenuItem3.Visible = true;
                            toolStripMenuItem5.Visible = true;
                            toolStripMenuItem6.Visible = true;
                            toolStripMenuItem7.Visible = true;
                            toolStripSeparator5.Visible = true;
                            toolStripSeparator6.Visible = true;
                            toolStripSeparator7.Visible = true;
                            toolStripSeparator8.Visible = false;
                            toolStripMenuItem8.Visible = false;
                        }
                        else
                        {
                            toolStripMenuItem1.Text = "Unpack";
                            toolStripMenuItem2.Text = "Repack";
                            toolStripMenuItem2.Visible = true;
                            toolStripComboBox1.Visible = false;
                            toolStripMenuItem3.Visible = false;
                            toolStripMenuItem5.Visible = false;
                            toolStripMenuItem6.Visible = false;
                            toolStripMenuItem7.Visible = false;
                            toolStripSeparator5.Visible = false;
                            toolStripSeparator6.Visible = false;
                            toolStripSeparator7.Visible = false;
                            toolStripSeparator8.Visible = true;
                            toolStripMenuItem8.Visible = true;
                        }
                        Form1.CurrentForm.LicenseContextMenu.Show(Form1.CurrentForm.treeView1, e.X, e.Y);
                        */
                    }
                    else
                    {
                        Prompt.Popup("License required for this feature.");
                    }
                }
            }
        }

        public void KeyInfo(string filepath)
        {
            DetectSettings(filepath);
            Prompt.Popup($"Dat File: {filepath}" + Environment.NewLine +
               $"Use 64Bit? {_configurator.Use64Bit.Checked}" + Environment.NewLine +
               $"Use Compression? {_configurator.UseCompression.Checked} | Level: {_configurator.Compression.Value}" + Environment.NewLine +
               $"Use Encryption? {_configurator.UseEncryption.Checked} | Key: {_configurator.Encryption.SelectedItem.ToString()}" + Environment.NewLine +
               $"Use Signature? {_configurator.UseSignature.Checked} | Xor: {_configurator.Signature.SelectedItem.ToString()}" + Environment.NewLine +
               $"XML? {_configurator.BXML.Checked} | BXMLv3? {_configurator.BXMLv3.Checked} | BXMLv4? {_configurator.BXMLv4.Checked}");
        }

        // Extract new Dat
        public void ExtractDat(string filepath)
        {
            DetectSettings(filepath);
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
            bool? reuseResult = null;
            double value = bnscompression.ExtractToDirectory(filepath, filepath + ".files", array, encryptionKeySize, delegate (string entryName, ulong entrySize)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(filepath + ".files");
                FileInfo fileInfo = new FileInfo(Path.Combine(filepath + ".files", entryName));
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

        // Repack extracted Dat
        public void RepackDat(string folderpath, string datname)
        {
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

        // Detect Dat Settings
        public void DetectSettings(string filepath)
        {
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
                    //MessageBox.Show("Unsupported RSA private key or invalid signature.");
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
