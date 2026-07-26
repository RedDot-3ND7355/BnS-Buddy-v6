using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace BnSBuddy2.LegacyBin
{
    /// <summary>
    /// In-memory bin session: open / save / export without requiring disk XML round-trip.
    /// </summary>
    public sealed class BinSession
    {
        private static readonly XmlSerializer BxmlListSerializer = new XmlSerializer(typeof(BXML_LIST));

        public string FilePath { get; private set; }

        public bool Is64Bit { get; private set; }

        public bool IsDirty { get; private set; }

        public bool IsOpen => Content != null && Content.Lists != null;

        public BDAT_CONTENT Content { get; private set; }

        public void MarkDirty()
        {
            IsDirty = true;
        }

        public void ClearDirty()
        {
            IsDirty = false;
        }

        /// <summary>Resolve 32/64 from filename, then header layout.</summary>
        public static bool ResolveIs64Bit(string filePath, BinaryReader br)
        {
            string name = Path.GetFileName(filePath) ?? string.Empty;
            if (name.IndexOf("64.bin", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }
            if (name.IndexOf("datafile64", StringComparison.OrdinalIgnoreCase) >= 0
                || name.IndexOf("localfile64", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }
            return BDAT_CONTENT.DetectIs64Bit(br);
        }

        public void Open(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                throw new FileNotFoundException("Bin file not found.", path);
            }

            BinEditOptions.Report("Opening " + Path.GetFileName(path) + "...");
            var content = new BDAT_CONTENT();
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var br = new BinaryReader(fs))
            {
                bool is64 = ResolveIs64Bit(path, br);
                BinEditOptions.Report(is64 ? "Reading 64-bit bin..." : "Reading 32-bit bin...");
                if (is64)
                {
                    content.Read64(br);
                }
                else
                {
                    content.Read(br);
                }
                Is64Bit = content.Is64Bit || is64;
            }

            Content = content;
            FilePath = path;
            IsDirty = false;
            BinEditOptions.Report("Loaded " + content.ListCount + " tables (" + (Is64Bit ? "64-bit" : "32-bit") + ")");
        }

        public void Save()
        {
            if (string.IsNullOrEmpty(FilePath))
            {
                throw new InvalidOperationException("No file path. Use Save As.");
            }
            SaveAs(FilePath);
        }

        public void SaveAs(string path)
        {
            if (!IsOpen)
            {
                throw new InvalidOperationException("No bin is open.");
            }
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path is required.", nameof(path));
            }

            BinEditOptions.Report(Is64Bit ? "Saving 64-bit bin..." : "Saving 32-bit bin...");
            string tempPath = path + ".tmp";
            try
            {
                using (var fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None))
                using (var bw = new BinaryWriter(fs))
                {
                    if (Is64Bit)
                    {
                        Content.Write64(bw);
                    }
                    else
                    {
                        Content.Write(bw);
                    }
                    bw.Flush();
                }
                File.Copy(tempPath, path, true);
                File.Delete(tempPath);
            }
            catch
            {
                try
                {
                    if (File.Exists(tempPath))
                    {
                        File.Delete(tempPath);
                    }
                }
                catch
                {
                    // ignore cleanup failures
                }
                throw;
            }

            FilePath = path;
            IsDirty = false;
            BinEditOptions.Report("Saved " + Path.GetFileName(path));
        }

        public void ExportXml(string directory)
        {
            if (!IsOpen)
            {
                throw new InvalidOperationException("No bin is open.");
            }
            if (string.IsNullOrEmpty(directory))
            {
                throw new ArgumentException("Directory is required.", nameof(directory));
            }
            Directory.CreateDirectory(directory);

            var settings = new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
                Indent = true,
                NewLineHandling = NewLineHandling.Entitize
            };

            for (int i = 0; i < Content.ListCount; i++)
            {
                BinEditOptions.Report("Export XML " + (i + 1) + "/" + Content.ListCount);
                BDAT_LIST list = Content.Lists[i];
                try
                {
                    var xmlList = new BXML_LIST();
                    xmlList.Convert(list);
                    using (var ms = new MemoryStream())
                    {
                        using (var xw = XmlWriter.Create(ms, settings))
                        {
                            BxmlListSerializer.Serialize(xw, xmlList);
                        }
                        string text = Encoding.UTF8.GetString(ms.ToArray());
                        string fileName = string.Format("datafile_{0:000}.xml", list.ID);
                        File.WriteAllText(Path.Combine(directory, fileName), text, Encoding.UTF8);
                    }
                }
                catch (Exception ex)
                {
                    BinEditOptions.Report("Export skip id=" + list.ID + ": " + ex.Message);
                }
            }
            BinEditOptions.Report("XML export done → " + directory);
        }

        public string GetTableKind(BDAT_LIST list)
        {
            if (list?.Collection == null)
            {
                return "?";
            }
            if (list.Collection.Compressed >= 1)
            {
                return "Archive";
            }
            return "Loose";
        }

        public int GetRecordCount(BDAT_LIST list)
        {
            if (list?.Collection == null)
            {
                return 0;
            }
            if (list.Collection.Loose != null)
            {
                return list.Collection.Loose.FieldCount;
            }
            if (list.Collection.Archive?.SubArchives != null)
            {
                int n = 0;
                foreach (var sub in list.Collection.Archive.SubArchives)
                {
                    if (sub?.Fields != null)
                    {
                        n += sub.Fields.Length;
                    }
                }
                return n;
            }
            return 0;
        }
    }
}
