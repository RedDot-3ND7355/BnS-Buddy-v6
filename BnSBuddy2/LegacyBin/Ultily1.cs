namespace BnSBuddy2.LegacyBin
{
	public static class Ultily
	{
		public static int ReadIntFrom2Bytes(byte[] bytes)
		{
			return bytes[0] | (bytes[1] << 8);
		}

		public static byte[] WriteIntTo2Bytes(int value)
		{
			return new byte[2]
			{
				(byte)((uint)value & 0xFFu),
				(byte)((uint)(value >> 8) & 0xFFu)
			};
		}
	}
}

