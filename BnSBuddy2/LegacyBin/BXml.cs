using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace BnSBuddy2.LegacyBin
{
    [XmlRoot("list")]
    public class BXML_LIST
    {
        [XmlAttribute("id")]
        public ushort id;

        [XmlAttribute("size")]
        public uint size;

        [XmlAttribute("unk1")]
        public uint unk1;

        [XmlAttribute("unk2")]
        public uint unk2;

        [XmlAttribute("unk3")]
        public uint unk3;

        public BXML_COLLECTION collection;

        public void Convert(BDAT_LIST list)
        {
            id = (ushort)list.ID;
            size = (uint)list.Size;
            unk1 = list.Unknown1;
            unk2 = (uint)list.Unknown2;
            unk3 = (uint)list.Unknown3;
            collection = new BXML_COLLECTION();
            collection.Convert(list.Collection);
        }
    }

    public enum BXML_TYPE
    {
        BXML_XML = 1,
        BXML_PLAIN,
        BXML_BINARY,
        BXML_UNKNOWN
    }

    internal class BXML_CONTENT
    {
        public byte[] XOR_KEY;

        private bool Keep_XML_WhiteSpace = true;

        private byte[] Signature;

        private int Version;

        private int FileSize;

        private byte[] Padding;

        private bool Unknown;

        private int OriginalPathLength;

        private byte[] OriginalPath;

        private int AutoID;

        private XmlDocument Nodes = new XmlDocument();

        private void Xor(byte[] buffer, int size)
        {
            for (int i = 0; i < size; i++)
            {
                buffer[i] ^= XOR_KEY[i % XOR_KEY.Length];
            }
        }

        public void Read(Stream iStream, BXML_TYPE iType)
        {
            switch (iType)
            {
                case BXML_TYPE.BXML_PLAIN:
                    {
                        Signature = new byte[8] { 76, 77, 88, 66, 79, 83, 76, 66 };
                        Version = 3;
                        FileSize = 85;
                        Padding = new byte[64];
                        Unknown = true;
                        OriginalPathLength = 0;
                        Nodes.PreserveWhitespace = Keep_XML_WhiteSpace;
                        Nodes.Load(iStream);
                        XmlNode xmlNode = null;
                        try
                        {
                            xmlNode = Nodes.DocumentElement.ChildNodes.OfType<XmlComment>().First();
                        }
                        catch
                        {
                        }
                        finally
                        {
                            if (xmlNode != null && xmlNode.NodeType == XmlNodeType.Comment)
                            {
                                string innerText = xmlNode.InnerText;
                                OriginalPathLength = innerText.Length;
                                OriginalPath = Encoding.Unicode.GetBytes(innerText);
                                Xor(OriginalPath, 2 * OriginalPathLength);
                                if (Nodes.PreserveWhitespace && xmlNode.NextSibling.NodeType == XmlNodeType.Whitespace)
                                {
                                    Nodes.DocumentElement.RemoveChild(xmlNode.NextSibling);
                                }
                            }
                            else
                            {
                                OriginalPath = new byte[2 * OriginalPathLength];
                            }
                        }
                        break;
                    }
                case BXML_TYPE.BXML_BINARY:
                    {
                        Signature = new byte[8];
                        BinaryReader binaryReader = new BinaryReader(iStream);
                        binaryReader.BaseStream.Position = 0L;
                        Signature = binaryReader.ReadBytes(8);
                        Version = binaryReader.ReadInt32();
                        FileSize = binaryReader.ReadInt32();
                        Padding = binaryReader.ReadBytes(64);
                        Unknown = binaryReader.ReadByte() == 1;
                        OriginalPathLength = binaryReader.ReadInt32();
                        OriginalPath = binaryReader.ReadBytes(2 * OriginalPathLength);
                        AutoID = 1;
                        ReadNode(iStream);
                        byte[] originalPath = OriginalPath;
                        Xor(originalPath, 2 * OriginalPathLength);
                        XmlComment newChild = Nodes.CreateComment(Encoding.Unicode.GetString(originalPath));
                        Nodes.DocumentElement.PrependChild(newChild);
                        XmlNode newChild2 = Nodes.CreateXmlDeclaration("1.0", "utf-8", null);
                        Nodes.PrependChild(newChild2);
                        if (FileSize != iStream.Position)
                        {
                            throw new Exception($"Filesize Mismatch, expected size was {FileSize} while actual size was {iStream.Position}.");
                        }
                        break;
                    }
            }
        }

        public void Write(Stream oStream, BXML_TYPE oType)
        {
            switch (oType)
            {
                case BXML_TYPE.BXML_PLAIN:
                    Nodes.Save(oStream);
                    break;
                case BXML_TYPE.BXML_BINARY:
                    {
                        BinaryWriter binaryWriter = new BinaryWriter(oStream);
                        binaryWriter.Write(Signature);
                        binaryWriter.Write(Version);
                        binaryWriter.Write(FileSize);
                        binaryWriter.Write(Padding);
                        binaryWriter.Write(Unknown);
                        binaryWriter.Write(OriginalPathLength);
                        binaryWriter.Write(OriginalPath);
                        AutoID = 1;
                        WriteNode(oStream);
                        FileSize = (int)oStream.Position;
                        oStream.Position = 12L;
                        binaryWriter.Write(FileSize);
                        break;
                    }
            }
        }

        private void ReadNode(Stream iStream, XmlNode parent = null)
        {
            XmlNode xmlNode = null;
            BinaryReader binaryReader = new BinaryReader(iStream);
            int num = 1;
            if (parent != null)
            {
                num = binaryReader.ReadInt32();
            }
            KeyValuePair<string, string>[] array = null;
            switch (num)
            {
                case 2:
                    {
                        xmlNode = Nodes.CreateTextNode("");
                        int num5 = binaryReader.ReadInt32();
                        byte[] array4 = binaryReader.ReadBytes(num5 * 2);
                        Xor(array4, 2 * num5);
                        ((XmlText)xmlNode).Value = Encoding.Unicode.GetString(array4);
                        break;
                    }
                case 1:
                    {
                        xmlNode = Nodes.CreateElement("Text");
                        int num2 = binaryReader.ReadInt32();
                        array = new KeyValuePair<string, string>[num2];
                        for (int i = 0; i < num2; i++)
                        {
                            int num3 = binaryReader.ReadInt32();
                            byte[] array2 = binaryReader.ReadBytes(2 * num3);
                            Xor(array2, 2 * num3);
                            int num4 = binaryReader.ReadInt32();
                            byte[] array3 = binaryReader.ReadBytes(2 * num4);
                            Xor(array3, 2 * num4);
                            array[i] = new KeyValuePair<string, string>(Encoding.Unicode.GetString(array2), Encoding.Unicode.GetString(array3));
                        }
                        break;
                    }
            }
            if (num > 2)
            {
                throw new Exception("Unknown XML Node Type");
            }
            binaryReader.ReadByte();
            int num6 = binaryReader.ReadInt32();
            byte[] array5 = binaryReader.ReadBytes(2 * num6);
            Xor(array5, 2 * num6);
            if (num == 1)
            {
                xmlNode = Nodes.CreateElement(Encoding.Unicode.GetString(array5));
                KeyValuePair<string, string>[] array6 = array;
                for (int j = 0; j < array6.Length; j++)
                {
                    KeyValuePair<string, string> keyValuePair = array6[j];
                    ((XmlElement)xmlNode).SetAttribute(keyValuePair.Key, keyValuePair.Value);
                }
            }
            int num7 = binaryReader.ReadInt32();
            AutoID = binaryReader.ReadInt32();
            AutoID++;
            for (int k = 0; k < num7; k++)
            {
                ReadNode(iStream, xmlNode);
            }
            if (parent != null)
            {
                if (Keep_XML_WhiteSpace || num != 2 || !string.IsNullOrWhiteSpace(xmlNode.Value))
                {
                    parent.AppendChild(xmlNode);
                }
            }
            else
            {
                Nodes.AppendChild(xmlNode);
            }
        }

        private bool WriteNode(Stream oStream, XmlNode parent = null)
        {
            BinaryWriter binaryWriter = new BinaryWriter(oStream);
            XmlNode xmlNode = null;
            int num = 1;
            if (parent != null)
            {
                xmlNode = parent;
                switch (xmlNode.NodeType)
                {
                    case XmlNodeType.Element:
                        num = 1;
                        break;
                    case XmlNodeType.Comment:
                        return false;
                    case XmlNodeType.Text:
                    case XmlNodeType.Whitespace:
                        num = 2;
                        break;
                }
                binaryWriter.Write(num);
            }
            else
            {
                xmlNode = Nodes.DocumentElement;
            }
            switch (num)
            {
                case 2:
                    {
                        string value2 = xmlNode.Value;
                        int length3 = value2.Length;
                        binaryWriter.Write(length3);
                        byte[] bytes3 = Encoding.Unicode.GetBytes(value2);
                        Xor(bytes3, 2 * length3);
                        binaryWriter.Write(bytes3);
                        break;
                    }
                case 1:
                    {
                        int num2 = (int)oStream.Position;
                        int num3 = 0;
                        binaryWriter.Write(num3);
                        foreach (object attribute in xmlNode.Attributes)
                        {
                            XmlAttribute xmlAttribute = (XmlAttribute)attribute;
                            string name = xmlAttribute.Name;
                            int length = name.Length;
                            binaryWriter.Write(length);
                            byte[] bytes = Encoding.Unicode.GetBytes(name);
                            Xor(bytes, 2 * length);
                            binaryWriter.Write(bytes);
                            string value = xmlAttribute.Value;
                            int length2 = value.Length;
                            binaryWriter.Write(length2);
                            byte[] bytes2 = Encoding.Unicode.GetBytes(value);
                            Xor(bytes2, 2 * length2);
                            binaryWriter.Write(bytes2);
                            num3++;
                        }
                        int num4 = (int)oStream.Position;
                        oStream.Position = num2;
                        binaryWriter.Write(num3);
                        oStream.Position = num4;
                        break;
                    }
            }
            if (num > 2)
            {
                throw new Exception($"ERROR: XML NODE TYPE [{xmlNode.NodeType.ToString()}] UNKNOWN");
            }
            bool value3 = true;
            binaryWriter.Write(value3);
            string name2 = xmlNode.Name;
            int length4 = name2.Length;
            binaryWriter.Write(length4);
            byte[] bytes4 = Encoding.Unicode.GetBytes(name2);
            Xor(bytes4, 2 * length4);
            binaryWriter.Write(bytes4);
            int num5 = (int)oStream.Position;
            int num6 = 0;
            binaryWriter.Write(num6);
            binaryWriter.Write(AutoID);
            AutoID++;
            foreach (object childNode in xmlNode.ChildNodes)
            {
                XmlNode parent2 = (XmlNode)childNode;
                if (WriteNode(oStream, parent2))
                {
                    num6++;
                }
            }
            int num7 = (int)oStream.Position;
            oStream.Position = num5;
            binaryWriter.Write(num6);
            oStream.Position = num7;
            return true;
        }
    }

    public class BXML_COLLECTION
    {
        [XmlAttribute("compressed")]
        public byte compressed;

        [XmlElement("loose", IsNullable = true)]
        public BXML_LOOSE loose;

        [XmlElement("archive", IsNullable = true)]
        public BXML_ARCHIVE archive;

        public void Convert(BDAT_COLLECTION list)
        {
            compressed = list.Compressed;
            if (compressed >= 1)
            {
                archive = new BXML_ARCHIVE();
                archive.Convert(list.Archive);
            }
            else
            {
                loose = new BXML_LOOSE();
                loose.Convert(list.Loose);
            }
        }
    }

    public class BXML_ARCHIVE
    {
        [XmlAttribute("count")]
        public int count;

        public BXML_SUBARCHIVE[] SubArchives;

        public void Convert(BDAT_ARCHIVE barchive)
        {
            count = barchive.SubArchives.Length;
            SubArchives = new BXML_SUBARCHIVE[barchive.SubArchives.Length];
            for (int i = 0; i < barchive.SubArchives.Length; i++)
            {
                SubArchives[i] = new BXML_SUBARCHIVE();
                SubArchives[i].Convert(barchive.SubArchives[i]);
            }
        }
    }

    public class BXML_SUBARCHIVE
    {
        [XmlAttribute("fieldLookupCount")]
        public uint FieldLookupCount;

        public BXML_FIELDTABLE[] fields;

        public BXML_LOOKUPTABLE[] lookup;

        public void Convert(BDAT_SUBARCHIVE bsubarchive)
        {
            FieldLookupCount = (uint)bsubarchive.FieldLookupCount;
            fields = new BXML_FIELDTABLE[bsubarchive.Fields.Length];
            lookup = new BXML_LOOKUPTABLE[bsubarchive.Lookups.Length];
            for (int i = 0; i < bsubarchive.Fields.Length; i++)
            {
                fields[i] = new BXML_FIELDTABLE();
                fields[i].Convert(bsubarchive.Fields[i]);
            }
            for (int j = 0; j < bsubarchive.Lookups.Length; j++)
            {
                lookup[j] = new BXML_LOOKUPTABLE();
                lookup[j].Convert(bsubarchive.Lookups[j]);
            }
        }
    }

    public class BXML_LOOSE
    {
        [XmlAttribute("countFieldsUnfixed")]
        public uint countFieldsUnfixed;

        [XmlAttribute("countFields")]
        public uint countFields;

        [XmlAttribute("sizeFields")]
        public uint sizeFields;

        [XmlAttribute("sizePadding")]
        public int sizePadding;

        [XmlAttribute("sizeLookup")]
        public uint sizeLookup;

        [XmlAttribute("unk")]
        public uint unk;

        public BXML_FIELDTABLE[] fields;

        public string padding;

        public BXML_LOOKUPTABLE lookup;

        public static int index;

        public void Convert(BDAT_LOOSE loose)
        {
            countFieldsUnfixed = (uint)loose.FieldCountUnfixed;
            countFields = (uint)loose.FieldCount;
            sizeFields = (uint)loose.SizeFields;
            sizePadding = loose.SizePadding;
            sizeLookup = (uint)loose.SizeLookup;
            unk = loose.Unknown;
            fields = new BXML_FIELDTABLE[loose.Fields.Length];
            for (int i = 0; i < loose.Fields.Length; i++)
            {
                BDAT_FIELDTABLE bfield = loose.Fields[i];
                if (loose.Fields[i] == null)
                {
                    fields[i] = null;
                    continue;
                }
                fields[i] = new BXML_FIELDTABLE();
                fields[i].Convert(bfield);
            }
            padding = bcrypt.BytesToHex(loose.Padding, (uint)loose.SizePadding);
            lookup = new BXML_LOOKUPTABLE();
            if (loose.Lookup != null)
            {
                lookup.Convert(loose.Lookup);
            }
        }
    }

    [XmlType(TypeName = "field")]
    public class BXML_FIELDTABLE
    {
        [XmlAttribute("id")]
        public uint id;

        [XmlAttribute("size")]
        public uint size;

        [XmlAttribute("unk1")]
        public ushort unk1;

        [XmlAttribute("unk2")]
        public ushort unk2;

        public string data;

        public static int index;

        public void Convert(BDAT_FIELDTABLE bfield)
        {
            id = (uint)bfield.ID;
            size = (uint)bfield.Size;
            unk1 = (ushort)bfield.Unknown1;
            unk2 = (ushort)bfield.Unknown2;
            if (bfield.Size >= 12 && bfield.Data != null)
            {
                // Payload is Size - 12 (header is unk1/unk2/size/id); helper size args are unused for length
                uint payloadHint = (uint)Math.Max(0, bfield.Size - 12);
                bool useInt = BinEditOptions.UseIntData;
                if (useInt)
                {
                    data = bcrypt.BytesToInt(bfield.Data, payloadHint);
                }
                else
                {
                    data = bcrypt.BytesToHex(bfield.Data, payloadHint);
                }
            }
            else
            {
                data = string.Empty;
            }
        }
    }

    public class BXML_LOOKUPTABLE
    {
        [XmlAttribute("count")]
        public int count;

        [XmlAttribute("empty_count")]
        public int empty_count;

        [XmlAttribute("reall_count")]
        public int reall_count;

        public string[] words;

        public void Convert(BDAT_LOOKUPTABLE bLookup)
        {
            if (bLookup == null || bLookup.Data == null || bLookup.Size <= 0)
            {
                count = 0;
                empty_count = 0;
                reall_count = 0;
                words = new string[0];
                return;
            }
            List<string> list = bnsTool.LookupSplitToWords(bLookup.Data, (uint)bLookup.Size);
            count = list.Count;
            words = new string[list.Count];
            Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
            List<string> list2 = new List<string>();
            int num = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (string.IsNullOrEmpty(list[i]) | (list[i] == "invalidzhangjieyong"))
                {
                    num++;
                    // Keep empty slots as empty strings so round-trip preserves word count/indices
                    words[i] = list[i] ?? string.Empty;
                    continue;
                }
                words[i] = list[i];
                if (!dictionary.ContainsKey(list[i]))
                {
                    dictionary.Add(list[i], value: true);
                    list2.Add(list[i]);
                }
            }
            empty_count = num;
            reall_count = list2.Count;
        }
    }
}
