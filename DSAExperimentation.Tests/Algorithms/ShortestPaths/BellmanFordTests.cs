using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.Algorithms.ShortestPaths.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.ShortestPaths;

public sealed class BellmanFordTests
{
    private const string NodeA = "A";
    private const string NodeB = "B";
    private const string NodeC = "C";
    private const string NodeD = "D";
    private const string NodeE = "E";
    private const string NodeZ = "Z";

    [Fact]
    public void TryComputeDistances_SampleGraph_MatchesDijkstra()
    {
        const int expectedDistanceToC = 3;
        const int expectedDistanceToD = 4;

        var (a, b, c, d) = WeightedGraphs.SampleGraph();

        var succeeded = BellmanFord.TryComputeDistances<WeightedNode, WeightedTopology, ListEdges<WeightedNode, int>, int>(
            [a, b, c, d], a, out var distances);

        Assert.True(succeeded);
        Assert.Equal(0, distances[a]);
        Assert.Equal(1, distances[b]);
        Assert.Equal(expectedDistanceToC, distances[c]);
        Assert.Equal(expectedDistanceToD, distances[d]);
    }

    [Fact]
    public void TryComputeDistances_UnreachableNode_IsAbsentFromResult()
    {
        var (a, b, _, _) = WeightedGraphs.SampleGraph();
        var unreachable = new WeightedNode(NodeZ);

        var succeeded = BellmanFord.TryComputeDistances<WeightedNode, WeightedTopology, ListEdges<WeightedNode, int>, int>(
            [a, b, unreachable], a, out var distances);

        Assert.True(succeeded);
        Assert.False(distances.ContainsKey(unreachable));
    }

    // A -[4]-> B -[-2]-> C makes the negative edge the whole point: the cheapest A->C is
    // 4 + -2 = 2, cheaper than skipping B entirely, so this only passes if relaxation
    // actually crosses a negative-weight edge rather than being blocked by one (the way
    // ShortestPath.Dijkstra's own non-negative-weight law would silently mis-answer here).
    [Fact]
    public void TryComputeDistances_NegativeEdgeWithoutCycle_ComputesCorrectDistances()
    {
        const int weightAToB = 4;
        const int weightBToC = -2;
        const int expectedDistanceToB = 4;
        const int expectedDistanceToC = 2;

        var a = new WeightedNode(NodeA);
        var b = new WeightedNode(NodeB);
        var c = new WeightedNode(NodeC);
        a.Edges.Add((weightAToB, b));
        b.Edges.Add((weightBToC, c));

        var succeeded = BellmanFord.TryComputeDistances<WeightedNode, WeightedTopology, ListEdges<WeightedNode, int>, int>(
            [a, b, c], a, out var distances);

        Assert.True(succeeded);
        Assert.Equal(0, distances[a]);
        Assert.Equal(expectedDistanceToB, distances[b]);
        Assert.Equal(expectedDistanceToC, distances[c]);
    }

    // A -> B enters a cycle B -> C -> B whose total weight is 1 + -3 = -2, reachable from
    // source - every extra trip around it lowers B/C's distance further, which is exactly
    // what the |V|th relaxation round is meant to catch.
    [Fact]
    public void TryComputeDistances_NegativeCycleReachableFromSource_ReturnsFalse()
    {
        const int weightCToB = -3;

        var a = new WeightedNode(NodeA);
        var b = new WeightedNode(NodeB);
        var c = new WeightedNode(NodeC);
        a.Edges.Add((1, b));
        b.Edges.Add((1, c));
        c.Edges.Add((weightCToB, b));

        var succeeded = BellmanFord.TryComputeDistances<WeightedNode, WeightedTopology, ListEdges<WeightedNode, int>, int>(
            [a, b, c], a, out _);

        Assert.False(succeeded);
    }

    // D/E's negative cycle is real, but D is omitted from `vertices`, so CollectEdges never
    // walks it and the cycle is never relaxed at all - the "can only under-report, never a
    // false positive" guarantee the doc comment states, proven directly rather than assumed.
    [Fact]
    public void TryComputeDistances_NegativeCycleOutsideGivenVertices_DoesNotFalselyReportIt()
    {
        const int weightEToD = -3;

        var a = new WeightedNode(NodeA);
        var b = new WeightedNode(NodeB);
        var d = new WeightedNode(NodeD);
        var e = new WeightedNode(NodeE);
        a.Edges.Add((1, b));
        d.Edges.Add((1, e));
        e.Edges.Add((weightEToD, d));

        var succeeded = BellmanFord.TryComputeDistances<WeightedNode, WeightedTopology, ListEdges<WeightedNode, int>, int>(
            [a, b], a, out var distances);

        Assert.True(succeeded);
        Assert.Equal(0, distances[a]);
        Assert.Equal(1, distances[b]);
    }
}
