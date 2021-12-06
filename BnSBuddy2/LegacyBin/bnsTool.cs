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
            if (value[i] > 0xfffd)
            {
                return true;
            }
            if ((value[i] < ' ') && (((value[i] != '\t') & (value[i] != '\n')) & (value[i] != '\r')))
            {
                return true;
            }
        }
        return false;
    }

    public static List<string> LookupSplitToWords(byte[] data, uint size)
    {
        Encoding encoding = new UnicodeEncoding(false, false);
        uint num = 0;
        uint index = 0;
        List<string> list = new List<string>();
        uint num3 = 0;
        while (index < size)
        {
            if ((data[index] == 0) && (data[(int) ((IntPtr) (index + 1))] == 0))
            {
                num3 = num;
                byte[] destinationArray = new byte[index - num];
                Array.Copy(data, (long) num3, destinationArray, 0L, (long) (index - num));
                string str = encoding.GetString(destinationArray);
                if (!CheckUnicodeString(str))
                {
                    list.Add(str);
                }
                else
                {
                    string item = "invalidzhangjieyong";
                    int num4 = 0;
                    while (true)
                    {
                        if (num4 >= destinationArray.Length)
                        {
                            list.Add(item);
                            break;
                        }
                        item = (num4 >= (destinationArray.Length - 1)) ? (item + destinationArray[num4]) : (item + destinationArray[num4] + ",");
                        num4++;
                    }
                }
                num = index + 2;
            }
            index += 2;
        }
        return list;
    }

    public static byte[] WordToLookUpData(string[] newWorlds, ref int SizeLookup)
    {
        Encoding encoding = new UnicodeEncoding(false, false);
        SizeLookup = 0;
        int[] numArray = new int[newWorlds.Length];
        for (int i = 0; i < newWorlds.Length; i++)
        {
            if (string.IsNullOrEmpty(newWorlds[i]))
            {
                numArray[i] = 2;
            }
            else if (!newWorlds[i].StartsWith("invalidzhangjieyong"))
            {
                numArray[i] = (2 * newWorlds[i].Length) + 2;
            }
            else
            {
                char[] separator = new char[] { ',' };
                numArray[i] = newWorlds[i].Replace("invalidzhangjieyong", string.Empty).Split(separator).Length + 2;
            }
            SizeLookup += numArray[i];
        }
        byte[] destinationArray = new byte[SizeLookup];
        for (int j = 0; j < SizeLookup; j++)
        {
            destinationArray[j] = 0;
        }
        int destinationIndex = 0;
        for (int k = 0; k < newWorlds.Length; k++)
        {
            if (!string.IsNullOrEmpty(newWorlds[k]))
            {
                if (!newWorlds[k].StartsWith("invalidzhangjieyong"))
                {
                    Array.Copy(encoding.GetBytes(newWorlds[k]), 0, destinationArray, destinationIndex, numArray[k] - 2);
                }
                else
                {
                    char[] separator = new char[] { ',' };
                    string[] strArray2 = newWorlds[k].Replace("invalidzhangjieyong", string.Empty).Split(separator);
                    byte[] sourceArray = new byte[strArray2.Length + 2];
                    int index = 0;
                    while (true)
                    {
                        if (index >= sourceArray.Length)
                        {
                            int num6 = 0;
                            while (true)
                            {
                                if (num6 >= strArray2.Length)
                                {
                                    Array.Copy(sourceArray, 0, destinationArray, destinationIndex, numArray[k] - 2);
                                    break;
                                }
                                sourceArray[num6] = byte.Parse(strArray2[num6]);
                                num6++;
                            }
                            break;
                        }
                        sourceArray[index] = 0;
                        index++;
                    }
                }
            }
            destinationIndex += numArray[k];
        }
        return destinationArray;
    }
}

