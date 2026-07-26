using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;


namespace BnSBuddy2.LegacyBin
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

        private void WriteString(BinaryWriter writer, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                writer.Write((byte)0); // Empty string
                return;
            }
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            if (bytes.Length > 255) throw new Exception("String too long for 1-byte length");
            writer.Write((byte)bytes.Length);
            writer.Write(bytes);
        }

        /// <summary>
        /// Buddy-specific: detect FAQ / common / command table indices on the shared BinHandler content.
        /// </summary>
        public void DetectIndices()
        {
            _indexFaqs = -1;
            _indexCommons = -1;
            _indexCommands = -1;
            for (int i = 0; i < BinHandler._content.ListCount; i++)
            {
                BDAT_LIST bDAT_LIST = BinHandler._content.Lists[i];
                if (bDAT_LIST.Unknown1 == 2 && bDAT_LIST.Unknown2 == 0)
                {
                    int num = 0;
                    if (bDAT_LIST.Collection.Archive != null && bDAT_LIST.Collection.Archive.SubArchiveCount > 0 && bDAT_LIST.Collection.Archive.SubArchives[0].FieldLookupCount > 0)
                    {
                        num = bDAT_LIST.Collection.Archive.SubArchives[0].Fields[0].Size;
                    }
                    else if (bDAT_LIST.Collection.Loose != null && bDAT_LIST.Collection.Loose.FieldCount > 0)
                    {
                        num = bDAT_LIST.Collection.Loose.Fields[0].Size;
                    }
                    if (num == 32 || num == 40)
                    {
                        _indexFaqs = i;
                    }
                }
                if (bDAT_LIST.Size > 5000000 && bDAT_LIST.Unknown1 == 1 && bDAT_LIST.Unknown2 == 0)
                {
                    int num = 0;
                    if (bDAT_LIST.Collection.Archive != null && bDAT_LIST.Collection.Archive.SubArchiveCount > 0 && bDAT_LIST.Collection.Archive.SubArchives[0].FieldLookupCount > 0)
                    {
                        num = bDAT_LIST.Collection.Archive.SubArchives[0].Fields[0].Size;
                    }
                    else if (bDAT_LIST.Collection.Loose != null && bDAT_LIST.Collection.Loose.FieldCount > 0)
                    {
                        num = bDAT_LIST.Collection.Loose.Fields[0].Size;
                    }
                    if (num == 28 || num == 36)
                    {
                        _indexCommons = i;
                    }
                }
                if (_indexCommons > -1 && _indexCommons < i && bDAT_LIST.Unknown1 == 1 && bDAT_LIST.Unknown2 == 0)
                {
                    int num = 0;
                    if (bDAT_LIST.Collection.Archive != null && bDAT_LIST.Collection.Archive.SubArchiveCount > 0 && bDAT_LIST.Collection.Archive.SubArchives[0].FieldLookupCount > 0)
                    {
                        num = bDAT_LIST.Collection.Archive.SubArchives[0].Fields[0].Size;
                    }
                    else if (bDAT_LIST.Collection.Loose != null && bDAT_LIST.Collection.Loose.FieldCount > 0)
                    {
                        num = bDAT_LIST.Collection.Loose.Fields[0].Size;
                    }
                    if (num == 28)
                    {
                        _indexCommands = i;
                    }
                }
            }
        }
    }

    public class BDAT_ARCHIVE
    {
        public int SubArchiveCount;

        public int Unknown;

        public BDAT_SUBARCHIVE[] SubArchives;

        public void Read(BinaryReader br)
        {
            SubArchiveCount = br.ReadInt32();
            Unknown = Ultily.ReadIntFrom2Bytes(br.ReadBytes(2));
            SubArchives = new BDAT_SUBARCHIVE[SubArchiveCount];
            for (int i = 0; i < SubArchiveCount; i++)
            {
                SubArchives[i] = new BDAT_SUBARCHIVE();
                SubArchives[i].Read(br);
            }
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(SubArchiveCount);
            bw.Write(Ultily.WriteIntTo2Bytes(Unknown));
            for (int i = 0; i < SubArchiveCount; i++)
            {
                SubArchives[i].Write(bw);
            }
        }

        public void UseChange(BXML_ARCHIVE newData)
        {
            for (int i = 0; i < SubArchives.Length; i++)
            {
                SubArchives[i].UseChange(newData.SubArchives[i]);
            }
        }

        public bool Compare(BXML_ARCHIVE newData)
        {
            for (int i = 0; i < SubArchives.Length; i++)
            {
                if (!SubArchives[i].Compare(newData.SubArchives[i]))
                {
                    return false;
                }
            }
            return true;
        }
    }

    public class BDAT_COLLECTION
    {
        public byte Compressed;

        public byte Deprecated;

        public BDAT_ARCHIVE Archive;

        public BDAT_LOOSE Loose;

        public void Read(BinaryReader br)
        {
            Compressed = br.ReadByte();
            if (Convert.ToBoolean(Compressed))
            {
                if (Compressed > 1)
                {
                    br.BaseStream.Seek(br.BaseStream.Position - 1, SeekOrigin.Begin);
                }
                Archive = new BDAT_ARCHIVE();
                Archive.Read(br);
                Loose = null;
                if (Compressed > 1)
                {
                    Deprecated = br.ReadByte();
                }
            }
            else
            {
                Loose = new BDAT_LOOSE();
                Loose.Read(br);
                Archive = null;
            }
        }

        /// <param name="elementCount">Table element count from list header. 64-bit loose tables with elementCount==1 pad FieldCount to 8 bytes.</param>
        public void Read64(BinaryReader br, byte elementCount)
        {
            Compressed = br.ReadByte();
            if (Convert.ToBoolean(Compressed))
            {
                if (Compressed > 1)
                {
                    br.BaseStream.Seek(br.BaseStream.Position - 1, SeekOrigin.Begin);
                }
                // Compressed blocks use the same layout on 32/64
                Archive = new BDAT_ARCHIVE();
                Archive.Read(br);
                Loose = null;
                if (Compressed > 1)
                {
                    Deprecated = br.ReadByte();
                }
            }
            else
            {
                Loose = new BDAT_LOOSE();
                Loose.Read64(br, elementCount);
                Archive = null;
            }
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(Compressed);
            if (Convert.ToBoolean(Compressed))
            {
                if (Compressed > 1)
                {
                    bw.BaseStream.Seek(bw.BaseStream.Position - 1, SeekOrigin.Begin);
                }
                Archive.Write(bw);
                if (Compressed > 1)
                {
                    bw.Write(Deprecated);
                }
            }
            else
            {
                Loose.Write(bw);
            }
        }

        public void Write64(BinaryWriter bw)
        {
            bw.Write(Compressed);
            if (Convert.ToBoolean(Compressed))
            {
                if (Compressed > 1)
                {
                    bw.BaseStream.Seek(bw.BaseStream.Position - 1, SeekOrigin.Begin);
                }
                Archive.Write(bw);
                if (Compressed > 1)
                {
                    bw.Write(Deprecated);
                }
            }
            else
            {
                Loose.Write64(bw);
            }
        }
    }

    public class BDAT_CONTENT
    {
        public byte[] Signature;

        public int Version;

        public byte[] Unknown;

        public int ListCount;

        public BDAT_HEAD HeadList;

        public BDAT_LIST[] Lists;

        /// <summary>True when this content was read (or should be written) as a 64-bit BNS bin.</summary>
        public bool Is64Bit;

        /// <summary>
        /// Detect 32 vs 64-bit datafile/localfile layout from the header.
        /// 32-bit uses 4-byte size fields after version; 64-bit uses 8-byte size fields.
        /// </summary>
        public static bool DetectIs64Bit(BinaryReader br)
        {
            long saved = br.BaseStream.Position;
            try
            {
                if (br.BaseStream.Length < 48)
                {
                    return false;
                }

                br.BaseStream.Position = 0;
                br.ReadBytes(8); // magic
                br.ReadBytes(9); // datafile version (1) + client version (8)

                long afterVersion = br.BaseStream.Position;

                // Interpret as 32-bit: TotalTableSize(int32), ListCount(int32)
                int total32 = br.ReadInt32();
                int count32 = br.ReadInt32();
                bool looks32 = count32 >= 1 && count32 <= 512 && total32 > 0;

                // Interpret as 64-bit: TotalTableSize(int64), ListCount(int64)
                br.BaseStream.Position = afterVersion;
                long total64 = br.ReadInt64();
                long count64 = br.ReadInt64();
                bool looks64 = count64 >= 1 && count64 <= 512 && total64 > 0;

                // On a real 32-bit file, the 64-bit "list count" is TotalTableSize|(ListCount<<32) and is huge.
                if (looks64 && !looks32)
                {
                    return true;
                }
                if (looks32 && !looks64)
                {
                    return false;
                }
                if (looks64 && looks32)
                {
                    // Ambiguous: prefer 64 when TotalTableSize does not fit in 32 bits
                    return total64 > int.MaxValue;
                }
                return false;
            }
            finally
            {
                br.BaseStream.Position = saved;
            }
        }

        public void Read(BinaryReader br)
        {
            Is64Bit = false;
            Signature = br.ReadBytes(8);
            Version = br.ReadInt32();
            Unknown = br.ReadBytes(9);
            ListCount = br.ReadInt32();
            HeadList = new BDAT_HEAD();
            // Name table is present only when table count > 10 (matches BNS client / BnsBinTool)
            HeadList.Complement = ListCount <= 10;
            HeadList.Read(br);
            Lists = new BDAT_LIST[ListCount];
            for (int i = 0; i < ListCount; i++)
            {
                Lists[i] = new BDAT_LIST();
                Lists[i].Read(br);
            }
        }

        public void Read64(BinaryReader br)
        {
            Is64Bit = true;
            BinEditOptions.Report("BDAT_CONTENT (64-bit)...");
            Signature = br.ReadBytes(8);
            Version = br.ReadInt32();
            // 64-bit: version byte + client version + start of 8-byte TotalTableSize already partially in Version/Unknown
            // Unknown is 13 bytes so Version(4)+Unknown(13) = datafileVersion(1)+clientVersion(8)+TotalTableSize(8)
            Unknown = br.ReadBytes(13);
            ListCount = (int)br.ReadInt64();
            HeadList = new BDAT_HEAD();
            HeadList.Complement = ListCount <= 10;
            BinEditOptions.Report("BDAT_HEAD (64-bit)...");
            HeadList.Read64(br);
            Lists = new BDAT_LIST[ListCount];
            BinEditOptions.Report("BDAT_LIST (64-bit)...");
            for (int i = 0; i < ListCount; i++)
            {
                if (i % 10 == 0 || i == ListCount - 1)
                {
                    BinEditOptions.Report("BDAT_LIST " + (i + 1) + "/" + ListCount);
                }
                Lists[i] = new BDAT_LIST();
                Lists[i].Read64(br);
            }
        }

        public void Write(BinaryWriter bw)
        {
            Is64Bit = false;
            bw.Write(Signature);
            bw.Write(Version);
            bw.Write(Unknown);
            bw.Write(ListCount);
            HeadList.Write(bw);
            for (int i = 0; i < ListCount; i++)
            {
                Lists[i].Write(bw);
            }
        }

        public void Write64(BinaryWriter bw)
        {
            Is64Bit = true;
            bw.Write(Signature);
            bw.Write(Version);
            bw.Write(Unknown);
            bw.Write((long)ListCount);
            HeadList.Write64(bw);
            for (int i = 0; i < ListCount; i++)
            {
                Lists[i].Write64(bw);
            }
        }
    }

    public class BDAT_FIELDTABLE
    {
        public int Unknown1;

        public int Unknown2;

        public int Size;

        public byte[] Data;

        public int ID;

        public void Read(BinaryReader br)
        {
            Unknown1 = Ultily.ReadIntFrom2Bytes(br.ReadBytes(2));
            Unknown2 = Ultily.ReadIntFrom2Bytes(br.ReadBytes(2));
            // Unknown1 == 255 => size is stored as uint16; otherwise as int32
            if (Unknown1 == 255)
            {
                Size = Ultily.ReadIntFrom2Bytes(br.ReadBytes(2));
            }
            else
            {
                Size = br.ReadInt32();
            }
            if (Size >= 12)
            {
                ID = br.ReadInt32();
                Data = br.ReadBytes(Size - 12);
            }
            else
            {
                Data = new byte[0];
            }
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(Ultily.WriteIntTo2Bytes(Unknown1));
            bw.Write(Ultily.WriteIntTo2Bytes(Unknown2));
            // Mirror Read: size width depends on Unknown1, not on Size alone
            if (Unknown1 == 255)
            {
                bw.Write(Ultily.WriteIntTo2Bytes(Size));
            }
            else
            {
                bw.Write(Size);
            }
            if (Size >= 12)
            {
                bw.Write(ID);
                if (Data != null && Data.Length > 0)
                {
                    bw.Write(Data);
                }
            }
        }

        public void UseChange(BXML_FIELDTABLE newData)
        {
            if (newData == null)
            {
                // Keep a writable zero-size placeholder (8-byte header), not a skipped null slot
                ID = 0;
                Unknown1 = 0;
                Unknown2 = 0;
                Size = 0;
                Data = new byte[0];
                return;
            }
            ID = (int)newData.id;
            Unknown1 = newData.unk1;
            Unknown2 = newData.unk2;
            Size = (int)newData.size;
            if (Size >= 12)
            {
                bool useInt = BinEditOptions.UseIntData;
                if (useInt)
                {
                    Data = bcrypt.IntToBytes(newData.data, newData.size - 8);
                }
                else
                {
                    Data = string.IsNullOrEmpty(newData.data)
                        ? new byte[0]
                        : bcrypt.HexToBytes(newData.data, newData.size - 8);
                }
            }
            else
            {
                // Size==0 placeholders still occupy a field header in the bin — do not null out
                Data = new byte[0];
            }
        }

        public bool IsEmpty()
        {
            return Data == null;
        }
    }

    public class BDAT_HEAD
    {
        public bool Complement;

        public int Size_1;

        public int Size_2;

        public int Size_3;

        public byte[] Padding;

        public byte[] Data;

        public void Read(BinaryReader br)
        {
            Size_1 = br.ReadInt32();
            Size_2 = br.ReadInt32();
            Size_3 = br.ReadInt32();
            Padding = br.ReadBytes(62);
            Data = new byte[Size_1];
            if (!Complement)
            {
                Data = br.ReadBytes(Size_1);
            }
        }

        public void Read64(BinaryReader br)
        {
            Size_1 = (int)br.ReadInt64();
            Size_2 = (int)br.ReadInt64();
            Size_3 = (int)br.ReadInt64();
            Padding = br.ReadBytes(62);
            Data = new byte[Size_1];
            if (!Complement)
            {
                Data = br.ReadBytes(Size_1);
            }
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(Size_1);
            bw.Write(Size_2);
            bw.Write(Size_3);
            bw.Write(Padding);
            if (!Complement)
            {
                bw.Write(Data);
            }
        }

        public void Write64(BinaryWriter bw)
        {
            bw.Write((long)Size_1);
            bw.Write((long)Size_2);
            bw.Write((long)Size_3);
            bw.Write(Padding);
            if (!Complement)
            {
                bw.Write(Data);
            }
        }
    }

    public class BDAT_LIST
    {
        public byte Unknown1;

        public int ID;

        public int Unknown2;

        public int Unknown3;

        public int Size;

        public BDAT_COLLECTION Collection;

        /// <summary>
        /// Bytes after the logical collection payload up to the declared Size.
        /// Official bins often pad each table with trailing zeros; clients can be picky if these are dropped.
        /// </summary>
        public byte[] TrailingPadding;

        private void CaptureTrailingPadding(BinaryReader br, long payloadStart)
        {
            long afterContent = br.BaseStream.Position;
            long expectedEnd = payloadStart + Size;
            if (expectedEnd > afterContent)
            {
                int padLen = (int)(expectedEnd - afterContent);
                TrailingPadding = br.ReadBytes(padLen);
            }
            else
            {
                TrailingPadding = null;
                if (expectedEnd < afterContent)
                {
                    // Collection over-read; snap to declared end so next table aligns
                    br.BaseStream.Seek(expectedEnd, SeekOrigin.Begin);
                }
            }
        }

        private void WriteTrailingPadding(BinaryWriter bw)
        {
            if (TrailingPadding != null && TrailingPadding.Length > 0)
            {
                bw.Write(TrailingPadding);
            }
        }

        public void Read(BinaryReader br)
        {
            Unknown1 = br.ReadByte();
            ID = Ultily.ReadIntFrom2Bytes(br.ReadBytes(2));
            Unknown2 = Ultily.ReadIntFrom2Bytes(br.ReadBytes(2));
            Unknown3 = Ultily.ReadIntFrom2Bytes(br.ReadBytes(2));
            Size = br.ReadInt32();
            long position = br.BaseStream.Position;
            Collection = new BDAT_COLLECTION();
            Collection.Read(br);
            CaptureTrailingPadding(br, position);
        }

        public void Read64(BinaryReader br)
        {
            Unknown1 = br.ReadByte(); // ElementCount
            ID = Ultily.ReadIntFrom2Bytes(br.ReadBytes(2));
            Unknown2 = Ultily.ReadIntFrom2Bytes(br.ReadBytes(2));
            Unknown3 = Ultily.ReadIntFrom2Bytes(br.ReadBytes(2));
            Size = br.ReadInt32(); // table payload size is still 32-bit
            long position = br.BaseStream.Position;
            Collection = new BDAT_COLLECTION();
            Collection.Read64(br, Unknown1);
            CaptureTrailingPadding(br, position);
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(Unknown1);
            bw.Write(Ultily.WriteIntTo2Bytes(ID));
            bw.Write(Ultily.WriteIntTo2Bytes(Unknown2));
            bw.Write(Ultily.WriteIntTo2Bytes(Unknown3));
            long sizePos = bw.BaseStream.Position;
            bw.Write(Size);
            long position = bw.BaseStream.Position;
            Collection.Write(bw);
            WriteTrailingPadding(bw);
            long position2 = bw.BaseStream.Position;
            Size = (int)(position2 - position);
            long end = bw.BaseStream.Position;
            bw.BaseStream.Seek(sizePos, SeekOrigin.Begin);
            bw.Write(Size);
            bw.BaseStream.Seek(end, SeekOrigin.Begin);
        }

        public void Write64(BinaryWriter bw)
        {
            bw.Write(Unknown1);
            bw.Write(Ultily.WriteIntTo2Bytes(ID));
            bw.Write(Ultily.WriteIntTo2Bytes(Unknown2));
            bw.Write(Ultily.WriteIntTo2Bytes(Unknown3));
            long sizePos = bw.BaseStream.Position;
            bw.Write(Size);
            long position = bw.BaseStream.Position;
            Collection.Write64(bw);
            WriteTrailingPadding(bw);
            long position2 = bw.BaseStream.Position;
            Size = (int)(position2 - position);
            long end = bw.BaseStream.Position;
            bw.BaseStream.Seek(sizePos, SeekOrigin.Begin);
            bw.Write(Size);
            bw.BaseStream.Seek(end, SeekOrigin.Begin);
        }
    }

    public class BDAT_LOOKUPTABLE
    {
        public int Size;

        public byte[] Data;

        public void Read(BinaryReader br)
        {
            Data = br.ReadBytes(Size);
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(Data);
        }

        public int UseChange(BXML_LOOKUPTABLE newData)
        {
            if (newData == null || newData.words == null)
            {
                Data = Data ?? new byte[0];
                Size = Data.Length;
                return Size;
            }
            int SizeLookup = 0;
            Data = bnsTool.WordToLookUpData(newData.words, ref SizeLookup);
            Size = SizeLookup;
            return SizeLookup;
        }

        public int Compare(BXML_LOOKUPTABLE newData)
        {
            if (newData == null || newData.words == null)
            {
                // Nothing to compare from XML — treat as OK if we have no lookup either
                return (Data == null || Data.Length == 0) ? 0 : 1;
            }
            if (Data == null)
            {
                return newData.words.Length == 0 ? 0 : 1;
            }
            int SizeLookup = 0;
            byte[] array = bnsTool.WordToLookUpData(newData.words, ref SizeLookup);
            if (SizeLookup != Size)
            {
                return 1;
            }
            for (int i = 0; i < Size; i++)
            {
                if (Data[i] != array[i])
                {
                    return 2;
                }
            }
            return 0;
        }
    }
    public class BDAT_LOOSE
    {
        public int FieldCountUnfixed;

        public int FieldCount;

        public int SizeFields;

        public int SizeLookup;

        public byte Unknown;

        public BDAT_FIELDTABLE[] Fields;

        public int SizePadding;

        public byte[] Padding;

        public BDAT_LOOKUPTABLE Lookup;

        public bool Is64;

        /// <summary>
        /// Minimum bytes needed to start a field header (unk1 + unk2 + size16).
        /// Declared FieldCount can exceed what fits in SizeFields; never read past the field region.
        /// </summary>
        private const int MinFieldHeaderBytes = 6;

        private void ReadFieldsAndLookup(BinaryReader br)
        {
            long fieldsEnd = br.BaseStream.Position + SizeFields;
            Fields = new BDAT_FIELDTABLE[FieldCount];
            int actualCount = 0;
            for (int i = 0; i < FieldCount; i++)
            {
                long position = br.BaseStream.Position;
                long remaining = fieldsEnd - position;
                if (remaining < MinFieldHeaderBytes)
                {
                    break;
                }

                long fieldStart = position;
                var field = new BDAT_FIELDTABLE();
                field.Read(br);
                if (br.BaseStream.Position > fieldsEnd)
                {
                    // Partial/false field that spilled into padding or lookup — rewind
                    br.BaseStream.Seek(fieldStart, SeekOrigin.Begin);
                    break;
                }

                Fields[i] = field;
                actualCount++;
            }

            FieldCount = actualCount;
            if (Fields.Length != actualCount)
            {
                Array.Resize(ref Fields, actualCount);
            }

            long afterFields = br.BaseStream.Position;
            if (afterFields < fieldsEnd)
            {
                SizePadding = (int)(fieldsEnd - afterFields);
                Padding = br.ReadBytes(SizePadding);
            }
            else
            {
                SizePadding = 0;
                Padding = null;
                if (afterFields > fieldsEnd)
                {
                    br.BaseStream.Seek(fieldsEnd, SeekOrigin.Begin);
                }
            }

            Lookup = new BDAT_LOOKUPTABLE();
            Lookup.Size = SizeLookup;
            if (SizeLookup > 0)
            {
                Lookup.Read(br);
            }
            else
            {
                Lookup.Data = new byte[0];
            }
        }

        public void Read(BinaryReader br)
        {
            Is64 = false;
            FieldCount = br.ReadInt32();
            FieldCountUnfixed = FieldCount;
            SizeFields = br.ReadInt32();
            SizeLookup = br.ReadInt32();
            Unknown = br.ReadByte();
            ReadFieldsAndLookup(br);
        }

        /// <summary>
        /// 64-bit uncompressed tables: when elementCount == 1, FieldCount is followed by an extra int32 (0).
        /// SizeFields/SizeLookup remain 32-bit. Matches official BNS x64 datafile layout.
        /// </summary>
        public void Read64(BinaryReader br, byte elementCount)
        {
            FieldCount = br.ReadInt32();
            FieldCountUnfixed = FieldCount;
            // Only single-element loose tables pad FieldCount to 8 bytes on 64-bit
            Is64 = elementCount == 1;
            if (Is64)
            {
                br.ReadInt32(); // padding (always 0)
            }
            SizeFields = br.ReadInt32();
            SizeLookup = br.ReadInt32();
            Unknown = br.ReadByte();
            ReadFieldsAndLookup(br);
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(FieldCountUnfixed);
            long sizesPos = bw.BaseStream.Position;
            bw.Write(SizeFields);
            bw.Write(SizeLookup);
            bw.Write(Unknown);
            long fieldsStart = bw.BaseStream.Position;
            int writeCount = Fields != null ? Fields.Length : FieldCount;
            for (int i = 0; i < writeCount; i++)
            {
                if (Fields[i] != null)
                {
                    Fields[i].Write(bw);
                }
            }
            if (SizePadding >= 0)
            {
                if (SizePadding > 0 && Padding != null)
                {
                    bw.Write(Padding);
                }
                long afterFields = bw.BaseStream.Position;
                SizeFields = (int)(afterFields - fieldsStart);
                if (Lookup != null)
                {
                    Lookup.Size = SizeLookup;
                    Lookup.Write(bw);
                }
                SizeLookup = (int)(bw.BaseStream.Position - afterFields);
                long end = bw.BaseStream.Position;
                bw.BaseStream.Seek(sizesPos, SeekOrigin.Begin);
                bw.Write(SizeFields);
                bw.Write(SizeLookup);
                bw.BaseStream.Seek(end, SeekOrigin.Begin);
            }
        }

        public void Write64(BinaryWriter bw)
        {
            // Mirror Read64: pad FieldCount only when this loose table used 64-bit field-count layout
            if (Is64)
            {
                bw.Write(FieldCountUnfixed);
                bw.Write(0); // 32-bit zero pad (same as (long)FieldCount for counts that fit int)
            }
            else
            {
                bw.Write(FieldCountUnfixed);
            }
            long sizesPos = bw.BaseStream.Position;
            bw.Write(SizeFields);
            bw.Write(SizeLookup);
            bw.Write(Unknown);
            long fieldsStart = bw.BaseStream.Position;
            int writeCount = Fields != null ? Fields.Length : FieldCount;
            for (int i = 0; i < writeCount; i++)
            {
                if (Fields[i] != null)
                {
                    Fields[i].Write(bw);
                }
            }
            if (SizePadding >= 0)
            {
                if (SizePadding > 0 && Padding != null)
                {
                    bw.Write(Padding);
                }
                long afterFields = bw.BaseStream.Position;
                SizeFields = (int)(afterFields - fieldsStart);
                if (Lookup != null)
                {
                    Lookup.Size = SizeLookup;
                    Lookup.Write(bw);
                }
                SizeLookup = (int)(bw.BaseStream.Position - afterFields);
                long end = bw.BaseStream.Position;
                bw.BaseStream.Seek(sizesPos, SeekOrigin.Begin);
                bw.Write(SizeFields);
                bw.Write(SizeLookup);
                bw.BaseStream.Seek(end, SeekOrigin.Begin);
            }
        }

        public void UseChange(BXML_LOOSE newData)
        {
            if (newData == null)
            {
                return;
            }
            BXML_FIELDTABLE[] fields = newData.fields ?? new BXML_FIELDTABLE[0];
            // Preserve declared count from XML (may be > actual field array in some tables)
            FieldCountUnfixed = (int)newData.countFieldsUnfixed;
            if (FieldCountUnfixed < fields.Length)
            {
                FieldCountUnfixed = fields.Length;
            }
            Fields = new BDAT_FIELDTABLE[fields.Length];
            for (int i = 0; i < fields.Length; i++)
            {
                Fields[i] = new BDAT_FIELDTABLE();
                Fields[i].UseChange(fields[i]);
            }
            // Write every field including Size==0 placeholders (each is still an 8-byte header).
            // Previously empty Size==0 rows were dropped from FieldCount, shrinking bins (e.g. -560 on table 405).
            FieldCount = fields.Length;
            // SizePadding / Padding are intentionally left as read from the original bin
            if (Lookup == null)
            {
                Lookup = new BDAT_LOOKUPTABLE();
            }
            SizeLookup = Lookup.UseChange(newData.lookup);
        }

        public bool Compare(BXML_LOOSE newData)
        {
            if (newData == null)
            {
                return false;
            }
            if (Lookup == null)
            {
                return newData.lookup == null || newData.lookup.words == null || newData.lookup.words.Length == 0;
            }
            return Lookup.Compare(newData.lookup) <= 0;
        }
    }

    public class BDAT_SUBARCHIVE
    {
        public byte[] StartAndEndFieldId;

        public int SizeCompressed;

        public int SizeDecompressed;

        public int FieldLookupCount;

        public BDAT_FIELDTABLE[] Fields;

        public BDAT_LOOKUPTABLE[] Lookups;

        private BNSDat m_bnsDat = new BNSDat();

        private int m_maxSize;

        public void Read(BinaryReader br)
        {
            StartAndEndFieldId = br.ReadBytes(16);
            SizeCompressed = Ultily.ReadIntFrom2Bytes(br.ReadBytes(2));
            byte[] buffer = br.ReadBytes(SizeCompressed);
            SizeDecompressed = Ultily.ReadIntFrom2Bytes(br.ReadBytes(2));
            if (SizeDecompressed < 0)
            {
                Console.WriteLine("SizeCompressed: " + SizeCompressed + "|SizeDecompressed: " + SizeDecompressed);
            }
            byte[] buffer2 = m_bnsDat.Deflate(buffer, SizeCompressed, SizeDecompressed);
            FieldLookupCount = br.ReadInt32();
            Fields = new BDAT_FIELDTABLE[FieldLookupCount];
            Lookups = new BDAT_LOOKUPTABLE[FieldLookupCount];
            BinaryReader binaryReader = new BinaryReader(new MemoryStream(buffer2));
            int num = Ultily.ReadIntFrom2Bytes(br.ReadBytes(2));
            for (int i = 1; i <= FieldLookupCount; i++)
            {
                binaryReader.BaseStream.Seek(num, SeekOrigin.Begin);
                Fields[i - 1] = new BDAT_FIELDTABLE();
                Fields[i - 1].Read(binaryReader);
                num = ((i >= FieldLookupCount) ? SizeDecompressed : Ultily.ReadIntFrom2Bytes(br.ReadBytes(2)));
                Lookups[i - 1] = new BDAT_LOOKUPTABLE();
                Lookups[i - 1].Size = num - (int)binaryReader.BaseStream.Position;
                Lookups[i - 1].Read(binaryReader);
            }
        }

        /// <summary>Create a new empty block with one Size=0 placeholder field (safe to compress/write).</summary>
        public static BDAT_SUBARCHIVE CreateEmpty()
        {
            return new BDAT_SUBARCHIVE
            {
                StartAndEndFieldId = new byte[16],
                SizeCompressed = 0,
                SizeDecompressed = 0,
                FieldLookupCount = 1,
                Fields = new[]
                {
                    new BDAT_FIELDTABLE
                    {
                        ID = 0,
                        Unknown1 = 0,
                        Unknown2 = 0,
                        Size = 0,
                        Data = new byte[0]
                    }
                },
                Lookups = new[]
                {
                    new BDAT_LOOKUPTABLE { Size = 0, Data = new byte[0] }
                }
            };
        }

        public void Write(BinaryWriter bw)
        {
            if (StartAndEndFieldId == null || StartAndEndFieldId.Length != 16)
            {
                StartAndEndFieldId = new byte[16];
            }
            if (Fields == null || Fields.Length == 0)
            {
                // Avoid empty compressed blocks — keep one placeholder record
                var empty = CreateEmpty();
                Fields = empty.Fields;
                Lookups = empty.Lookups;
                FieldLookupCount = 1;
            }
            if (Lookups == null)
            {
                Lookups = new BDAT_LOOKUPTABLE[Fields.Length];
            }
            FieldLookupCount = Fields.Length;
            if (Lookups.Length != FieldLookupCount)
            {
                var resized = new BDAT_LOOKUPTABLE[FieldLookupCount];
                for (int i = 0; i < FieldLookupCount; i++)
                {
                    resized[i] = i < Lookups.Length && Lookups[i] != null
                        ? Lookups[i]
                        : new BDAT_LOOKUPTABLE { Size = 0, Data = new byte[0] };
                }
                Lookups = resized;
            }

            BinaryWriter binaryWriter = new BinaryWriter(new MemoryStream());
            int[] array = new int[FieldLookupCount];
            array[0] = 0;
            for (int i = 1; i <= FieldLookupCount; i++)
            {
                if (Fields[i - 1] == null)
                {
                    Fields[i - 1] = new BDAT_FIELDTABLE { Data = new byte[0], Size = 0 };
                }
                if (Lookups[i - 1] == null)
                {
                    Lookups[i - 1] = new BDAT_LOOKUPTABLE { Size = 0, Data = new byte[0] };
                }
                if (Lookups[i - 1].Data == null)
                {
                    Lookups[i - 1].Data = new byte[0];
                    Lookups[i - 1].Size = 0;
                }
                Fields[i - 1].Write(binaryWriter);
                Lookups[i - 1].Write(binaryWriter);
                if (i < FieldLookupCount)
                {
                    array[i] = (int)binaryWriter.BaseStream.Position;
                }
                if ((int)binaryWriter.BaseStream.Length < 65535)
                {
                    m_maxSize = i;
                }
            }
            SizeDecompressed = (int)binaryWriter.BaseStream.Length;
            byte[] array2 = ((MemoryStream)binaryWriter.BaseStream).ToArray();
            int sizeCompressed;
            byte[] buffer = m_bnsDat.Inflate(array2, SizeDecompressed, out sizeCompressed, 6);
            SizeCompressed = sizeCompressed;
            bw.Write(StartAndEndFieldId);
            bw.Write(Ultily.WriteIntTo2Bytes(SizeCompressed));
            bw.Write(buffer);
            bw.Write(Ultily.WriteIntTo2Bytes(SizeDecompressed));
            bw.Write(FieldLookupCount);
            for (int j = 0; j < FieldLookupCount; j++)
            {
                bw.Write(Ultily.WriteIntTo2Bytes(array[j]));
            }
        }

        public void UseChange(BXML_SUBARCHIVE newData)
        {
            BXML_FIELDTABLE[] fields = newData.fields;
            FieldLookupCount = fields.Length;
            Fields = new BDAT_FIELDTABLE[fields.Length];
            for (int i = 0; i < fields.Length; i++)
            {
                Fields[i] = new BDAT_FIELDTABLE();
                Fields[i].UseChange(newData.fields[i]);
            }
            BXML_LOOKUPTABLE[] lookup = newData.lookup;
            Lookups = new BDAT_LOOKUPTABLE[lookup.Length];
            for (int j = 0; j < lookup.Length; j++)
            {
                Lookups[j] = new BDAT_LOOKUPTABLE();
                Lookups[j].UseChange(newData.lookup[j]);
            }
        }

        public bool Compare(BXML_SUBARCHIVE newData)
        {
            if (newData == null || newData.lookup == null || Lookups == null)
            {
                return false;
            }
            int n = Math.Min(Lookups.Length, newData.lookup.Length);
            for (int i = 0; i < n; i++)
            {
                if (Lookups[i] == null)
                {
                    continue;
                }
                if (Lookups[i].Compare(newData.lookup[i]) > 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
