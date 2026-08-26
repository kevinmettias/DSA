using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.Algorithms.ShortestPaths.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.ShortestPaths;

public sealed class BellmanFordTests
{
    [Fact]
    public void TryComputeDistances_SampleGraph_MatchesDijkstra()
    {
        var (a, b, c, d) = WeightedGraphs.SampleGraph();

        var succeeded = BellmanFord.TryComputeDistances<WeightedNode, WeightedTopology, ListEdges<WeightedNode, int>, int>(
            [a, b, c, d], a, out var distances);

        Assert.True(succeeded);
        Assert.Equal(0, distances[a]);
        Assert.Equal(1, distances[b]);
        Assert.Equal(3, distances[c]);
        Assert.Equal(4, distances[d]);
    }

    [Fact]
    public void TryComputeDistances_UnreachableNode_IsAbsentFromResult()
    {
        var (a, b, _, _) = WeightedGraphs.SampleGraph();
        var unreachable = new WeightedNode("Z");

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
        var a = new WeightedNode("A");
        var b = new WeightedNode("B");
        var c = new WeightedNode("C");
        a.Edges.Add((4, b));
        b.Edges.Add((-2, c));

        var succeeded = BellmanFord.TryComputeDistances<WeightedNode, WeightedTopology, ListEdges<WeightedNode, int>, int>(
            [a, b, c], a, out var distances);

        Assert.True(succeeded);
        Assert.Equal(0, distances[a]);
        Assert.Equal(4, distances[b]);
        Assert.Equal(2, distances[c]);
    }

    // A -> B enters a cycle B -> C -> B whose total weight is 1 + -3 = -2, reachable from
    // source - every extra trip around it lowers B/C's distance further, which is exactly
    // what the |V|th relaxation round is meant to catch.
    [Fact]
    public void TryComputeDistances_NegativeCycleReachableFromSource_ReturnsFalse()
    {
        var a = new WeightedNode("A");
        var b = new WeightedNode("B");
        var c = new WeightedNode("C");
        a.Edges.Add((1, b));
        b.Edges.Add((1, c));
        c.Edges.Add((-3, b));

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
        var a = new WeightedNode("A");
        var b = new WeightedNode("B");
        var d = new WeightedNode("D");
        var e = new WeightedNode("E");
        a.Edges.Add((1, b));
        d.Edges.Add((1, e));
        e.Edges.Add((-3, d));

        var succeeded = BellmanFord.TryComputeDistances<WeightedNode, WeightedTopology, ListEdges<WeightedNode, int>, int>(
            [a, b], a, out var distances);

        Assert.True(succeeded);
        Assert.Equal(0, distances[a]);
        Assert.Equal(1, distances[b]);
    }
}
