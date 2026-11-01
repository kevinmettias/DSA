using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PeakIndexInAMountainArrayBenchmarks (ARCHITECTURE 17.9): the baseline's peak
// index against the one the workload's shape fixes, which the generic check cannot supply - it holds
// the predicate search to the baseline, and agreeing on the wrong index would pass it. Setup builds
// the mountain from Length alone with its peak at the midpoint.
public sealed partial class PeakIndexInAMountainArrayBenchmarksTests
{
    private const int SmallestLength = 200;

    // Setup places the peak at the array's midpoint, the same halving the shared constant names.
    private const int ExpectedPeakIndex = SmallestLength / AlgorithmConstants.HalvingFactor;

    [Fact]
    public void LinearScan_MidpointPeakMountain_FindsTheMidpoint() =>
        Assert.Equal(ExpectedPeakIndex, BuildHarness().LinearScan());

    private static PeakIndexInAMountainArrayBenchmarks BuildHarness()
    {
        var harness = new PeakIndexInAMountainArrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
