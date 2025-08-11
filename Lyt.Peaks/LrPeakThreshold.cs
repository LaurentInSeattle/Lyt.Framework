namespace Lyt.Peaks;

/// <summary> Stores peak threshold information. </summary>
/// <remarks> 
/// Threshold represents how much a peak rises above its neighboring valleys.
/// This structure contains the threshold values for the left and right sides of the peak.
/// </remarks>
public struct LrPeakThreshold
{
    public double left_threshold;  // Height difference between peak and left neighbor valley 

    public double right_threshold; // Height difference between peak and right neighbor valley 
}
