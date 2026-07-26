using Mono.Math;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BnSBuddy2.Packets
{
    public static class _3RDPartyProvider
    {
        // PreFlight
        private static string LoginUrl = "https://thirdparty.plaync.com";
        private static string AppId = "DE0112AE-63E3-4888-A235-14B01AD1691A";
        private static string Kind = "";
        private static string State = "";
        public static _RsaProvider rsaProvider;
        public static string PostData = "";
        public static string ProviderCode = "np";

        public static string Init(string _Kind)
        {
            Kind = _Kind;
            rsaProvider = new _RsaProvider();
            State = DateTime.Now.ToString("yyyyMMddHHmmssff");
            // Post Data Builder
            PostData = $"app_id={AppId}&alg=RSA256&key={rsaProvider.PublicKey}&return_uri=ncop:%2F%2Fpolling-thirdparty-authntoken&state={State}{(_Kind == "google" ? "&custom_params=access_type%3Doffline%26prompt%3Dconsent" : "" )}";
            // Uri Builder
            ProviderCode = _Kind;
            return $"{LoginUrl}/{ProviderCode}/login/authntoken";
        }
    }

    internal static class CryptoHelper
    {
        // Token: 0x06000060 RID: 96 RVA: 0x00005EE2 File Offset: 0x000040E2
        public static RijndaelManaged CreateAES256()
        {
            RijndaelManaged rijndaelManaged = new RijndaelManaged();
            rijndaelManaged.KeySize = 256;
            rijndaelManaged.BlockSize = 128;
            rijndaelManaged.Mode = CipherMode.CBC;
            rijndaelManaged.Padding = PaddingMode.PKCS7;
            rijndaelManaged.IV = new byte[16];
            rijndaelManaged.GenerateKey();
            return rijndaelManaged;
        }

        // Token: 0x06000061 RID: 97 RVA: 0x00005F20 File Offset: 0x00004120
        public static string AES256Encrypt(byte[] plainData, byte[] key)
        {
            if (plainData == null)
            {
                throw new ArgumentNullException("plainData");
            }
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            try
            {
                using (RijndaelManaged rijndaelManaged = new RijndaelManaged
                {
                    KeySize = 256,
                    BlockSize = 128,
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.PKCS7,
                    Key = key,
                    IV = new byte[16]
                })
                {
                    ICryptoTransform cryptoTransform = rijndaelManaged.CreateEncryptor(rijndaelManaged.Key, rijndaelManaged.IV);
                    byte[] array = null;
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(plainData, 0, plainData.Length);
                        }
                        array = memoryStream.ToArray();
                    }
                    return Convert.ToBase64String(array);
                }
            }
            catch (Exception)
            {
            }
            return string.Empty;
        }

        // Token: 0x06000062 RID: 98 RVA: 0x00006030 File Offset: 0x00004230
        public static string AES256Encrypt(string plainText, string key)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(plainText);
            byte[] bytes2 = Encoding.UTF8.GetBytes(key);
            return CryptoHelper.AES256Encrypt(bytes, bytes2);
        }

        // Token: 0x06000063 RID: 99 RVA: 0x0000605C File Offset: 0x0000425C
        public static string AES256Decrypt(string cipherText, string key)
        {
            string text;
            try
            {
                using (RijndaelManaged rijndaelManaged = new RijndaelManaged
                {
                    KeySize = 256,
                    BlockSize = 128,
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.PKCS7,
                    Key = Encoding.UTF8.GetBytes(key),
                    IV = new byte[16]
                })
                {
                    ICryptoTransform cryptoTransform = rijndaelManaged.CreateDecryptor();
                    byte[] array = null;
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
                        {
                            byte[] array2 = Convert.FromBase64String(cipherText);
                            cryptoStream.Write(array2, 0, array2.Length);
                        }
                        array = memoryStream.ToArray();
                    }
                    text = Encoding.UTF8.GetString(array);
                }
            }
            catch (Exception)
            {
                text = null;
            }
            return text;
        }

        // Token: 0x06000064 RID: 100 RVA: 0x00006158 File Offset: 0x00004358
        public static string AES128Decrypt(string textToDecrypt, string key)
        {
            string text;
            try
            {
                RijndaelManaged rijndaelManaged = new RijndaelManaged();
                rijndaelManaged.Mode = CipherMode.CBC;
                rijndaelManaged.Padding = PaddingMode.PKCS7;
                rijndaelManaged.KeySize = 128;
                rijndaelManaged.BlockSize = 128;
                byte[] array = Convert.FromBase64String(textToDecrypt);
                byte[] bytes = Encoding.UTF8.GetBytes(key);
                byte[] array2 = new byte[16];
                int num = bytes.Length;
                if (num > array2.Length)
                {
                    num = array2.Length;
                }
                Array.Copy(bytes, array2, num);
                rijndaelManaged.Key = array2;
                rijndaelManaged.IV = array2;
                byte[] array3 = rijndaelManaged.CreateDecryptor().TransformFinalBlock(array, 0, array.Length);
                text = Encoding.UTF8.GetString(array3);
            }
            catch (Exception)
            {
                text = null;
            }
            return text;
        }

        // Token: 0x06000065 RID: 101 RVA: 0x00006204 File Offset: 0x00004404
        public static string AES128Encrypt(string textToEncrypt, string key)
        {
            string text;
            try
            {
                RijndaelManaged rijndaelManaged = new RijndaelManaged();
                rijndaelManaged.Mode = CipherMode.CBC;
                rijndaelManaged.Padding = PaddingMode.PKCS7;
                rijndaelManaged.KeySize = 128;
                rijndaelManaged.BlockSize = 128;
                byte[] bytes = Encoding.UTF8.GetBytes(key);
                byte[] array = new byte[16];
                int num = bytes.Length;
                if (num > array.Length)
                {
                    num = array.Length;
                }
                Array.Copy(bytes, array, num);
                rijndaelManaged.Key = array;
                rijndaelManaged.IV = array;
                ICryptoTransform cryptoTransform = rijndaelManaged.CreateEncryptor();
                byte[] bytes2 = Encoding.UTF8.GetBytes(textToEncrypt);
                text = Convert.ToBase64String(cryptoTransform.TransformFinalBlock(bytes2, 0, bytes2.Length));
            }
            catch (Exception)
            {
                text = null;
            }
            return text;
        }

        // Token: 0x06000066 RID: 102 RVA: 0x000062AC File Offset: 0x000044AC
        public static string RSA2048Decrypt(string decrypted, string privateKey)
        {
            try
            {
                RSACryptoServiceProvider rsacryptoServiceProvider = new RSACryptoServiceProvider(2048);
                rsacryptoServiceProvider.FromXmlString(privateKey);
                byte[] array = rsacryptoServiceProvider.Decrypt(Convert.FromBase64String(decrypted), false);
                return Encoding.UTF8.GetString(array);
            }
            catch (Exception)
            {
            }
            return null;
        }

        // Token: 0x06000067 RID: 103 RVA: 0x000062FC File Offset: 0x000044FC
        public static AsymmetricCipherKeyPair GetRsaKeyPair(int keySize)
        {
            if (keySize <= 0)
            {
                throw new ArgumentException("Key size must be greater than zero");
            }
            RsaKeyPairGenerator rsaKeyPairGenerator = new RsaKeyPairGenerator();
            rsaKeyPairGenerator.Init(new KeyGenerationParameters(new SecureRandom(), keySize));
            return rsaKeyPairGenerator.GenerateKeyPair();
        }

        // Token: 0x06000068 RID: 104 RVA: 0x00006328 File Offset: 0x00004528
        public static string GetRsaPrivateKey(AsymmetricCipherKeyPair keyPair)
        {
            if (keyPair == null)
            {
                throw new ArgumentNullException("keyPair");
            }
            return Convert.ToBase64String(PrivateKeyInfoFactory.CreatePrivateKeyInfo(keyPair.Private).ToAsn1Object().GetDerEncoded());
        }

        // Token: 0x06000069 RID: 105 RVA: 0x00006352 File Offset: 0x00004552
        public static string GetRsaPublicKey(AsymmetricCipherKeyPair keyPair)
        {
            if (keyPair == null)
            {
                throw new ArgumentNullException("keyPair");
            }
            return Convert.ToBase64String(SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(keyPair.Public).ToAsn1Object().GetDerEncoded());
        }

        // Token: 0x0600006A RID: 106 RVA: 0x0000637C File Offset: 0x0000457C
        public static string DecryptRsa(string encryptedData, AsymmetricKeyParameter key)
        {
            if (string.IsNullOrEmpty(encryptedData))
            {
                throw new ArgumentNullException("encryptedData");
            }
            byte[] array = Utils.Base64ForUrlDecode(encryptedData);
            Pkcs1Encoding pkcs1Encoding = new Pkcs1Encoding(new RsaEngine());
            pkcs1Encoding.Init(false, key);
            return Encoding.UTF8.GetString(pkcs1Encoding.ProcessBlock(array, 0, array.Length));
        }
    }

    internal static class Utils
    {
        // Token: 0x06000077 RID: 119 RVA: 0x00006500 File Offset: 0x00004700
        public static byte[] Combine(byte[] first, byte[] second)
        {
            if (first == null)
            {
                throw new ArgumentNullException("first");
            }
            if (second == null)
            {
                throw new ArgumentNullException("second");
            }
            byte[] array = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, array, 0, first.Length);
            Buffer.BlockCopy(second, 0, array, first.Length, second.Length);
            return array;
        }

        // Token: 0x06000078 RID: 120 RVA: 0x00006554 File Offset: 0x00004754
        public static byte[] GenerateRandomBytes(int byteSize)
        {
            byte[] array = new byte[byteSize];
            new RNGCryptoServiceProvider().GetBytes(array);
            return array;
        }

        // Token: 0x06000079 RID: 121 RVA: 0x00006574 File Offset: 0x00004774
        public static void CreateRC4EncryptKey(byte[] serverRandomKey, byte[] clientRandomKey, byte[] premasterSecret, out byte[] serverEncKeyData, out byte[] clientEncKeyData)
        {
            if (serverRandomKey == null)
            {
                throw new ArgumentNullException("serverRandomKey");
            }
            if (clientRandomKey == null)
            {
                throw new ArgumentNullException("clientRandomKey");
            }
            if (premasterSecret == null)
            {
                throw new ArgumentNullException("premasterSecret");
            }
            byte[] array = new byte[48];
            int num = 0;
            int num2 = 0;
            do
            {
                byte[] array2 = new byte[premasterSecret.Length + serverRandomKey.Length + clientRandomKey.Length + num + 1];
                int num3 = num % 26 + 1;
                for (int i = 0; i < num3; i++)
                {
                    array2[i] = (byte)(65 + num3);
                }
                int num4 = 0;
                num4 += num3;
                Array.Copy(premasterSecret, 0, array2, num4, premasterSecret.Length);
                num4 += premasterSecret.Length;
                Array.Copy(clientRandomKey, 0, array2, num4, clientRandomKey.Length);
                num4 += clientRandomKey.Length;
                Array.Copy(serverRandomKey, 0, array2, num4, serverRandomKey.Length);
                byte[] array3 = new SHA256Managed().ComputeHash(array2);
                byte[] array4 = new byte[premasterSecret.Length + array3.Length];
                Array.Copy(premasterSecret, array4, premasterSecret.Length);
                Array.Copy(array3, 0, array4, premasterSecret.Length, array3.Length);
                byte[] array5 = new MD5CryptoServiceProvider().ComputeHash(array4);
                int num5 = array5.Length * num;
                int num6 = array5.Length;
                if (array.Length - num2 < num6)
                {
                    num6 = array.Length - num2;
                }
                Array.Copy(array5, 0, array, num5, num6);
                num++;
                num2 += num6;
            }
            while (num2 != array.Length);
            num = 0;
            num2 = 0;
            byte[] array6 = new byte[288];
            do
            {
                byte[] array7 = new byte[array.Length + clientRandomKey.Length + serverRandomKey.Length + num + 1];
                int num7 = num % 26 + 1;
                for (int j = 0; j < num7; j++)
                {
                    array7[j] = (byte)(65 + num7);
                }
                int num8 = 0;
                num8 += num7;
                Array.Copy(array, 0, array7, num8, array.Length);
                num8 += array.Length;
                Array.Copy(serverRandomKey, 0, array7, num8, serverRandomKey.Length);
                num8 += serverRandomKey.Length;
                Array.Copy(clientRandomKey, 0, array7, num8, clientRandomKey.Length);
                byte[] array8 = new SHA256Managed().ComputeHash(array7);
                byte[] array9 = new byte[array.Length + array8.Length];
                Array.Copy(array, array9, array.Length);
                Array.Copy(array8, 0, array9, array.Length, array8.Length);
                byte[] array10 = new MD5CryptoServiceProvider().ComputeHash(array9);
                int num9 = array10.Length * num;
                int num10 = array10.Length;
                if (array6.Length - num2 < num10)
                {
                    num10 = array6.Length - num2;
                }
                Array.Copy(array10, 0, array6, num9, num10);
                num++;
                num2 += num10;
            }
            while (num2 != array6.Length);
            clientEncKeyData = new byte[128];
            serverEncKeyData = new byte[128];
            int num11 = 0;
            num11 += 16;
            num11 += 16;
            Array.Copy(array6, num11, clientEncKeyData, 0, clientEncKeyData.Length);
            num11 += clientEncKeyData.Length;
            Array.Copy(array6, num11, serverEncKeyData, 0, serverEncKeyData.Length);
        }

        // Token: 0x0600007A RID: 122 RVA: 0x00006814 File Offset: 0x00004A14
        public static byte[] EncryptDataByRSA(byte[] source, byte[] serverPublicKey)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            byte[] array = (byte[])source.Clone();
            Array.Reverse(array);
            uint num = BitConverter.ToUInt32(serverPublicKey, 0);
            byte[] array2 = new byte[num];
            Array.Copy(serverPublicKey, 4, array2, 0, array2.Length);
            Array.Reverse(array2);
            byte[] array3 = new byte[BitConverter.ToUInt32(serverPublicKey, (int)(4U + num))];
            Array.Copy(serverPublicKey, (long)((ulong)(8U + num)), array3, 0L, (long)array3.Length);
            Array.Reverse(array3);
            BigInteger bigInteger = new BigInteger(array);
            BigInteger bigInteger2 = new BigInteger(array3);
            BigInteger bigInteger3 = new BigInteger(array2);
            byte[] bytes = bigInteger.modPow(bigInteger3, bigInteger2).getBytes();
            Array.Reverse(bytes);
            return bytes;
        }

        // Token: 0x0600007B RID: 123 RVA: 0x000068B0 File Offset: 0x00004AB0
        public static bool IsNamespaceExist(string aNamespace)
        {
            return (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                    from type in assembly.GetTypes()
                    where type.Namespace == aNamespace
                    select type).Any<Type>();
        }

        // Token: 0x0600007C RID: 124 RVA: 0x00006954 File Offset: 0x00004B54
        public static string StringListToBase64String(List<string> strList)
        {
            if (strList == null)
            {
                return string.Empty;
            }
            string text = string.Empty;
            foreach (string text2 in strList)
            {
                text = text + Utils.EncodingBase64String(text2) + "\n";
            }
            return text.Trim();
        }

        // Token: 0x0600007D RID: 125 RVA: 0x000069C4 File Offset: 0x00004BC4
        public static string StringDictionaryToString(Dictionary<string, string> dic)
        {
            if (dic == null)
            {
                return string.Empty;
            }
            string text = string.Empty;
            foreach (KeyValuePair<string, string> keyValuePair in dic)
            {
                text = string.Concat(new string[]
                {
                    text,
                    keyValuePair.Key,
                    "=",
                    Utils.EncodingBase64String(keyValuePair.Value),
                    "\n"
                });
            }
            return text.Trim();
        }

        // Token: 0x0600007E RID: 126 RVA: 0x00006A5C File Offset: 0x00004C5C
        public static string LongDictionaryToString(Dictionary<string, long> dic)
        {
            if (dic == null)
            {
                return string.Empty;
            }
            string text = string.Empty;
            foreach (KeyValuePair<string, long> keyValuePair in dic)
            {
                text = string.Concat(new string[]
                {
                    text,
                    keyValuePair.Key,
                    "=",
                    Utils.EncodingBase64String(keyValuePair.Value.ToString()),
                    "\n"
                });
            }
            return text.Trim();
        }

        // Token: 0x0600007F RID: 127 RVA: 0x00006AFC File Offset: 0x00004CFC
        public static IntPtr StringDictionaryToStringPtr(Dictionary<string, string> dic)
        {
            string text = Utils.StringDictionaryToString(dic);
            if (string.IsNullOrEmpty(text))
            {
                return IntPtr.Zero;
            }
            return Marshal.StringToHGlobalAuto(text);
        }

        // Token: 0x06000080 RID: 128 RVA: 0x00006B24 File Offset: 0x00004D24
        public static IntPtr LongDictionaryToStringStr(Dictionary<string, long> dic)
        {
            string text = Utils.LongDictionaryToString(dic);
            if (string.IsNullOrEmpty(text))
            {
                return IntPtr.Zero;
            }
            return Marshal.StringToHGlobalAuto(text);
        }

        // Token: 0x06000081 RID: 129 RVA: 0x00006B4C File Offset: 0x00004D4C
        public static string EnumListToString<T>(List<T> enumList)
        {
            if (enumList == null)
            {
                return string.Empty;
            }
            string text = string.Empty;
            foreach (T t in enumList)
            {
                text = text + Utils.EncodingBase64String(t.ToString()) + "\n";
            }
            return text.Trim();
        }

        // Token: 0x06000082 RID: 130 RVA: 0x00006BC8 File Offset: 0x00004DC8
        public static string StringListToString(List<string> strList)
        {
            if (strList == null)
            {
                return string.Empty;
            }
            string text = string.Empty;
            foreach (string text2 in strList)
            {
                text = text + Utils.EncodingBase64String(text2) + "\n";
            }
            return text.Trim();
        }

        // Token: 0x06000083 RID: 131 RVA: 0x00006C38 File Offset: 0x00004E38
        public static string EncodingBase64String(string inputText)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(inputText));
        }

        // Token: 0x06000084 RID: 132 RVA: 0x00006C4A File Offset: 0x00004E4A
        public static string Base64Encode(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                return string.Empty;
            }
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
        }

        // Token: 0x06000085 RID: 133 RVA: 0x00006C6C File Offset: 0x00004E6C
        public static string Base64Decode(string base64EncodedData)
        {
            if (string.IsNullOrEmpty(base64EncodedData))
            {
                return string.Empty;
            }
            byte[] array = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(array);
        }

        // Token: 0x06000086 RID: 134 RVA: 0x00006C99 File Offset: 0x00004E99
        public static string Base64ForUrlEncode(string plainText)
        {
            return Utils.Base64Encode(plainText).Replace("=", string.Empty).Replace("+", "-")
                .Replace("/", "_");
        }

        // Token: 0x06000087 RID: 135 RVA: 0x00006CCE File Offset: 0x00004ECE
        public static string Base64ForUrlEncode(byte[] data)
        {
            return Convert.ToBase64String(data).Replace("=", string.Empty).Replace("+", "-")
                .Replace("/", "_");
        }

        // Token: 0x06000088 RID: 136 RVA: 0x00006D04 File Offset: 0x00004F04
        public static byte[] Base64ForUrlDecode(string base64Data)
        {
            if (string.IsNullOrEmpty(base64Data))
            {
                throw new ArgumentNullException("base64Data");
            }
            string text = base64Data.Replace('_', '/').Replace('-', '+');
            int num = base64Data.Length % 4;
            if (num != 2)
            {
                if (num == 3)
                {
                    text += "=";
                }
            }
            else
            {
                text += "==";
            }
            return Convert.FromBase64String(text);
        }
    }

    internal class RsaKeyPair
    {
        // Token: 0x0600006B RID: 107 RVA: 0x000063CB File Offset: 0x000045CB
        public RsaKeyPair(AsymmetricCipherKeyPair keyPair)
        {
            this.KeyPair = keyPair;
            this.PrivateKey = CryptoHelper.GetRsaPrivateKey(keyPair);
            this.PublicKey = CryptoHelper.GetRsaPublicKey(keyPair);
        }

        // Token: 0x17000004 RID: 4
        // (get) Token: 0x0600006C RID: 108 RVA: 0x000063F2 File Offset: 0x000045F2
        // (set) Token: 0x0600006D RID: 109 RVA: 0x000063FA File Offset: 0x000045FA
        public AsymmetricCipherKeyPair KeyPair { get; set; }

        // Token: 0x17000005 RID: 5
        // (get) Token: 0x0600006E RID: 110 RVA: 0x00006403 File Offset: 0x00004603
        // (set) Token: 0x0600006F RID: 111 RVA: 0x0000640B File Offset: 0x0000460B
        public string PublicKey { get; set; }

        // Token: 0x17000006 RID: 6
        // (get) Token: 0x06000070 RID: 112 RVA: 0x00006414 File Offset: 0x00004614
        // (set) Token: 0x06000071 RID: 113 RVA: 0x0000641C File Offset: 0x0000461C
        public string PrivateKey { get; set; }
    }

    public class _RsaProvider : IDisposable
    {
        // Token: 0x06000072 RID: 114 RVA: 0x00006428 File Offset: 0x00004628
        public _RsaProvider()
        {
            AsymmetricCipherKeyPair rsaKeyPair = CryptoHelper.GetRsaKeyPair(2048);
            this._rsaKeyPair = new RsaKeyPair(rsaKeyPair);
        }

        // Token: 0x17000007 RID: 7
        // (get) Token: 0x06000073 RID: 115 RVA: 0x00006452 File Offset: 0x00004652
        public string PublicKey
        {
            get
            {
                return this._rsaKeyPair.PublicKey;
            }
        }

        // Token: 0x06000074 RID: 116 RVA: 0x0000645F File Offset: 0x0000465F
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Token: 0x06000075 RID: 117 RVA: 0x0000646E File Offset: 0x0000466E
        protected virtual void Dispose(bool disposable)
        {
            if (this._disposed)
            {
                return;
            }
            if (disposable)
            {
                this._rsaKeyPair = null;
            }
            this._disposed = true;
        }

        // Token: 0x06000076 RID: 118 RVA: 0x0000648C File Offset: 0x0000468C
        public string GetDecryptedToken(IList<string> encryptedData)
        {
            if (encryptedData == null)
            {
                throw new ArgumentNullException("encryptedData");
            }
            string text = string.Empty;
            foreach (string text2 in encryptedData)
            {
                string text3 = CryptoHelper.DecryptRsa(text2, this._rsaKeyPair.KeyPair.Private);
                text += text3;
            }
            return text;
        }

        // Token: 0x04000016 RID: 22
        private const int RsaKeySize = 2048;

        // Token: 0x04000017 RID: 23
        private bool _disposed;

        // Token: 0x04000018 RID: 24
        private RsaKeyPair _rsaKeyPair;
    }
}
