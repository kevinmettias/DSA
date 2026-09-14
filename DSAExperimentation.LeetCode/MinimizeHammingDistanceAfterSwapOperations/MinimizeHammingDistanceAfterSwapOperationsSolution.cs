using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.MinimizeHammingDistanceAfterSwapOperations;

// LeetCode 1722. Minimize Hamming Distance After Swap Operations: an allowed swap
// may be applied any number of times, so the indices reachable from one another
// through a chain of allowed swaps form a connected component and any permutation
// of a component's own values is achievable. A component therefore contributes
// exactly the number of target values its own source multiset cannot supply -
// which is a frequency count, not a matching problem - and the answer is the sum
// over components.
//
// Both strategies answer the same question, "which indices share a component", over
// the array's own index positions; they differ only in how the components are
// discovered. This is the SmallestStringWithSwaps (LC 1202) shape with a multiset
// comparison at the end instead of a per-component sort.
internal static class MinimizeHammingDistanceAfterSwapOperationsSolution
{
    // The textbook answer: materialize an adjacency list over the indices and BFS
    // out of each unvisited index to collect its component, counting that
    // component's shortfall with a BCL Dictionary. Deliberately BCL-only - it is
    // the arm the DisjointSet composition below has to justify itself against.
    public static int MinimumHammingDistanceByAdjacencyListBfs(int[] source, int[] target, int[][] allowedSwaps)
    {
        var adjacency = BuildAdjacencyList(source.Length, allowedSwaps);
        var visited = new bool[source.Length];
        var values = new ValueArrays(source, target);
        var distance = 0;

        for (var start = 0; start < source.Length; start++)
        {
            distance += ComponentDistanceFromStart(start, adjacency, visited, values);
        }

        return distance;
    }

    private static List<int>[] BuildAdjacencyList(int length, int[][] allowedSwaps)
    {
        var adjacency = new List<int>[length];

        for (var index = 0; index < length; index++)
        {
            adjacency[index] = [];
        }

        foreach (var swap in allowedSwaps)
        {
            adjacency[swap[0]].Add(swap[1]);
            adjacency[swap[1]].Add(swap[0]);
        }

        return adjacency;
    }

    // Contributes nothing for an index an earlier component already reached, so
    // every component is counted exactly once however it is entered.
    private static int ComponentDistanceFromStart(
        int start, List<int>[] adjacency, bool[] visited, ValueArrays values)
    {
        if (visited[start])
        {
            return 0;
        }

        return UnsuppliedCountWithDictionary(CollectComponent(start, adjacency, visited), values);
    }

    private static List<int> CollectComponent(int start, List<int>[] adjacency, bool[] visited)
    {
        var queue = new Queue<int>();
        queue.Enqueue(start);
        visited[start] = true;

        var component = new List<int> { start };

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            foreach (var next in adjacency[current])
            {
                VisitNeighbor(next, visited, component, queue);
            }
        }

        return component;
    }

    private static void VisitNeighbor(int next, bool[] visited, List<int> component, Queue<int> queue)
    {
        if (visited[next])
        {
            return;
        }

        visited[next] = true;
        component.Add(next);
        queue.Enqueue(next);
    }

    // This repo's own DisjointSet over the array's index positions: O(1) amortized
    // Union per allowed swap and O(a(n)) Find per index, with no adjacency list and
    // no per-component BFS queue allocated at all - one pass groups the indices by
    // root and one pass counts each group's shortfall.
    public static int MinimumHammingDistanceByDisjointSet(int[] source, int[] target, int[][] allowedSwaps)
    {
        var components = BuildComponents(source.Length, allowedSwaps);
        var indicesByRoot = GroupIndicesByRoot(components, source.Length);
        var values = new ValueArrays(source, target);
        var distance = 0;

        foreach (var indices in indicesByRoot.Values)
        {
            distance += UnsuppliedCountWithHashMap(indices, values);
        }

        return distance;
    }

    private static DisjointSet BuildComponents(int length, int[][] allowedSwaps)
    {
        var components = new DisjointSet(length);

        foreach (var swap in allowedSwaps)
        {
            components.Union(swap[0], swap[1]);
        }

        return components;
    }

    private static HashMap<int, List<int>> GroupIndicesByRoot(DisjointSet components, int length)
    {
        var indicesByRoot = new HashMap<int, List<int>>();

        for (var index = 0; index < length; index++)
        {
            var root = components.Find(index);

            if (!indicesByRoot.TryGetValue(root, out var indices))
            {
                indices = [];
                indicesByRoot.Set(root, indices);
            }

            indices.Add(index);
        }

        return indicesByRoot;
    }

    // A component's minimum contribution to the Hamming distance: how many of the
    // target values at those positions the component's own source multiset cannot
    // already supply. Every value it can supply is placeable by some sequence of
    // allowed swaps, so nothing beyond the shortfall has to be paid for.
    private static int UnsuppliedCountWithHashMap(List<int> indices, ValueArrays values)
    {
        var availableCounts = new HashMap<int, int>();

        foreach (var index in indices)
        {
            availableCounts.TryGetValue(values.Source[index], out var count);
            availableCounts.Set(values.Source[index], count + 1);
        }

        var distance = 0;

        foreach (var index in indices)
        {
            if (availableCounts.TryGetValue(values.Target[index], out var count) && count > 0)
            {
                availableCounts.Set(values.Target[index], count - 1);
            }
            else
            {
                distance++;
            }
        }

        return distance;
    }

    // The same shortfall count as UnsuppliedCountWithHashMap, over a BCL Dictionary,
    // so the baseline arm stays what you would write without this repo.
    private static int UnsuppliedCountWithDictionary(List<int> indices, ValueArrays values)
    {
        var availableCounts = new Dictionary<int, int>();

        foreach (var index in indices)
        {
            availableCounts.TryGetValue(values.Source[index], out var count);
            availableCounts[values.Source[index]] = count + 1;
        }

        var distance = 0;

        foreach (var index in indices)
        {
            if (availableCounts.TryGetValue(values.Target[index], out var count) && count > 0)
            {
                availableCounts[values.Target[index]] = count - 1;
            }
            else
            {
                distance++;
            }
        }

        return distance;
    }

    // The two arrays travel together everywhere below the entry points; naming the
    // pair keeps the component helpers from growing a fifth and sixth parameter.
    private readonly record struct ValueArrays(int[] Source, int[] Target);
}
