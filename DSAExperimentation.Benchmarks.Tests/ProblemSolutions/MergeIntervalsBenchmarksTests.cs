using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MergeIntervalsBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - sorting once and merging in a single linear pass against inserting
// each interval one at a time into this repo's own IntervalSet<TKey> - so a harness whose arms disagree
// is timing two different problems.
//
// Agreement here is weak by construction. Both arms return only the merged list's Count, not the merged
// intervals, so a green pair witnesses that the two strategies collapsed the input into the same NUMBER
// of intervals, not that they produced the same intervals - an arm that shifted or split a boundary
// while keeping the count would still pass. The input is non-empty (every generated interval has length
// at least one) and Length is at least one, so at least one interval always survives, which is the one
// independent bound the arms' shared count is also checked against. Neither arm mutates the interval
// array, so one harness is safe to read twice in either order; Setup draws from one fixed seed, so the
// same Length must rebuild the same intervals.
public sealed partial class MergeIntervalsBenchmarksTests
{
    private const int SmallestLength = 200;

    // Every interval is non-empty and the input is non-empty, so the merge can never collapse to zero.
    private const int MinimumMergedIntervalCount = 1;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BatchSortAndMerge(), BuildHarness().BatchSortAndMerge());

    [Fact]
    public void BatchSortAndMerge_OverlappingSeededIntervals_AgreesWithIncrementalIntervalSet()
    {
        var harness = BuildHarness();

        Assert.InRange(
            harness.BatchSortAndMerge(),
            MinimumMergedIntervalCount,
            SmallestLength);
        Assert.Equal(harness.IncrementalIntervalSet(), harness.BatchSortAndMerge());
    }

    [Fact]
    public void IncrementalIntervalSet_OverlappingSeededIntervals_AgreesWithBatchSortAndMerge()
    {
        var harness = BuildHarness();

        Assert.InRange(
            harness.IncrementalIntervalSet(),
            MinimumMergedIntervalCount,
            SmallestLength);
        Assert.Equal(harness.BatchSortAndMerge(), harness.IncrementalIntervalSet());
    }

    private static MergeIntervalsBenchmarks BuildHarness()
    {
        var harness = new MergeIntervalsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
