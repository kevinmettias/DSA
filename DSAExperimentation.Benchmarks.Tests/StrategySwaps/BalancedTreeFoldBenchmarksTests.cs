using DSAExperimentation.Benchmarks.StrategySwaps;

namespace DSAExperimentation.Benchmarks.Tests.StrategySwaps;

// Harness coverage for BalancedTreeFoldBenchmarks (ARCHITECTURE 17.9): the generic check holds the
// arms to agreeing with each other, which a shared wrong count would pass. SizeAlgebra answers with
// the node count, so the fixture pins the baseline to that decisive value.
public sealed partial class BalancedTreeFoldBenchmarksTests
{
    private const int SmallestNodeCount = 10_000;

    [Fact]
    public void Recursive_SizeAlgebra_CountsEveryNode() =>
        Assert.Equal(SmallestNodeCount, BuildHarness().Recursive());

    private static BalancedTreeFoldBenchmarks BuildHarness()
    {
        var harness = new BalancedTreeFoldBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
