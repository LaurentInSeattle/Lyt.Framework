namespace Lyt.AdobeSwatchExchange;

public static class AseBuilder
{
    public static AseDocument Build(IEnumerable<ColorGroup> colorGroups)
    {
        var ase = new AseDocument();
        foreach (var colorGroup in colorGroups)
        {
            ase.Groups.Add(colorGroup);
        } 

        return ase;
    }
}
