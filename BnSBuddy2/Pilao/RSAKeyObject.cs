using System;
using System.Formats.Asn1;
using System.Linq;
using System.Security.Cryptography;

namespace bnstool
{
	public class RSAKeyObject
	{
		private string _str;

		private byte[] _keyBlob;

		public byte[] KeyBlob => _keyBlob;

		public unsafe RSAKeyObject(byte[] keyBlob)
		{
			AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER);
			asnWriter.PushSequence();
			fixed (byte* ptr = keyBlob)
			{
				BCRYPT_RSAKEY_BLOB* ptr2 = (BCRYPT_RSAKEY_BLOB*)ptr;
				int num = sizeof(BCRYPT_RSAKEY_BLOB);
				asnWriter.WriteInteger(new ReadOnlySpan<byte>(ptr + num + ptr2->cbPublicExp, ptr2->cbModulus));
				asnWriter.WriteInteger(new ReadOnlySpan<byte>(ptr + num, ptr2->cbPublicExp));
			}
			asnWriter.PopSequence();
			byte[] buffer = asnWriter.Encode();
			using (SHA1 sHA = SHA1.Create())
			{
				_str = BitConverter.ToString(sHA.ComputeHash(buffer)).Replace("-", string.Empty);
			}
			_keyBlob = keyBlob;
		}

		public override bool Equals(object obj)
		{
			RSAKeyObject rSAKeyObject = obj as RSAKeyObject;
			if (rSAKeyObject != null)
			{
				return _keyBlob.SequenceEqual(rSAKeyObject._keyBlob);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(_keyBlob);
		}

		public override string ToString()
		{
			return _str;
		}
	}
}
