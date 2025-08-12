namespace Lyt.Peaks;

/// <summary> Stores peak threshold information. </summary>
/// <remarks> 
/// Threshold represents how much a peak rises above its neighboring valleys.
/// This structure contains the threshold values for the left and right sides of the peak.
/// </remarks>
public struct LrPeakThreshold
{
    public double LeftThreshold;  // Height difference between peak and left neighbor valley 

    public double RightThreshold; // Height difference between peak and right neighbor valley 

    public readonly string ToDebugString()
        => string.Format(
            "Left Threshold: {0:F2}   Right Threshold: {1:F2}", this.LeftThreshold, this.RightThreshold);
}
