namespace Lyt.AdobeSwatchExchange.Structures;

public enum ColorType
{
    Global,
    Spot,
    Normal
}

public class ColorEntry : Block
{
    public ColorEntry() { }

    //public ColorEntry(Color color) : this(color.Name, color) { }

    //public ColorEntry(string name, Color color) : this(name, color.R, color.G, color.B) { }

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

    public ColorType Type { get; set; }

    //public Color ToColor()
    //{
    //    return Color.FromArgb(this.R, this.G, this.B);
    //}
}

public class ColorEntryCollection : Collection<ColorEntry> { }

public class ColorGroup : Block, IEnumerable<ColorEntry>
{
    public ColorGroup() => this.Colors = [];

    public ColorEntryCollection Colors { get; set; }

    /// <summary> Returns an enumerator that iterates through the collection. </summary>
    public IEnumerator<ColorEntry> GetEnumerator() => this.Colors.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}

public class ColorGroupCollection : Collection<ColorGroup> { }
