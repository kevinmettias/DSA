using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CollectCoinsInATree;

// LeetCode 2603. Collect Coins in a Tree: starting anywhere, collect every coin
// (coins within distance 2 of the current vertex are free) and return to the start,
// counting each edge traversal separately.
//
// The editorial reduction: trim leaves with no coin to a fixed point (a coin-less
// dead end is never worth walking into), then trim two more leaf layers
// unconditionally - the radius-2 collection rule means whatever leaves survive the
// coin-aware trim are still within 2 hops of an inner node once that inner node is
// visited, so the outermost two layers are always collected for free. The answer is
// twice whatever tree remains, since every surviving edge is walked out and back.
// This is the same "peel the degree-1 frontier with a queue" shape
// MinimumHeightTreesTests.cs uses for its own centroid search, and the same
// TopologicalSort.cs's Kahn's algorithm uses for in-degree - just undirected degree,
// gated by a coin predicate for the first round only.
public sealed partial class CollectCoinsInATreeTests
{
    public static TheoryData<int[][], int[], int> Examples =>
        new()
        {
            { [[0, 1], [1, 2], [2, 3], [3, 4], [4, 5]], [1, 0, 0, 0, 0, 1], 2 },
            { [[0, 1], [0, 2], [1, 3], [1, 4], [2, 5], [5, 6], [5, 7]], [0, 0, 0, 1, 1, 0, 0, 1], 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinEdgesByRescan_LeetCodeExamples_ReturnsRoundTripEdgeCount(
        int[][] edges, int[] coins, int expected) =>
        Assert.Equal(expected, MinEdgesByRescan(edges, coins));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinEdgesByLeafQueue_LeetCodeExamples_ReturnsRoundTripEdgeCount(
        int[][] edges, int[] coins, int expected) =>
        Assert.Equal(expected, MinEdgesByLeafQueue(edges, coins));

    [Fact]
    public void MinEdgesByLeafQueue_SingleNode_ReturnsZero() =>
        Assert.Equal(0, MinEdgesByLeafQueue([], [1]));

    // Baseline: no queue - repeatedly rescans every node for a removable zero-coin
    // leaf until a full pass finds none, the O(n) primitive-based queue below has to
    // beat.
    private static int MinEdgesByRescan(int[][] edges, int[] coins)
    {
        var n = coins.Length;

        if (n <= 1)
        {
            return 0;
        }

        var (adjacency, degree) = BuildTree(n, edges);
        var removed = new bool[n];

        TrimZeroCoinLeavesByRescanning(adjacency, degree, coins, removed);
        TrimLeafLayerUnconditionally(adjacency, degree, removed);
        TrimLeafLayerUnconditionally(adjacency, degree, removed);

        return 2 * RemainingEdgeCount(adjacency, removed);
    }

    // This repo's own Queue<T>: every zero-coin leaf is enqueued once and dequeued
    // once, so the fixed-point trim runs in one O(n) pass instead of rescanning
    // every node on every round the way MinEdgesByRescan does.
    private static int MinEdgesByLeafQueue(int[][] edges, int[] coins)
    {
        var n = coins.Length;

        if (n <= 1)
        {
            return 0;
        }

        var (adjacency, degree) = BuildTree(n, edges);
        var removed = new bool[n];

        TrimZeroCoinLeavesByQueue(adjacency, degree, coins, removed);
        TrimLeafLayerUnconditionally(adjacency, degree, removed);
        TrimLeafLayerUnconditionally(adjacency, degree, removed);

        return 2 * RemainingEdgeCount(adjacency, removed);
    }

    private static (List<int>[] Adjacency, int[] Degree) BuildTree(int n, int[][] edges)
    {
        var adjacency = new List<int>[n];

        for (var node = 0; node < n; node++)
        {
            adjacency[node] = [];
        }

        foreach (var edge in edges)
        {
            adjacency[edge[0]].Add(edge[1]);
            adjacency[edge[1]].Add(edge[0]);
        }

        var degree = new int[n];

        for (var node = 0; node < n; node++)
        {
            degree[node] = adjacency[node].Count;
        }

        return (adjacency, degree);
    }

    private static void TrimZeroCoinLeavesByRescanning(
        List<int>[] adjacency, int[] degree, int[] coins, bool[] removed)
    {
        bool removedAny;

        do
        {
            removedAny = false;

            for (var node = 0; node < adjacency.Length; node++)
            {
                if (!removed[node] && degree[node] == 1 && coins[node] == 0)
                {
                    RemoveNode(adjacency, degree, removed, node);
                    removedAny = true;
                }
            }
        } while (removedAny);
    }

    private static void TrimZeroCoinLeavesByQueue(
        List<int>[] adjacency, int[] degree, int[] coins, bool[] removed)
    {
        var queue = new RepoQueue();

        for (var node = 0; node < adjacency.Length; node++)
        {
            if (degree[node] == 1 && coins[node] == 0)
            {
                removed[node] = true;
                queue.Enqueue(node);
            }
        }

        while (queue.TryDequeue(out var node))
        {
            foreach (var neighbor in adjacency[node])
            {
                if (removed[neighbor])
                {
                    continue;
                }

                degree[neighbor]--;

                if (degree[neighbor] == 1 && coins[neighbor] == 0)
                {
                    removed[neighbor] = true;
                    queue.Enqueue(neighbor);
                }
            }
        }
    }

    // The radius-2 collection rule: two unconditional layers, regardless of coins,
    // account for the "free within 2 hops" allowance.
    private static void TrimLeafLayerUnconditionally(List<int>[] adjacency, int[] degree, bool[] removed)
    {
        var leaves = new List<int>();

        for (var node = 0; node < adjacency.Length; node++)
        {
            if (!removed[node] && degree[node] <= 1)
            {
                leaves.Add(node);
            }
        }

        foreach (var leaf in leaves)
        {
            RemoveNode(adjacency, degree, removed, leaf);
        }
    }

    private static void RemoveNode(List<int>[] adjacency, int[] degree, bool[] removed, int node)
    {
        removed[node] = true;

        foreach (var neighbor in adjacency[node])
        {
            if (!removed[neighbor])
            {
                degree[neighbor]--;
            }
        }
    }

    private static int RemainingEdgeCount(List<int>[] adjacency, bool[] removed)
    {
        var edgeCount = 0;

        for (var node = 0; node < adjacency.Length; node++)
        {
            if (removed[node])
            {
                continue;
            }

            foreach (var neighbor in adjacency[node])
            {
                if (!removed[neighbor])
                {
                    edgeCount++;
                }
            }
        }

        return edgeCount / 2;
    }
}
