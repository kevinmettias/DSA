using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumCostPathWithEdgeReversalsBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - Dijkstra over adjacency lists the arm builds
// itself from LeetCode's own (n, edges) shape against the same search over a prepared ReversalGraph
// - so a harness whose arms disagree is timing two different graphs. The graph is seeded, so the same
// NodeCount must rebuild the same edges; ReversalGraph.Build is pure, so a rebuilt graph is the same
// graph.
public sealed partial class MinimumCostPathWithEdgeReversalsBenchmarksTests
{
    private const int SmallestNodeCount = 50;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameGraph() =>
        Assert.Equal(BuildHarness().BruteForceDijkstra(), BuildHarness().BruteForceDijkstra());

    [Fact]
    public void BruteForceDijkstra_SeededEdgeList_AgreesWithShortestPathDijkstra()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ShortestPathDijkstra(), harness.BruteForceDijkstra());
    }

    [Fact]
    public void ShortestPathDijkstra_SeededEdgeList_AgreesWithBruteForceDijkstra()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceDijkstra(), harness.ShortestPathDijkstra());
    }

    private static MinimumCostPathWithEdgeReversalsBenchmarks BuildHarness()
    {
        var harness = new MinimumCostPathWithEdgeReversalsBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
