using DSAExperimentation.Benchmarks.StrategySwaps;

namespace DSAExperimentation.Benchmarks.Tests.StrategySwaps;

// Harness coverage for ReduceOrderBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// traversal orders for one reduce over one balanced tree, and DistanceMapReduceAlgebra records
// every reachable node, so both arms must answer with the node count as well as with each other.
public sealed partial class ReduceOrderBenchmarksTests
{
    private const int SmallestNodeCount = 10_000;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BreadthFirst()),
            AnswerText.Of(BuildHarness().BreadthFirst()));

    [Fact]
    public void BreadthFirst_AgreesWithDepthFirst()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BreadthFirst(), harness.DepthFirst());
    }

    [Fact]
    public void DepthFirst_AgreesWithBreadthFirst()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DepthFirst(), harness.BreadthFirst());
    }

    [Fact]
    public void BreadthFirst_DistanceMap_RecordsEveryNode() =>
        Assert.Equal(SmallestNodeCount, BuildHarness().BreadthFirst());

    [Fact]
    public void DepthFirst_DistanceMap_RecordsEveryNode() =>
        Assert.Equal(SmallestNodeCount, BuildHarness().DepthFirst());

    private static ReduceOrderBenchmarks BuildHarness()
    {
        var harness = new ReduceOrderBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
