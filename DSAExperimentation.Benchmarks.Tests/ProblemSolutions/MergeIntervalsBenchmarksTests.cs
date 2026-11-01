using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MergeIntervalsBenchmarks (ARCHITECTURE 17.9): each arm returns the merged
// intervals themselves, and BenchmarkArmsTests holds the two to the same list. What this adds is the
// one bound independent of both arms, checked on the baseline: the input is non-empty (every
// generated interval has length at least one) and Length is at least one, so at least one interval
// always survives, and the merge can never return more intervals than it was given.
public sealed partial class MergeIntervalsBenchmarksTests
{
    private const int SmallestLength = 200;

    // Every interval is non-empty and the input is non-empty, so the merge can never collapse to zero.
    private const int MinimumMergedIntervalCount = 1;

    [Fact]
    public void BatchSortAndMerge_OverlappingSeededIntervals_KeepsBetweenOneAndEveryInterval() =>
        Assert.InRange(
            BuildHarness().BatchSortAndMerge().Count,
            MinimumMergedIntervalCount,
            SmallestLength);

    private static MergeIntervalsBenchmarks BuildHarness()
    {
        var harness = new MergeIntervalsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
