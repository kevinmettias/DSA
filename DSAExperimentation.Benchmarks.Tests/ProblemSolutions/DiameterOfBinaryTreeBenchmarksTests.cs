using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DiameterOfBinaryTreeBenchmarks (ARCHITECTURE 17.9): an expected value known
// from the workload's shape. The class comment says the workload is a left-skewed chain, never a
// bushy tree, and a chain's longest path is the one from its head to its tail: NodeCount nodes
// joined by NodeCount - 1 edges, which is the diameter LC 543 measures. The second assertion calls
// the arm again on the same harness, as BenchmarkDotNet does across iterations, and compares it with
// a fresh build.
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

    private static DiameterOfBinaryTreeBenchmarks BuildHarness()
    {
        var harness = new DiameterOfBinaryTreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
