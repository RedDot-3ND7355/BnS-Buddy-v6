using System;
using System.IO;
using Ionic.Zlib;

namespace BnSBuddy2.LegacyBin
{
	public class BNSDat
	{
		public static bool newversion = false;

		public string BytesToHex(byte[] bytes)
		{
			char[] array = new char[bytes.Length * 2];
			int num = 0;
			int num2 = 0;
			while (num < bytes.Length)
			{
				byte b = (byte)(bytes[num] >> 4);
				array[num2] = (char)((b > 9) ? (b + 55 + 32) : (b + 48));
				b = (byte)(bytes[num] & 0xFu);
				array[++num2] = (char)((b > 9) ? (b + 55 + 32) : (b + 48));
				num++;
				num2++;
			}
			return new string(array);
		}

		public byte[] Deflate(byte[] buffer, int sizeCompressed, int sizeDecompressed)
		{
			byte[] array = ZlibStream.UncompressBuffer(buffer);
			byte[] array2 = new byte[sizeDecompressed];
			if (array.Length > sizeDecompressed)
			{
				Array.Copy(array, 0, array2, 0, sizeDecompressed);
			}
			else
			{
				Array.Copy(array, 0, array2, 0, array.Length);
			}
            array = array2;
			array2 = null;
            return array;
		}

		public byte[] Inflate(byte[] buffer, int sizeDecompressed, out int sizeCompressed, int compressionLevel)
		{
			MemoryStream memoryStream = new MemoryStream();
			ZlibStream zlibStream = new ZlibStream(memoryStream, CompressionMode.Compress, (CompressionLevel)compressionLevel, leaveOpen: true);
			zlibStream.Write(buffer, 0, sizeDecompressed);
			zlibStream.Flush();
			zlibStream.Close();
			sizeCompressed = (int)memoryStream.Length;
			return memoryStream.ToArray();
		}
	}
}
