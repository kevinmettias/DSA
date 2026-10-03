using DSAExperimentation.Benchmarks.StrategySwaps;

namespace DSAExperimentation.Benchmarks.Tests.StrategySwaps;

// Harness coverage for FoldTierBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests already holds the
// three tiers to one answer, and what it cannot know is that the answer is the tree's size -
// SizeAlgebra makes that an expected value independent of every arm.
public sealed partial class FoldTierBenchmarksTests
{
    private const int SmallestNodeCount = 10_000;

    [Fact]
    public void TreeTier_BalancedTree_AnswersTheNodeCount() =>
        Assert.Equal(SmallestNodeCount, BuildHarness().TreeTier());

    [Fact]
    public void DagTier_BalancedTree_AnswersTheNodeCount() =>
        Assert.Equal(SmallestNodeCount, BuildHarness().DagTier());

    [Fact]
    public void GraphTier_BalancedTree_AnswersTheNodeCount() =>
        Assert.Equal(SmallestNodeCount, BuildHarness().GraphTier());

    private static FoldTierBenchmarks BuildHarness()
    {
        var harness = new FoldTierBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
