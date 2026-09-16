using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NonOverlappingIntervalsBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - repeated minimum-end rescans against the sort-by-end
// greedy count of removals - so a harness whose arms disagree is erasing two different interval sets.
// Setup generates the seeded, mostly non-overlapping intervals and shuffles them, so the same Length
// must rebuild the same intervals in the same order.
//
// One harness serves both arms: the greedy arm reads the workload through IntervalEndOrder.SortedByEnd,
// which sorts a copy, so neither arm can leave the shared array reordered for the other and the call
// order does not matter.
public sealed partial class NonOverlappingIntervalsBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForceRepeatedMinEndScan(), BuildHarness().BruteForceRepeatedMinEndScan());

    [Fact]
    public void BruteForceRepeatedMinEndScan_AgreesWithSortByEndThenGreedyScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SortByEndThenGreedyScan(), harness.BruteForceRepeatedMinEndScan());
    }

    [Fact]
    public void SortByEndThenGreedyScan_AgreesWithBruteForceRepeatedMinEndScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceRepeatedMinEndScan(), harness.SortByEndThenGreedyScan());
    }

    private static NonOverlappingIntervalsBenchmarks BuildHarness()
    {
        var harness = new NonOverlappingIntervalsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
