using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.Algorithms.ShortestPaths.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumCostOfAPathWithSpecialRoads;

// LeetCode 2662. Minimum Cost of a Path With Special Roads: start, target, and every
// special road's two endpoints become nodes in a small weighted graph - a direct
// (Manhattan-distance) edge between every pair of nodes, since you can always walk
// straight from any point to any other for that cost, plus each special road's own
// one-way discounted edge - and the answer is just ShortestPath.Dijkstra's settled
// distance to the target node. Same WeightedNode/WeightedTopology/ListEdges fixtures
// NetworkDelayTimeTests already uses for LC 743.
public sealed class MinimumCostOfAPathWithSpecialRoadsTests
{
    [Fact]
    public void MinimumCost_LeetCodeExampleOne_ChainsThroughBothSpecialRoads()
    {
        int[] start = [1, 1];
        int[] target = [4, 5];
        int[][] specialRoads = [[1, 2, 3, 3, 2], [3, 4, 4, 5, 1]];

        var cost = MinimumCost(start, target, specialRoads);

        Assert.Equal(5, cost);
    }

    [Fact]
    public void MinimumCost_SpecialRoadMoreExpensiveThanWalking_IgnoresTheSpecialRoad()
    {
        int[] start = [0, 0];
        int[] target = [3, 4];
        int[][] specialRoads = [[0, 0, 3, 4, 100]];

        var cost = MinimumCost(start, target, specialRoads);

        Assert.Equal(7, cost); // direct Manhattan distance beats the overpriced road
    }

    [Fact]
    public void MinimumCost_SpecialRoadDirectlyToTarget_TakesTheDiscountedShortcut()
    {
        int[] start = [0, 0];
        int[] target = [10, 10];
        int[][] specialRoads = [[0, 0, 10, 10, 1]];

        var cost = MinimumCost(start, target, specialRoads);

        Assert.Equal(1, cost);
    }

    private static int MinimumCost(int[] start, int[] target, int[][] specialRoads)
    {
        var nodesByPoint = new Dictionary<(int X, int Y), WeightedNode>();
        var startNode = GetOrCreateNode(nodesByPoint, start[0], start[1]);
        var targetNode = GetOrCreateNode(nodesByPoint, target[0], target[1]);

        foreach (var road in specialRoads)
        {
            GetOrCreateNode(nodesByPoint, road[0], road[1]);
            GetOrCreateNode(nodesByPoint, road[2], road[3]);
        }

        WireCompleteManhattanGraph(nodesByPoint);
        WireSpecialRoadEdges(nodesByPoint, specialRoads);

        var distances = ShortestPath.Dijkstra<WeightedNode, WeightedTopology, ListEdges<WeightedNode, int>, int>(startNode);

        return distances[targetNode];
    }

    private static void WireCompleteManhattanGraph(Dictionary<(int X, int Y), WeightedNode> nodesByPoint)
    {
        var points = nodesByPoint.ToArray();

        for (var i = 0; i < points.Length; i++)
        {
            for (var j = i + 1; j < points.Length; j++)
            {
                var distance = ManhattanDistance(points[i].Key, points[j].Key);
                points[i].Value.Edges.Add((distance, points[j].Value));
                points[j].Value.Edges.Add((distance, points[i].Value));
            }
        }
    }

    private static void WireSpecialRoadEdges(Dictionary<(int X, int Y), WeightedNode> nodesByPoint, int[][] specialRoads)
    {
        foreach (var road in specialRoads)
        {
            var from = nodesByPoint[(road[0], road[1])];
            var to = nodesByPoint[(road[2], road[3])];
            from.Edges.Add((road[4], to));
        }
    }

    private static WeightedNode GetOrCreateNode(Dictionary<(int X, int Y), WeightedNode> nodesByPoint, int x, int y)
    {
        var key = (x, y);
        if (!nodesByPoint.TryGetValue(key, out var node))
        {
            node = new WeightedNode($"({x},{y})");
            nodesByPoint[key] = node;
        }

        return node;
    }

    private static int ManhattanDistance((int X, int Y) a, (int X, int Y) b)
        => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
}
