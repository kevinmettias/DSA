using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumHeightTrees;

// LeetCode 310. Minimum Height Trees: repeatedly peel the current leaves (degree-1
// nodes) layer by layer, the same "peel the zero-remaining-degree frontier with a
// queue" shape TopologicalSort's Kahn's algorithm already uses for in-degree
// (TopologicalSort.cs's own BuildInDegree/ReleaseChildren pair) - just tracking plain
// undirected degree instead of in-degree, and stopping once 2 or fewer nodes remain
// instead of running to exhaustion. This repo's own Queue<int> is the frontier, the
// same RemoveInvalidParenthesesTests.cs uses for its own level-by-level BFS peel. The
// last layer standing are the roots whose eccentricity - and therefore tree height -
// is minimal, since they are the tree's own centroid(s) (at most 2 for any tree).
public sealed partial class MinimumHeightTreesTests
{
    [Fact]
    public void FindRoots_SingleNode_ReturnsThatNodeAsTheOnlyRoot()
    {
        var roots = FindRoots(1, []);

        Assert.Equal([0], roots);
    }

    [Fact]
    public void FindRoots_StarShape_ReturnsTheSingleCenterRoot()
    {
        int[][] edges = [[1, 0], [1, 2], [1, 3]];

        var roots = FindRoots(4, edges);

        Assert.Equal([1], roots);
    }

    [Fact]
    public void FindRoots_SixNodeTreeWithOffCenterBranch_ReturnsBothCenterRoots()
    {
        int[][] edges = [[0, 3], [1, 3], [2, 3], [4, 3], [5, 4]];

        var roots = FindRoots(6, edges);

        Assert.Equal([3, 4], roots.Order());
    }

    private static List<int> FindRoots(int n, int[][] edges)
    {
        if (n == 1)
        {
            return [0];
        }

        var adjacency = new List<int>[n];
        var degree = new int[n];
        for (var i = 0; i < n; i++)
        {
            adjacency[i] = [];
        }

        foreach (var edge in edges)
        {
            adjacency[edge[0]].Add(edge[1]);
            adjacency[edge[1]].Add(edge[0]);
            degree[edge[0]]++;
            degree[edge[1]]++;
        }

        var leaves = new RepoQueue();
        var remaining = n;
        for (var i = 0; i < n; i++)
        {
            if (degree[i] == 1)
            {
                leaves.Enqueue(i);
            }
        }

        while (remaining > 2)
        {
            var leafCount = leaves.Count;
            remaining -= leafCount;

            for (var i = 0; i < leafCount; i++)
            {
                leaves.TryDequeue(out var leaf);

                foreach (var neighbor in adjacency[leaf])
                {
                    if (--degree[neighbor] == 1)
                    {
                        leaves.Enqueue(neighbor);
                    }
                }
            }
        }

        var roots = new List<int>();
        while (leaves.TryDequeue(out var root))
        {
            roots.Add(root);
        }

        return roots;
    }
}
