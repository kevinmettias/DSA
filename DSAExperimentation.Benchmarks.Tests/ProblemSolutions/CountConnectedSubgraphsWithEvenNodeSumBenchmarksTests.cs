using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountConnectedSubgraphsWithEvenNodeSumBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for the same question - the BFS connectivity check over adjacency
// lists against this repo's DisjointSet unioning only the mask's own edges - so a harness whose arms
// disagree is timing two different problems. Both arms return an int, so they are compared directly.
// Setup builds the graph and the node values from one fixed seed, so the same NodeCount must rebuild
// the same workload; its documented shape is a random connected graph, so at least one even-sum
// subset (a single even-valued node) is always there to be counted.
public sealed partial class CountConnectedSubgraphsWithEvenNodeSumBenchmarksTests
{
    private const int SmallestNodeCount = 8;

    // Every even-valued single node is an even-sum connected subgraph on its own.
    private const int FewestEvenSumSubgraphs = 1;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload()
    {
        Assert.Equal(BuildHarness().BruteForceBfs(), BuildHarness().BruteForceBfs());

        Assert.True(BuildHarness().DisjointSet() >= FewestEvenSumSubgraphs);
    }

    [Fact]
    public void BruteForceBfs_EightNodeGraph_AgreesWithDisjointSet()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DisjointSet(), harness.BruteForceBfs());
    }

    [Fact]
    public void DisjointSet_EightNodeGraph_AgreesWithBruteForceBfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceBfs(), harness.DisjointSet());
    }

    private static CountConnectedSubgraphsWithEvenNodeSumBenchmarks BuildHarness()
    {
        var harness = new CountConnectedSubgraphsWithEvenNodeSumBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
