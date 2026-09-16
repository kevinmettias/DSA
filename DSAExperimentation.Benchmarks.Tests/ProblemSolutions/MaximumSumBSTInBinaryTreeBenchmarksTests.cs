using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumSumBSTInBinaryTreeBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the per-node subtree revalidation against the single
// bottom-up pass - so a harness whose arms disagree is timing two different problems. Setup rebuilds
// the skewed fixture tree from the same NodeCount, so the same NodeCount must rebuild the same
// workload; otherwise two published numbers were never comparable in the first place.
public sealed partial class MaximumSumBSTInBinaryTreeBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().OnePassBottomUpScan(), BuildHarness().OnePassBottomUpScan());

    [Fact]
    public void RevalidatePerNode_SkewedFixtureChain_AgreesWithOnePassBottomUpScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.OnePassBottomUpScan(), harness.RevalidatePerNode());
    }

    [Fact]
    public void OnePassBottomUpScan_SkewedFixtureChain_AgreesWithRevalidatePerNode()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RevalidatePerNode(), harness.OnePassBottomUpScan());
    }

    private static MaximumSumBSTInBinaryTreeBenchmarks BuildHarness()
    {
        var harness = new MaximumSumBSTInBinaryTreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
