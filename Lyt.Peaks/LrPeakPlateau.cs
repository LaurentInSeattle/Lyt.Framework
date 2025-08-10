namespace Lyt.Peaks;

/// <summary> Stores information about flat peak plateaus. </summary>
/// <remarks> 
/// Some peaks may have a flat top (plateau) rather than a single highest point.
/// This structure contains information about the plateau size and its edges.
/// </remarks>
public struct LprPeakPlateau
{
    public int plateau_size; // Number of samples in the peak's plateau 
    public int left_edge;    // Index of the leftmost sample in the plateau 
    public int right_edge;   // Index of the rightmost sample in the plateau 
}
