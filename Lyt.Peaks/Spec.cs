namespace Lyt.Peaks;

public struct Spec
{
    public int max_peaks_count;
    public int peaks_size;
    public PeakResult[] peaks_data;
    public int[] mask;
    public int[] priority_to_position;
    //    size_t* peak_idx;
}
