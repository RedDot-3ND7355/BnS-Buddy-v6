using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BnSBuddy2.Packets
{
	// Token: 0x02000062 RID: 98
	public class RC4
	{
		// Token: 0x06000254 RID: 596 RVA: 0x00009770 File Offset: 0x00007970
		public RC4(byte[] key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			this._s = this.EncryptInitialize(key);
		}

		// Token: 0x06000255 RID: 597 RVA: 0x00009794 File Offset: 0x00007994
		private static void Swap(byte[] s, int i, int j)
		{
			byte b = s[i];
			s[i] = s[j];
			s[j] = b;
		}

		// Token: 0x06000256 RID: 598 RVA: 0x000097B0 File Offset: 0x000079B0
		public string Encrypt(string data)
		{
			Encoding unicode = Encoding.Unicode;
			return Convert.ToBase64String(this.Encrypt(unicode.GetBytes(data)));
		}

		// Token: 0x06000257 RID: 599 RVA: 0x000097D5 File Offset: 0x000079D5
		public string Decrypt(string data)
		{
			return Encoding.Unicode.GetString(this.Encrypt(Convert.FromBase64String(data)));
		}

		// Token: 0x06000258 RID: 600 RVA: 0x000097ED File Offset: 0x000079ED
		public byte[] Encrypt(byte[] data)
		{
			return this.EncryptOutput(data).ToArray<byte>();
		}

		// Token: 0x06000259 RID: 601 RVA: 0x000097FB File Offset: 0x000079FB
		public byte[] Decrypt(byte[] data)
		{
			return this.EncryptOutput(data).ToArray<byte>();
		}

		// Token: 0x0600025A RID: 602 RVA: 0x0000980C File Offset: 0x00007A0C
		private byte[] EncryptInitialize(byte[] key)
		{
			byte[] array = (from i in Enumerable.Range(0, 256)
				select (byte)i).ToArray<byte>();
			int j = 0;
			int num = 0;
			while (j < 256)
			{
				num = (num + (int)key[j % key.Length] + (int)array[j]) & 255;
				RC4.Swap(array, j, num);
				j++;
			}
			return array;
		}

        // Token: 0x0600025B RID: 603 RVA: 0x0000987D File Offset: 0x00007A7D
        private IEnumerable<byte> EncryptOutput(IEnumerable<byte> data)
        {
            return data.Select(delegate (byte b)
            {
                _i = (_i + 1) & 0xFF;
                _j = (_j + _s[_i]) & 0xFF;
                Swap(_s, _i, _j);
                return (byte)(b ^ _s[(_s[_i] + _s[_j]) & 0xFF]);
            });
        }

        // Token: 0x0400016A RID: 362
        private readonly byte[] _s;

		// Token: 0x0400016B RID: 363
		private int _i;

		// Token: 0x0400016C RID: 364
		private int _j;
	}
}
