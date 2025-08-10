namespace Lyt.Peaks;

/// <summary> Represent a range of double values </summary>
public struct DoubleRange
{
    public DoubleRange() { }

    public double min = double.MinValue;
    
    public double max = double.MaxValue;

    /// <summary> Set minimum and maximum values for a double range. </summary>
    public void set_mn_mx(double min, double max)
    {
        this.min = min;
        this.max = max;
    }

    /// <summary> Set minimum value for a double range (maximum set to DBL_MAX) </summary>
    public void set_mn(double min)
    {
        this.min = min;
        this.max = double.MaxValue;
    }
}
