using System;
using System.IO;
using System.Text;

public static class MessageSerializer
{
    public static void Read(byte[] buffer, ref int index, ref byte data)
    {
        int num;
        index = (num = index) + 1;
        data = buffer[num];
    }

    public static unsafe void Read(byte[] buffer, ref int index, ref byte[] data)
    {
        byte[] nums = new byte[0];
        for (int i = 0; i < data.Length; i++)
        {
            int num3;
            index = (num3 = index) + 1;
            nums[i] = buffer[num3];
        }
        data = nums;
    }

    public static unsafe void Read(byte[] buffer, ref int index, ref short data)
    {
        short num = 0;
        byte* numPtr = (byte*) &num;
        for (int i = 0; i < 2; i++)
        {
            int num3;
            index = (num3 = index) + 1;
            *(numPtr++) = buffer[num3];
        }
        data = num;
    }

    public static unsafe void Read(byte[] buffer, ref int index, ref int data)
    {
        int num = 0;
        byte* numPtr = (byte*) &num;
        for (int i = 0; i < 4; i++)
        {
            int num3;
            index = (num3 = index) + 1;
            *(numPtr++) = buffer[num3];
        }
        data = num;
    }

    public static unsafe void Read(byte[] buffer, ref int index, ref long data)
    {
        long num = 0L;
        byte* numPtr = (byte*) &num;
        for (int i = 0; i < 8; i++)
        {
            int num3;
            index = (num3 = index) + 1;
            *(numPtr++) = buffer[num3];
        }
        data = num;
    }

    public static unsafe void Read(byte[] buffer, ref int index, ref float data)
    {
        float num = 0f;
        byte* numPtr = (byte*) &num;
        for (int i = 0; i < 4; i++)
        {
            int num3;
            index = (num3 = index) + 1;
            *(numPtr++) = buffer[num3];
        }
        data = num;
    }

    public static void Read(byte[] buffer, ref int index, ref string data)
    {
        short num = 0;
        Read(buffer, ref index, ref num);
        data = Encoding.UTF8.GetString(buffer, index, num);
        index += num;
    }

    public static unsafe void Read(byte[] buffer, ref int index, ref ushort data)
    {
        ushort num = 0;
        byte* numPtr = (byte*) &num;
        for (int i = 0; i < 2; i++)
        {
            int num3;
            index = (num3 = index) + 1;
            *(numPtr++) = buffer[num3];
        }
        data = num;
    }

    public static unsafe void Read(byte[] buffer, ref int index, ref uint data)
    {
        uint num = 0;
        byte* numPtr = (byte*) &num;
        for (int i = 0; i < 4; i++)
        {
            int num3;
            index = (num3 = index) + 1;
            *(numPtr++) = buffer[num3];
        }
        data = num;
    }

    public static unsafe void Read(byte[] buffer, ref int index, ref ulong data)
    {
        ulong num = 0UL;
        byte* numPtr = (byte*) &num;
        for (int i = 0; i < 8; i++)
        {
            int num3;
            index = (num3 = index) + 1;
            *(numPtr++) = buffer[num3];
        }
        data = num;
    }

    public static void Read<T>(byte[] buffer, ref int index, ref T data) where T: IMessage, new()
    {
        data = Activator.CreateInstance<T>();
        data.unserialize(buffer, ref index);
    }

    public static void Write(MemoryStream ms, IMessage data)
    {
        data.serialize(ms);
    }

    public static void Write(MemoryStream ms, byte data)
    {
        ms.WriteByte(data);
    }

    public static unsafe void Write(MemoryStream ms, short data)
    {
        byte* numPtr = (byte*) &data;
        for (int i = 0; i < 2; i++)
        {
            ms.WriteByte(*(numPtr++));
        }
    }

    public static unsafe void Write(MemoryStream ms, int data)
    {
        byte* numPtr = (byte*) &data;
        for (int i = 0; i < 4; i++)
        {
            ms.WriteByte(*(numPtr++));
        }
    }

    public static unsafe void Write(MemoryStream ms, long data)
    {
        byte* numPtr = (byte*) &data;
        for (int i = 0; i < 8; i++)
        {
            ms.WriteByte(*(numPtr++));
        }
    }

    public static unsafe void Write(MemoryStream ms, float data)
    {
        byte* numPtr = (byte*) &data;
        for (int i = 0; i < 4; i++)
        {
            ms.WriteByte(*(numPtr++));
        }
    }

    public static void Write(MemoryStream ms, string data)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(data);
        Write(ms, (short) bytes.Length);
        ms.Write(bytes, 0, bytes.Length);
    }

    public static unsafe void Write(MemoryStream ms, ushort data)
    {
        byte* numPtr = (byte*) &data;
        for (int i = 0; i < 2; i++)
        {
            ms.WriteByte(*(numPtr++));
        }
    }

    public static unsafe void Write(MemoryStream ms, uint data)
    {
        byte* numPtr = (byte*) &data;
        for (int i = 0; i < 4; i++)
        {
            ms.WriteByte(*(numPtr++));
        }
    }

    public static unsafe void Write(MemoryStream ms, ulong data)
    {
        byte* numPtr = (byte*) &data;
        for (int i = 0; i < 8; i++)
        {
            ms.WriteByte(*(numPtr++));
        }
    }
}

