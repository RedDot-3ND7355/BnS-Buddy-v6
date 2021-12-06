using System.IO;

public interface IMessage
{
    void serialize(MemoryStream ms);
    void unserialize(byte[] buffer, ref int index);
}
