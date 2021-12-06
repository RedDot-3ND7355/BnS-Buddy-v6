using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

public class bcrypt
{
    private static byte[] init_array;
    private static readonly uint[] _lookup32 = CreateLookup32();

    private static uint[] CreateLookup32()
    {
        var result = new uint[256];
        for (int i = 0; i < 256; i++)
        {
            string s = i.ToString("X2");
            result[i] = ((uint)s[0]) + ((uint)s[1] << 16);
        }
        return result;
    }
    public uint SwapBytes(uint x)
    {
        // swap adjacent 16-bit blocks
        x = (x >> 16) | (x << 16);
        // swap adjacent 8-bit blocks
        return ((x & 0xFF00FF00) >> 8) | ((x & 0x00FF00FF) << 8);
    }
    public static string BytesToInt(byte[] bytes, uint size)
    {
        if (bytes == null)
        {
            return string.Empty;
        }
        string result = "";
        /*
        for (var pos = 0; pos < bytes.Length; pos += 4)
        {
            int x = bytes[pos] | (bytes[pos + 1] << 8) | (bytes[pos + 2] << 16) | (bytes[pos + 3] << 24);
            result += x + ",";
        }
        */
        BinaryReader br = new BinaryReader(new MemoryStream(bytes));
        while (br.BaseStream.Position < (br.BaseStream.Length))
        {
            result += br.ReadInt32() + ",";
        }
        return result.TrimEnd(',');
    }
    public static string BytesToHex(byte[] bytes, uint size)
    {
        if (bytes == null)
        {
            return string.Empty;
        }
        var lookup32 = _lookup32;
        var result = new char[bytes.Length * 2];
        for (int i = 0; i < bytes.Length; i++)
        {
            var val = lookup32[bytes[i]];
            result[2 * i] = (char)val;
            result[2 * i + 1] = (char)(val >> 16);
        }
        return new string(result);
    }
    public static byte[] IntToBytes(string input, uint size)
    {
        byte[] result = new byte[0];
            int[] intArray = Array.ConvertAll(input.Split(','), int.Parse);
            result = new byte[intArray.Length * sizeof(int)];
            Buffer.BlockCopy(intArray, 0, result, 0, result.Length);
        return result;
    }
    public static byte[] HexToBytes(string input, uint size)
    {
        var outputLength = input.Length / 2;
        var output = new byte[outputLength];
        using (var sr = new StringReader(input))
        {
            for (var i = 0; i < outputLength; i++)
                output[i] = Convert.ToByte(new string(new char[2] { (char)sr.Read(), (char)sr.Read() }), 16);
        }
        return output;
    }
    public static string BytesToHex1(byte[] buffer, uint size)
    {
        if (buffer == null)
        {
            return string.Empty;
        }
        byte[] bytes = new byte[(3 * size) - 1];
        for (uint i = 0; i < size; i++)
        {
            string str = string.Format("{0:X2}", buffer[i]);
            bytes[(int)((IntPtr)(3 * i))] = (byte)str[0];
            bytes[(int)((IntPtr)((3 * i) + 1))] = (byte)str[1];
            if (i < (size - 1))
            {
                bytes[(int)((IntPtr)((3 * i) + 2))] = 0x2d;
            }
        }
        return Encoding.ASCII.GetString(bytes);
    }

    public static byte[] Deflate(byte[] buffer, int sizeCompressed, int sizeDecompressed)
    {
        byte[] buffer2 = new byte[sizeDecompressed];
        new ZlibStream(new MemoryStream(buffer, 0, sizeCompressed), CompressionMode.Decompress).Read(buffer2, 0, sizeDecompressed);
        return buffer2;
    }

    public static byte[] HexToBytes1(string buffer, uint size)
    {
        if (buffer == string.Empty)
        {
            return null;
        }
        byte[] bytes = Encoding.ASCII.GetBytes(buffer);
        byte[] buffer3 = new byte[(bytes.Length + 1) / 3];
        for (uint i = 0; i < size; i++)
        {
            string s = ((char)bytes[(int)((IntPtr)(3 * i))]) + string.Empty + ((char)bytes[(int)((IntPtr)((3 * i) + 1))]);
            buffer3[i] = byte.Parse(s, NumberStyles.HexNumber);
        }
        return buffer3;
    }

    public static byte[] Inflate(byte[] buffer, uint sizeDecompressed, ref uint sizeCompressed, uint compressionLevel)
    {
        byte[] buffer2 = null;
        using (MemoryStream stream = new MemoryStream((int)sizeDecompressed))
        {
            using (ZlibStream stream2 = new ZlibStream(stream, CompressionMode.Compress, (CompressionLevel)compressionLevel))
            {
                stream2.Write(buffer, 0, (int)sizeDecompressed);
            }
            buffer2 = stream.ToArray();
            sizeCompressed = (uint)buffer2.Length;
            return buffer2;
        }
    }
}
public static class EnumerableEx
{
    public static IEnumerable<string> SplitBy(this string str, int chunkLength)
    {
        if (String.IsNullOrEmpty(str)) throw new ArgumentException();
        if (chunkLength < 1) throw new ArgumentException();

        for (int i = 0; i < str.Length; i += chunkLength)
        {
            if (chunkLength + i > str.Length)
                chunkLength = str.Length - i;

            yield return str.Substring(i, chunkLength);
        }
    }
}
