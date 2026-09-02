using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.LeetCode.MaximizeAlternatingSumUsingSwaps;

// LeetCode 3695. Maximize Alternating Sum Using Swaps: swaps generate arbitrary
// permutations within each connected component of indices (the same "connected
// transpositions generate the full symmetric group on that component" fact LC 1202
// Smallest String With Swaps relies on), so the puzzle reduces to grouping indices
// by connectivity and, within each group, handing the largest values to the
// even-index (added) slots and the smallest to the odd-index (subtracted) slots.
// Both strategies below share that greedy assignment; they differ only in how
// connectivity is discovered.
internal static class MaximizeAlternatingSumUsingSwapsSolution
{
    // The textbook answer: a hand-rolled adjacency list plus BFS, no repo primitive -
    // deliberately written this way, the arm the composed strategy below has to
    // justify itself against.
    public static long MaximumAlternatingSumByComponentBfs(int[] nums, int[][] swaps)
    {
        var n = nums.Length;
        var neighbors = BuildAdjacencyList(n, swaps);
        var visited = new bool[n];
        long total = 0;

        for (var start = 0; start < n; start++)
        {
            if (visited[start])
            {
                continue;
            }

            total += ComponentSumFromBfs(start, neighbors, visited, nums);
        }

        return total;
    }

    private static List<int>[] BuildAdjacencyList(int n, int[][] swaps)
    {
        var neighbors = new List<int>[n];

        for (var i = 0; i < n; i++)
        {
            neighbors[i] = [];
        }

        foreach (var swap in swaps)
        {
            neighbors[swap[0]].Add(swap[1]);
            neighbors[swap[1]].Add(swap[0]);
        }

        return neighbors;
    }

    private static long ComponentSumFromBfs(int start, List<int>[] neighbors, bool[] visited, int[] nums)
    {
        var component = new List<int> { start };
        visited[start] = true;

        var queue = new Queue<int>();
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            foreach (var neighbor in neighbors[current])
            {
                if (visited[neighbor])
                {
                    continue;
                }

                visited[neighbor] = true;
                component.Add(neighbor);
                queue.Enqueue(neighbor);
            }
        }

        return MaximumComponentContribution(component, nums);
    }

    // This repo's own DisjointSet unions indices directly - swap endpoints are
    // already dense int ids, so no KeyedDisjointSet indirection is needed (§7's
    // "prefer an already-existing self-hosted structure" precedent, the same move
    // ShortestPath makes switching from a hand-rolled priority queue to Heap).
    public static long MaximumAlternatingSumByDisjointSet(int[] nums, int[][] swaps)
    {
        var n = nums.Length;
        var disjointSet = new DisjointSet(n);

        foreach (var swap in swaps)
        {
            disjointSet.Union(swap[0], swap[1]);
        }

        var componentsByRoot = new Dictionary<int, List<int>>();

        for (var i = 0; i < n; i++)
        {
            var root = disjointSet.Find(i);

            if (!componentsByRoot.TryGetValue(root, out var component))
            {
                component = [];
                componentsByRoot[root] = component;
            }

            component.Add(i);
        }

        long total = 0;

        foreach (var component in componentsByRoot.Values)
        {
            total += MaximumComponentContribution(component, nums);
        }

        return total;
    }

    // Sort the group's values descending; the top evenSlotCount of them occupy the
    // component's even (added) indices, everything left over occupies the odd
    // (subtracted) indices - any assignment within the same sign group sums the same.
    private static long MaximumComponentContribution(List<int> componentIndices, int[] nums)
    {
        var evenSlotCount = componentIndices.Count(index => index % 2 == 0);
        var values = componentIndices.Select(index => (long)nums[index]).OrderDescending().ToArray();

        long sum = 0;

        for (var rank = 0; rank < values.Length; rank++)
        {
            sum += rank < evenSlotCount ? values[rank] : -values[rank];
        }

        return sum;
    }
}
