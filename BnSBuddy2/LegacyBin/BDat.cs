using Microsoft.Win32.SafeHandles;
using Revamped_BnS_Buddy.Functions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace Revamped_BnS_Buddy.LegacyBin
{
    public class BDat : IDisposable
    {
        private bool _disposed = false;
        private SafeHandle _safeHandle = new SafeFileHandle(IntPtr.Zero, true);
        public void Dispose() => Dispose(true);
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
                _content = null;
                xml_list = null;
                compresssizeMap = null;
                sizeMap = null;
                compresssizeMap2 = null;
                sizeMap2 = null;
                _safeHandle?.Dispose();
            }
            _disposed = true;
        }

        public BDAT_CONTENT _content;
        private int _indexFaqs;
        private int _indexCommons;
        private int _indexCommands;

        public List<BXML_LIST> xml_list = new List<BXML_LIST>();
        private Dictionary<uint, int> compresssizeMap = new Dictionary<uint, int>();
        private Dictionary<uint, int> sizeMap = new Dictionary<uint, int>();
        private static int index;
        private static int index2;
        public static int index3;
        private Dictionary<uint, int> compresssizeMap2 = new Dictionary<uint, int>();
        private Dictionary<uint, int> sizeMap2 = new Dictionary<uint, int>();
        private bool checkresult = true;
        public static int compress_lv = 6;
        public bool bIntData = false;
        public static int CurrentFile = 1;

        public List<string> GetFileList(byte[] InputBin, bool Integer, bool is64)
        {
            List<string> FileList = new List<string>();
            MemoryStream byteBuff = new MemoryStream(InputBin);
            bIntData = Integer;
            Load(byteBuff, BDAT_TYPE.BDAT_BINARY, is64, true);
            for (int l = 0; l < _content.ListCount; l++)
            {
                Form1.CurrentForm.SortOutputHandler(string.Format("Reading: {0}/{1}", (l + 1), _content.ListCount));
                //BDAT_LIST blist = _content.Lists[l];
                FileList.Add($"datafile_{_content.Lists[l].ID:000}.xml");
            }
            Form1.CurrentForm.SortOutputHandler("Reading: done");
            byteBuff.Close();
            return FileList;
        }

        public Dictionary<string, byte[]> GetFileListAndContent(byte[] InputBin, bool Integer, bool is64)
        {
            MemoryStream byteBuff = new MemoryStream(InputBin);
            bIntData = Integer;
            Load(byteBuff, BDAT_TYPE.BDAT_BINARY, is64);
            Dictionary<string, byte[]> FileAndContent = new Dictionary<string, byte[]>();
            for (int l = 0; l < _content.ListCount; l++)
            {
                try
                {
                    Form1.CurrentForm.SortOutputHandler(string.Format("Reading: {0}/{1}", (l + 1), _content.ListCount));
                    BDAT_LIST blist = _content.Lists[l];
                    BXML_LIST list = new BXML_LIST();
                    list.Convert(blist, bIntData);
                    XmlWriterSettings setting = new XmlWriterSettings
                    {
                        Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
                        Indent = true,
                        NewLineHandling = NewLineHandling.Entitize
                    };
                    MemoryStream ms = new MemoryStream();
                    XmlWriter writer = XmlWriter.Create(ms, setting);
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(BXML_LIST));
                    xmlSerializer.Serialize(writer, list);
                    //string OutputXmlString = Encoding.UTF8.GetString(ms.ToArray());
                    string fileName = $"datafile_{blist.ID:000}.xml";
                    FileAndContent.Add(fileName, ms.ToArray());
                    //File.WriteAllText(dir + "/" + fileName, OutputXmlString, Encoding.UTF8);
                }
                catch (Exception e)
                {
                    if (e.InnerException != null)
                    {
                        string err = e.InnerException.Message;
                        Prompt.Popup(err);
                    }
                    Prompt.Popup("XMLSERIALIZER: " + e.Message);
                }
            }
            byteBuff.Close();
            return FileAndContent;
        }

        public void PackFromDictionary(Dictionary<string, byte[]> FileAndContent)
        {
            for (int i = 0; i < this._content.ListCount; i++)
            {
                Form1.CurrentForm.SortOutputHandler(string.Format("Reading XML: {0}/{1}", (i + 1), this._content.ListCount));
                BDAT_LIST blist = this._content.Lists[i];
                string fileName = $"datafile_{blist.ID:000}.xml";
                byte[] MMS = FileAndContent[fileName];
                BXML_LIST deserialized;
                using (XmlReader xmlReader = XmlReader.Create(new MemoryStream(MMS)))
                {
					//xmlReader.Normalization = false;
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(BXML_LIST));
                    deserialized = (BXML_LIST)xmlSerializer.Deserialize(xmlReader);
                }
                xml_list.Add(deserialized);
            }
        }

        public void DumpXML(string dir, bool is64 = false)
        {
            for (int l = 0; l < _content.ListCount; l++)
            {
                try
                {
                    Form1.CurrentForm.SortOutputHandler(string.Format("Dumping: {0}/{1}", (l + 1), _content.ListCount));
                    BDAT_LIST blist = _content.Lists[l];
                    BXML_LIST list = new BXML_LIST();
                    list.Convert(blist, bIntData);
                    XmlWriterSettings setting = new XmlWriterSettings
                    {
                        Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
                        Indent = true,
                        NewLineHandling = NewLineHandling.Entitize
                    };
                    MemoryStream ms = new MemoryStream();
                    XmlWriter writer = XmlWriter.Create(ms, setting);
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(BXML_LIST));
                    xmlSerializer.Serialize(writer, list);
                    string OutputXmlString = Encoding.UTF8.GetString(ms.ToArray());
                    string fileName = $"datafile_{blist.ID:000}.xml";
                    File.WriteAllText(dir + "/" + fileName, OutputXmlString, Encoding.UTF8);
                }
                catch (Exception e)
                {
                    if (e.InnerException != null)
                    {
                        string err = e.InnerException.Message;
                        Prompt.Popup(err);
                    }
                    Prompt.Popup(e.Message);
                }
            }
        }

        public void loadAllXml(string dir)
        {
            for (int i = 0; i < this._content.ListCount; i++)
            {
                Form1.CurrentForm.SortOutputHandler(string.Format("Reading XML: {0}/{1}", (i + 1), this._content.ListCount));
                BDAT_LIST blist = this._content.Lists[i];
                string fileName = $"datafile_{blist.ID:000}.xml";
                if (File.Exists(dir + "/" + fileName))
                {
                    BXML_LIST deserialized;
                    using (XmlReader xmlReader = XmlReader.Create(dir + "/" + fileName))
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(BXML_LIST));
                        deserialized = (BXML_LIST)xmlSerializer.Deserialize(xmlReader);
                    }
                    xml_list.Add(deserialized);
                }
            }
        }

        public void check()
        {
            checkresult = true;
            for (int j = 0; j < this._content.ListCount; j++)
            {
                BDAT_LIST blist = this._content.Lists[j];
                for (int i = 0; i < xml_list.Count; i++)
                {
                    if (xml_list[i].id == blist.ID)
                    {
                        if (blist.Collection.Compressed >= 1)
                        {
                            string fileName2 = $"datafile_{blist.ID:000}.xml";
                            BXML_ARCHIVE xml_archive = xml_list[i].collection.archive;
                            checkresult = blist.Collection.Archive.Compare(xml_archive);
                        }
                        else
                        {
                            string fileName = $"datafile_{blist.ID:000}.xml";
                            BXML_LOOSE xml_loose = xml_list[i].collection.loose;
                            checkresult = blist.Collection.Loose.Compare(xml_loose);
                        }
                    }
                }
            }
        }
        public void replaceDatFromXml(bool is64)
        {
            for (int j = 0; j < this._content.ListCount; j++)
            {
                BDAT_LIST blist = this._content.Lists[j];
                for (int i = 0; i < xml_list.Count; i++)
                {
                    if (xml_list[i].id == blist.ID)
                    {
                        string fileName = $"datafile_{blist.ID:000}.xml";
                        if (blist.Collection.Compressed >= 1)
                        {
                            BXML_ARCHIVE archive = xml_list[i].collection.archive;
                            blist.Collection.Archive.UseChange(archive, bIntData);
                        }
                        else
                        {
                            BXML_LOOSE loose = xml_list[i].collection.loose;
                            blist.Collection.Loose.UseChange(loose, bIntData);
                        }
                    }
                }
            }
        }
        private void DetectIndices()
        {
            for (int i = 0; i < this._content.ListCount; i++)
            {
                uint size;
                BDAT_LIST bdat_list = this._content.Lists[i];
                if ((bdat_list.Unknown1 == 2) && (bdat_list.Unknown2 == 0))
                {
                    size = 0;
                    if ((bdat_list.Collection.Compressed >= 1) && ((bdat_list.Collection.Archive.SubArchiveCount > 0) && (bdat_list.Collection.Archive.SubArchives[0].FieldLookupCount > 0)))
                    {
                        size = bdat_list.Collection.Archive.SubArchives[0].Fields[0].Size;
                    }
                    else if ((bdat_list.Collection.Compressed < 1) && (bdat_list.Collection.Loose.FieldCount > 0))
                    {
                        size = bdat_list.Collection.Loose.Fields[0].Size;
                    }
                    if (size == 0x20)
                    {
                        this._indexFaqs = i;
                    }
                }
                if ((bdat_list.Size > 0x4c4b40) && ((bdat_list.Unknown1 == 1) && (bdat_list.Unknown2 == 0)))
                {
                    size = 0;
                    if ((bdat_list.Collection.Compressed >= 1) && ((bdat_list.Collection.Archive.SubArchiveCount > 0) && (bdat_list.Collection.Archive.SubArchives[0].FieldLookupCount > 0)))
                    {
                        size = bdat_list.Collection.Archive.SubArchives[0].Fields[0].Size;
                    }
                    else if ((bdat_list.Collection.Compressed < 1) && (bdat_list.Collection.Loose.FieldCount > 0))
                    {
                        size = bdat_list.Collection.Loose.Fields[0].Size;
                    }
                    if (size == 0x1c)
                    {
                        this._indexCommons = i;
                    }
                }
                if ((this._indexCommons > -1) && (((this._indexCommons) < i) && ((bdat_list.Unknown1 == 1) && (bdat_list.Unknown2 == 0))))
                {
                    size = 0;
                    if ((bdat_list.Collection.Compressed >= 1) && ((bdat_list.Collection.Archive.SubArchiveCount > 0) && (bdat_list.Collection.Archive.SubArchives[0].FieldLookupCount > 0)))
                    {
                        size = bdat_list.Collection.Archive.SubArchives[0].Fields[0].Size;
                    }
                    else if ((bdat_list.Collection.Compressed < 1) && (bdat_list.Collection.Loose.FieldCount > 0))
                    {
                        size = bdat_list.Collection.Loose.Fields[0].Size;
                    }
                    if (size == 0x1c)
                    {
                        this._indexCommands = i;
                    }
                }
            }
            //object[] objArray1 = new object[] { "List Index Faqs (ID): ", this._indexFaqs, " (", this._content.Lists[this._indexFaqs].ID, string.Empty };
            //Console.WriteLine(string.Concat(objArray1));
            //object[] objArray2 = new object[] { "List Index Commons (ID): ", this._indexCommons, " (", this._content.Lists[this._indexCommons].ID, ")" };
            //Console.WriteLine(string.Concat(objArray2));
            //object[] objArray3 = new object[] { "List Index Commands (ID): ", this._indexCommands, " (", this._content.Lists[this._indexCommands].ID, ")" };
            //Console.WriteLine(string.Concat(objArray3));
        }

        public void Load(MemoryStream iStream, BDAT_TYPE iType, bool is64, bool OnlyIDS = false)
        {
            if (iType == BDAT_TYPE.BDAT_PLAIN || iType == BDAT_TYPE.BDAT_BINARY)
            {
                this._content = new BDAT_CONTENT();
                this._content.Read(iStream, iType, is64, OnlyIDS);
                this.DetectIndices();
            }
        }

        public void Save(MemoryStream oStream, BDAT_TYPE oType, bool is64)
        {
            if (oType == BDAT_TYPE.BDAT_BINARY)
            {
                this._content.Write(oStream, oType, is64);
            }
        }

    }
    public enum BDAT_TYPE
    {
        BDAT_XML = 1,
        BDAT_PLAIN = 2,
        BDAT_BINARY = 3,
        BDAT_UNKNOWN = 4
    }
    public class BDAT_ARCHIVE
    {
        public uint SubArchiveCount;
        public ushort Unknown;
        public BDAT_SUBARCHIVE[] SubArchives;

        public bool Compare(BXML_ARCHIVE newData)
        {
            for (int i = 0; i < this.SubArchives.Length; i++)
            {
                if (!this.SubArchives[i].Compare(newData.SubArchives[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public int GetSize()
        {
            int num = 0;
            for (int i = 0; i < this.SubArchives.Length; i++)
            {
                num += this.SubArchives[i].GetSize();
            }
            return num;
        }

        public void Read(BinaryReader iStream, BDAT_TYPE iType)
        {
            this.SubArchiveCount = iStream.ReadUInt32();
            this.Unknown = iStream.ReadUInt16();
            this.SubArchives = new BDAT_SUBARCHIVE[this.SubArchiveCount];
            for (int i = 0; i < this.SubArchiveCount; i++)
            {
                this.SubArchives[i] = new BDAT_SUBARCHIVE();
                this.SubArchives[i].Read(iStream, iType);
            }
        }

        public void UseChange(BXML_ARCHIVE newData, bool bIntData)
        {
            for (int i = 0; i < this.SubArchives.Length; i++)
            {
                this.SubArchives[i].UseChange(newData.SubArchives[i], bIntData);
            }
        }

        public void Write(BinaryWriter oStream, BDAT_TYPE oType)
        {
            oStream.Write(this.SubArchiveCount);
            oStream.Write(this.Unknown);
            for (uint i = 0; i < this.SubArchiveCount; i++)
            {
                this.SubArchives[i].Write(oStream, oType);
            }
        }
    }
    public class BDAT_COLLECTION
    {
        public byte Compressed;
        public byte Deprecated;
        public BDAT_ARCHIVE Archive;
        public BDAT_LOOSE Loose;

        public void Read(BinaryReader iStream, BDAT_TYPE iType)
        {
            this.Compressed = iStream.ReadByte();
            if (this.Compressed < 1)
            {
                this.Loose = new BDAT_LOOSE();
                this.Loose.Read(iStream, iType);
            }
            else
            {
                if (this.Compressed > 1)
                {
                    iStream.BaseStream.Position = (iStream.BaseStream.Position - 1);
                }
                this.Archive = new BDAT_ARCHIVE();
                this.Archive.Read(iStream, iType);
                if (this.Compressed > 1)
                {
                    this.Deprecated = iStream.ReadByte();
                }
            }
        }

        public void Write(BinaryWriter oStream, BDAT_TYPE oType)
        {
            oStream.Write(this.Compressed);
            if (this.Compressed < 1)
            {
                this.Loose.Write(oStream, oType);
            }
            else
            {
                if (this.Compressed > 1)
                {
                    oStream.BaseStream.Seek(oStream.BaseStream.Position - 1, SeekOrigin.Begin);
                }
                this.Archive.Write(oStream, oType);
                if (this.Compressed > 1)
                {
                    oStream.Write(this.Deprecated);
                }
            }
        }
    }

    public class BDAT_CONTENT
    {
        public byte[] Signature;
        public uint Version;
        public byte[] Unknown;
        public uint ListCount;
        public BDAT_HEAD HeadList;
        public BDAT_LIST[] Lists;

        public void Read(MemoryStream _iStream, BDAT_TYPE iType, bool is64 = false, bool OnlyIDS = false)
        {
            BinaryReader iStream = new BinaryReader(_iStream);
            this.Signature = iStream.ReadBytes(8);
            this.Version = iStream.ReadUInt32();
            this.Unknown = iStream.ReadBytes((is64 ? 13 : 9));
            this.ListCount = is64 ? (uint)iStream.ReadUInt64() : iStream.ReadUInt32();
            this.HeadList = new BDAT_HEAD();
            this.HeadList.Complement = false;
            if (this.ListCount < 20)
            {
                this.HeadList.Complement = true;
            }
            this.HeadList.Read(iStream, iType, is64, OnlyIDS);
            this.Lists = new BDAT_LIST[this.ListCount];
            for (uint k = 0; k < this.ListCount; k++)
            {
                BDat.CurrentFile = (int)k;
                try
                {
                    this.Lists[k] = new BDAT_LIST();
                    this.Lists[k].Read(iStream, iType);
                }
                catch(Exception e)
                {
                    Prompt.Popup("Error Reading file: " + k + " | Stacktrace: " + e.StackTrace);
                }
            }
        }

        public void Write(MemoryStream _oStream, BDAT_TYPE oType, bool is64)
        {
            BinaryWriter oStream = new BinaryWriter(_oStream);
            oStream.Write(this.Signature);
            oStream.Write(this.Version);
            oStream.Write(this.Unknown);
            if (is64) oStream.Write((ulong)this.ListCount); else oStream.Write(this.ListCount);
            this.HeadList.Write(oStream, oType, is64);
            for (uint k = 0; k < this.ListCount; k++)
            {
                Form1.CurrentForm.SortOutputHandler(string.Format("\rPacking XML: {0}/{1}", (k + 1), this.ListCount));
                try
                {
                    this.Lists[k].Write(oStream, oType);
                }
                catch (Exception e)
                {
                    if (e.InnerException != null)
                    {
                        string err = e.InnerException.Message;
                        Prompt.Popup(err);
                    }
                    Prompt.Popup(e.Message);
                }
            }
        }
    }
    public class BDAT_FIELDTABLE
    {
        public ushort Unknown1;
        public ushort Unknown2;
        public uint Size;
        public uint ID;
        public byte[] Data;

        public int GetSize()
        {
            return ((this.Data != null) ? this.Data.Length : 0);
        }

        public bool isEmpty()
        {
            return (this.Data == null);
        }

        public void Read(BinaryReader iStream, BDAT_TYPE iType)
        {
            this.Unknown1 = iStream.ReadUInt16();
            this.Unknown2 = iStream.ReadUInt16();
            this.Size = iStream.ReadUInt32();
            this.ID = iStream.ReadUInt32();
            //this.Data = new byte[this.Size - 12]; // all 12 was 8
            if (((int)this.Size - 12) > 0)
            {
                this.Data = iStream.ReadBytes((int)this.Size - 12);
            }
            else
            {
                this.Data = new byte[0];
            }
        }

        public void UseChange(BXML_FIELDTABLE newData, bool bIntData)
        {
            if (newData != null)
            {
                this.Unknown1 = newData.unk1;
                this.Unknown2 = newData.unk2;
                this.Size = newData.size;
                this.ID = newData.ID;
                if (bIntData)
                {
                    this.Data = bcrypt.IntToBytes(newData.data, newData.size - 12);
                }
                else
                {
                    this.Data = bcrypt.HexToBytes(newData.data, newData.size - 12);
                }
            }
        }

        public void Write(BinaryWriter oStream, BDAT_TYPE iType)
        {
            oStream.Write(this.Unknown1);
            oStream.Write(this.Unknown2);
            oStream.Write(this.Size);
            oStream.Write(this.ID);
            for (int i = 0; i < (this.Size - 12); i++)
            {
                oStream.Write(this.Data[i]);
            }
        }
    }
    public class BDAT_HEAD
    {
        public bool Complement;
        public uint Size_1;
        public uint Size_2;
        public uint Size_3;
        public byte[] Padding;
        public byte[] Data;

        public void Read(BinaryReader iStream, BDAT_TYPE iType, bool is64, bool OnlyIDS)
        {
            this.Size_1 = (is64 ? (uint)iStream.ReadUInt64() : iStream.ReadUInt32());
            this.Size_2 = (is64 ? (uint)iStream.ReadUInt64() : iStream.ReadUInt32());
            this.Size_3 = (is64 ? (uint)iStream.ReadUInt64() : iStream.ReadUInt32());
            this.Padding = iStream.ReadBytes(62);
            this.Data = new byte[this.Size_1];
            if (!this.Complement)
            {
                if (OnlyIDS)
                {
                    iStream.ReadBytes((int)this.Size_1);
                }
                else
                {
                    this.Data = iStream.ReadBytes((int)this.Size_1);
                }
            }
        }

        public void Write(BinaryWriter oStream, BDAT_TYPE oType, bool is64)
        {
            if (is64) oStream.Write((ulong)this.Size_1); else oStream.Write(this.Size_1);
            if (is64) oStream.Write((ulong)this.Size_2); else oStream.Write(this.Size_2);
            if (is64) oStream.Write((ulong)this.Size_3); else oStream.Write(this.Size_3);
            oStream.Write(this.Padding);
            if (!this.Complement)
            {
                oStream.Write(this.Data);
            }
        }
    }
    public class BDAT_LIST
    {
        public static int curSaveId;
        public byte Unknown1;
        public ushort ID;
        public ushort Unknown2;
        public ushort Unknown3;
        public uint Size;
        public BDAT_COLLECTION Collection;

        public void Read(BinaryReader iStream, BDAT_TYPE iType)
        {
            this.Unknown1 = iStream.ReadByte();
            this.ID = iStream.ReadUInt16();
            this.Unknown2 = iStream.ReadUInt16();
            this.Unknown3 = iStream.ReadUInt16();
            this.Size = iStream.ReadUInt32();
            uint num = (uint)iStream.BaseStream.Position;
            this.Collection = new BDAT_COLLECTION();
            this.Collection.Read(iStream, iType);
            uint num2 = (uint)iStream.BaseStream.Position;
            if ((num + this.Size) != num2)
            {
                iStream.BaseStream.Position = (num + this.Size);
            }
        }

        public void Write(BinaryWriter oStream, BDAT_TYPE oType)
        {
            curSaveId = this.ID;
            oStream.Write(this.Unknown1);
            oStream.Write(this.ID);
            oStream.Write(this.Unknown2);
            oStream.Write(this.Unknown3);
            oStream.Write(this.Size);
            uint num = (uint)oStream.BaseStream.Position;
            this.Collection.Write(oStream, oType);
            uint num2 = (uint)oStream.BaseStream.Position;
            oStream.BaseStream.Seek(num - 4, SeekOrigin.Begin);
            this.Size = num2 - num;
            oStream.Write(this.Size);
            oStream.BaseStream.Seek(this.Size, SeekOrigin.Current);
        }
    }
    public class BDAT_LOOKUPTABLE
    {
        public uint Size;
        public byte[] Data;

        public int Compare(BXML_LOOKUPTABLE newData)
        {
            int sizeLookup = 0;
            byte[] buffer = bnsTool.WordToLookUpData(newData.words, ref sizeLookup);
            if (sizeLookup != this.Size)
            {
                return 1;
            }
            for (int i = 0; i < this.Size; i++)
            {
                if (this.Data[i] != buffer[i])
                {
                    return 2;
                }
            }
            return 0;
        }

        public int GetSize()
        {
            return this.Data.Length;
        }

        public void Read(BinaryReader iStream, BDAT_TYPE iType)
        {
            this.Data = new byte[this.Size];
            this.Data = iStream.ReadBytes((int)this.Size);
        }

        public int UseChange(BXML_LOOKUPTABLE newData)
        {
            string[] words = newData.words;
            int sizeLookup = 0;
            this.Data = bnsTool.WordToLookUpData(newData.words, ref sizeLookup);
            this.Size = (uint)sizeLookup;
            return sizeLookup;
        }

        public void Write(BinaryWriter iStream, BDAT_TYPE iType)
        {
            iStream.Write(this.Data);
        }
    }
    public class BDAT_LOOSE
    {
        public List<string> words;
        public uint FieldCountUnfixed;
        public uint FieldCount;
        public uint SizeFields;
        public uint SizeLookup;
        public byte Unknown;
        public BDAT_FIELDTABLE[] Fields;
        public int SizePadding;
        public byte[] Padding;
        public BDAT_LOOKUPTABLE Lookup;

        public bool Compare(BXML_LOOSE newData)
        {
            return (this.Lookup.Compare(newData.lookup) <= 0);
        }

        public int GetSize()
        {
            int num = 0;
            for (int i = 0; i < this.Fields.Length; i++)
            {
                num += this.Fields[i].GetSize();
                if (this.Fields[i].Data == null)
                {
                    //Console.Write("\rFieldCount" + this.FieldCount);
                    //Console.Write("\rFieldCountUnfixed" + this.FieldCountUnfixed);
                    object[] objArray1 = new object[] { "Fields[", i, "].GetSize", this.Fields[i].Size };
                    //Console.WriteLine(string.Concat(objArray1));
                }
            }
            return (num + this.Lookup.GetSize());
        }


        public void Read(BinaryReader iStream, BDAT_TYPE iType)
        {
            this.FieldCount = iStream.ReadUInt32();
            this.FieldCountUnfixed = this.FieldCount;
            if (iType == BDAT_TYPE.BDAT_XML) 
                this.SizeFields = (uint)iStream.ReadUInt64();
            else
            {
                this.SizeFields = iStream.ReadUInt32();
            }
            this.SizeLookup = iStream.ReadUInt32();
            this.Unknown = iStream.ReadByte();
            uint num3 = (uint)iStream.BaseStream.Position + this.SizeFields;
            this.Fields = new BDAT_FIELDTABLE[this.FieldCount];
            uint index = 0;
            while (true)
            {
                uint num;
                if (index < this.FieldCount)
                {
                    num = (uint)iStream.BaseStream.Position;
                    object[] objArray1 = new object[] { "VERBOSE_OUT FieldCount:", this.FieldCount, " FieldCountUnfixed:", this.FieldCountUnfixed };
                    if (num < num3)
                    {
                        try
                        {
                            this.Fields[index] = new BDAT_FIELDTABLE();
                            this.Fields[index].Read(iStream, iType); // here <- out of bounds
                        }
                        catch { Prompt.Popup("File ID: " + BDat.CurrentFile + " could not be read. Too big. At Index: " + index + " | Field Count: " + FieldCount); /*Prompt.Popup(string.Concat(objArray1));*/ }
                        index++;
                        continue;
                    }
                    this.FieldCount = index;
                    try
                    {
                            iStream.BaseStream.Position += (num3 - num);
                    } catch(Exception ex) { MessageBox.Show("CurrentFile: " + BDat.CurrentFile + " | Position: " + iStream.BaseStream.Position + " | Length: " + iStream.BaseStream.Length + Environment.NewLine + ex.ToString()); }
                }
                num = (uint)iStream.BaseStream.Position;
                this.SizePadding = (int)(num3 - num);
                if (this.SizePadding < 0)
                {
                    Prompt.Popup("CRITICAL ERROR: Negative psize of padding bytes");
                }
                if (this.SizePadding > 0)
                {
                    this.SizePadding = (int)(num3 - num);
                    this.Padding = iStream.ReadBytes(this.SizePadding);
                }
                this.Lookup = new BDAT_LOOKUPTABLE();
                this.Lookup.Size = this.SizeLookup;
                this.Lookup.Read(iStream, iType);
                return;
            }
        }

        public void UseChange(BXML_LOOSE newData, bool bIntData)
        {
            BXML_FIELDTABLE[] fields = newData.fields;
            this.FieldCountUnfixed = (uint)fields.Length;
            this.Fields = new BDAT_FIELDTABLE[fields.Length];
            int num = 0;
            for (int i = 0; i < fields.Length; i++)
            {
                this.Fields[i] = new BDAT_FIELDTABLE();
                this.Fields[i].UseChange(fields[i], bIntData);
                if (this.Fields[i].isEmpty())
                {
                    num++;
                }
            }
            this.FieldCount = this.FieldCountUnfixed - ((uint)num);
            this.Lookup = new BDAT_LOOKUPTABLE();
            this.SizeLookup = (uint)this.Lookup.UseChange(newData.lookup);
        }

        public void Write(BinaryWriter oStream, BDAT_TYPE oType)
        {
            oStream.Write(this.FieldCountUnfixed);
            uint pos = (uint)oStream.BaseStream.Position;
            if (oType == BDAT_TYPE.BDAT_XML) {
                oStream.Write((ulong)this.SizeFields);
            }
            else
            {
                oStream.Write(this.SizeFields);
            }
            oStream.Write(this.SizeLookup);
            oStream.Write(this.Unknown);
            uint num2 = (uint)oStream.BaseStream.Position;
            for (uint i = 0; i < this.FieldCount; i++)
            {
                this.Fields[i].Write(oStream, oType);
            }
            if (this.SizePadding < 0)
            {
                Prompt.Popup("CRITICAL ERROR: Negative psize of padding bytes");
            }
            if (this.SizePadding > 0)
            {
                oStream.Write(this.Padding);
            }
            this.SizeFields = (uint)oStream.BaseStream.Position - num2;
            this.Lookup.Size = this.SizeLookup;
            this.Lookup.Write(oStream, oType);
            this.SizeLookup = ((uint)oStream.BaseStream.Position - num2) - this.SizeFields;
            oStream.BaseStream.Seek(pos, SeekOrigin.Begin);
            oStream.Write(this.SizeFields);
            oStream.Write(this.SizeLookup);
            oStream.BaseStream.Seek((1 + this.SizeFields) + this.SizeLookup, SeekOrigin.Current);
        }
    }
    public class BDAT_SUBARCHIVE
    {
        public uint readStart;
        public uint readEnd;
        public uint writeStart;
        public uint writeEnd;
        public byte[] Unknown;
        public ushort SizeCompressed;
        public ushort SizeDecompressed;
        public uint FieldLookupCount;
        public BDAT_FIELDTABLE[] Fields;
        public BDAT_LOOKUPTABLE[] Lookups;

        public bool Compare(BXML_SUBARCHIVE newData)
        {
            for (int i = 0; i < this.Lookups.Length; i++)
            {
                if (this.Lookups[i].Compare(newData.lookup[i]) > 0)
                {
                    return false;
                }
            }
            return true;
        }

        public int GetSize()
        {
            int num = 0;
            for (int i = 0; i < this.Fields.Length; i++)
            {
                num += this.Fields[i].GetSize();
            }
            for (int j = 0; j < this.Lookups.Length; j++)
            {
                num += this.Lookups[j].GetSize();
            }
            return num;
        }

        public void Read(BinaryReader iStream, BDAT_TYPE iType)
        {
            this.Unknown = iStream.ReadBytes(16);
            this.readStart = (uint)iStream.BaseStream.Position;
            this.SizeCompressed = iStream.ReadUInt16();
            byte[] buffer = iStream.ReadBytes(this.SizeCompressed);
            this.readEnd = (uint)iStream.BaseStream.Position;
            this.SizeDecompressed = iStream.ReadUInt16();
            this.FieldLookupCount = iStream.ReadUInt32();
            this.Fields = new BDAT_FIELDTABLE[this.FieldLookupCount];
            this.Lookups = new BDAT_LOOKUPTABLE[this.FieldLookupCount];
            byte[] destinationArray = new byte[this.SizeDecompressed];
            Array.Copy(bcrypt.Deflate(buffer, this.SizeCompressed, this.SizeDecompressed), 0, destinationArray, 0, this.SizeDecompressed);
            BinaryReader buffer4 = new BinaryReader(new MemoryStream(destinationArray));
            ushort pos = iStream.ReadUInt16();
            for (uint k = 1; k <= this.FieldLookupCount; k++)
            {
                buffer4.BaseStream.Position = pos;
                this.Fields[(int)((IntPtr)(k - 1))] = new BDAT_FIELDTABLE();
                this.Fields[(int)((IntPtr)(k - 1))].Read(buffer4, iType);
                pos = (k >= this.FieldLookupCount) ? this.SizeDecompressed : iStream.ReadUInt16();
                this.Lookups[(int)((IntPtr)(k - 1))] = new BDAT_LOOKUPTABLE();
                this.Lookups[(int)((IntPtr)(k - 1))].Size = (uint)(pos - buffer4.BaseStream.Position);
                this.Lookups[(int)((IntPtr)(k - 1))].Read(buffer4, iType);
            }
        }

        public void UseChange(BXML_SUBARCHIVE newData, bool bIntData)
        {
            BXML_FIELDTABLE[] fields = newData.fields;
            this.FieldLookupCount = (uint)fields.Length;
            this.Fields = new BDAT_FIELDTABLE[fields.Length];
            for (int i = 0; i < fields.Length; i++)
            {
                this.Fields[i] = new BDAT_FIELDTABLE();
                this.Fields[i].UseChange(newData.fields[i], bIntData);
            }
            BXML_LOOKUPTABLE[] lookup = newData.lookup;
            this.Lookups = new BDAT_LOOKUPTABLE[lookup.Length];
            for (int j = 0; j < lookup.Length; j++)
            {
                this.Lookups[j] = new BDAT_LOOKUPTABLE();
                this.Lookups[j].UseChange(newData.lookup[j]);
            }
        }

        public void Write(BinaryWriter oStream, BDAT_TYPE oType)
        {
            MemoryStream VirginStream = new MemoryStream();
            BinaryWriter buffer = new BinaryWriter(VirginStream);
            ushort[] numArray = new ushort[this.FieldLookupCount];
            for (uint i = 1; i <= this.FieldLookupCount; i++)
            {
                this.Fields[(int)((IntPtr)(i - 1))].Write(buffer, oType);
                this.Lookups[(int)((IntPtr)(i - 1))].Write(buffer, oType);
                if (i < this.FieldLookupCount)
                {
                    numArray[i] = (ushort)buffer.BaseStream.Position;
                }
            }
            this.SizeDecompressed = (ushort)buffer.BaseStream.Length;
            if (buffer.BaseStream.Length > 0xffff)
            {
                Prompt.Popup("CRITICAL ERROR: Subarchive decompressed size overflow");
            }
            byte[] destinationArray = new byte[this.SizeDecompressed];
            Array.Copy(VirginStream.ToArray(), 0, destinationArray, 0, this.SizeDecompressed);
            uint sizeCompressed = 0;
            byte[] buffer4 = bcrypt.Inflate(destinationArray, this.SizeDecompressed, ref sizeCompressed, (uint)BDat.compress_lv);
            this.SizeCompressed = (ushort)sizeCompressed;
            oStream.Write(this.Unknown);
            this.writeStart = (uint)oStream.BaseStream.Position;
            oStream.Write(this.SizeCompressed);
            oStream.Write(buffer4);
            this.writeEnd = (uint)oStream.BaseStream.Position;
            oStream.Write(this.SizeDecompressed);
            oStream.Write(this.FieldLookupCount);
            for (uint m = 0; m < this.FieldLookupCount; m++)
            {
                oStream.Write(numArray[m]);
            }
        }
    }
}
