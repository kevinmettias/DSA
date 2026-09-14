using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.LeetCode.CountTheNumberOfCompleteComponents;

// LeetCode 2685. Count the Number of Complete Components: in an undirected graph
// of n nodes, count the connected components in which every pair of nodes is
// directly connected.
//
// Both strategies rest on the same restatement - a component of k nodes is
// complete exactly when it holds k*(k-1)/2 edges - and differ only in how they
// decide which nodes share a component, and therefore in whether "complete" costs
// a pairwise membership test or a counter increment.
internal static class CountTheNumberOfCompleteComponentsSolution
{
    // The textbook answer, and the arm the composed solution has to justify
    // itself against: build adjacency sets, collect each component by BFS, then
    // check completeness the way the problem states it - every unordered pair of
    // the component's nodes must appear in the other's adjacency set, an O(k^2)
    // scan per component. Deliberately all BCL (ARCHITECTURE.md 17.5).
    public static int CountCompleteComponentsByAdjacencySetScan(int n, int[][] edges)
    {
        var adjacency = BuildAdjacency(n, edges);
        var visited = new bool[n];
        var complete = 0;

        for (var start = 0; start < n; start++)
        {
            if (visited[start])
            {
                continue;
            }

            var component = CollectComponentByBfs(start, adjacency, visited);

            if (IsCompleteByPairwiseScan(component, adjacency))
            {
                complete++;
            }
        }

        return complete;
    }

    private static List<HashSet<int>> BuildAdjacency(int n, int[][] edges)
    {
        var adjacency = new List<HashSet<int>>(n);

        for (var i = 0; i < n; i++)
        {
            adjacency.Add([]);
        }

        foreach (var edge in edges)
        {
            adjacency[edge[0]].Add(edge[1]);
            adjacency[edge[1]].Add(edge[0]);
        }

        return adjacency;
    }

    private static List<int> CollectComponentByBfs(int start, List<HashSet<int>> adjacency, bool[] visited)
    {
        var component = new List<int>();
        var queue = new Queue<int>();
        queue.Enqueue(start);
        visited[start] = true;

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            component.Add(node);

            foreach (var neighbor in adjacency[node])
            {
                if (!visited[neighbor])
                {
                    visited[neighbor] = true;
                    queue.Enqueue(neighbor);
                }
            }
        }

        return component;
    }

    private static bool IsCompleteByPairwiseScan(List<int> component, List<HashSet<int>> adjacency)
    {
        for (var i = 0; i < component.Count; i++)
        {
            for (var j = i + 1; j < component.Count; j++)
            {
                if (!adjacency[component[i]].Contains(component[j]))
                {
                    return false;
                }
            }
        }

        return true;
    }

    // This repo's DisjointSet already answers "which component is this node in"
    // in near-constant time, so no adjacency structure is needed at all: union
    // every edge once, tally each root's nodes and edges in two linear sweeps,
    // and compare the two counts. The k*(k-1)/2 identity replaces every pairwise
    // membership test with one multiplication per component.
    public static int CountCompleteComponentsByDisjointSetTally(int n, int[][] edges)
    {
        var components = new DisjointSet(n);

        foreach (var edge in edges)
        {
            components.Union(edge[0], edge[1]);
        }

        var nodeCount = new int[n];
        var edgeCount = new int[n];

        for (var node = 0; node < n; node++)
        {
            nodeCount[components.Find(node)]++;
        }

        foreach (var edge in edges)
        {
            edgeCount[components.Find(edge[0])]++;
        }

        return CountCompleteRoots(components, nodeCount, edgeCount);
    }

    // Only a root carries its component's tallies, so every other node is skipped
    // rather than double-counted.
    private static int CountCompleteRoots(DisjointSet components, int[] nodeCount, int[] edgeCount)
    {
        var complete = 0;

        for (var node = 0; node < components.Count; node++)
        {
            if (components.Find(node) == node && edgeCount[node] == nodeCount[node] * (nodeCount[node] - 1) / 2)
            {
                complete++;
            }
        }

        return complete;
    }
}
