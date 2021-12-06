using BnSBuddy2.Functions;
using bnstool;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace BnSBuddy2.Pilao
{
    public static class FastColoredHandler
    {
        // Globals
        public static DatHandler DatHandler = new DatHandler();
        // End Globals

        public static void SaveFile()
        {
            Form1.CurrentForm.SortOutputHandler("Saving...");
            string SelectedDat = PAK.PakHandler.FileList[Form1.CurrentForm.materialComboBox5.SelectedItem.ToString()];
            DatHandler.DetectSettings(SelectedDat);
            string filename = Form1.CurrentForm.treeView1.SelectedNode.FullPath.Replace(Path.GetFileName(SelectedDat) + "\\", "");
            string filepath = Path.GetTempPath() + "\\" + Form1.CurrentForm.treeView1.Nodes[0].Text + "\\" + filename;
            string text = Form1.CurrentForm.fastColoredTextBox1.Text;
            string inputfolder = Path.GetDirectoryName(filepath);
            if (filename.EndsWith(".xml"))
                File.WriteAllText(filepath, text, Encoding.Unicode);
            else
                File.WriteAllText(filepath, text);
            //
            AESKeyObject aESKeyObject = (DatHandler._configurator.UseEncryption.Checked ? ((AESKeyObject)DatHandler._configurator.Encryption.SelectedItem) : null);
            RSAKeyObject rSAKeyObject = (DatHandler._configurator.UseSignature.Checked ? ((RSAKeyObject)DatHandler._configurator.Signature.SelectedItem) : null);
            bnscompression.BinaryXmlVersion binaryXmlVersion = bnscompression.BinaryXmlVersion.None;
            if (DatHandler._configurator.BXMLv3.Checked)
            {
                binaryXmlVersion = bnscompression.BinaryXmlVersion.Version3;
            }
            else if (DatHandler._configurator.BXMLv4.Checked)
            {
                binaryXmlVersion = bnscompression.BinaryXmlVersion.Version4;
            }
            double value = bnscompression.CreateFromDirectory(inputfolder, SelectedDat, DatHandler._configurator.Use64Bit.Checked, (bnscompression.CompressionLevel)DatHandler._configurator.Compression.Value, aESKeyObject?.Key, (aESKeyObject != null) ? ((uint)aESKeyObject.Key.Length) : 0u, rSAKeyObject?.KeyBlob, (rSAKeyObject != null) ? ((uint)rSAKeyObject.KeyBlob.Length) : 0u, binaryXmlVersion, delegate
            {
                return bnscompression.DelegateResult.Continue;
            });
            Form1.CurrentForm.SortOutputHandler("Saved!");
        }

        private static bool CheckXmlTypos()
        {
            if (Form1.CurrentForm.fastColoredTextBox1.Text.Contains("<!-- "))
            {
                Form1.CurrentForm.fastColoredTextBox1.Text = Form1.CurrentForm.fastColoredTextBox1.Text.Replace("<!-- ", "<!--");
            }
            if (Form1.CurrentForm.fastColoredTextBox1.Text.Contains(" -->"))
            {
                Form1.CurrentForm.fastColoredTextBox1.Text = Form1.CurrentForm.fastColoredTextBox1.Text.Replace(" -->", "-->");
            }
            if (Form1.CurrentForm.treeView1.SelectedNode != null)
            {
                if (Form1.CurrentForm.treeView1.SelectedNode.Text.EndsWith(".x16") || Form1.CurrentForm.treeView1.SelectedNode.Text.EndsWith(".xml"))
                {
                    // Fix 0xfeff at start instead of reading string as is
                    byte[] encodedString = Encoding.Unicode.GetBytes(Form1.CurrentForm.fastColoredTextBox1.Text);
                    MemoryStream ms = new MemoryStream(encodedString);
                    ms.Flush();
                    ms.Position = 0;
                    //
                    XmlDocument xmlDocument = new XmlDocument();
                    try
                    {
                        xmlDocument.Load(ms);
                        ms.Close();
                    }
                    catch
                    {
                        Form1.CurrentForm.SortOutputHandler("Failed!");
                        ms.Close();
                        return false;
                    }
                    xmlDocument = new XmlDocument();
                }
            }
            return true;
        }

        public static void Beautify()
        {
            if (Form1.CurrentForm.materialComboBox5.SelectedItem != null && Form1.CurrentForm.materialComboBox5.SelectedItem.ToString().EndsWith(".dat") && Form1.CurrentForm.treeView1.SelectedNode != null)
            {
                if (Form1.CurrentForm.treeView1.SelectedNode.Text.Contains(".xml") || Form1.CurrentForm.treeView1.SelectedNode.Text.Contains(".x16"))
                    Form1.CurrentForm.fastColoredTextBox1.Text = FormatXml(Form1.CurrentForm.fastColoredTextBox1.Text);
            }
            else { Prompt.Popup("No file selected."); }
        }

        public static string FormatXml(string xml)
        {
            string result = xml;
            MemoryStream mStream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.Unicode);
            XmlDocument document = new XmlDocument();
            MemoryStream ms = new MemoryStream();
            try
            {
                // Load the XmlDocument with the XML. 
                byte[] encodedString = Encoding.Unicode.GetBytes(xml); // was fastColoredTextBox1.Text
                ms = new MemoryStream(encodedString);
                ms.Flush();
                ms.Position = 0;
                document.Load(ms);
                writer.Formatting = Formatting.Indented;
                document.WriteContentTo(writer);
                writer.Flush();
                mStream.Flush();
                mStream.Position = 0;
                StreamReader sReader = new StreamReader(mStream);
                string formattedXml = sReader.ReadToEnd();
                result = formattedXml;
                Form1.CurrentForm.SortOutputHandler("Formatted");
            }
            catch (XmlException)
            {
                Form1.CurrentForm.SortOutputHandler("Can't Format!");
            }
            mStream.Close();
            writer.Close();
            ms.Close();
            return result;
        }

        public static void SaveAs()
        {
            if (Form1.CurrentForm.treeView1.SelectedNode != null && Form1.CurrentForm.treeView1.SelectedNode.FullPath.ToString().Contains(".dat"))
            {
                Form1.CurrentForm.SortOutputHandler("Saving as...");
                if (CheckXmlTypos())
                {
                    SaveFileDialog SaveAs = new SaveFileDialog();
                    SaveAs.FileName = Form1.CurrentForm.treeView1.SelectedNode.Text;
                    if (Form1.CurrentForm.treeView1.SelectedNode != null)
                    {
                        if (Form1.CurrentForm.treeView1.SelectedNode.Text.EndsWith(".xml"))
                        {
                            SaveAs.Filter = "xml files (*.xml)|*.xml|text files (*.txt)|*.txt|xml files (*.x16)|*.x16";
                            SaveAs.DefaultExt = "xml";
                        }
                        if (Form1.CurrentForm.treeView1.SelectedNode.Text.EndsWith(".txt"))
                        {
                            SaveAs.Filter = "text files (*.txt)|*.txt|xml files (*.xml)|*.xml|xml files (*.x16)|*.x16";
                            SaveAs.DefaultExt = "txt";
                        }
                        if (Form1.CurrentForm.treeView1.SelectedNode.Text.EndsWith(".x16"))
                        {
                            SaveAs.Filter = "xml files (*.x16)|*.x16|text files (*.txt)|*.txt|xml files (*.xml)|*.xml";
                            SaveAs.DefaultExt = "x16";
                        }
                    }
                    SaveAs.Title = "Save as file";
                    SaveAs.RestoreDirectory = true;
                    SaveAs.InitialDirectory = Application.ExecutablePath;
                    DialogResult result = SaveAs.ShowDialog();
                    if (result == DialogResult.Cancel || result == DialogResult.Abort)
                        Form1.CurrentForm.SortOutputHandler("Cancelled!");
                    else if (result == DialogResult.OK)
                    {
                        File.WriteAllText(SaveAs.FileName, Form1.CurrentForm.fastColoredTextBox1.Text, Encoding.Unicode);
                        Form1.CurrentForm.SortOutputHandler("Saved.");
                    }
                }
            }
        }
    }
}
