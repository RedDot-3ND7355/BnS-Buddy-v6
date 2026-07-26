# Blade & Soul datafile / localfile binary format

Developer reference for the client **BDAT** containers used by Blade & Soul:

| File (typical) | Role | Arch |
|----------------|------|------|
| `datafile.bin` | Main game tables | 32-bit client |
| `localfile.bin` | Localized / lighter tables | 32-bit client |
| `datafile64.bin` | Main game tables | 64-bit client |
| `localfile64.bin` | Localized / lighter tables | 64-bit client |

This document describes the on-disk layout as implemented by **LegacyBin** (`BDat.cs` and related code). Endianness is **little-endian** throughout.

---

## 1. Big picture

```
┌─────────────────────────────────────────────────────────────┐
│  File header (magic + version + sizes + table count)        │
├─────────────────────────────────────────────────────────────┤
│  Head / name-table block (alias map) — only if tableCount>10│
├─────────────────────────────────────────────────────────────┤
│  Table 0  (list header + collection payload [+ trail pad])  │
│  Table 1                                                    │
│  ...                                                        │
│  Table N-1                                                  │
└─────────────────────────────────────────────────────────────┘
```

Each **table** is one game data list (e.g. items, skills). A table’s payload is either:

- **Loose** — uncompressed records + string lookup, or  
- **Archive** — one or more zlib-compressed blocks of records + per-record lookups.

LegacyBin maps this to classes:

| On disk concept | Code type |
|-----------------|-----------|
| Whole file | `BDAT_CONTENT` |
| Name-table sizes + body | `BDAT_HEAD` |
| One table | `BDAT_LIST` |
| Loose vs compressed | `BDAT_COLLECTION` |
| Uncompressed body | `BDAT_LOOSE` |
| One record/row | `BDAT_FIELDTABLE` |
| UTF-16 string heap | `BDAT_LOOKUPTABLE` |
| Compressed body | `BDAT_ARCHIVE` → `BDAT_SUBARCHIVE[]` |

---

## 2. Detecting 32-bit vs 64-bit

LegacyBin resolves mode in this order:

1. **Filename** — contains `64.bin`, `datafile64`, or `localfile64` (case-insensitive).
2. **Header heuristic** — after magic + version area, interpret size fields as `int32` vs `int64` and see which yields a plausible table count (1…512).

Rule of thumb:

- **32-bit:** size / count fields after the version blob are **4 bytes**.
- **64-bit:** the same logical fields are **8 bytes**.

Compressed sub-archive layout is **the same** on both arches. The important 64-bit differences are the **file header**, **head sizes**, and **loose FieldCount padding** when `elementCount == 1`.

---

## 3. File header

### 3.1 Logical fields (both arches)

Aligned with client / BnsBinTool naming:

| Field | Type (32) | Type (64) | Notes |
|-------|-----------|-----------|--------|
| Magic | `char[8]` | `char[8]` | e.g. `TADBOSLB` |
| DatafileVersion | `uint8` | `uint8` | |
| ClientVersion | `uint16[4]` | `uint16[4]` | |
| TotalTableSize | `int32` | `int64` | |
| TableCount | `int32` | `int64` | Number of tables that follow |
| AliasMapSize | `int32` | `int64` | Size of name-table blob (`Size_1`) |
| AliasCount | `int32` | `int64` | |
| MaxBufferSize | `int32` | `int64` | |
| CreatedAt | `uint32` | `uint32` | Unix time (seconds) |
| Reserved | `byte[58]` | `byte[58]` | |

**Header size after magic:**

- 32-bit: `1 + 8 + 5×4 + 4 + 58` = **91** bytes → total **99** with magic  
- 64-bit: `1 + 8 + 5×8 + 4 + 58` = **111** bytes → total **119** with magic  

### 3.2 How LegacyBin stores the header

For round-trip fidelity the tool does **not** split every logical field. It packs:

| Member | 32-bit read | 64-bit read |
|--------|-------------|-------------|
| `Signature` | 8 bytes | 8 bytes |
| `Version` | `int32` (first 4 of version area) | `int32` |
| `Unknown` | **9** bytes | **13** bytes |
| `ListCount` | `int32` | `int64` (stored as `int`) |

So:

- 32-bit: `Version(4) + Unknown(9)` = version byte + client version + `TotalTableSize`  
- 64-bit: `Version(4) + Unknown(13)` = version byte + client version + `TotalTableSize` (8)

Then `ListCount` is the table count. The next structure is `BDAT_HEAD`.

---

## 4. Head / name table (`BDAT_HEAD`)

### 4.1 Layout

| Field | 32-bit | 64-bit |
|-------|--------|--------|
| `Size_1` (AliasMapSize) | `int32` | `int64` |
| `Size_2` (AliasCount) | `int32` | `int64` |
| `Size_3` (MaxBufferSize) | `int32` | `int64` |
| Padding | `byte[62]` | `byte[62]` | Includes `CreatedAt` (4) + reserved (58) in the logical header |
| `Data` | `Size_1` bytes | `Size_1` bytes | **Only if name table present** |

### 4.2 When is the name table present?

```
Complement = (ListCount <= 10)
```

- **`ListCount <= 10`** (typical `localfile` / `localfile64`): **no** name-table body. Only the size fields + 62-byte pad are read; `Data` is empty.  
- **`ListCount > 10`** (typical `datafile` / `datafile64`): read **`Size_1` bytes** of opaque name-table data.

Inside the name table (for tools that parse it; LegacyBin keeps it opaque):

| Entry field | 32-bit | 64-bit |
|-------------|--------|--------|
| String offset | `int32` | `int64` |
| Begin / End | `uint32` + `uint32` | same |
| Entry size | **12** bytes | **16** bytes |

LegacyBin does **not** rewrite name-table entries; it copies `Data` through on repack.

---

## 5. Table list entry (`BDAT_LIST`)

Same layout for 32- and 64-bit:

| Offset (relative) | Type | Name in code | Meaning |
|-------------------|------|--------------|---------|
| 0 | `uint8` | `Unknown1` | **ElementCount** |
| 1 | `uint16` | `ID` | Table id (used in XML as `datafile_XXX.xml`) |
| 3 | `uint16` | `Unknown2` | Major version |
| 5 | `uint16` | `Unknown3` | Minor version |
| 7 | `int32` | `Size` | Byte length of **collection payload only** (not including this 11-byte header) |
| 11 | … | `Collection` | Loose or archive |
| 11+Size−pad | … | `TrailingPadding` | Optional bytes until `Size` is consumed |

```
List header (11 bytes)
┌────────┬──────┬──────┬──────┬──────────┐
│ elem   │ id   │ maj  │ min  │ size     │
│ u8     │ u16  │ u16  │ u16  │ i32      │
└────────┴──────┴──────┴──────┴──────────┘
         │
         ▼  Size bytes
┌────────────────────────────┬────────────┐
│ Collection (loose/archive) │ trail pad  │
└────────────────────────────┴────────────┘
```

### 5.1 Trailing table padding

Official files often append **trailing zeros** after the logical collection so that:

```
len(Collection) + len(TrailingPadding) == Size
```

Common length: **20 bytes** of `0x00` per table (seen on both `localfile64` and many `datafile64` tables).

**Clients can be picky** if this is dropped on repack. LegacyBin stores it as `BDAT_LIST.TrailingPadding` and rewrites it.

---

## 6. Collection (`BDAT_COLLECTION`)

First byte: **`Compressed`**.

| Value | Meaning |
|-------|---------|
| `0` | **Loose** (uncompressed) → `BDAT_LOOSE` |
| `1` | **Archive** (compressed blocks) → `BDAT_ARCHIVE` |
| `> 1` | Legacy quirk: byte is rewound and treated as start of archive; an extra `Deprecated` byte may follow the archive |

Compressed layout is **identical** on 32- and 64-bit.

---

## 7. Loose (uncompressed) tables (`BDAT_LOOSE`)

### 7.1 Header

**32-bit (always):**

| Field | Type |
|-------|------|
| FieldCount (unfixed / declared) | `int32` |
| SizeFields | `int32` |
| SizeLookup | `int32` |
| Unknown | `uint8` (typically `1`) |

**64-bit:**

| Condition | FieldCount encoding |
|-----------|---------------------|
| `elementCount == 1` | `int32` count + **`int32` pad `0`** (8 bytes total) |
| `elementCount != 1` | plain `int32` only |

`SizeFields` and `SizeLookup` stay **`int32`** on both arches.

In code, `Is64 == true` means “this loose table used the 8-byte FieldCount form,” not “the whole file is 64-bit.”

### 7.2 Body layout

```
┌──────────────────────────────────────────────────────────┐
│ SizeFields bytes:                                        │
│   [ Field 0 ][ Field 1 ]…[ Field k-1 ][ region tail ]    │
│                                              ▲           │
│                              Padding / SizePadding       │
├──────────────────────────────────────────────────────────┤
│ SizeLookup bytes: UTF-16LE string heap (LOOKUPTABLE)     │
└──────────────────────────────────────────────────────────┘
```

- **`FieldCount` / `FieldCountUnfixed`:** declared number of fields.  
  - Declared count may be **greater** than how many complete fields fit in `SizeFields`.  
  - Readers must **not** read past `SizeFields`.  
  - `FieldCountUnfixed` is what was declared; `FieldCount` is how many were actually parsed.
- **`SizeFields`:** total size of field region, including any **region tail** after the last field.
- **`Padding` (`SizePadding`):** bytes from end of last successfully parsed field to end of `SizeFields`.  
  - Often zeros.  
  - Can contain **non-zero structured data** (e.g. table 405 in modern `datafile64` has ~80KB of mixed zero runs and data islands). Treat as **opaque**; preserve on repack.  
  - This is **not** the same as list trailing padding (section 5.1).
- **`Lookup`:** raw UTF-16LE strings separated by `U+0000` (double null byte pair).

### 7.3 Field / record (`BDAT_FIELDTABLE`)

| Field | Type | Notes |
|-------|------|--------|
| `Unknown1` (`unk1`) | `uint16` | If **`255`**, size is stored as `uint16`; else as `int32` |
| `Unknown2` (`unk2`) | `uint16` | |
| `Size` | `uint16` or `int32` | Total record size including header (for normal records) |
| `ID` | `int32` | Present when `Size >= 12` |
| `Data` | `Size - 12` bytes | Present when `Size >= 12` |

**Read rules:**

```
unk1 = u16
unk2 = u16
if unk1 == 255:
    size = u16          # 6-byte header so far
else:
    size = i32          # 8-byte header so far
if size >= 12:
    id = i32
    data = bytes(size - 12)
else:
    data = empty        # still a valid record; Size==0 is an 8-byte placeholder
```

**Important for repack:**

- Records with **`Size == 0`** still occupy a full short header (typically **8 bytes**: `unk1`, `unk2`, `size=0`).  
- They must be written back. Dropping them shrinks the file (e.g. −560 bytes on table 405 = 70 × 8).  
- Never reduce field count by treating `Size == 0` as “skip.”

### 7.4 String lookup (`BDAT_LOOKUPTABLE`)

- Raw blob of length `SizeLookup`.  
- Split into words on UTF-16LE null terminators (`00 00`).  
- Non-text segments may be exported as a special `invalidzhangjieyong…` encoding in XML and rebuilt on pack.  
- Empty words are significant (preserve count / slots).

---

## 8. Archive (compressed) tables (`BDAT_ARCHIVE`)

```
SubArchiveCount : int32
Unknown         : uint16     // official files use 8
SubArchives[SubArchiveCount]
```

Each **`BDAT_SUBARCHIVE`** (one compression block):

| Field | Type |
|-------|------|
| Start/End key | `byte[16]` (two `uint64` / field-id range keys) |
| SizeCompressed | `uint16` |
| Compressed payload | `SizeCompressed` bytes (zlib) |
| SizeDecompressed | `uint16` |
| FieldLookupCount | `int32` |
| Offsets | `uint16[FieldLookupCount]` into decompressed buffer |

Decompressed buffer layout per entry: **field record + that field’s lookup bytes**, packed back-to-back. Offsets point at the start of each field; the last entry runs to `SizeDecompressed`.

**No 32/64 difference** in this block format.

---

## 9. End-to-end diagrams

### 9.1 32-bit file

```
Magic[8]
VersionBlob[4+9]          // version + client + TotalTableSize (int32)
ListCount int32
Head:
  Size_1/2/3 int32 each
  Pad[62]
  Data[Size_1]            // if ListCount > 10
Tables[]:
  ListHeader (11 bytes, Size is int32)
  Collection...
  TrailingPadding?
```

### 9.2 64-bit file

```
Magic[8]
VersionBlob[4+13]         // version + client + TotalTableSize (int64)
ListCount int64
Head:
  Size_1/2/3 int64 each
  Pad[62]
  Data[Size_1]            // if ListCount > 10
Tables[]:
  ListHeader (11 bytes, Size still int32!)
  Collection...
    if Loose && elementCount==1:
      FieldCount = int32 + int32(0)
    else if Loose:
      FieldCount = int32
  TrailingPadding?
```

### 9.3 Loose collection (detail)

```
FieldCountUnfixed [+ pad0 if 64 && elem==1]
SizeFields : int32
SizeLookup : int32
Unknown    : u8
── SizeFields region ──────────────────────
  fields...
  optional region tail (Padding)   ← may be non-zero; preserve
── SizeLookup region ──────────────────────
  UTF-16LE string heap
```

---

## 10. XML mapping (LegacyBin tool)

Unpack writes one file per table:

```text
datafile_<ID:000>.xml
```

Root element `list` (`BXML_LIST`) includes:

- Attributes: `id`, `size`, `unk1` (element count), `unk2`, `unk3`
- `collection/@compressed`
- Either `loose` or `archive` with fields + lookup words

Repack flow:

1. Read original bin (preserves 32/64 mode, paddings, name table).  
2. Load XML.  
3. Apply field/lookup changes (`UseChange`).  
4. Write bin with the **same** 32/64 writers; keep region tail + trailing list padding.

---

## 11. Round-trip / client safety checklist

To keep official and repacked files matching (and clients happy):

| Item | Action |
|------|--------|
| Arch mode | Same as source (header + loose FieldCount pad rules) |
| Name table | Copy opaque `Head.Data` when present |
| List `Size` | Must equal collection + trailing pad |
| Trailing list pad | Preserve (`TrailingPadding`, often 20 zero bytes) |
| Region tail in `SizeFields` | Preserve (`Loose.Padding`) even if non-zero |
| `Size == 0` fields | Write 8-byte headers; do not drop |
| `FieldCountUnfixed` | Prefer declared count from source/XML |
| Lookup blob | Rebuild only from full word list (including empties) |
| Compressed blocks | Re-deflate may change bytes even if logic matches; pure structural rewrite without XML can still be bit-identical if inflate/deflate matches |

Verified in practice:

- `localfile64.bin` — bit-identical XML round-trip  
- `datafile64.bin` (~267 MB) — bit-identical XML round-trip  

---

## 12. Code map

| Concern | Primary file / type |
|---------|---------------------|
| Read/write all structures | `BDat.cs` |
| 32/64 detection, UI unpack/repack | `Form1.cs` |
| Field ↔ hex/int, inflate helpers | `bcrypt.cs` |
| Lookup string split/join | `bnsTool.cs` |
| XML DTOs | `BXml.cs` |
| Zlib inflate/deflate | `BNSDat.cs` (DotNetZip) |

---

## 13. Glossary

| Term | Meaning |
|------|---------|
| **Table / list** | One `BDAT_LIST` (one game table id) |
| **ElementCount** | First byte of list header; drives 64-bit loose FieldCount padding when `== 1` |
| **Loose** | Uncompressed table body |
| **Archive** | Zlib-compressed multi-block table body |
| **Field** | One row/record (`BDAT_FIELDTABLE`) |
| **Lookup** | Shared or per-block UTF-16 string heap |
| **Trailing padding** | Bytes after collection up to list `Size` |
| **Region tail / SizePadding** | Bytes after last field inside `SizeFields` |
| **Complement** | `ListCount <= 10` → no name-table body |

---

## 14. Quick reference: what changes with 64-bit?

| Component | Changes on x64? |
|-----------|-----------------|
| Magic | No |
| Header size fields / table count | **Yes** (`int32` → `int64`) |
| Head Size_1/2/3 | **Yes** (`int64`) |
| Name-table entry width (in blob) | **Yes** (12 → 16) — treated opaque here |
| List header (elem, id, ver, Size) | **No** (`Size` stays `int32`) |
| Loose FieldCount | **Sometimes** (+4 zero pad if `elementCount == 1`) |
| Field record layout | **No** |
| Lookup layout | **No** |
| Compressed archive blocks | **No** |
| Trailing / region padding rules | **No** (preserve on both) |

---

*Document version aligned with LegacyBin’s 32/64 implementation and verified against live `datafile64` / `localfile64` clients files.*
