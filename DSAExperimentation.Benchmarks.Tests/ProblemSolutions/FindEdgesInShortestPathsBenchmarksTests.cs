using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindEdgesInShortestPathsBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - a plain BCL Dijkstra from each end against this
// repo's own ShortestPath.Dijkstra over EdgeGraphTopology - so a harness whose arms disagree is
// reporting two different sets of shortest-path edges under one benchmark. Both answers are one
// verdict per edge in the input's own edge order, so they are compared as ordered sequences.
// Setup's workload guarantees reachability (every node i > 0 gets a back edge to an earlier node),
// so nodes 0 and NodeCount - 1 are connected and at least one edge lies on a shortest path.
public sealed partial class FindEdgesInShortestPathsBenchmarksTests
{
    // The smaller of Setup's [Params(50, 500)] node counts, so the harness drives the real
    // workload with the same seeded construction and without the larger run's cost.
    private const int SmallestNodeCount = 50;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameEdgeVerdicts() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForceDijkstra()),
            AnswerText.Of(BuildHarness().BruteForceDijkstra()));

    [Fact]
    public void BruteForceDijkstra_ConnectedWeightedGraph_AgreesWithShortestPathDijkstra()
    {
        var harness = BuildHarness();

        Assert.Contains(true, harness.BruteForceDijkstra());
        Assert.Equal(
            AnswerText.Of(harness.ShortestPathDijkstra()),
            AnswerText.Of(harness.BruteForceDijkstra()));
    }

    [Fact]
    public void ShortestPathDijkstra_ConnectedWeightedGraph_AgreesWithBruteForceDijkstra()
    {
        var harness = BuildHarness();

        Assert.Contains(true, harness.ShortestPathDijkstra());
        Assert.Equal(
            AnswerText.Of(harness.BruteForceDijkstra()),
            AnswerText.Of(harness.ShortestPathDijkstra()));
    }

    private static FindEdgesInShortestPathsBenchmarks BuildHarness()
    {
        var harness = new FindEdgesInShortestPathsBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
