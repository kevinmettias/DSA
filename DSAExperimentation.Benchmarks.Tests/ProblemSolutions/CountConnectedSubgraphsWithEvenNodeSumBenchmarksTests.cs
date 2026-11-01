using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountConnectedSubgraphsWithEvenNodeSumBenchmarks (ARCHITECTURE 17.9):
// BenchmarkArmsTests already holds the arms to agreeing on a rebuilt workload, so this asserts only a
// bound that comes from the workload's documented shape: a random connected graph, so at least one
// even-sum subset (a single even-valued node) is always there to be counted.
public sealed partial class CountConnectedSubgraphsWithEvenNodeSumBenchmarksTests
{
    private const int SmallestNodeCount = 8;

    // Every even-valued single node is an even-sum connected subgraph on its own.
    private const int FewestEvenSumSubgraphs = 1;

    [Fact]
    public void DisjointSet_EightNodeGraph_CountsAtLeastOneEvenSumSubgraph() =>
        Assert.True(BuildHarness().DisjointSet() >= FewestEvenSumSubgraphs);

    private static CountConnectedSubgraphsWithEvenNodeSumBenchmarks BuildHarness()
    {
        var harness = new CountConnectedSubgraphsWithEvenNodeSumBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
