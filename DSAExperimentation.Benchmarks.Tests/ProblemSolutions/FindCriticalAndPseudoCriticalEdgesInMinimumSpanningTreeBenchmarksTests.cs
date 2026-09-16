using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindCriticalAndPseudoCriticalEdgesInMinimumSpanningTreeBenchmarks
// (ARCHITECTURE 17.9): its two arms are competing strategies for the same question - a fresh BFS per
// connectivity question against this repo's own DisjointSet - so a harness whose arms disagree is
// classifying two different graphs. Both arms return only the combined length of the two classified
// index lists, which is a proxy: agreement witnesses that both classified the same NUMBER of edges,
// not that they put the same edges in the same list. Setup's spanning chain makes the graph
// connected, so an MST exists and every one of its edges lands in one of the two lists, which is the
// lower bound the proxy can be held to; no more edges can be classified than exist. The same
// NodeCount must rebuild the same edge list.
public sealed partial class FindCriticalAndPseudoCriticalEdgesInMinimumSpanningTreeBenchmarksTests
{
    private const int SmallestNodeCount = 40;
    private const int ExtraEdgeMultiplier = 3;

    // Every edge of a minimum spanning tree is critical or pseudo-critical, and a connected graph's
    // tree has one fewer edge than it has nodes.
    private const int MinClassifiedEdgeCount = SmallestNodeCount - 1;

    // Setup adds a spanning chain plus extra random edges, dropping only the self-loops it draws.
    private const int MaxClassifiedEdgeCount =
        (SmallestNodeCount - 1) + (SmallestNodeCount * ExtraEdgeMultiplier);

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameEdgeList()
    {
        Assert.InRange(BuildHarness().NaiveBfsConnectivity(), MinClassifiedEdgeCount, MaxClassifiedEdgeCount);

        Assert.Equal(BuildHarness().NaiveBfsConnectivity(), BuildHarness().NaiveBfsConnectivity());
    }

    [Fact]
    public void NaiveBfsConnectivity_SpanningChainPlusRandomEdges_AgreesWithDisjointSetKruskal()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DisjointSetKruskal(), harness.NaiveBfsConnectivity());
    }

    [Fact]
    public void DisjointSetKruskal_SpanningChainPlusRandomEdges_AgreesWithNaiveBfsConnectivity()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.NaiveBfsConnectivity(), harness.DisjointSetKruskal());
    }

    private static FindCriticalAndPseudoCriticalEdgesInMinimumSpanningTreeBenchmarks BuildHarness()
    {
        var harness = new FindCriticalAndPseudoCriticalEdgesInMinimumSpanningTreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
