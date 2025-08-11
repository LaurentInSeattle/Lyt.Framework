namespace Lyt.Peaks;

/// <summary> Stores peak prominence information. </summary>
/// <remarks>  
///  Prominence quantifies how much a peak stands out from the surrounding baseline.
/// This structure contains the prominence value and the indices of the left and right
///  base points used to calculate it.
/// </remarks>
public struct LprPeakProminence
{
    public int left_base;       // Index of the left base point used for prominence calculation 

    public double prominence;   // Calculated prominence value of the peak 

    public int right_base;      // Index of the right base point used for prominence calculation 
}
