namespace Lyt.Peaks;

/// <summary> Stores information about flat peak plateaus. </summary>
/// <remarks> 
/// Some peaks may have a flat top (plateau) rather than a single highest point.
/// This structure contains information about the plateau size and its edges.
/// </remarks>
public struct LprPeakPlateau
{
    public int PlateauSize; // Number of samples in the peak's plateau 

    public int LeftEdge;    // Index of the leftmost sample in the plateau 

    public int RightEdge;   // Index of the rightmost sample in the plateau 
}
