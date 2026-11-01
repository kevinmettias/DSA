using DSAExperimentation.Benchmarks.StrategySwaps;

namespace DSAExperimentation.Benchmarks.Tests.StrategySwaps;

// Harness coverage for SkewedTreeFoldBenchmarks (ARCHITECTURE 17.9): same fold and algebra as the
// balanced case, over the degenerate right-only chain where recursion depth grows one step per
// node. The generic check holds the arms to agreeing with each other, which a shared wrong count
// would pass; SizeAlgebra answers with the node count, which is pinned on the baseline.
public sealed partial class SkewedTreeFoldBenchmarksTests
{
    private const int SmallestNodeCount = 1_000;

    [Fact]
    public void Recursive_SizeAlgebra_CountsEveryNode() =>
        Assert.Equal(SmallestNodeCount, BuildHarness().Recursive());

    private static SkewedTreeFoldBenchmarks BuildHarness()
    {
        var harness = new SkewedTreeFoldBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
