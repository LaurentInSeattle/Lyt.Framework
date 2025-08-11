using Lyt.Peaks;

namespace Lyt.Peaks.Tests;

[TestClass]
public sealed class TestPeaks
{
    [TestMethod]
    public void NoDetection()
    {
        // Test with flat signal(no peaks) with all default conditions 
        double[] flat_signal = [1, 1, 1, 1, 1];
        bool result = PeakFinder.Analyse(flat_signal, new Conditions(), out List<PeakResult> peaks);
        Assert.IsFalse(result);
        Assert.AreEqual(0, peaks.Count);
    }

    [TestMethod]
    public void BasicDetection()
    {
        // Sample signal for testing
        double[] simple_signal = [0, 1, 0, 2, 0, 3, 0, 2, 0, 1, 0];
        bool result = PeakFinder.Analyse(simple_signal, new Conditions (), out List<PeakResult> peaks);
        Assert.IsTrue(result);

        //    Should find 5 peaks
        Assert.AreEqual(5, peaks.Count);

        // Peak at index 1 with height 1
        Assert.AreEqual(1, peaks[0].Peak);

        // Peak at index 3 with height 2
        Assert.AreEqual(3, peaks[1].Peak);

        // Peak at index 5 with height 3
        Assert.AreEqual(5, peaks[2].Peak);

        // Peak at index 7 with height 2
        Assert.AreEqual(7, peaks[3].Peak);

        // Peak at index 9 with height 1
        Assert.AreEqual(9, peaks[4].Peak);
    }

    [TestMethod]
    public void DetectionWithHeightFiltering()
    {
        // Test detection with height filtering
        // Sample signal for testing
        double[] simple_signal = [0, 1, 0, 2, 0, 3, 0, 2, 0, 1, 0];

        var conditions = new Conditions();
        conditions.Height.Min = 2.5; 
        bool result = PeakFinder.Analyse(simple_signal, conditions, out List<PeakResult> peaks);
        Assert.IsTrue(result);

        // Only one peak, the peak with height 3 at index 5 
        Assert.AreEqual(1, peaks.Count);
        Assert.AreEqual(5, peaks[0].Peak);
        Assert.AreEqual(3.0, peaks[0].PeakHeight, delta: 0.000_1);
    }

    [TestMethod]
    public void DetectionWithDistanceFiltering()
    {
        // Test detection with minimum distance between peaks
        double[] simple_signal = [0, 1, 0, 2, 0, 3, 0, 2, 0, 1, 0];
        var conditions = new Conditions
        {
            Distance = 3
        };
        bool result = PeakFinder.Analyse(simple_signal, conditions, out List<PeakResult> peaks);
        Assert.IsTrue(result);

        Assert.AreEqual(3, peaks.Count);
        Assert.AreEqual(1, peaks[0].Peak); // First peak at index 1
        Assert.AreEqual(5, peaks[1].Peak); // Second peak at index 5, the peak in index 3 is skipped
        Assert.AreEqual(9, peaks[2].Peak); // Second peak at index 9, the peak in index 7 is skipped
    }

    [TestMethod]
    public void DetectionWithPlateauDetection()
    {
        // Test detection with plateau detection
        double[] plateau_signal = [0, 1, 1, 1, 0, 2, 2, 0, 3, 3, 3, 0];
        var conditions = new Conditions
        {
            Distance = 3
        };
        bool result = PeakFinder.Analyse(plateau_signal, conditions, out List<PeakResult> peaks);
        Assert.IsTrue(result);
        Assert.AreEqual(3, peaks.Count);

        // Check first plateau peak
        Assert.AreEqual(2, peaks[0].Peak); // The middle of the plateau
        Assert.AreEqual(3, peaks[0].Plateau.PlateauSize); // 3 samples wide
        Assert.AreEqual(1, peaks[0].Plateau.LeftEdge);
        Assert.AreEqual(3, peaks[0].Plateau.RightEdge);

        // Check second plateau peak
        Assert.AreEqual(5, peaks[1].Peak); // The middle of the plateau
        Assert.AreEqual(2, peaks[1].Plateau.PlateauSize); // 2 samples wide

        // Check third plateau peak
        Assert.AreEqual(9, peaks[2].Peak); // The middle of the plateau
        Assert.AreEqual(3, peaks[2].Plateau.PlateauSize); // 3 samples wide
    }

    [TestMethod]
    public void DetectionWithProminance()
    {
        // Test detection with prominence calculation and filtering
        double[] simple_signal = [0, 1, 0, 2, 0, 3, 0, 2, 0, 1, 0];
        var conditions = new Conditions();
        conditions.Prominence.Min = 1.5; 

        bool result = PeakFinder.Analyse(simple_signal, conditions, out List<PeakResult> peaks);
        Assert.IsTrue(result);
        Assert.AreEqual(3, peaks.Count);

        Assert.AreEqual(3, peaks[0].Peak); // Peak with height 2 has enough prominence
        Assert.IsTrue(peaks[0].Prominence.Prominence >= 1.5);

        Assert.AreEqual(5, peaks[1].Peak); // Peak with height 3 has enough prominence
        Assert.IsTrue(peaks[1].Prominence.Prominence >= 1.5);

        Assert.AreEqual(7, peaks[2].Peak); // Peak with height 2 has enough prominence
        Assert.IsTrue(peaks[2].Prominence.Prominence >= 1.5);
    }

    [TestMethod]
    public void DetectionWithWidthFiltering()
    {
        // Test detection with width calculation and filtering
        double[] simple_signal = [0, 1, 0, 2, 0, 3, 0, 2, 0, 1, 0];
        var conditions = new Conditions();
        conditions.Width.Min =  2.0; // Only peaks with width >= 2.0
        conditions.RelativeHeight = 0.5; // Measure width at 50% of peak height

        bool result = PeakFinder.Analyse(simple_signal, conditions, out List<PeakResult> peaks);
        Assert.IsFalse(result);
        Assert.IsTrue(peaks.Count==0);

        // We expect only peaks that are wide enough at half height
        // There should be none 
        for (int i = 0; i < peaks.Count; i++)
        {
            Assert.IsTrue(peaks[i].Width.Width >= 2.0);
        }
    }

    [TestMethod]
    public void DetectionWithThresholdFiltering()
    {
        // Test detection with threshold filtering
        double[] simple_signal = [0, 1, 0, 2, 0, 3, 0, 2, 0, 1, 0];
        var conditions = new Conditions();
        conditions.Threshold.Min = 1.5; // Peak must exceed neighbors by at least 1.5

        bool result = PeakFinder.Analyse(simple_signal, conditions, out List<PeakResult> peaks);
        Assert.IsTrue(result);

        for (int i = 0; i < peaks.Count; i++)
        {
            Assert.IsTrue(peaks[i].Threshold.LeftThreshold >= 1.5);
            Assert.IsTrue(peaks[i].Threshold.RightThreshold >= 1.5);
        }
    }

    [TestMethod]
    public void DetectionWithNoisySignal()
    {
        // Test detection with noisy signal 
        double[] noisy_signal = [1.2, 0.8, 1.9, 1.5, 2.7, 1.8, 1.1, 2.5, 3.2, 2.8, 1.6, 0.9];
        var conditions = new Conditions();
        bool result = PeakFinder.Analyse(noisy_signal, conditions, out List<PeakResult> peaks);
        Assert.IsTrue(result);

        // Verify that peaks are correctly identified in noisy signal
        Assert.IsTrue(peaks.Count > 0);

        // Check if the highest peak is detected (index 8, value 3.2)
        bool found_max_peak = false;
        for (int i = 0; i < peaks.Count; i++)
        {
            if (peaks[i].Peak == 8)
            {
                found_max_peak = true;
                Assert.AreEqual(3.2, peaks[i].PeakHeight, delta: 0.000_1);
                break;
            }
        }

        Assert.IsTrue(found_max_peak);
    }



    //// Test combined filtering criteria
    //TEST_F(FindPeaksCTest, CombinedFilters) {
    //    fp_conditions_t cond = fp_get_default_conditions();
    //    fp_cond_set_height_mn(&cond, 1.5);         // Height >= 1.5
    //    fp_cond_set_prominence_mn(&cond, 1.0);     // Prominence >= 1.0
    //    fp_cond_set_distance_mn(&cond, 2);         // At least 2 samples between peaks
    //    fp_cond_set_width_mn_mx(&cond, 1.0, 4.0);  // Width between 1.0 and 4.0

    //    ASSERT_EQ(FP_STATUS_OK, find_peaks(simple_signal, 11, cond, &peaks, &num_peaks));

    //    // Check that all peaks satisfy all conditions
    //    for (size_t i = 0; i < num_peaks; i++)
    //    {
    //        EXPECT_GE(peaks[i].peak_height, 1.5);
    //        EXPECT_GE(peaks[i].prominence.prominence, 1.0);
    //        EXPECT_GE(peaks[i].width.width, 1.0);
    //        EXPECT_LE(peaks[i].width.width, 4.0);
    //    }

    //    // Make sure peaks are at least 2 samples apart
    //    for (size_t i = 1; i < num_peaks; i++)
    //    {
    //        EXPECT_GE(peaks[i].peak - peaks[i - 1].peak, 2);
    //    }
    //}

    [TestMethod]
    public void DetectionWithCombinedFilters()
    {
        // Test detection with Combined Filters
        double[] simple_signal = [0, 1, 0, 2, 0, 3, 0, 2, 0, 1, 0];
        var conditions = new Conditions
        {
            Distance = 2         // At least 2 samples between peaks
        };
        conditions.Height.Min = 1.5;         // Height >= 1.5
        conditions.Prominence.Min = 1.0;     // Prominence >= 1.0
        conditions.Width.Min = 1.0;  // Width between 1.0 and 4.0
        conditions.Width.Max = 4.0;  

        bool result = PeakFinder.Analyse(simple_signal, conditions, out List<PeakResult> peaks);
        Assert.IsTrue(result);

        // Check that all peaks satisfy all conditions
        for (int i = 0; i < peaks.Count; i++)
        {
            Assert.IsTrue(peaks[i].PeakHeight >= 1.5);
            Assert.IsTrue(peaks[i].Prominence.Prominence >= 1.0);
            Assert.IsTrue(peaks[i].Width.Width >= 1.0);
            Assert.IsTrue(peaks[i].Width.Width <= 4.0);
        }

        // Make sure peaks are at least 2 samples apart
        for (int  i = 1; i < peaks.Count; i++)
        {
            Assert.IsTrue(peaks[i].Peak - peaks[i - 1].Peak >= 2);
        }
    }

    // Test against SciPy-like for compatibility
    [TestMethod]
    public void DetectionSciPyCompatibility()
    {
        // Test against SciPy-like sample for compatibility
        // This is a known test signal with peaks that match SciPy's find_peaks results
        double[] scipy_signal = [ 0, 1, 0, 2, 0, 3, 0, 2, 0, 1, 0 ];
        // These are the expected peak indices from SciPy's find_peaks
        int [] expected_peaks = [ 1, 3, 5, 7, 9 ];
        var conditions = new Conditions();
        conditions.Height.Min = 0.5; // Same as SciPy test:  Height >= 0.5

        bool result = PeakFinder.Analyse(scipy_signal, conditions, out List<PeakResult> peaks);
        Assert.IsTrue(result);

        Assert.IsTrue(5 == peaks.Count);

        for (int i = 0; i < peaks.Count; i++)
        {
            Assert.AreEqual(expected_peaks[i], peaks[i].Peak);
        }
    }
}
