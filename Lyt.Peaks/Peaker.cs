namespace Lyt.Peaks;

public sealed class PeakFinder
{
    public static bool Explore(double[] x, Conditions conditions, out List<PeakResult> results)
    {
        results = [];

        // Step 1: Find all local maxima in the input array 
        bool ret = LocalMaximaOneDimension(x, out List<LmrPeakIndex>? peaks);
        if (!ret || peaks is null || peaks.Count == 0)
        {
            // throw ? 
            goto cleanup;
        }

        // Count of peaks found 
        int peaks_size = peaks.Count;

        // Step 2: Allocate memory for peak properties 

        // Array of peak indices 
        int[] peak_idx = new int[peaks_size];

        // Array of peak heights 
        double[] heights = new double[peaks_size];

        // Mask array to track which peaks pass filters 
        int[] mask = new int[peaks_size];

        // Information about peak plateaus
        var plateaus = new LprPeakPlateau[peaks_size];

        // Threshold information for each peak 
        var thresholds = new LrPeakThreshold[peaks_size];

        // Prominence information for each peak 
        var prominences = new LprPeakProminence[peaks_size];

        // Width information for each peak 
        var widths = new WhlrPeakWidth[peaks_size];

        // Initialize all peaks as valid (passing filters) 
        for (int i = 0; i < peaks_size; i++)
        {
            mask[i] = 1;
        }

        /* Step 3: Extract peak indices from the raw peak data */
        PeakIndices(peaks, peak_idx);

        /* Step 4: Calculate height of each peak */
        PeakHeights(x, peaks, heights);

        /* Step 5: Identify plateau characteristics for each peak */
        PeakPlateaus(peaks, plateaus);

        /* Step 6: Calculate threshold information for each peak */
        PeakThresholds(x, peak_idx, thresholds);

        /* Step 7: Filter peaks based on height, plateau size, and threshold criteria */
        for (int i = 0; i < peaks_size; i++)
        {
            if (!(mask[i] != 0))
            {
                // Skip peaks that have already been filtered out 
                continue;
            }

            // Apply height filter 
            if (heights[i] > conditions.height.max || heights[i] < conditions.height.min)
            {
                mask[i] = 0;
            }

            // Apply plateau size filter 
            if (plateaus[i].plateau_size > conditions.plateau_size.max || plateaus[i].plateau_size < conditions.plateau_size.min)
            {
                mask[i] = 0;
            }

            // Apply threshold filter 
            if (Math.Min(thresholds[i].right_threshold, thresholds[i].left_threshold) < conditions.threshold.min ||
               Math.Max(thresholds[i].right_threshold, thresholds[i].left_threshold) > conditions.threshold.max)
            {
                mask[i] = 0;
            }
        }

        /* Step 8: Filter peaks based on minimum distance between peaks */
        ret = SelectByPeakDistance(peak_idx, peaks_size, heights, conditions.distance, mask);
        if (!ret)
        {
            goto cleanup;
        }

        /* Step 9: Calculate peak prominences */
        PeakProminences(x, peak_idx, conditions.wlen, mask, prominences);

        /* Step 10: Calculate peak widths */
        PeakWidths(x, peak_idx, conditions.rel_height, prominences, mask, widths);

        /* Step 11: Filter peaks based on prominence and width criteria */
        for (int i = 0; i < peaks_size; i++)
        {
            if (!(mask[i] != 0))
            {
                // Skip peaks that have already been filtered out 
                continue;
            }

            // Apply prominence filter 
            if (prominences[i].prominence > conditions.prominence.max || prominences[i].prominence < conditions.prominence.min)
            {
                mask[i] = 0;
            }

            // Apply width filter  
            if (widths[i].width > conditions.width.max || widths[i].width < conditions.width.min)
            {
                mask[i] = 0;
            }
        }

        // Step 12: Count how many peaks passed all filters 
        int counter = 0;
        for (int i = 0; i < peaks_size; i++)
        {
            if (mask[i] != 0)
            {
                counter++;
            }
        }

        // If no peaks pass the filters, return early 
        if (counter == 0)
        {
            ret = false;
            goto cleanup;
        }

        // Step 13: Not needed for C#: Allocate memory for the final results array 

        // Step 14: Fill the results array with peaks that passed all filters 
        for (int i = 0; i < peaks_size; i++)
        {
            if (!(mask[i] != 0))
            {
                // Skip peaks that have already been filtered out 
                continue;
            }

            // Store all peak properties in the result structure 
            var result = new PeakResult
            {
                peak = peak_idx[i],
                peak_height = heights[i],
                plateau = plateaus[i],
                threshold = thresholds[i],
                prominence = prominences[i],
                width = widths[i]
            };

            results.Add(result);
        }

    cleanup:

        return ret;
    }

    private static int[] Sort(double[] x, int size)
    {
        int[] idx = new int[size];
        var comp_array = new ValueIndex[size];
        for (int i = 0; i < size; i++)
        {
            comp_array[i].value = x[i];
            comp_array[i].index = i;
        }

        // Sort in descending order : note minus sign !
        Array.Sort(comp_array, (a, b) => -a.value.CompareTo(b.value));
        for (int i = 0; i < size; i++)
        {
            idx[i] = comp_array[i].index;
        }

        return idx;
    }

    private static bool LocalMaximaOneDimension(double[] x, out List<LmrPeakIndex>? peaks)
    {
        int size = x.Length;
        if (x.Length == 0 || size == 0)
        {
            peaks = null;
            return false;
        }

        peaks = new List<LmrPeakIndex>(size / 2);

        int i_ahead;
        int i_max = size - 1;
        int peak_idx = 0;

        for (int i = 1; i < i_max; i++)
        {
            // `i` is the Pointer to current sample, first one can't be maxima

            //Test if previous sample is smaller
            if (x[i - 1] < x[i])
            {
                //Index to look ahead of current sample
                i_ahead = i + 1;

                //Find next sample that is unequal to x[i]
                while (i_ahead < i_max && x[i_ahead] == x[i])
                {
                    i_ahead++;
                }

                //Maxima is found if next unequal sample is smaller than x[i]
                if (x[i_ahead] < x[i])
                {
                    var peak = new LmrPeakIndex
                    {
                        left_edge = i,
                        right_edge = i_ahead - 1,
                    };
                    peak.mid_point = (peak.left_edge + peak.right_edge) / 2;
                    peaks.Add(peak);
                    peak_idx++;

                    //Skip samples that can't be maximum
                    i = i_ahead;
                }
            }
        }

        if (peak_idx == 0)
        {
            peaks = null;
            return false;
        }

        return true;
    }

    private static bool SelectByPeakDistance(int[] peaks, int size, double[] priority, int distance, int[] keep)
    {
        //Create map from `i` (index for `peaks` sorted by `priority`) to `j` (index
        //for `peaks` sorted by position). This allows to iterate `peaks` and `keep`
        //with `j` by order of `priority` while still maintaining the ability to
        //step to neighbouring peaks with (`j` + 1) or (`j` - 1).
        int[] priority_to_position = Sort(priority, size);

        //    //Round up because actual peak distance can only be natural number
        //    size_t distance_ = distance;
        //    distance_ = distance_ == distance ? distance_ : distance_ + 1;

        // Highest priority first -> iterate in reverse order (decreasing)
        for (int i = 0; i < size; i++)
        {
            // "Translate" `i` to `j` which points to current peak whose
            // neighbours are to be evaluated
            int j = priority_to_position[size - 1 - i];

            // Skip evaluation for peak already marked as "don't keep"
            if (keep[j] == 0)
            {
                continue;
            }

            // Flag "earlier" peaks for removal until minimal distance is exceeded
            int k = 1;
            while (k <= j && peaks[j] - peaks[j - k] < distance)
            {
                keep[j - k] = 0;
                k++;
            }

            k = j + 1;

            // Flag "later" peaks for removal until minimal distance is exceeded
            while (k < size && peaks[k] - peaks[j] < distance)
            {
                keep[k] = 0;
                k++;
            }
        }

        return true;
    }

    private static void PeakProminences(
        double[] x,
        int[] peaks,
        int wlen,
        int[] mask, // TODO: use bool 
        LprPeakProminence[] prominences)
    {
        int i;
        double left_min, right_min;
        int peak, i_min, i_max;

        int half_wlen = wlen / 2;

        //    printf("wlen: %d, %d", wlen, half_wlen);
        //    std::cout << "wlen: " << wlen << ", " << half_wlen << std::endl;

        int size_x = x.Length;
        int size_peaks = peaks.Length;
        for (int peak_nr = 0; peak_nr < size_peaks; peak_nr++)
        {
            // 0 false, 1 true 
            // !0 true, !1 false 
            if (!(mask[peak_nr] != 0))
            {
                continue;
            }

            LprPeakProminence prominence;
            peak = peaks[peak_nr];
            i_min = 0;
            i_max = size_x - 1;

            if (wlen >= 2)
            {
                // Adjust window around the evaluated peak (within bounds);
                // if wlen is even the resulting window length is is implicitly
                // rounded to next odd integer
                i_min = Math.Max(peak - half_wlen, i_min);
                i_max = Math.Min(peak + half_wlen, i_max);
            }

            // Find the left base in interval [i_min, peak]
            i = peak;
            prominence.left_base = peak;
            left_min = x[peak];

            while (i_min <= i && x[i] <= x[peak])
            {
                if (x[i] < left_min)
                {
                    left_min = x[i];
                    prominence.left_base = i;
                }

                if (i == 0 && i_min == 0)
                {
                    break;
                }

                i--;
            }

            // Find the right base in interval [peak, i_max]
            i = peak;
            prominence.right_base = peak;
            right_min = x[peak];

            while (i <= i_max && x[i] <= x[peak])
            {
                if (x[i] < right_min)
                {
                    right_min = x[i];
                    prominence.right_base = i;
                }
                i++;
            }

            prominence.prominence = x[peak] - Math.Max(left_min, right_min);
            prominences[peak_nr] = prominence;
        }
    }

    private static void PeakWidths(
        double[] x,
        int[] peaks,
        double rel_height,
        LprPeakProminence[] prominences,
        int[] mask,
        WhlrPeakWidth[] widths)
    {
        for (int p = 0; p < peaks.Length; p++)
        {
            if (!(mask[p] != 0))
            {
                continue;
            }

            WhlrPeakWidth width_data;
            int i_min = prominences[p].left_base;
            int i_max = prominences[p].right_base;
            int peak = peaks[p];
            double height = x[peak] - prominences[p].prominence * rel_height;
            width_data.width_height = x[peak] - prominences[p].prominence * rel_height;

            // Find intersection point on left side
            int i = peak;
            while (i_min < i && height < x[i])
            {
                i -= 1;
            }

            // Interpolate if true intersection height is between samples
            double left_ip = (double)i;
            if (x[i] < height)
            {
                left_ip += (height - x[i]) / (x[i + 1] - x[i]);
            }

            // Find intersection point on right side
            i = peak;
            while (i < i_max && height < x[i])
            {
                i += 1;
            }

            // Interpolate if true intersection height is between samples
            double right_ip = (double)i;
            if (x[i] < height)
            {
                right_ip -= (height - x[i]) / (x[i - 1] - x[i]);
            }

            width_data.width = right_ip - left_ip;
            width_data.left_ip = left_ip;
            width_data.right_ip = right_ip;

            widths[p] = width_data;
        }
    }

    private static void PeakThresholds(double[] x, int[] peaks, LrPeakThreshold[] thresholds)
    {
        for (int peak_idx = 0; peak_idx < peaks.Length; peak_idx++)
        {
            LrPeakThreshold thr;
            int peakIndex = peaks[peak_idx];
            thr.left_threshold = x[peakIndex] - x[peakIndex - 1];
            thr.right_threshold = x[peakIndex] - x[peakIndex + 1];
            thresholds[peak_idx] = thr;
        }
    }

    private static void PeakPlateaus(List<LmrPeakIndex> peaks, LprPeakPlateau[] plateaus)
    {
        for (int p = 0; p < peaks.Count; p++)
        {
            LprPeakPlateau plateau;
            plateau.right_edge = peaks[p].right_edge;
            plateau.left_edge = peaks[p].left_edge;
            plateau.plateau_size = plateau.right_edge - plateau.left_edge + 1;

            plateaus[p] = plateau;
        }
    }

    private static void PeakHeights(double[] x, List<LmrPeakIndex> peaks, double[] heights)
    {
        for (int p = 0; p < peaks.Count; p++)
        {
            heights[p] = x[peaks[p].mid_point];
        }
    }

    private static void PeakIndices(List<LmrPeakIndex> peaks, int[] peak_indices)
    {
        for (int p = 0; p < peaks.Count; p++)
        {
            peak_indices[p] = peaks[p].mid_point;
        }
    }
}
