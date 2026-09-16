using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumSumOfAlternatingSubsequenceWithDistanceAtLeastKBenchmarks (ARCHITECTURE
// 17.9): its two arms are competing strategies for the same question - the O(n^2) pairwise scan
// against the O(n log n) two-segment-tree sweep - so a harness whose arms disagree is timing two
// different problems. Setup draws the values from one fixed seed and derives the minimum distance
// from the length, so the same Length must rebuild the same workload; otherwise two published numbers
// were never comparable in the first place.
public sealed partial class MaximumSumOfAlternatingSubsequenceWithDistanceAtLeastKBenchmarksTests
{
    private const int SmallestLength = 2_000;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_TenthOfLengthAsMinimumDistance_AgreesWithSegmentTreeSweep()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SegmentTreeSweep(), harness.BruteForce());
    }

    [Fact]
    public void SegmentTreeSweep_TenthOfLengthAsMinimumDistance_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.SegmentTreeSweep());
    }

    private static MaximumSumOfAlternatingSubsequenceWithDistanceAtLeastKBenchmarks BuildHarness()
    {
        var harness = new MaximumSumOfAlternatingSubsequenceWithDistanceAtLeastKBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
