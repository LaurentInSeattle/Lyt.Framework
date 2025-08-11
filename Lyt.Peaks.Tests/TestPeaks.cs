using Lyt.Peaks;

namespace Lyt.Peaks.Tests;

[TestClass]
public sealed class TestPeaks
{
    //    // Sample signals for testing
    //    double[] simple_signal = [0, 1, 0, 2, 0, 3, 0, 2, 0, 1, 0];
    //    double[] plateau_signal = [0, 1, 1, 1, 0, 2, 2, 0, 3, 3, 3, 0];
    //    double[] noisy_signal = [1.2, 0.8, 1.9, 1.5, 2.7, 1.8, 1.1, 2.5, 3.2, 2.8, 1.6, 0.9];
    //    double[] flat_signal=  [1, 1, 1, 1, 1];

    [TestMethod]
    public void NoDetection()
    {
        // Test with flat signal(no peaks) with all default conditions 
        double[] flat_signal = [1, 1, 1, 1, 1];
        bool result = PeakFinder.find_peaks(flat_signal, new Conditions(), out List<PeakResult> peaks);
        Assert.IsTrue(result);
        Assert.AreEqual(0, peaks.Count);
    }

    [TestMethod]
    public void BasicDetection()
    {
        // Sample signal for testing
        double[] simple_signal = [0, 1, 0, 2, 0, 3, 0, 2, 0, 1, 0];
        bool result = PeakFinder.find_peaks(simple_signal, new Conditions (), out List<PeakResult> peaks);
        Assert.IsTrue(result);

        //    Should find 5 peaks
        Assert.AreEqual(5, peaks.Count);

        // Peak at index 1 with height 1
        Assert.AreEqual(1, peaks[0].peak);

        // Peak at index 3 with height 2
        Assert.AreEqual(3, peaks[1].peak);

        // Peak at index 5 with height 3
        Assert.AreEqual(5, peaks[2].peak);

        // Peak at index 7 with height 2
        Assert.AreEqual(7, peaks[3].peak);

        // Peak at index 9 with height 1
        Assert.AreEqual(9, peaks[4].peak);
    }

    [TestMethod]
    public void DetectionWithHeightFiltering()
    {
        // Test detection with height filtering
        // Sample signal for testing
        double[] simple_signal = [0, 1, 0, 2, 0, 3, 0, 2, 0, 1, 0];

        var conditions = new Conditions();
        conditions.height.min = 2.5; 
        bool result = PeakFinder.find_peaks(simple_signal, conditions, out List<PeakResult> peaks);
        Assert.IsTrue(result);

        // Only one peak, the peak with height 3 at index 5 
        Assert.AreEqual(1, peaks.Count);
        Assert.AreEqual(5, peaks[0].peak);
        Assert.AreEqual(3.0, peaks[0].peak_height, delta: 0.000_1);
    }
}
