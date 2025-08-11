namespace Lyt.Peaks;

/// <summary> Complete peak information. </summary>
/// <remarks> 
/// This structure aggregates all information about a detected peak, including its position, 
/// height, and various characteristics like prominence, width, threshold, and plateau information.
/// </remarks>
public struct PeakResult
{
    public int Peak;                    // Index of the peak in the input data 

    public double PeakHeight;           //  Height (value) of the peak 

    public LprPeakPlateau Plateau;       // Information about the peak's plateau 

    public LrPeakThreshold Threshold;    // Threshold values for the peak 

    public LprPeakProminence Prominence; // Prominence information for the peak 

    public WhlrPeakWidth Width;          // Width information for the peak 
}
