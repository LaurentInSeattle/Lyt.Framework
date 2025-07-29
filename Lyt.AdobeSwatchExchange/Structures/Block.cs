namespace Lyt.AdobeSwatchExchange.Structures;

internal enum BlockType
{
    Color = 0x0001,
    GroupStart = 0xc001,
    GroupEnd = 0xc002
}

public abstract class Block
{
    public byte[] ExtraData { get; set; } = new byte[4];

    public string Name { get; set; } = string.Empty;
}
