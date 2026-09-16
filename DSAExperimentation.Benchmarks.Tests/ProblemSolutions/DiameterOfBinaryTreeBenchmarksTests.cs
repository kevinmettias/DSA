using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DiameterOfBinaryTreeBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - a height walk recomputed at every node against this
// repo's own TreeMetrics.Diameter fold - so a harness whose arms disagree is timing two different
// problems. The class comment says the workload is a left-skewed chain, never a bushy tree, and a
// chain's longest path is the one from its head to its tail: NodeCount nodes joined by NodeCount - 1
// edges, which is the diameter LC 543 measures. Setup walks one fixed chain, so the same NodeCount
// must rebuild the same workload.
public sealed partial class DiameterOfBinaryTreeBenchmarksTests
{
    private const int SmallestNodeCount = 200;
    private const int ExpectedChainDiameter = SmallestNodeCount - 1;

    [Fact]
    public void Setup_LeftSkewedChain_HasTheChainDiameterAndRebuildsTheSameWorkload()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedChainDiameter, harness.RecomputedHeightPerNode());
        Assert.Equal(harness.RecomputedHeightPerNode(), BuildHarness().RecomputedHeightPerNode());
    }

    [Fact]
    public void RecomputedHeightPerNode_LeftSkewedChain_AgreesWithTreeMetricsDiameter()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.TreeMetricsDiameter(), harness.RecomputedHeightPerNode());
    }

    [Fact]
    public void TreeMetricsDiameter_LeftSkewedChain_AgreesWithRecomputedHeightPerNode()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RecomputedHeightPerNode(), harness.TreeMetricsDiameter());
    }

    private static DiameterOfBinaryTreeBenchmarks BuildHarness()
    {
        var harness = new DiameterOfBinaryTreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
