using DSAExperimentation.Algorithms.Folding.Dags;
using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.LeetCodeCoverage.NumberOfWaysToArriveAtDestination.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfWaysToArriveAtDestination;

// LeetCode 1976. Number of Ways to Arrive at Destination: this repo's own
// ShortestPath.Dijkstra, run from the destination so every node's Dist becomes its
// own distance to it, then DagFold.Fold counts paths from node 0 that only ever
// step to a neighbor exactly on a shortest path (WaysChildTopology) - the same
// Dijkstra-then-DagFold composition NumberOfRestrictedPathsFromFirstToLastNodeTests
// already proves, just with an exact-distance child condition instead of a
// merely-decreasing one, since this problem counts only shortest paths.
public sealed partial class NumberOfWaysToArriveAtDestinationTests
{
    [Fact]
    public void CountWays_ClassicExample_ReturnsFour()
    {
        int[][] roads =
        [
            [0, 6, 7], [0, 1, 2], [1, 2, 3], [1, 3, 3], [6, 3, 3],
            [3, 5, 1], [6, 5, 1], [2, 5, 1], [0, 4, 5], [4, 6, 2],
        ];

        var ways = CountWaysToArrive(n: 7, roads);

        Assert.Equal(4, ways);
    }

    [Fact]
    public void CountWays_SingleRoad_ReturnsOne()
    {
        int[][] roads = [[0, 1, 1]];

        var ways = CountWaysToArrive(n: 2, roads);

        Assert.Equal(1, ways);
    }

    private static long CountWaysToArrive(int n, int[][] roads)
    {
        var nodes = BuildNodes(n, roads);
        AssignDistancesFromDestination(nodes, n - 1);

        return DagFold.Fold<
            WaysNode, WaysChildTopology, ListChildren<WaysNode>,
            NaturalChildOrder<WaysNode, ListChildren<WaysNode>>, ListChildren<WaysNode>,
            WaysCountAlgebra, long>(nodes[0]);
    }

    private static Dictionary<int, WaysNode> BuildNodes(int n, int[][] roads)
    {
        var nodes = new Dictionary<int, WaysNode>();

        for (var id = 0; id < n; id++)
        {
            nodes[id] = new WaysNode(id);
        }

        foreach (var road in roads)
        {
            var (from, to, weight) = (road[0], road[1], (long)road[2]);
            nodes[from].Edges.Add((weight, nodes[to]));
            nodes[to].Edges.Add((weight, nodes[from]));
        }

        return nodes;
    }

    private static void AssignDistancesFromDestination(Dictionary<int, WaysNode> nodes, int destinationId)
    {
        var distances = ShortestPath.Dijkstra<
            WaysNode, WaysEdgeTopology, ListEdges<WaysNode, long>, long>(nodes[destinationId]);

        foreach (var node in nodes.Values)
        {
            node.Dist = distances[node];
        }
    }
}
