namespace Lyt.AdobeSwatchExchange.Structures;

internal enum BlockType
{
    Color = 0x0001,
    GroupStart = 0xc001,
    GroupEnd = 0xc002
}

public abstract class Block(string name)
{
    public string Name { get; set; } = name;

    public byte[] ExtraData { get; set; } = new byte[4];
}
