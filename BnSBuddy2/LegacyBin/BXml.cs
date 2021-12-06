using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Revamped_BnS_Buddy.LegacyBin
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

        public void Convert(BDAT_LIST list, bool bIntData)
        {
            this.id = list.ID;
            this.size = list.Size;
            this.unk1 = list.Unknown1;
            this.unk2 = list.Unknown2;
            this.unk3 = list.Unknown3;
            this.collection = new BXML_COLLECTION();
            this.collection.Convert(list.Collection, bIntData);
        }
    }
    public class BXML_COLLECTION
    {
        [XmlAttribute("compressed")]
        public byte compressed;
        public BXML_LOOSE loose;
        public BXML_ARCHIVE archive;

        public void Convert(BDAT_COLLECTION list, bool bIntData)
        {
            this.compressed = list.Compressed;
            if (this.compressed >= 1)
            {
                this.archive = new BXML_ARCHIVE();
                this.archive.Convert(list.Archive, bIntData);
            }
            else
            {
                this.loose = new BXML_LOOSE();
                this.loose.Convert(list.Loose, bIntData);
            }
        }

        public bool ShouldSerializearchive()
        {
            return (this.compressed >= 1);
        }

        public bool ShouldSerializeloose()
        {
            return (this.compressed < 1);
        }
    }
    public class BXML_ARCHIVE
    {
        [XmlAttribute("count")]
        public int count;
        public BXML_SUBARCHIVE[] SubArchives;

        public void Convert(BDAT_ARCHIVE barchive, bool bIntData)
        {
            this.count = barchive.SubArchives.Length;
            this.SubArchives = new BXML_SUBARCHIVE[barchive.SubArchives.Length];
            for (int i = 0; i < barchive.SubArchives.Length; i++)
            {
                this.SubArchives[i] = new BXML_SUBARCHIVE();
                this.SubArchives[i].Convert(barchive.SubArchives[i], bIntData);
            }
        }

        public void Dump()
        {
            string[] words = this.SubArchives[0].lookup[0].words;
            for (int i = 0; i < words.Length; i++)
            {
                object[] objArray1 = new object[] { "BXML_ARCHIVE words[", i, "]", words[i] };
            }
        }
    }
    public class BXML_SUBARCHIVE
    {
        [XmlAttribute("fieldLookupCount")]
        public uint FieldLookupCount;
        public BXML_FIELDTABLE[] fields;
        public BXML_LOOKUPTABLE[] lookup;

        public void Convert(BDAT_SUBARCHIVE bsubarchive, bool bIntData)
        {
            this.FieldLookupCount = bsubarchive.FieldLookupCount;
            this.fields = new BXML_FIELDTABLE[bsubarchive.Fields.Length];
            this.lookup = new BXML_LOOKUPTABLE[bsubarchive.Lookups.Length];
            for (int i = 0; i < bsubarchive.Fields.Length; i++)
            {
                this.fields[i] = new BXML_FIELDTABLE();
                this.fields[i].Convert(bsubarchive.Fields[i], bIntData);
            }
            for (int j = 0; j < bsubarchive.Lookups.Length; j++)
            {
                this.lookup[j] = new BXML_LOOKUPTABLE();
                this.lookup[j].Convert(bsubarchive.Lookups[j], bIntData);
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

        public void Convert(BDAT_LOOSE loose, bool bIntData)
        {
            this.countFieldsUnfixed = loose.FieldCountUnfixed;
            this.countFields = loose.FieldCount;
            this.sizeFields = loose.SizeFields;
            this.sizePadding = loose.SizePadding;
            this.sizeLookup = loose.SizeLookup;
            this.unk = loose.Unknown;
            this.fields = new BXML_FIELDTABLE[loose.Fields.Length];
            for (int i = 0; i < loose.Fields.Length; i++)
            {
                BDAT_FIELDTABLE bfield = loose.Fields[i];
                if (loose.Fields[i] == null)
                {
                    this.fields[i] = null;
                }
                else
                {
                    this.fields[i] = new BXML_FIELDTABLE();
                    this.fields[i].Convert(bfield, bIntData);
                }
            }
            this.padding = bcrypt.BytesToHex(loose.Padding, (uint)loose.SizePadding);
            this.lookup = new BXML_LOOKUPTABLE();
            this.lookup.Convert(loose.Lookup, bIntData);
        }

        public void Dump()
        {
        }
    }
    [XmlType(TypeName = "field")]
    public class BXML_FIELDTABLE
    {
        [XmlAttribute("size")]
        public uint size;
        [XmlAttribute("unk1")]
        public ushort unk1;
        [XmlAttribute("unk2")]
        public ushort unk2;
        public string data;
        public uint ID;
        public static int index;

        public void Convert(BDAT_FIELDTABLE bfield, bool bIntData)
        {
            this.size = bfield.Size;
            this.unk1 = bfield.Unknown1;
            this.unk2 = bfield.Unknown2;
            this.ID = bfield.ID;
            if (bIntData)
            {
                this.data = bcrypt.BytesToInt(bfield.Data, bfield.Size - 8);
            }
            else
            {
                this.data = bcrypt.BytesToHex(bfield.Data, bfield.Size - 8);
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

        public void Convert(BDAT_LOOKUPTABLE bLookup, bool bIntData)
        {
            List<string> splitToWords = bnsTool.LookupSplitToWords(bLookup.Data, bLookup.Size);
            this.count = splitToWords.Count;
            this.words = new string[splitToWords.Count];
            Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
            List<string> list2 = new List<string>();
            int num = 0;
            for (int i = 0; i < splitToWords.Count; i++)
            {
                if (string.IsNullOrEmpty(splitToWords[i]))
                {
                    num++;
                }
                else
                {
                    this.words[i] = splitToWords[i];
                    if (!dictionary.ContainsKey(splitToWords[i]))
                    {
                        dictionary.Add(splitToWords[i], true);
                        list2.Add(splitToWords[i]);
                    }
                }
            }
            this.empty_count = num;
            this.reall_count = list2.Count;
        }
    }
}
