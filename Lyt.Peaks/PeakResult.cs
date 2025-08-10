namespace Lyt.Peaks;

/// <summary> Complete peak information structure. </summary>
/// <remarks> 
/// This structure aggregates all information about a detected peak,
/// including its position, height, and various characteristics like
/// prominence, width, threshold, and plateau information.
/// </remarks>
public struct PeakResult
{
    public int peak;         // Index of the peak in the input data 
    public double peak_height;  //  Height (value) of the peak 

    public LprPeakPlateau plateau;       // Information about the peak's plateau 
    public LrPeakThreshold threshold;    // Threshold values for the peak 
    public LprPeakProminence prominence; // Prominence information for the peak 
    public WhlrPeakWidth width;          // Width information for the peak 
}
