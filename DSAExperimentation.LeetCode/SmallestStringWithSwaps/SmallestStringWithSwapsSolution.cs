using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.LeetCode.SmallestStringWithSwaps;

// LeetCode 1202. Smallest String With Swaps: each pair [a, b] may be swapped any
// number of times, so indices reachable from one another through a chain of pairs
// form a connected component and any permutation within a component is achievable.
// The lexicographically smallest result therefore sorts each component's own
// characters into that component's own sorted index slots, independently.
//
// Both strategies answer the same question - "which indices share a component" -
// over the string's own index positions; they differ only in how the components are
// discovered. This is the same shape as LexicographicallySmallestEquivalentString
// (LC 1061), over index positions instead of the 26 letters.
internal static class SmallestStringWithSwapsSolution
{
    // The textbook answer: materialize an adjacency list over the indices and BFS out
    // of each unvisited index to collect its component. Deliberately BCL-only - it is
    // the arm the DisjointSet composition below has to justify itself against.
    public static string SmallestStringByAdjacencyListBfs(string s, int[][] pairs)
    {
        var adjacency = BuildAdjacencyList(s.Length, pairs);
        var visited = new bool[s.Length];
        var result = s.ToCharArray();

        for (var start = 0; start < s.Length; start++)
        {
            AssignComponentFromStart(start, s, adjacency, new ComponentScan(visited, result));
        }

        return new string(result);
    }

    private static List<int>[] BuildAdjacencyList(int length, int[][] pairs)
    {
        var adjacency = new List<int>[length];

        for (var index = 0; index < length; index++)
        {
            adjacency[index] = [];
        }

        foreach (var pair in pairs)
        {
            adjacency[pair[0]].Add(pair[1]);
            adjacency[pair[1]].Add(pair[0]);
        }

        return adjacency;
    }

    // BFSes out of `start` (unless it was already reached from an earlier component),
    // then writes that component's sorted characters back into the scan's result
    // buffer at the component's own ascending positions.
    private static void AssignComponentFromStart(int start, string s, List<int>[] adjacency, ComponentScan scan)
    {
        if (scan.Visited[start])
        {
            return;
        }

        var component = CollectComponent(start, adjacency, scan.Visited);
        component.Sort();
        AssignSortedChars(s, scan.Result, component);
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

    private readonly record struct ComponentScan(bool[] Visited, char[] Result);

    // This repo's own DisjointSet over the string's index positions: O(1) amortized
    // Union per swap pair and O(a(n)) Find per index, with no adjacency list and no
    // per-component BFS queue allocated at all - one pass groups indices by root and
    // one pass writes each group's sorted characters back.
    public static string SmallestStringByDisjointSet(string s, int[][] pairs)
    {
        var components = BuildComponents(s.Length, pairs);
        var groups = GroupIndicesByRoot(components, s.Length);
        var result = s.ToCharArray();

        foreach (var indices in groups.Values)
        {
            AssignSortedChars(s, result, indices);
        }

        return new string(result);
    }

    private static DisjointSet BuildComponents(int length, int[][] pairs)
    {
        var components = new DisjointSet(length);

        foreach (var pair in pairs)
        {
            components.Union(pair[0], pair[1]);
        }

        return components;
    }

    // Indices are visited in ascending order, so each root's list is already ascending
    // - the precondition AssignSortedChars relies on.
    private static Dictionary<int, List<int>> GroupIndicesByRoot(DisjointSet components, int length)
    {
        var groups = new Dictionary<int, List<int>>();

        for (var index = 0; index < length; index++)
        {
            var root = components.Find(index);

            if (!groups.TryGetValue(root, out var indices))
            {
                indices = [];
                groups[root] = indices;
            }

            indices.Add(index);
        }

        return groups;
    }

    // Writes the lexicographically sorted characters found at `positions` (assumed
    // ascending) back into `result` at those same positions.
    private static void AssignSortedChars(string s, char[] result, List<int> positions)
    {
        var sortedChars = positions.Select(position => s[position]).OrderBy(character => character).ToArray();

        for (var slot = 0; slot < positions.Count; slot++)
        {
            result[positions[slot]] = sortedChars[slot];
        }
    }
}
