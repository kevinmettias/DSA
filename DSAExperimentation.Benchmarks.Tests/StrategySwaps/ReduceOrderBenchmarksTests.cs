using DSAExperimentation.Benchmarks.StrategySwaps;

namespace DSAExperimentation.Benchmarks.Tests.StrategySwaps;

// Harness coverage for ReduceOrderBenchmarks (ARCHITECTURE 17.9): the generic check holds the arms
// to agreeing with each other, which a shared wrong count would pass. DistanceMapReduceAlgebra
// records every reachable node, so the baseline must answer with the node count.
public sealed partial class ReduceOrderBenchmarksTests
{
    private const int SmallestNodeCount = 10_000;

    [Fact]
    public void BreadthFirst_DistanceMap_RecordsEveryNode() =>
        Assert.Equal(SmallestNodeCount, BuildHarness().BreadthFirst());

    private static ReduceOrderBenchmarks BuildHarness()
    {
        var harness = new ReduceOrderBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
