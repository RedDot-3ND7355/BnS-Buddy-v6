using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

/*
public enum KeyBlobMagicNumber
{
  BCRYPT_RSAPUBLIC_MAGIC = 0x31415352,
  BCRYPT_RSAPRIVATE_MAGIC = 0x32415352,
}

[StructLayout(LayoutKind.Sequential)]
public struct BCRYPT_RSAKEY_BLOB
{
  public KeyBlobMagicNumber Magic;
  public int BitLength;
  public int cbPublicExp;
  public int cbModulus;
  public int cbPrime1;
  public int cbPrime2;
}*/

public static class bnscompression
{
    public enum CompressionLevel
    {
        None = -1,
        Fastest,
        Fast,
        Normal,
        Maximum,
    }

    public enum BinaryXmlVersion
    {
        None = -1,
        Version3 = 3,
        Version4 = 4
    }

    public enum DelegateResult
    {
        Continue,
        Skip,
        Cancel
    };

    [Flags]
    public enum ParseResult
    {
        OK = 0x0,
        InvalidIdentifier = 0x1,
        UnsupportedVersion = 0x2,
        InvalidCentralDirectoryHeader = 0x4,
        UnsupportedAesEncryptionKey = 0x8,
        UnsupportedRsaPrivateKeyOrInvalidSignature = 0x10,
        UnsupportedBinaryXmlVersion = 0x20
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Emit(byte[] rsaBlob, ref int offset, byte[] value)
    {
        Buffer.BlockCopy(value, 0, rsaBlob, offset, value.Length);
        offset += value.Length;
    }

    // Helper to construct BCRYPT_RSAKEY_BLOB structure from byte arrays
    public static byte[] GetRSAKeyBlob(byte[] exp, byte[] mod, byte[] p, byte[] q)
    {
        if (exp == null || mod == null || p == null || q == null)
            throw new CryptographicException();

        var blobSize = Marshal.SizeOf<BCRYPT_RSAKEY_BLOB>() + exp.Length + mod.Length + p.Length + q.Length;
        var keyBlob = new byte[blobSize];
        unsafe
        {
            fixed (byte* pRsaBlob = keyBlob)
            {
                var pBcryptBlob = (BCRYPT_RSAKEY_BLOB*)pRsaBlob;
                pBcryptBlob->Magic = KeyBlobMagicNumber.BCRYPT_RSAPRIVATE_MAGIC;
                pBcryptBlob->BitLength = mod.Length * 8;
                pBcryptBlob->cbPublicExp = exp.Length;
                pBcryptBlob->cbModulus = mod.Length;
                pBcryptBlob->cbPrime1 = p.Length;
                pBcryptBlob->cbPrime2 = q.Length;

                int offset = Marshal.SizeOf<BCRYPT_RSAKEY_BLOB>();
                Emit(keyBlob, ref offset, exp);
                Emit(keyBlob, ref offset, mod);
                Emit(keyBlob, ref offset, p);
                Emit(keyBlob, ref offset, q);
            }
        }
        return keyBlob;
    }


    // These functions can and will throw exceptions if something goes wrong, so you should catch
    // System.Runtime.InteropServices.SEHException and tell the user something went wrong.
    // Unfortunately, for now there is no way to convey this exception information back to you on the C# end.
    // Most of the time, it will probably mean a write permissions error (if not running as admin).

    [DllImport("bnscompression.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern ParseResult GetCreateParams([MarshalAs(UnmanagedType.LPWStr)] string fileName, [MarshalAs(UnmanagedType.U1)] out bool use64Bit, out CompressionLevel compressionLevel, out IntPtr encryptionKey, out uint encryptionKeySize, out IntPtr privateKeyBlob, out uint privateKeyBlobSize, out BinaryXmlVersion binaryXmlVersion);

    // Pre-process the file entry list and filter out anything you don't want, or cancel entirely.
    // Delegate Results
    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public delegate DelegateResult Delegate([MarshalAs(UnmanagedType.LPWStr)] string name, ulong approxSize);
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public delegate bool EnumerateEntriesDelegate([MarshalAs(UnmanagedType.LPWStr)] string name, ulong approxSize);

    // Static Functions
    [DllImport("bnscompression.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern double ExtractToDirectory([MarshalAs(UnmanagedType.LPWStr)] string sourceFileName, [MarshalAs(UnmanagedType.LPWStr)] string destinationDirectoryName, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1, SizeParamIndex = 3)] byte[] encryptionKey, uint encryptionKeySize, Delegate d);

    [DllImport("bnscompression.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern double CreateFromDirectory([MarshalAs(UnmanagedType.LPWStr)] string sourceDirectoryName, [MarshalAs(UnmanagedType.LPWStr)] string destinationFileName, [MarshalAs(UnmanagedType.U1)] bool use64Bit, CompressionLevel compressionLevel, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1, SizeParamIndex = 5)] byte[] encryptionKey, uint encryptionKeySize, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1, SizeParamIndex = 7)] byte[] privateKeyBlob, uint privateKeyBlobSize, BinaryXmlVersion binaryXmlVersion, Delegate d);

    [DllImport("bnscompression.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern void EnumerateEntries([MarshalAs(UnmanagedType.LPWStr)] string fileName, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1, SizeParamIndex = 2)] byte[] encryptionKey, uint encryptionKeySize, EnumerateEntriesDelegate d);

}
