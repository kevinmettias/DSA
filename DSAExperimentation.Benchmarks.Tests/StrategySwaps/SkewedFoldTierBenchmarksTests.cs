using DSAExperimentation.Benchmarks.StrategySwaps;

namespace DSAExperimentation.Benchmarks.Tests.StrategySwaps;

// Harness coverage for SkewedFoldTierBenchmarks (ARCHITECTURE 17.9): as for FoldTierBenchmarks, the
// pinned value is the chain's length, which SizeAlgebra fixes independently of the arms.
public sealed partial class SkewedFoldTierBenchmarksTests
{
    private const int SmallestNodeCount = 1_000;

    [Fact]
    public void TreeTier_SkewedChain_AnswersTheNodeCount() =>
        Assert.Equal(SmallestNodeCount, BuildHarness().TreeTier());

    [Fact]
    public void DagTier_SkewedChain_AnswersTheNodeCount() =>
        Assert.Equal(SmallestNodeCount, BuildHarness().DagTier());

    [Fact]
    public void GraphTier_SkewedChain_AnswersTheNodeCount() =>
        Assert.Equal(SmallestNodeCount, BuildHarness().GraphTier());

    private static SkewedFoldTierBenchmarks BuildHarness()
    {
        var harness = new SkewedFoldTierBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
