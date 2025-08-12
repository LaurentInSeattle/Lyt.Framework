namespace Lyt.Peaks;

/// <summary> Stores the indices defining a peak's position. </summary>
/// <remarks>
/// This structure contains the left edge, middle point, and right edge indices
/// of a detected peak, defining its position and extent in the input data.
/// </remarks>
public struct LmrPeakIndex
{

    public int LeftEdge;   // Index of the leftmost sample belonging to the peak 

    public int MidPoint;   // Index of the peak's highest point (or middle of plateau) 
    
    public int RightEdge;  // Index of the rightmost sample belonging to the peak 

    public readonly string ToDebugString()
        => string.Format(
            "Left: {0}  Mid: {1}  Right: {2}", this.LeftEdge, this.MidPoint, this.RightEdge);
}
