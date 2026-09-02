using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfGoodPaths;

// LeetCode 2421. Number of Good Paths: process the tree's edges in increasing order
// of their higher-valued endpoint with this repo's own DisjointSet (Union-Find).
// Every single node is trivially its own good path (n of them, counted up front).
// Each DisjointSet.Union call merges two components whose "component max value" and
// "count of nodes achieving it" are tracked alongside it - when both sides share the
// same max value, every max-value node on one side pairs with every max-value node
// on the other to form a newly-connected good path (their connecting path's highest
// value is exactly that shared max, since every earlier-processed edge already
// guaranteed both sides never exceed it).
public sealed partial class NumberOfGoodPathsTests
{
    [Fact]
    public void CountGoodPaths_ClassicExample_ReturnsSixGoodPaths()
    {
        int[] vals = [1, 3, 2, 1, 3];
        int[][] edges = [[0, 1], [0, 2], [2, 3], [2, 4]];

        var count = CountGoodPaths(vals, edges);

        Assert.Equal(6, count);
    }

    [Fact]
    public void CountGoodPaths_AllNodesSameValue_CountsEveryPairPlusTrivialPaths()
    {
        // A path graph 0-1-2 with every value equal to 1: 3 trivial single-node
        // paths plus all 3 pairs, since the unique connecting path between any two
        // never exceeds their shared value.
        int[] vals = [1, 1, 1];
        int[][] edges = [[0, 1], [1, 2]];

        var count = CountGoodPaths(vals, edges);

        Assert.Equal(6, count);
    }

    [Fact]
    public void CountGoodPaths_SingleNode_ReturnsOneTrivialPath()
    {
        int[] vals = [5];
        int[][] edges = [];

        var count = CountGoodPaths(vals, edges);

        Assert.Equal(1, count);
    }

    private static int CountGoodPaths(int[] vals, int[][] edges)
    {
        var n = vals.Length;
        var components = new DisjointSet(n);
        var componentMaxValue = (int[])vals.Clone();
        var componentMaxCount = new int[n];
        Array.Fill(componentMaxCount, 1);

        var goodPaths = n;

        foreach (var edge in edges.OrderBy(e => Math.Max(vals[e[0]], vals[e[1]])))
        {
            var rootA = components.Find(edge[0]);
            var rootB = components.Find(edge[1]);

            if (rootA == rootB)
            {
                continue;
            }

            var maxA = componentMaxValue[rootA];
            var maxB = componentMaxValue[rootB];

            if (maxA == maxB)
            {
                goodPaths += componentMaxCount[rootA] * componentMaxCount[rootB];
            }

            var mergedMax = Math.Max(maxA, maxB);
            var mergedCount = maxA == maxB
                ? componentMaxCount[rootA] + componentMaxCount[rootB]
                : maxA > maxB ? componentMaxCount[rootA] : componentMaxCount[rootB];

            components.Union(edge[0], edge[1]);
            var newRoot = components.Find(edge[0]);
            componentMaxValue[newRoot] = mergedMax;
            componentMaxCount[newRoot] = mergedCount;
        }

        return goodPaths;
    }
}
