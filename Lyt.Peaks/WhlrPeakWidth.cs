namespace Lyt.Peaks;

/// <summary> Stores peak width information. </summary>
/// <remarks>
/// Width characterizes the horizontal extent of a peak at a specific height.
/// This structure contains the calculated width and the interpolated positions
/// where the peak crosses the width height level.
/// </remarks>
public struct WhlrPeakWidth
{
    public double Width;        // Calculated width of the peak 

    public double WidthHeight; // Height level at which width is measured 

    public double LeftIp;      // Interpolated position of the left width crossing point 

    public double RightIp;     // Interpolated position of the right width crossing point 
}
