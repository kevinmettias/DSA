using DSAExperimentation.Algorithms.MinimumSpanningTrees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.Algorithms.ShortestPaths.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinCostToConnectAllPoints;

// LeetCode 1584. Min Cost to Connect All Points: "minimum total edge weight to
// connect every point" is exactly what a minimum spanning tree computes, with edge
// weight defined as Manhattan distance between every pair of points (a complete
// graph, one edge per pair) - a direct read of this repo's own
// Algorithms.MinimumSpanningTrees.MinimumSpanningTree.Kruskal's reported total
// weight over the same WeightedNode/WeightedTopology fixtures NetworkDelayTimeTests
// already reuses for a weighted graph.
public sealed partial class MinCostToConnectAllPointsTests
{
    [Fact]
    public void MinCostConnectPoints_ClassicExample_ReturnsMstWeight()
    {
        int[][] points = [[0, 0], [2, 2], [3, 10], [5, 2], [7, 0]];

        Assert.Equal(20, MinCostConnectPoints(points));
    }

    [Fact]
    public void MinCostConnectPoints_SinglePoint_ReturnsZero()
    {
        int[][] points = [[0, 0]];

        Assert.Equal(0, MinCostConnectPoints(points));
    }

    private static int MinCostConnectPoints(int[][] points)
    {
        var nodes = points.Select((_, i) => new WeightedNode($"p{i}")).ToArray();

        for (var i = 0; i < points.Length; i++)
        {
            for (var j = i + 1; j < points.Length; j++)
            {
                var weight = ManhattanDistance(points[i], points[j]);
                nodes[i].Edges.Add((weight, nodes[j]));
                nodes[j].Edges.Add((weight, nodes[i]));
            }
        }

        var mst = MinimumSpanningTree.Kruskal<WeightedNode, WeightedTopology, ListEdges<WeightedNode, int>, int>(
            nodes);

        return mst.Sum(edge => edge.Weight);
    }

    private static int ManhattanDistance(int[] a, int[] b) => Math.Abs(a[0] - b[0]) + Math.Abs(a[1] - b[1]);
}
