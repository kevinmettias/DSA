using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumScoreOfAGoodSubarrayBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the O(n^2) brute force that re-scans every window
// anchored at the required index against the two monotonic-stack boundary sweeps - so a harness whose
// arms disagree is timing two different problems. Setup draws the values from one fixed seed and
// derives the required index from the length, so the same Length must rebuild the same workload;
// otherwise two published numbers were never comparable in the first place.
public sealed partial class MaximumScoreOfAGoodSubarrayBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForceExpand(), BuildHarness().BruteForceExpand());

    [Fact]
    public void BruteForceExpand_RequiredIndexAtMidpoint_AgreesWithMonotonicStackBoundaries()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MonotonicStackBoundaries(), harness.BruteForceExpand());
    }

    [Fact]
    public void MonotonicStackBoundaries_RequiredIndexAtMidpoint_AgreesWithBruteForceExpand()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceExpand(), harness.MonotonicStackBoundaries());
    }

    private static MaximumScoreOfAGoodSubarrayBenchmarks BuildHarness()
    {
        var harness = new MaximumScoreOfAGoodSubarrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
