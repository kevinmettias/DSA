using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MergeIntervalsBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - sorting once and merging in a single linear pass against inserting
// each interval one at a time into this repo's own IntervalSet<TKey> - so a harness whose arms disagree
// is timing two different problems.
//
// Each arm returns the merged intervals themselves, and BenchmarkArmsTests holds the two to the same
// list. What this adds is the one bound independent of both arms: the input is non-empty (every
// generated interval has length at least one) and Length is at least one, so at least one interval
// always survives, and the merge can never return more intervals than it was given. Neither arm
// mutates the interval array, so one harness is safe to read twice in either order; Setup draws from
// one fixed seed, so the same Length must rebuild the same intervals.
public sealed partial class MergeIntervalsBenchmarksTests
{
    private const int SmallestLength = 200;

    // Every interval is non-empty and the input is non-empty, so the merge can never collapse to zero.
    private const int MinimumMergedIntervalCount = 1;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BatchSortAndMerge(), BuildHarness().BatchSortAndMerge());

    [Fact]
    public void BatchSortAndMerge_OverlappingSeededIntervals_KeepsBetweenOneAndEveryInterval() =>
        Assert.InRange(
            BuildHarness().BatchSortAndMerge().Count,
            MinimumMergedIntervalCount,
            SmallestLength);

    [Fact]
    public void IncrementalIntervalSet_OverlappingSeededIntervals_KeepsBetweenOneAndEveryInterval() =>
        Assert.InRange(
            BuildHarness().IncrementalIntervalSet().Count,
            MinimumMergedIntervalCount,
            SmallestLength);

    private static MergeIntervalsBenchmarks BuildHarness()
    {
        var harness = new MergeIntervalsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
