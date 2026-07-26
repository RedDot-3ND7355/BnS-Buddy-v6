using BnSBuddy2.Functions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace BnSBuddy2.LegacyBin
{
    public static class BinHandler
    {
        static bool isBusyWorking = false;
        public static readonly BDAT_CONTENT _content = new BDAT_CONTENT();
        static List<BXML_LIST> xml_list = new List<BXML_LIST>();
        static bool checkresult;
        public static AutoResetEvent waitbw = new AutoResetEvent(initialState: false);
        private static readonly XmlSerializer BxmlListSerializer = new XmlSerializer(typeof(BXML_LIST));
        private static bool _lastIs64Bit = true;

        /// <summary>
        /// Resolve 32/64-bit mode: prefer filename (datafile64.bin / localfile64.bin),
        /// fall back to header layout detection for renamed files.
        /// </summary>
        public static bool ResolveIs64Bit(string filePath, BinaryReader br)
        {
            return BinSession.ResolveIs64Bit(filePath, br);
        }

        public static void PackBin(string bin_folderxfile, bool integer, TreeNode TouchedNode)
        {
            bin_folderxfile = bin_folderxfile.Replace(".files", "");
            if (!File.Exists(bin_folderxfile))
            {
                Prompt.Popup("Please extract the dat file first!");
                return;
            }
            if (!Directory.Exists(bin_folderxfile + ".files"))
            {
                Prompt.Popup("Please extract the bin file first!");
                return;
            }
            Task.Delay(50).ContinueWith(delegate
            {
                using (BDat LegacyBinActor = new BDat())
                {
                    isBusyWorking = true;
                    Control.CheckForIllegalCrossThreadCalls = false;
                    BinEditOptions.UseIntData = integer;
                    BinEditOptions.Progress = msg => Form1.CurrentForm.SortOutputHandler(msg);
                    Form1.CurrentForm.SortOutputHandler("Reading bin...");
                    bool is64;
                    using (FileStream fileStream = new FileStream(bin_folderxfile, FileMode.Open, FileAccess.Read, FileShare.Read))
                    using (BinaryReader br = new BinaryReader(fileStream))
                    {
                        is64 = ResolveIs64Bit(bin_folderxfile, br);
                        LegacyBinActor.bIntData = integer;
                        读取BIN(br, is64);
                        LegacyBinActor.DetectIndices();
                    }
                    _lastIs64Bit = is64;
                    Form1.CurrentForm.SortOutputHandler(is64 ? "Reading 64-bit XML..." : "Reading 32-bit XML...");
                    加载XML(bin_folderxfile + ".files");
                    Form1.CurrentForm.SortOutputHandler("Verifying xml...");
                    检查Xml();
                    Form1.CurrentForm.SortOutputHandler("Updating bin...");
                    Xml转BIN();
                    Form1.CurrentForm.SortOutputHandler("Saving bin...");
                    using (FileStream fileStream2 = new FileStream(bin_folderxfile, FileMode.Create, FileAccess.Write, FileShare.None))
                    using (BinaryWriter binaryWriter = new BinaryWriter(fileStream2))
                    {
                        编写BIN(binaryWriter, is64);
                    }
                    Directory.Delete(bin_folderxfile + ".files", true);
                }
                GC.Collect();
                Form1.CurrentForm.SortOutputHandler("Cleaning Nodes...");
                CleanNodes(TouchedNode);
                Form1.CurrentForm.SortOutputHandler("Done! (" + (_lastIs64Bit ? "64-bit" : "32-bit") + ")");
                isBusyWorking = false;
            });
        }

        private static void CleanNodes(TreeNode TouchedNode)
        {
            Form1.CurrentForm.treeView1.Nodes[0].Nodes[TouchedNode.Index].Nodes.Clear();
            Form1.CurrentForm.treeView1.Nodes[0].Nodes[TouchedNode.Index].Text = Form1.CurrentForm.treeView1.Nodes[0].Nodes[TouchedNode.Index].Text.Replace(".files", "");
        }

        private static void 编写BIN(BinaryWriter bw, bool is64)
        {
            if (is64)
            {
                _content.Write64(bw);
            }
            else
            {
                _content.Write(bw);
            }
            bw.Flush();
        }

        public static void Xml转BIN()
        {
            for (int i = 0; i < _content.ListCount; i++)
            {
                BDAT_LIST bDAT_LIST = _content.Lists[i];
                for (int j = 0; j < xml_list.Count; j++)
                {
                    if (xml_list[j].id == bDAT_LIST.ID)
                    {
                        if (bDAT_LIST.Collection.Compressed >= 1)
                        {
                            BXML_ARCHIVE archive = xml_list[j].collection.archive;
                            bDAT_LIST.Collection.Archive.UseChange(archive);
                        }
                        else
                        {
                            BXML_LOOSE loose = xml_list[j].collection.loose;
                            bDAT_LIST.Collection.Loose.UseChange(loose);
                        }
                    }
                }
            }
        }

        public static void 检查Xml()
        {
            checkresult = true;
            for (int i = 0; i < _content.ListCount; i++)
            {
                BDAT_LIST bDAT_LIST = _content.Lists[i];
                for (int j = 0; j < xml_list.Count; j++)
                {
                    if (xml_list[j].id == bDAT_LIST.ID)
                    {
                        if (bDAT_LIST.Collection.Compressed >= 1)
                        {
                            BXML_ARCHIVE archive = xml_list[j].collection.archive;
                            checkresult = bDAT_LIST.Collection.Archive.Compare(archive);
                        }
                        else
                        {
                            BXML_LOOSE loose = xml_list[j].collection.loose;
                            checkresult = bDAT_LIST.Collection.Loose.Compare(loose);
                        }
                    }
                }
            }
        }

        public static void 加载XML(string dir)
        {
            xml_list.Clear();
            for (int i = 0; i < _content.ListCount; i++)
            {
                Form1.CurrentForm.SortOutputHandler("Reading: " + (i + 1) + "/" + _content.ListCount);
                BDAT_LIST bDAT_LIST = _content.Lists[i];
                string text = $"datafile_{bDAT_LIST.ID:000}.xml";
                string s = File.ReadAllText(dir + "/" + text, Encoding.UTF8);
                BXML_LIST item;
                using (MemoryStream input = new MemoryStream(Encoding.UTF8.GetBytes(s)))
                {
                    using (XmlTextReader xmlTextReader = new XmlTextReader(input))
                    {
                        xmlTextReader.Normalization = false;
                        item = (BXML_LIST)BxmlListSerializer.Deserialize(xmlTextReader);
                    }
                }
                xml_list.Add(item);
                Application.DoEvents();
            }
        }

        public static void DumpBin(string binpath, bool integer, TreeNode TouchedNode)
        {
            if (isBusyWorking)
            {
                Prompt.Popup("Worker busy! Please wait.");
                return;
            }
            if (!File.Exists(binpath))
            {
                Prompt.Popup("Please Unpack dat first!");
                return;
            }
            Form1.CurrentForm.SortOutputHandler("Dumping bin file...");
            Task.Delay(50).ContinueWith(delegate
            {
                using (BDat LegacyBinActor = new BDat())
                {
                    isBusyWorking = true;
                    Control.CheckForIllegalCrossThreadCalls = false;
                    BinEditOptions.UseIntData = integer;
                    BinEditOptions.Progress = msg => Form1.CurrentForm.SortOutputHandler(msg);
                    Form1.CurrentForm.SortOutputHandler("Reading bin...");
                    using (FileStream fileStream = new FileStream(binpath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    using (BinaryReader binaryReader = new BinaryReader(fileStream))
                    {
                        bool is64 = ResolveIs64Bit(binpath, binaryReader);
                        LegacyBinActor.bIntData = integer;
                        读取BIN(binaryReader, is64);
                        _lastIs64Bit = is64;
                        LegacyBinActor.DetectIndices();
                        Form1.CurrentForm.SortOutputHandler(is64 ? "Exporting 64-bit XML..." : "Exporting 32-bit XML...");
                        if (Directory.Exists(binpath + ".files"))
                            Directory.Delete(binpath + ".files", true);
                        Directory.CreateDirectory(binpath + ".files");
                        输出保存XML(binpath + ".files");
                    }
                    Form1.CurrentForm.SortOutputHandler("GC Cleanup...");
                }
                GC.Collect();
                Form1.CurrentForm.SortOutputHandler("Populating...");
                Form1.CurrentForm.SortOutputHandler("Done! (" + (_lastIs64Bit ? "64-bit" : "32-bit") + ")");
                isBusyWorking = false;
                waitbw.Set();
            });
            waitbw.Reset();
            waitbw.WaitOne();
            PopulateBin(binpath + ".files", TouchedNode);
        }

        private static void PopulateBin(string path, TreeNode TouchedNode)
        {
            foreach (string file in Directory.GetFiles(path))
                Form1.CurrentForm.treeView1.Nodes[0].Nodes[TouchedNode.Index].Nodes.Add(Path.GetFileName(file));
            Form1.CurrentForm.treeView1.Nodes[0].Nodes[TouchedNode.Index].Text += ".files";
            Form1.CurrentForm.treeView1.Nodes[0].Nodes[TouchedNode.Index].Expand();
        }

        public static void 输出保存XML(string dir)
        {
            Form1.CurrentForm.SortOutputHandler("Saving to XML...");
            for (int i = 0; i < _content.ListCount; i++)
            {
                try
                {
                    Form1.CurrentForm.SortOutputHandler("Saving: " + (i + 1) + "/" + _content.ListCount);
                    BDAT_LIST bDAT_LIST = _content.Lists[i];
                    BXML_LIST bXML_LIST = new BXML_LIST();
                    bXML_LIST.Convert(bDAT_LIST);
                    XmlWriterSettings settings = new XmlWriterSettings
                    {
                        Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
                        Indent = true,
                        NewLineHandling = NewLineHandling.Entitize
                    };
                    MemoryStream memoryStream = new MemoryStream();
                    XmlWriter xmlWriter = XmlWriter.Create(memoryStream, settings);
                    BxmlListSerializer.Serialize(xmlWriter, bXML_LIST);
                    string @string = Encoding.UTF8.GetString(memoryStream.ToArray());
                    string text = $"datafile_{bDAT_LIST.ID:000}.xml";
                    File.WriteAllText(dir + "/" + text, @string, Encoding.UTF8);
                    Application.DoEvents();
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine(ex.InnerException.Message);
                    }
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static void 读取BIN(BinaryReader br, bool is64 = false)
        {
            if (is64)
            {
                _content.Read64(br);
            }
            else
            {
                _content.Read(br);
            }
        }

        /// <summary>Open the in-app bin editor (optionally pre-open a path).</summary>
        public static void OpenBinEditor(string optionalPath = null)
        {
            foreach (Form f in Application.OpenForms)
            {
                if (f is BinEditorForm existing)
                {
                    existing.Show();
                    existing.BringToFront();
                    if (existing.WindowState == FormWindowState.Minimized)
                    {
                        existing.WindowState = FormWindowState.Normal;
                    }
                    if (!string.IsNullOrEmpty(optionalPath) && File.Exists(optionalPath))
                    {
                        existing.OpenPath(optionalPath);
                    }
                    return;
                }
            }
            var editor = new BinEditorForm();
            editor.Show();
            if (!string.IsNullOrEmpty(optionalPath) && File.Exists(optionalPath))
            {
                editor.OpenPath(optionalPath);
            }
        }
    }
}
