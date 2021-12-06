using System;
using System.Linq;
using System.Numerics;
using System.Text;

namespace bnstool
{
	public class AESKeyObject
	{
		private string _str;

		private byte[] _key;

		public byte[] Key => _key;

		public AESKeyObject(string str)
		{
			_str = str;
			_key = Encoding.ASCII.GetBytes(str);
		}

		public AESKeyObject(byte[] key)
		{
			_str = BitConverter.ToString(key).Replace("-", string.Empty);
			_key = key;
		}

		public override string ToString()
		{
			return _str;
		}

		public override bool Equals(object obj)
		{
			AESKeyObject aESKeyObject = obj as AESKeyObject;
			if (aESKeyObject == null)
			{
				return false;
			}
			return _key.SequenceEqual(aESKeyObject._key);
		}

		public override int GetHashCode()
		{
			return new BigInteger(_key).GetHashCode();
		}
	}
}
