using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.Algorithms.ShortestPaths.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NetworkDelayTime;

// LeetCode 743. Network Delay Time: minutes for a signal from node k to reach every
// other node, or -1 if some node is unreachable - a direct read of
// ShortestPath.Dijkstra's own distances-from-source map.
public sealed partial class NetworkDelayTimeTests
{
    [Fact]
    public void MinutesToReachAll_ClassicExample_ReturnsMaxDistanceFromSource()
    {
        var nodes = new Dictionary<int, WeightedNode>
        {
            [1] = new WeightedNode("1"),
            [2] = new WeightedNode("2"),
            [3] = new WeightedNode("3"),
            [4] = new WeightedNode("4"),
        };

        WireEdge(nodes, 2, 1, 1);
        WireEdge(nodes, 2, 3, 1);
        WireEdge(nodes, 3, 4, 1);

        var minutes = MinutesToReachAll(nodes, source: 2);

        Assert.Equal(2, minutes);
    }

    [Fact]
    public void MinutesToReachAll_UnreachableNode_ReturnsNegativeOne()
    {
        var nodes = new Dictionary<int, WeightedNode>
        {
            [1] = new WeightedNode("1"),
            [2] = new WeightedNode("2"),
        };

        var minutes = MinutesToReachAll(nodes, source: 1);

        Assert.Equal(-1, minutes);
    }

    private static void WireEdge(Dictionary<int, WeightedNode> nodes, int from, int to, int weight)
        => nodes[from].Edges.Add((weight, nodes[to]));

    private static int MinutesToReachAll(Dictionary<int, WeightedNode> nodes, int source)
    {
        var distances = ShortestPath.Dijkstra<WeightedNode, WeightedTopology, ListEdges<WeightedNode, int>, int>(
            nodes[source]);

        return distances.Count == nodes.Count ? distances.Values.Max() : -1;
    }
}
