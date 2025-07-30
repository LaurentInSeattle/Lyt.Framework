namespace Lyt.AdobeSwatchExchange.Structures;

public enum ColorType
{
    Global,
    Spot,
    Normal
}

public sealed class ColorEntry : Block
{
    public ColorEntry() : base(string.Empty) { }

    public ColorEntry(byte r, byte g, byte b) : this(string.Empty, r, g, b) { }

    public ColorEntry(string name, byte r, byte g, byte b) : this()
    {
        this.Name = name;
        this.R = r;
        this.G = g;
        this.B = b;
    }

    public byte B { get; set; }

    public byte G { get; set; }

    public byte R { get; set; }

    public ColorType Type { get; set; } = ColorType.Global;
}

public class ColorEntryCollection : Collection<ColorEntry> { }

