namespace Lyt.AdobeSwatchExchange.Structures;

public sealed class ColorGroup : Block, IEnumerable<ColorEntry>
{
    public ColorGroup(string name) : base(name) => this.Colors = [];

    public ColorGroup() : base(string.Empty) => this.Colors = [];

    public ColorEntryCollection Colors { get; set; }

    /// <summary> Returns an enumerator that iterates through the collection. </summary>
    public IEnumerator<ColorEntry> GetEnumerator() => this.Colors.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}

public class ColorGroupCollection : Collection<ColorGroup> { }
