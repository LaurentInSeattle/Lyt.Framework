namespace Lyt.Peaks;

/// <summary> Represent a range of integer values </summary>
public struct IntRange
{
    public IntRange() { }

    public int min = 0;

    public int max = int.MaxValue;

    /// <summary> Set minimum and maximum values for an integer range </summary>
    public void set_mn_mx(int min, int max)
    {
        this.min = min;
        this.max = max;
    }

    /// <summary> Set minimum value for an integer range (maximum set to SIZE_MAX) </summary>
    public void set_mn(int min)
    {
        this.min = min;
        this.max = int.MaxValue;
    }
}
