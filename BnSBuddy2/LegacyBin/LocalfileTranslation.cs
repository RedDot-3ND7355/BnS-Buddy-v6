using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace BnSBuddy2.LegacyBin
{
    /// <summary>
    /// localfile.bin / localfile64.bin translation helpers (BnsDatTool-compatible XML).
    /// Lookup layout for text records: words[0] = alias, words[1] = display text.
    /// </summary>
    public static class LocalfileTranslation
    {
        public sealed class Entry
        {
            public int AutoId;
            public string Alias;
            public int Priority;
            public string Original;
            public string Replacement;
            public string Type = "nc";
        }

        public sealed class ApplyResult
        {
            public int TablesTouched;
            public int RecordsScanned;
            public int AppliedByAlias;
            public int AppliedByOriginal;
            public int Unchanged;
            public int Skipped;
            public int BlocksSplit;
            public string TextTableSummary;
        }

        /// <summary>
        /// Load BnsDatTool-style Translation.xml:
        /// &lt;table&gt;&lt;text autoId alias priority&gt;&lt;original/&gt;&lt;replacement/&gt;&lt;/text&gt;...
        /// </summary>
        public static List<Entry> LoadXml(string path)
        {
            var doc = new XmlDocument();
            doc.Load(path);
            var list = new List<Entry>();
            XmlNodeList nodes = doc.SelectNodes("table/child::node()");
            if (nodes == null)
            {
                return list;
            }
            foreach (XmlNode node in nodes)
            {
                if (node.Name != "text" && node.NodeType != XmlNodeType.Element)
                {
                    continue;
                }
                var e = new Entry();
                if (node.Attributes != null)
                {
                    if (node.Attributes["autoId"] != null)
                    {
                        int.TryParse(node.Attributes["autoId"].Value, out e.AutoId);
                    }
                    if (node.Attributes["alias"] != null)
                    {
                        e.Alias = node.Attributes["alias"].Value;
                    }
                    if (node.Attributes["priority"] != null)
                    {
                        int.TryParse(node.Attributes["priority"].Value, out e.Priority);
                    }
                    if (node.Attributes["type"] != null)
                    {
                        e.Type = node.Attributes["type"].Value;
                    }
                }
                foreach (XmlNode child in node.ChildNodes)
                {
                    if (child.NodeType != XmlNodeType.Element)
                    {
                        continue;
                    }
                    if (child.Name == "original")
                    {
                        e.Original = child.InnerText;
                    }
                    else if (child.Name == "replacement")
                    {
                        e.Replacement = child.InnerText;
                    }
                }
                if (string.IsNullOrEmpty(e.Alias) && string.IsNullOrEmpty(e.Original))
                {
                    continue;
                }
                if (e.Replacement == null)
                {
                    e.Replacement = e.Original ?? string.Empty;
                }
                if (e.Original == null)
                {
                    e.Original = string.Empty;
                }
                list.Add(e);
            }
            return list;
        }

        public static void SaveXml(string path, IEnumerable<Entry> entries)
        {
            var table = new XElement("table");
            foreach (var line in entries)
            {
                var el = new XElement("text",
                    new XAttribute("autoId", line.AutoId),
                    new XAttribute("alias", line.Alias ?? string.Empty),
                    new XAttribute("priority", line.Priority),
                    new XAttribute("type", line.Type ?? "nc"));
                el.Add(new XElement("original", new XCData(line.Original ?? string.Empty)));
                el.Add(new XElement("replacement", new XCData(line.Replacement ?? string.Empty)));
                table.Add(el);
            }
            var settings = new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(false),
                Indent = true,
                OmitXmlDeclaration = false
            };
            using (var xw = XmlWriter.Create(path, settings))
            {
                table.Save(xw);
            }
        }

        /// <summary>
        /// Merge source translations into target by alias (BnsDatTool MergeTranslation).
        /// Returns merged entry list (target structure, source replacements where alias matches).
        /// </summary>
        public static List<Entry> MergeByAlias(List<Entry> target, List<Entry> source)
        {
            var byAlias = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var s in source)
            {
                if (string.IsNullOrEmpty(s.Alias))
                {
                    continue;
                }
                byAlias[s.Alias] = s.Replacement ?? string.Empty;
            }
            var merged = new List<Entry>(target.Count);
            foreach (var t in target)
            {
                var e = new Entry
                {
                    AutoId = t.AutoId,
                    Alias = t.Alias,
                    Priority = t.Priority,
                    Original = t.Original,
                    Replacement = t.Replacement,
                    Type = t.Type
                };
                if (!string.IsNullOrEmpty(e.Alias) && byAlias.TryGetValue(e.Alias, out string rep))
                {
                    e.Replacement = rep;
                    e.Type = "merged";
                }
                merged.Add(e);
            }
            return merged;
        }

        public static Dictionary<string, string> BuildAliasMap(List<Entry> entries)
        {
            var map = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var e in entries)
            {
                if (string.IsNullOrEmpty(e.Alias))
                {
                    continue;
                }
                map[e.Alias] = e.Replacement ?? string.Empty;
            }
            return map;
        }

        public static Dictionary<string, string> BuildOriginalMap(List<Entry> entries)
        {
            // Higher priority wins (BnsDatTool)
            var map = new Dictionary<string, string>(StringComparer.Ordinal);
            var prio = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (var e in entries)
            {
                if (string.IsNullOrEmpty(e.Original))
                {
                    continue;
                }
                if (!prio.TryGetValue(e.Original, out int p) || e.Priority >= p)
                {
                    map[e.Original] = e.Replacement ?? string.Empty;
                    prio[e.Original] = e.Priority;
                }
            }
            return map;
        }

        /// <summary>Find the main text/commons archive table (localfile heuristic from BnsDatTool).</summary>
        public static int FindTextTableIndex(BDAT_CONTENT content)
        {
            if (content?.Lists == null)
            {
                return -1;
            }
            int best = -1;
            long bestSize = 0;
            for (int i = 0; i < content.ListCount; i++)
            {
                var list = content.Lists[i];
                if (list == null || list.Collection == null || list.Collection.Compressed < 1
                    || list.Collection.Archive == null)
                {
                    continue;
                }
                // Unknown1 = ElementCount; text table typically elem=1
                if (list.Unknown1 != 1)
                {
                    continue;
                }
                int fieldSize = PeekFirstFieldSize(list);
                if (fieldSize != 28 && fieldSize != 36)
                {
                    // still allow large archives as fallback candidates
                    if (list.Size < 1_000_000)
                    {
                        continue;
                    }
                }
                if (list.Size > bestSize)
                {
                    bestSize = list.Size;
                    best = i;
                }
            }
            // Fallback: largest archive table overall
            if (best < 0)
            {
                for (int i = 0; i < content.ListCount; i++)
                {
                    var list = content.Lists[i];
                    if (list?.Collection?.Archive == null)
                    {
                        continue;
                    }
                    if (list.Size > bestSize)
                    {
                        bestSize = list.Size;
                        best = i;
                    }
                }
            }
            return best;
        }

        private static int PeekFirstFieldSize(BDAT_LIST list)
        {
            try
            {
                var arch = list.Collection.Archive;
                if (arch.SubArchives != null && arch.SubArchives.Length > 0
                    && arch.SubArchives[0].Fields != null && arch.SubArchives[0].Fields.Length > 0)
                {
                    return arch.SubArchives[0].Fields[0].Size;
                }
                if (list.Collection.Loose?.Fields != null && list.Collection.Loose.Fields.Length > 0)
                {
                    return list.Collection.Loose.Fields[0].Size;
                }
            }
            catch
            {
                // ignore
            }
            return 0;
        }

        public static List<Entry> ExportFromContent(BDAT_CONTENT content, int tableIndex = -1)
        {
            if (tableIndex < 0)
            {
                tableIndex = FindTextTableIndex(content);
            }
            if (tableIndex < 0 || tableIndex >= content.ListCount)
            {
                throw new InvalidOperationException("Could not locate a text/commons table in this bin.");
            }
            var list = content.Lists[tableIndex];
            var entries = new List<Entry>();
            int autoId = 1;
            if (list.Collection.Archive != null)
            {
                foreach (var sub in list.Collection.Archive.SubArchives ?? new BDAT_SUBARCHIVE[0])
                {
                    if (sub?.Fields == null)
                    {
                        continue;
                    }
                    for (int f = 0; f < sub.Fields.Length; f++)
                    {
                        var lu = (sub.Lookups != null && f < sub.Lookups.Length) ? sub.Lookups[f] : null;
                        TryAddEntryFromLookup(entries, ref autoId, lu);
                    }
                }
            }
            else if (list.Collection.Loose?.Lookup != null)
            {
                // Loose text tables are rare for localfile commons; still support word pairs sequential
                var words = bnsTool.LookupSplitToWords(list.Collection.Loose.Lookup.Data,
                    (uint)list.Collection.Loose.Lookup.Size);
                // Not structured as per-field alias/text — skip bulk dump for loose shared lookup
                for (int i = 0; i + 1 < words.Count; i += 2)
                {
                    entries.Add(new Entry
                    {
                        AutoId = autoId++,
                        Alias = words[i] ?? string.Empty,
                        Priority = 0,
                        Original = words[i + 1] ?? string.Empty,
                        Replacement = words[i + 1] ?? string.Empty,
                        Type = "nc"
                    });
                }
            }
            return entries;
        }

        private static void TryAddEntryFromLookup(List<Entry> entries, ref int autoId, BDAT_LOOKUPTABLE lu)
        {
            if (lu?.Data == null || lu.Size <= 0)
            {
                return;
            }
            var words = bnsTool.LookupSplitToWords(lu.Data, (uint)lu.Size);
            if (words.Count < 1)
            {
                return;
            }
            string alias = words.Count > 0 ? (words[0] ?? string.Empty) : string.Empty;
            string text = words.Count > 1 ? (words[1] ?? string.Empty) : string.Empty;
            if (string.IsNullOrEmpty(alias) && string.IsNullOrEmpty(text))
            {
                return;
            }
            entries.Add(new Entry
            {
                AutoId = autoId++,
                Alias = alias,
                Priority = 0,
                Original = text,
                Replacement = text,
                Type = "nc"
            });
        }

        /// <summary>
        /// Apply translation dictionary to the text table inside content (in-memory).
        /// Match order: alias first, then original text (BnsDatTool).
        /// </summary>
        public static ApplyResult Apply(BDAT_CONTENT content, List<Entry> entries, int tableIndex = -1, bool resplitOversizedBlocks = true)
        {
            var result = new ApplyResult();
            if (tableIndex < 0)
            {
                tableIndex = FindTextTableIndex(content);
            }
            if (tableIndex < 0)
            {
                throw new InvalidOperationException("Could not locate a text/commons table in this bin.");
            }

            var aliasMap = BuildAliasMap(entries);
            var originalMap = BuildOriginalMap(entries);
            var list = content.Lists[tableIndex];
            result.TextTableSummary = "tableIndex=" + tableIndex + " id=" + list.ID
                + " size=" + list.Size + " kind=" + (list.Collection.Compressed >= 1 ? "Archive" : "Loose");

            if (list.Collection.Archive != null)
            {
                result.TablesTouched = 1;
                var archive = list.Collection.Archive;
                var newSubs = new List<BDAT_SUBARCHIVE>();
                foreach (var sub in archive.SubArchives ?? new BDAT_SUBARCHIVE[0])
                {
                    ApplyToSubArchive(sub, aliasMap, originalMap, result);
                    if (resplitOversizedBlocks && NeedsSplit(sub, out int splitAt) && splitAt > 0 && splitAt < sub.FieldLookupCount)
                    {
                        SplitSubArchive(sub, splitAt, newSubs);
                        result.BlocksSplit++;
                    }
                    else
                    {
                        newSubs.Add(sub);
                    }
                }
                archive.SubArchives = newSubs.ToArray();
                archive.SubArchiveCount = newSubs.Count;
            }
            else if (list.Collection.Loose != null)
            {
                result.TablesTouched = 1;
                // Shared loose lookup is not the usual localfile pattern; leave as no-op with note
                result.Skipped = result.RecordsScanned;
            }

            return result;
        }

        private static void ApplyToSubArchive(BDAT_SUBARCHIVE sub,
            Dictionary<string, string> aliasMap,
            Dictionary<string, string> originalMap,
            ApplyResult result)
        {
            if (sub?.Fields == null || sub.Lookups == null)
            {
                return;
            }
            int n = Math.Min(sub.Fields.Length, sub.Lookups.Length);
            for (int f = 0; f < n; f++)
            {
                result.RecordsScanned++;
                var lu = sub.Lookups[f];
                if (lu?.Data == null || lu.Size <= 0)
                {
                    result.Skipped++;
                    continue;
                }
                var words = bnsTool.LookupSplitToWords(lu.Data, (uint)lu.Size);
                if (words.Count < 2)
                {
                    result.Skipped++;
                    continue;
                }
                string alias = words[0] ?? string.Empty;
                string original = words[1] ?? string.Empty;
                string translated = null;
                bool byAlias = false;
                if (!string.IsNullOrEmpty(alias) && aliasMap.TryGetValue(alias, out string aRep))
                {
                    translated = aRep;
                    byAlias = true;
                }
                else if (!string.IsNullOrEmpty(original) && originalMap.TryGetValue(original, out string oRep))
                {
                    translated = oRep;
                }

                if (translated == null || translated == original)
                {
                    result.Unchanged++;
                    continue;
                }

                words[1] = translated;
                // rebuild full word list (preserve extra words beyond 0/1 if any)
                string[] arr = words.ToArray();
                int sizeLookup = 0;
                lu.Data = bnsTool.WordToLookUpData(arr, ref sizeLookup);
                lu.Size = sizeLookup;
                if (byAlias)
                {
                    result.AppliedByAlias++;
                }
                else
                {
                    result.AppliedByOriginal++;
                }
            }
            sub.FieldLookupCount = sub.Fields.Length;
        }

        /// <summary>Estimate whether decompressed block would exceed ushort max (compression block limit).</summary>
        public static bool NeedsSplit(BDAT_SUBARCHIVE sub, out int splitAt)
        {
            splitAt = 0;
            if (sub?.Fields == null || sub.Fields.Length < 2)
            {
                return false;
            }
            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                int lastOk = 0;
                for (int i = 0; i < sub.Fields.Length; i++)
                {
                    long before = ms.Position;
                    sub.Fields[i]?.Write(bw);
                    if (sub.Lookups != null && i < sub.Lookups.Length && sub.Lookups[i]?.Data != null)
                    {
                        bw.Write(sub.Lookups[i].Data);
                    }
                    if (ms.Position > 65535)
                    {
                        splitAt = Math.Max(1, lastOk);
                        return lastOk > 0 && lastOk < sub.Fields.Length;
                    }
                    lastOk = i + 1;
                    // silence unused
                    _ = before;
                }
            }
            return false;
        }

        private static void SplitSubArchive(BDAT_SUBARCHIVE source, int splitAt, List<BDAT_SUBARCHIVE> output)
        {
            int total = source.FieldLookupCount > 0 ? source.FieldLookupCount : source.Fields.Length;
            int part2 = total - splitAt;
            if (part2 <= 0)
            {
                output.Add(source);
                return;
            }

            var a = new BDAT_SUBARCHIVE
            {
                StartAndEndFieldId = new byte[16],
                Fields = new BDAT_FIELDTABLE[splitAt],
                Lookups = new BDAT_LOOKUPTABLE[splitAt],
                FieldLookupCount = splitAt
            };
            Array.Copy(source.Fields, 0, a.Fields, 0, splitAt);
            if (source.Lookups != null)
            {
                Array.Copy(source.Lookups, 0, a.Lookups, 0, Math.Min(splitAt, source.Lookups.Length));
            }
            WriteStartEndIds(a);

            var b = new BDAT_SUBARCHIVE
            {
                StartAndEndFieldId = new byte[16],
                Fields = new BDAT_FIELDTABLE[part2],
                Lookups = new BDAT_LOOKUPTABLE[part2],
                FieldLookupCount = part2
            };
            Array.Copy(source.Fields, splitAt, b.Fields, 0, part2);
            if (source.Lookups != null)
            {
                Array.Copy(source.Lookups, splitAt, b.Lookups, 0, Math.Min(part2, source.Lookups.Length - splitAt));
            }
            WriteStartEndIds(b);

            output.Add(a);
            output.Add(b);
        }

        private static void WriteStartEndIds(BDAT_SUBARCHIVE sub)
        {
            if (sub.Fields == null || sub.Fields.Length == 0)
            {
                return;
            }
            if (sub.StartAndEndFieldId == null || sub.StartAndEndFieldId.Length != 16)
            {
                sub.StartAndEndFieldId = new byte[16];
            }
            Buffer.BlockCopy(BitConverter.GetBytes(sub.Fields[0].ID), 0, sub.StartAndEndFieldId, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(sub.Fields[sub.Fields.Length - 1].ID), 0, sub.StartAndEndFieldId, 8, 4);
        }
    }
}
