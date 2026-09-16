using System.Numerics;
using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.LeetCode.CountConnectedSubgraphsWithEvenNodeSum;

// LeetCode 3910. Count Connected Subgraphs with Even Node Sum: n <= 13, so every
// non-empty node subset (2^n - 1 of them) is small enough to enumerate directly
// as a bitmask - the only real question per subset is how cheaply "is the
// induced subgraph on this mask connected" gets answered.
//
// Both strategies enumerate the same masks and the same even-sum check; they
// differ only in the connectivity check itself - a BFS over adjacency lists
// (the textbook approach, ARCHITECTURE.md 17.5), or this repo's own
// DisjointSet, unioning only the edges whose two endpoints both lie in the mask
// and then checking every set bit lands in the same component as the first.
internal static class CountConnectedSubgraphsWithEvenNodeSumSolution
{
    public static int CountEvenSumSubgraphsByBruteForceBfs(int[] nums, int[][] edges)
    {
        var n = nums.Length;
        var adjacency = BuildAdjacency(n, edges);
        var count = 0;

        for (var mask = 1; mask < (1 << n); mask++)
        {
            if (HasEvenSum(mask, nums) && IsConnectedByBfs(mask, n, adjacency))
            {
                count++;
            }
        }

        return count;
    }

    private static List<int>[] BuildAdjacency(int nodeCount, int[][] edges)
    {
        var adjacency = new List<int>[nodeCount];

        for (var i = 0; i < nodeCount; i++)
        {
            adjacency[i] = [];
        }

        foreach (var edge in edges)
        {
            adjacency[edge[0]].Add(edge[1]);
            adjacency[edge[1]].Add(edge[0]);
        }

        return adjacency;
    }

    private static bool IsConnectedByBfs(int mask, int nodeCount, List<int>[] adjacency)
    {
        var start = BitOperations.TrailingZeroCount(mask);
        var visited = new HashSet<int> { start };
        var queue = new Queue<int>();
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();

            foreach (var neighbor in adjacency[node])
            {
                if (((mask >> neighbor) & 1) == 1 && visited.Add(neighbor))
                {
                    queue.Enqueue(neighbor);
                }
            }
        }

        return visited.Count == BitOperations.PopCount((uint)mask);
    }

    public static int CountEvenSumSubgraphsByDisjointSet(int[] nums, int[][] edges)
    {
        var n = nums.Length;
        var count = 0;

        for (var mask = 1; mask < (1 << n); mask++)
        {
            if (HasEvenSum(mask, nums) && IsConnectedByDisjointSet(mask, n, edges))
            {
                count++;
            }
        }

        return count;
    }

    private static bool IsConnectedByDisjointSet(int mask, int nodeCount, int[][] edges)
    {
        var forest = new DisjointSet(nodeCount);
        UnionMaskEdges(forest, mask, edges);

        return IsEverySetBitConnected(forest, mask, nodeCount);
    }

    // Only edges with both endpoints inside the mask may be unioned - an edge
    // leaving the induced subgraph would tie it to a node the mask excludes.
    private static void UnionMaskEdges(DisjointSet forest, int mask, int[][] edges)
    {
        foreach (var edge in edges)
        {
            if (((mask >> edge[0]) & 1) == 1 && ((mask >> edge[1]) & 1) == 1)
            {
                forest.Union(edge[0], edge[1]);
            }
        }
    }

    // Every node the mask keeps must land in the same component as the first one
    // seen, which is what makes the induced subgraph connected.
    private static bool IsEverySetBitConnected(DisjointSet forest, int mask, int nodeCount)
    {
        var root = -1;

        for (var i = 0; i < nodeCount; i++)
        {
            if (((mask >> i) & 1) == 0)
            {
                continue;
            }

            if (root == -1)
            {
                root = i;
            }
            else if (!forest.IsConnected(root, i))
            {
                return false;
            }
        }

        return true;
    }

    private static bool HasEvenSum(int mask, int[] nums)
    {
        var sum = 0;

        for (var i = 0; i < nums.Length; i++)
        {
            if (((mask >> i) & 1) == 1)
            {
                sum += nums[i];
            }
        }

        return sum % 2 == 0;
    }
}
