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
        string text = "";
        int num = 0;
        num = ((bytes.Length % 4 == 0) ? bytes.Length : (bytes.Length / 4 * 4));
        for (int i = 0; i < num; i += 4)
        {
            int num2 = bytes[i] | (bytes[i + 1] << 8) | (bytes[i + 2] << 16) | (bytes[i + 3] << 24);
            text = text + num2 + ",";
        }
        return text.TrimEnd(',');
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
        int num = input.Length / 2;
        byte[] array = new byte[num];
        using (StringReader stringReader = new StringReader(input))
        {
            for (int i = 0; i < num; i++)
            {
                array[i] = Convert.ToByte(new string(new char[2]
                {
                    (char)stringReader.Read(),
                    (char)stringReader.Read()
                }), 16);
            }
        }
        return array;
    }
    public static string BytesToHex1(byte[] buffer, uint size)
    {
        if (buffer == null)
        {
            return string.Empty;
        }
        byte[] array = new byte[3 * size - 1];
        for (uint num = 0u; num < size; num++)
        {
            string text = $"{buffer[num]:X2}";
            array[(int)(IntPtr)(3 * num)] = (byte)text[0];
            array[(int)(IntPtr)(3 * num + 1)] = (byte)text[1];
            if (num < size - 1)
            {
                array[(int)(IntPtr)(3 * num + 2)] = 45;
            }
        }
        return Encoding.ASCII.GetString(array);
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
        byte[] array = new byte[(bytes.Length + 1) / 3];
        for (uint num = 0u; num < size; num++)
        {
            char c = (char)bytes[(int)(IntPtr)(3 * num)];
            string text = c.ToString();
            string empty = string.Empty;
            c = (char)bytes[(int)(IntPtr)(3 * num + 1)];
            string s = text + empty + c;
            array[num] = byte.Parse(s, NumberStyles.HexNumber);
        }
        return array;
    }

    public static byte[] Inflate(byte[] buffer, uint sizeDecompressed, ref uint sizeCompressed, uint compressionLevel)
    {
        byte[] array = null;
        using (MemoryStream memoryStream = new MemoryStream((int)sizeDecompressed))
        {
            using (ZlibStream zlibStream = new ZlibStream(memoryStream, CompressionMode.Compress, (CompressionLevel)compressionLevel))
            {
                zlibStream.Write(buffer, 0, (int)sizeDecompressed);
            }

            array = memoryStream.ToArray();
        }
        sizeCompressed = (uint)array.Length;
        return array;
    }
}
public static class EnumerableEx
{
    public static IEnumerable<string> SplitBy(this string str, int chunkLength)
    {
        if (string.IsNullOrEmpty(str))
        {
            throw new ArgumentException();
        }
        if (chunkLength < 1)
        {
            throw new ArgumentException();
        }
        for (int i = 0; i < str.Length; i += chunkLength)
        {
            if (chunkLength + i > str.Length)
            {
                chunkLength = str.Length - i;
            }
            yield return str.Substring(i, chunkLength);
        }
    }
}