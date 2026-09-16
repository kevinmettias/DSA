using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumSumOfSubsequenceWithNonAdjacentElementsBenchmarks (ARCHITECTURE 17.9):
// its two arms are competing strategies for the same question - the full O(n) DP recomputation after
// each update against the segment-tree merge - so a harness whose arms disagree is timing two
// different problems. Setup draws the values and the full rewrite's query order from one fixed seed,
// so the same Length must rebuild the same workload; otherwise two published numbers were never
// comparable in the first place.
public sealed partial class MaximumSumOfSubsequenceWithNonAdjacentElementsBenchmarksTests
{
    private const int SmallestLength = 1_000;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().RecomputeDP(), BuildHarness().RecomputeDP());

    [Fact]
    public void RecomputeDP_FullRewriteQueryOrder_AgreesWithSegmentTreeMerge()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SegmentTreeMerge(), harness.RecomputeDP());
    }

    [Fact]
    public void SegmentTreeMerge_FullRewriteQueryOrder_AgreesWithRecomputeDP()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RecomputeDP(), harness.SegmentTreeMerge());
    }

    private static MaximumSumOfSubsequenceWithNonAdjacentElementsBenchmarks BuildHarness()
    {
        var harness = new MaximumSumOfSubsequenceWithNonAdjacentElementsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
