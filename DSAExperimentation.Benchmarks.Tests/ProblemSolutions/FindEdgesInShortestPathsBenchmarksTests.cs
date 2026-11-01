using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindEdgesInShortestPathsBenchmarks (ARCHITECTURE 17.9): the generic arms check
// only compares the arms with each other, and arms that all answer false for every edge would agree
// too, so this holds the baseline to what Setup's workload guarantees. It answers one verdict per
// edge. Setup's workload guarantees reachability (every node i > 0 gets a back edge to an earlier
// node), so nodes 0 and NodeCount - 1 are connected and at least one edge lies on a shortest path.
public sealed partial class FindEdgesInShortestPathsBenchmarksTests
{
    // The smaller of Setup's [Params(50, 500)] node counts, so the harness drives the real
    // workload with the same seeded construction and without the larger run's cost.
    private const int SmallestNodeCount = 50;

    [Fact]
    public void BruteForceDijkstra_ConnectedWeightedGraph_PutsSomeEdgeOnAShortestPath() =>
        Assert.Contains(true, BuildHarness().BruteForceDijkstra());

    private static FindEdgesInShortestPathsBenchmarks BuildHarness()
    {
        var harness = new FindEdgesInShortestPathsBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
