using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindInMountainArrayBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - a linear scan of the whole array against a peak
// bisection followed by one BinarySearch.Find per slope - so a harness whose arms disagree is
// searching two different arrays. Both answers are the index of the target (or -1), one int in a
// fixed position, so they are compared directly.
//
// Setup builds an ascending run of even values up to the peak and then a descending run continuing
// from that peak, and takes the target from the array's own tail. The tail value is therefore a
// value the array really contains, and at the smaller Length the ascending slope already carries it
// - so the linear scan may exit well before the whole array, which is exactly what the two arms
// still have to agree about.
public sealed partial class FindInMountainArrayBenchmarksTests
{
    // The smaller of Setup's [Params(1_000, 100_000)] lengths.
    private const int SmallestLength = 1_000;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameTargetIndex() =>
        Assert.Equal(BuildHarness().LinearScan(), BuildHarness().LinearScan());

    [Fact]
    public void LinearScan_TailTargetOnBothSlopes_AgreesWithPeakBisectionThenBinarySearch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PeakBisectionThenBinarySearch(), harness.LinearScan());
    }

    [Fact]
    public void PeakBisectionThenBinarySearch_TailTargetOnBothSlopes_AgreesWithLinearScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearScan(), harness.PeakBisectionThenBinarySearch());
    }

    private static FindInMountainArrayBenchmarks BuildHarness()
    {
        var harness = new FindInMountainArrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
