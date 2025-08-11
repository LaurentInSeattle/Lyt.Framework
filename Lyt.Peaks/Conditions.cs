namespace Lyt.Peaks;

/// <summary> Configuration parameters for peak detection </summary>
/// <remarks>
/// This structure contains all the configurable parameters that control which
/// features in the data are identified as peaks and which are filtered out.
/// Each field represents a different criterion for peak detection and filtering.
/// </remarks>
public struct Conditions
{
    public Conditions() { }

    // Height range for peak filtering
    // Height condition defines the absolute height range a peak must have to be detected.
    // Only peaks with heights within this range will be detected.
    // Default range is [-DBL_MAX, DBL_MAX] (all peaks regardless of height).
    public DoubleRange Height = new();

    // Threshold range for peak filtering
    // Threshold condition defines how much a data point needs to exceed its neighbors to be considered a peak.
    // Only peaks that rise above their neighboring valleys by an amount within
    // this range will be detected. This measures how distinct a peak is from its immediate surroundings.
    // Default range is [-DBL_MAX, DBL_MAX] (all peaks regardless of threshold).
    public DoubleRange Threshold = new();

    // Minimum distance between peaks
    // Distance condition ensures peaks are separated by at least the specified number of samples.
    // Ensures peaks are separated by at least this many samples. When multiple peaks are found within this
    // distance, only the highest one is kept.
    // Default value is 1 (adjacent peaks allowed).
    public int Distance = 1;

    // Prominence range for peak filtering
    // Prominence quantifies how much a peak stands out from surrounding baseline.
    // Only peaks with prominence values within this range will be detected. 
    // Default range is [-DBL_MAX, DBL_MAX] (all peaks regardless of prominence).
    public DoubleRange Prominence = new();

    // Width range for peak filtering
    // Defines the Required width condition of peaks in samples. Only peaks with widths within this
    // range will be detected. Width is measured at a height determined by rel_height.
    // Default range is [-DBL_MAX, DBL_MAX] (all peaks regardless of width).
    public DoubleRange Width = new();

    // Window length for prominence and width calculations
    // Set a window length in samples that optionally limits the evaluated area for each peak
    // Defines the size of the window used for calculating peak widths.
    // The peak is always placed in the middle of the window therefore the given length is rounded up
    // to the next odd integer
    // Used to limit the evaluated area for prominence and width calculations.
    // Default value is 0 (use the full data extent).
    public int WindowLength = 0;

    // Relative height for width calculation
    // Defines the relative height at which the peak width is measured as a percentage of its prominence.
    // 1.0 calculates the width of the peak at its lowest contour line while 0.5 evaluates at half the prominence height.
    // Must be at least 0
    // Determines the height level at which peak width is measured, as a proportion of the peak height.
    // For example, 0.5 means width is measured at half the peak's height above its base.
    // Default value is 0.5 (half height).
    public double RelativeHeight = 0.5;

    // Plateau size range for peak filtering
    // Plateau size defines the acceptable range for the number of samples at a peak's maximum value.
    // Only peaks with plateau sizes within this range will be detected.
    // Plateau size is the number of consecutive samples at the peak's maximum value.
    // Default range is [0, SIZE_MAX] (all peaks regardless of plateau size).
    public IntRange PlateauSize = new();
}
