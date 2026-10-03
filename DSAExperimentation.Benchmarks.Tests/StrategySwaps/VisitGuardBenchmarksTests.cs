using DSAExperimentation.Benchmarks.StrategySwaps;

namespace DSAExperimentation.Benchmarks.Tests.StrategySwaps;

// Harness coverage for VisitGuardBenchmarks (ARCHITECTURE 17.9): DistanceMapReduceAlgebra records
// every node it reaches, so both tiers must answer with the tree's node count - the guard may only
// cost time, never change what is reached.
public sealed partial class VisitGuardBenchmarksTests
{
    private const int SmallestNodeCount = 10_000;

    [Fact]
    public void Unguarded_BalancedTree_RecordsEveryNode() =>
        Assert.Equal(SmallestNodeCount, BuildHarness().Unguarded());

    [Fact]
    public void Tracked_BalancedTree_RecordsEveryNode() =>
        Assert.Equal(SmallestNodeCount, BuildHarness().Tracked());

    private static VisitGuardBenchmarks BuildHarness()
    {
        var harness = new VisitGuardBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
