using System;
using System.Collections.Generic;
using System.Text;

public class bnsTool
{
    public static int index;

    public static bool CheckUnicodeString(string value)
    {
        for (int i = 0; i < value.Length; i++)
        {
            if (value[i] > '\ufffd')
            {
                return true;
            }
            if (value[i] < ' ' && ((value[i] != '\t') & (value[i] != '\n') & (value[i] != '\r')))
            {
                return true;
            }
        }
        return false;
    }

    public static List<string> LookupSplitToWords(byte[] data, uint size)
    {
        Encoding encoding = new UnicodeEncoding(bigEndian: false, byteOrderMark: false);
        uint num = 0u;
        uint num2 = 0u;
        List<string> list = new List<string>();
        if (data == null || size == 0)
        {
            return list;
        }
        if (size > data.Length)
        {
            size = (uint)data.Length;
        }
        uint num3 = 0u;
        for (; num2 + 1 < size; num2 += 2)
        {
            if (data[num2] != 0 || data[num2 + 1] != 0)
            {
                continue;
            }
            num3 = num;
            byte[] array = new byte[num2 - num];
            Array.Copy(data, num3, array, 0L, num2 - num);
            string @string = encoding.GetString(array);
            if (!CheckUnicodeString(@string))
            {
                list.Add(@string);
            }
            else
            {
                string text = "invalidzhangjieyong";
                for (int i = 0; i < array.Length; i++)
                {
                    text = ((i >= array.Length - 1) ? (text + array[i]) : (text + array[i] + ","));
                }
                list.Add(text);
            }
            num = num2 + 2;
        }
        return list;
    }

	public static byte[] WordToLookUpData(string[] newWorlds, ref int SizeLookup)
	{
		Encoding encoding = new UnicodeEncoding(bigEndian: false, byteOrderMark: false);
		SizeLookup = 0;
		if (newWorlds == null || newWorlds.Length == 0)
		{
			return new byte[0];
		}
		int[] array = new int[newWorlds.Length];
		for (int i = 0; i < newWorlds.Length; i++)
		{
			if (string.IsNullOrEmpty(newWorlds[i]))
			{
				array[i] = 2;
			}
			else if (!newWorlds[i].StartsWith("invalidzhangjieyong"))
			{
				array[i] = 2 * newWorlds[i].Length + 2;
			}
			else
			{
				char[] separator = new char[1] { ',' };
				string rest = newWorlds[i].Replace("invalidzhangjieyong", string.Empty);
				array[i] = (string.IsNullOrEmpty(rest) ? 0 : rest.Split(separator).Length) + 2;
			}
			SizeLookup += array[i];
		}
		byte[] array2 = new byte[SizeLookup];
		for (int j = 0; j < SizeLookup; j++)
		{
			array2[j] = 0;
		}
		int num = 0;
		for (int k = 0; k < newWorlds.Length; k++)
		{
			if (!string.IsNullOrEmpty(newWorlds[k]))
			{
				if (!newWorlds[k].StartsWith("invalidzhangjieyong"))
				{
					Array.Copy(encoding.GetBytes(newWorlds[k]), 0, array2, num, array[k] - 2);
				}
				else
				{
					char[] separator2 = new char[1] { ',' };
					string[] array3 = newWorlds[k].Replace("invalidzhangjieyong", string.Empty).Split(separator2);
					byte[] array4 = new byte[array3.Length + 2];
					for (int l = 0; l < array4.Length; l++)
					{
						array4[l] = 0;
					}
					for (int m = 0; m < array3.Length; m++)
					{
						array4[m] = byte.Parse(array3[m]);
					}
					Array.Copy(array4, 0, array2, num, array[k] - 2);
				}
			}
			num += array[k];
		}
		return array2;
	}
}

