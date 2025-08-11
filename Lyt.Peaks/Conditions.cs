namespace Lyt.Peaks;

/// <summary> Configuration parameters for peak detection </summary>
/// <remarks>
/// This structure contains all the configurable parameters that control which
/// features in the data are identified as peaks and which are filtered out.
/// Each field represents a different criterion for peak detection and filtering.
/// </remarks>
public struct Conditions
{
    public Conditions()
    {
        
    }

    //     * @brief Height range for peak filtering
    //     *
    //     * Only peaks with heights within this range will be detected.
    //     * Default range is [-DBL_MAX, DBL_MAX] (all peaks regardless of height).
    public DoubleRange height = new DoubleRange();

    //     * @brief Threshold range for peak filtering
    //     *
    //     * Only peaks that rise above their neighboring valleys by an amount within
    //     * this range will be detected. This measures how distinct a peak is from its
    //     * immediate surroundings.
    //     * Default range is [-DBL_MAX, DBL_MAX] (all peaks regardless of threshold).
    //     */
    public DoubleRange threshold = new DoubleRange();

    //     * @brief Minimum distance between peaks
    //     *
    //     * Ensures peaks are separated by at least this many samples. When multiple
    //     * peaks are found within this distance, only the highest one is kept.
    //     * Default value is 1 (adjacent peaks allowed).
    public int distance = 1;

    //     * @brief Prominence range for peak filtering
    //     *
    //     * Only peaks with prominence values within this range will be detected.
    //     * Prominence measures how much a peak stands out from its surrounding baseline.
    //     * Default range is [-DBL_MAX, DBL_MAX] (all peaks regardless of prominence).
    public DoubleRange prominence = new DoubleRange();

    //     * @brief Width range for peak filtering
    //     *
    //     * Only peaks with widths within this range will be detected. Width is
    //     * measured at a height determined by rel_height.
    //     * Default range is [-DBL_MAX, DBL_MAX] (all peaks regardless of width).
    //     */
    public DoubleRange width = new DoubleRange();

    //     * @brief Window length for prominence and width calculations
    //     *
    //     * Used to limit the evaluated area for prominence and width calculations.
    //     * Default value is 0 (use the full data extent).
    //     */
    public int wlen = 0;

    //     * @brief Relative height for width calculation
    //     *
    //     * Determines the height level at which peak width is measured, as a proportion
    //     * of the peak height. For example, 0.5 means width is measured at half the
    //     * peak's height above its base.
    //     * Default value is 0.5 (half height).
    public double rel_height = 0.5;

    //     * @brief Plateau size range for peak filtering
    //     *
    //     * Only peaks with plateau sizes within this range will be detected.
    //     * Plateau size is the number of consecutive samples at the peak's maximum value.
    //     * Default range is [0, SIZE_MAX] (all peaks regardless of plateau size).
    //     */
    public IntRange plateau_size = new IntRange();

    ///**
    // * @brief Set minimum and maximum height condition for peak detection
    // *
    // * Height condition defines the absolute height range a peak must have to be detected.
    // *
    // * @param cond Pointer to a fp_conditions_t structure to be modified
    // * @param min Minimum peak height to be considered
    // * @param max Maximum peak height to be considered
    // */
    //void fp_cond_set_height_mn_mx(fp_conditions_t* cond, double min, double max);

    ///**
    // * @brief Set minimum height condition for peak detection (maximum set to DBL_MAX)
    // *
    // * Height condition defines the absolute height range a peak must have to be detected.
    // *
    // * @param cond Pointer to a fp_conditions_t structure to be modified
    // * @param min Minimum peak height to be considered
    // */
    //void fp_cond_set_height_mn(fp_conditions_t* cond, double min);

    ///**
    // * @brief Initialize height condition for peak detection to default range (-DBL_MAX to DBL_MAX)
    // *
    // * Height condition defines the absolute height range a peak must have to be detected.
    // *
    // * @param cond Pointer to a fp_conditions_t structure to be modified
    // */
    //void fp_cond_init_height(fp_conditions_t* cond);


    ///**
    // * @brief Set minimum and maximum threshold condition for peak detection
    // *
    // * Threshold condition defines how much a data point needs to exceed its neighbors to be considered a peak.
    // *
    // * @param cond Pointer to a fp_conditions_t structure to be modified
    // * @param min Minimum threshold value
    // * @param max Maximum threshold value
    // */
    //void fp_cond_set_threshold_mn_mx(fp_conditions_t* cond, double min, double max);


    ///**
    // * @brief Set minimum threshold condition for peak detection (maximum set to DBL_MAX)
    // *
    // * Threshold condition defines how much a data point needs to exceed its neighbors to be considered a peak.
    // *
    // * @param cond Pointer to a fp_conditions_t structure to be modified
    // * @param min Minimum threshold value
    // */
    //void fp_cond_set_threshold_mn(fp_conditions_t* cond, double min);

    ///**
    // * @brief Initialize threshold condition to default range (-DBL_MAX to DBL_MAX)
    // *
    // * Threshold condition defines how much a data point needs to exceed its neighbors to be considered a peak.
    // *
    // * @param cond Pointer to a fp_conditions_t structure to be modified
    // */
    //void fp_cond_init_threshold(fp_conditions_t* cond);


    ///**
    // * @brief Set minimum distance between detected peaks
    // *
    // * Distance condition ensures peaks are separated by at least the specified number of samples.
    // *
    // * @param cond Pointer to a fp_conditions_t structure to be modified
    // * @param distance Minimum samples between peaks
    // */
    //void fp_cond_set_distance_mn(fp_conditions_t* cond, size_t distance);

    ///**
    // * @brief Initialize distance condition to default value (1)
    // *
    // * Distance condition ensures peaks are separated by at least the specified number of samples.
    // *
    // * @param cond Pointer to a fp_conditions_t structure to be modified
    // */
    //void fp_cond_init_distance(fp_conditions_t* cond);

    ///**
    // * @brief Set minimum and maximum prominence condition for peak detection
    // *
    // * Prominence quantifies how much a peak stands out from surrounding baseline.
    // *
    // * @param cond Pointer to a fp_conditions_t structure to be modified
    // * @param min Minimum peak prominence
    // * @param max Maximum peak prominence
    // */
    //void fp_cond_set_prominence_mn_mx(fp_conditions_t* cond, double min, double max);

    ///**
    // * @brief Set minimum prominence condition for peak detection (maximum set to DBL_MAX)
    // *
    // * Prominence quantifies how much a peak stands out from surrounding baseline.
    // *
    // * @param cond Pointer to a fp_conditions_t structure to be modified
    // * @param min Minimum peak prominence
    // */
    //void fp_cond_set_prominence_mn(fp_conditions_t* cond, double min);

    ///**
    // * @brief Initialize prominence condition to default range (-DBL_MAX to DBL_MAX)
    // *
    // * Prominence quantifies how much a peak stands out from surrounding baseline.
    // *
    // * @param cond Pointer to a fp_conditions_t structure to be modified
    // */
    //void fp_cond_init_prominence(fp_conditions_t* cond);

    ///**
    // * @brief Set minimum and maximum width condition for peak detection
    // *
    // * Defines the Required width condition of peaks in samples.
    // *
    // * @param cond Pointer to a fp_conditions_t structure to be modified
    // * @param min Minimum peak width
    // * @param max Maximum peak width
    // */
    //void fp_cond_set_width_mn_mx(fp_conditions_t* cond, double min, double max);

    ///**
    // * @brief Set minimum width condition for peak detection (maximum set to DBL_MAX)
    // *
    // * Defines the Required width condition of peaks in samples.
    // *
    // * @param cond Pointer to a fp_conditions_t structure to be modified
    // * @param min Minimum peak width
    // */
    //void fp_cond_set_width_mn(fp_conditions_t* cond, double min);


    ///**
    // * @brief Initialize width condition to default range (-DBL_MAX to DBL_MAX)
    // *
    // * Defines the Required width condition of peaks in samples.
    // *
    // * @param cond Pointer to a fp_conditions_t structure to be modified
    // */
    //void fp_cond_init_width(fp_conditions_t* cond);

    ///**
    // * @brief Set a window length in samples that optionally limits the evaluated area for each peak
    // *
    // * Defines the size of the window used for calculating peak widths.
    // * The peak is always placed in the middle of the window therefore the given length is rounded up to the next odd integer
    // *
    // * @param cond Pointer to a fp_conditions_t structure to be modified
    // * @param wlen Window length in samples
    // */
    //void fp_cond_set_wlen_mn(fp_conditions_t* cond, size_t wlen);

    ///**
    // * @brief Initialize window length (wlen) that limits the evaluated area for each peak to default value (0)
    // *
    // * Defines the size of the window used for calculating peak widths.
    // * The peak is always placed in the middle of the window therefore the given length is rounded up to the next odd integer
    // *
    // * @param cond Pointer to a fp_conditions_t structure to be modified
    // */
    //void fp_cond_init_wlen(fp_conditions_t* cond);

    ///**
    // * @brief Set relative height for peak width calculation
    // *
    // * Defines the relative height at which the peak width is measured as a percentage of its prominence.
    // * 1.0 calculates the width of the peak at its lowest contour line while 0.5 evaluates at half the prominence height.
    // * Must be at least 0
    // *
    // * @param cond Pointer to a fp_conditions_t structure to be modified
    // * @param rel_height Relative height (0.0 to 1.0, where 1.0 is the peak height)
    // */
    //void fp_cond_set_rel_height_mn(fp_conditions_t* cond, double rel_height);

    ///**
    // * @brief Initialize relative height for peak width calculation to default value (0.5)
    // *
    // * Defines the relative height at which the peak width is measured as a percentage of its prominence.
    // * 1.0 calculates the width of the peak at its lowest contour line while 0.5 evaluates at half the prominence height.
    // * Must be at least 0
    // *
    // * @param cond Pointer to a fp_conditions_t structure to be modified
    // */
    //void fp_cond_init_rel_height(fp_conditions_t* cond);

    ///**
    // * @brief Set minimum and maximum plateau size condition for peak detection
    // *
    // * Plateau size defines the acceptable range for the number of samples at a peak's maximum value.
    // *
    // * @param cond Pointer to a fp_conditions_t structure to be modified
    // * @param min Minimum number of samples at peak level
    // * @param max Maximum number of samples at peak level
    // */
    //void fp_cond_set_plateau_size_mn_mx(fp_conditions_t* cond, size_t min, size_t max);

    ///**
    // * @brief Set minimum plateau size condition for peak detection (maximum set to SIZE_MAX)
    // *
    // * Plateau size defines the acceptable range for the number of samples at a peak's maximum value.
    // *
    // * @param cond Pointer to a fp_conditions_t structure to be modified
    // * @param min Minimum number of samples at peak level
    // */
    //void fp_cond_set_plateau_size_mn(fp_conditions_t* cond, size_t min);

    ///**
    // * @brief Initialize plateau size condition to default range (0 to SIZE_MAX)
    // *
    // * Plateau size defines the acceptable range for the number of samples at a peak's maximum value.
    // *
    // * @param cond Pointer to a fp_conditions_t structure to be modified
    // */
    //void fp_cond_init_plateau_size(fp_conditions_t* cond);


    ///**
    // * @brief Initialize all conditions in a conditions structure to their default values
    // *
    // * This function initializes all peak detection parameters to their default values,
    // * providing a clean starting point for condition configuration.
    // *
    // * @param cond Pointer to a fp_conditions_t structure to be initialized
    // */
    //void fp_init_conditions(fp_conditions_t* cond);

    ///**
    // * @brief Get a new conditions structure with all default values
    // *
    // * Creates and returns a new fp_conditions_t structure with all conditions
    // * initialized to their default values.
    // *
    // * @return A new fp_conditions_t structure with default values
    // */
    //fp_conditions_t fp_get_default_conditions();
}
