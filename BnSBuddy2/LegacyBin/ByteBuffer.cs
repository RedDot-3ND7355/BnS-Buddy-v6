using System.IO;

public class ByteBuffer
{
    private MemoryStream stream;
    public byte msg_cate;
    public short msg_id;
    private byte[] data;
    private int index;

    public ByteBuffer()
    {
        this.stream = new MemoryStream();
    }

    public bool IsClosed()
    {
        if (stream == null)
            return true;
        else
            return false;
    }

    public ByteBuffer(byte[] data)
    {
        this.data = data;
        this.index = 0;
    }

    public void Close()
    {
        this.stream.Close();
        this.stream = null;
    }

    public uint GetSize()
    {
        return (uint) this.stream.Length;
    }

    public byte ReadByte()
    {
        byte data = 0;
        MessageSerializer.Read(this.data, ref this.index, ref data);
        return data;
    }

    public byte[] ReadBytes(uint length)
    {
        byte[] data = new byte[length];
        MessageSerializer.Read(this.data, ref this.index, ref data);
        return data;
    }

    public float ReadFloat()
    {
        float data = 0f;
        MessageSerializer.Read(this.data, ref this.index, ref data);
        return data;
    }

    public int ReadInt()
    {
        int data = 0;
        MessageSerializer.Read(this.data, ref this.index, ref data);
        return data;
    }

    public long ReadLong()
    {
        long data = 0L;
        MessageSerializer.Read(this.data, ref this.index, ref data);
        return data;
    }

    public short ReadShort()
    {
        short data = 0;
        MessageSerializer.Read(this.data, ref this.index, ref data);
        return data;
    }

    public string ReadString()
    {
        string data = null;
        MessageSerializer.Read(this.data, ref this.index, ref data);
        return data;
    }

    public byte ReadUByte()
    {
        byte data = 0;
        MessageSerializer.Read(this.data, ref this.index, ref data);
        return data;
    }

    public uint ReadUInt()
    {
        uint data = 0;
        MessageSerializer.Read(this.data, ref this.index, ref data);
        return data;
    }

    public ulong ReadULong()
    {
        ulong data = 0UL;
        MessageSerializer.Read(this.data, ref this.index, ref data);
        return data;
    }

    public ushort ReadUShort()
    {
        ushort data = 0;
        MessageSerializer.Read(this.data, ref this.index, ref data);
        return data;
    }

    public void SeekI(uint pos, wxSeekMode seekMode = 0)
    {
        if (seekMode == wxSeekMode.wxFromStart)
        {
            this.index = (int) pos;
        }
        else if (seekMode == wxSeekMode.wxFromCurrent)
        {
            this.index += (int) pos;
        }
    }

    public void SeekO(uint pos, wxSeekMode seekMode = 0)
    {
        if (seekMode == wxSeekMode.wxFromStart)
        {
            this.stream.Seek((long) pos, SeekOrigin.Begin);
        }
        else if (seekMode == wxSeekMode.wxFromCurrent)
        {
            this.stream.Seek((long) pos, SeekOrigin.Current);
        }
    }

    public void SendInit(byte msg_cate, short msg_id)
    {
        this.msg_cate = msg_cate;
        this.msg_id = msg_id;
        MessageSerializer.Write(this.stream, (byte) 0x2f);
        MessageSerializer.Write(this.stream, msg_cate);
        MessageSerializer.Write(this.stream, msg_id);
        MessageSerializer.Write(this.stream, (short) 0);
    }

    public uint TellI()
    {
        return (uint) this.index;
    }

    public uint TellO()
    {
        return (uint) this.stream.Position;
    }

    public byte[] ToBytes()
    {
        return this.stream.ToArray();
    }

    public void WriteByte(byte data)
    {
        MessageSerializer.Write(this.stream, data);
    }

    public void WriteFloat(float data)
    {
        MessageSerializer.Write(this.stream, data);
    }

    public void WriteInt(int data)
    {
        MessageSerializer.Write(this.stream, data);
    }

    public void WriteLong(long data)
    {
        MessageSerializer.Write(this.stream, data);
    }

    public void WriteShort(short data)
    {
        MessageSerializer.Write(this.stream, data);
    }

    public void WriteString(string data)
    {
        MessageSerializer.Write(this.stream, data);
    }

    public void WriteUByte(byte data)
    {
        MessageSerializer.Write(this.stream, data);
    }

    public void WriteUInt(uint data)
    {
        MessageSerializer.Write(this.stream, data);
    }

    public void WriteULong(ulong data)
    {
        MessageSerializer.Write(this.stream, data);
    }

    public void WriteUShort(ushort data)
    {
        MessageSerializer.Write(this.stream, data);
    }
}

