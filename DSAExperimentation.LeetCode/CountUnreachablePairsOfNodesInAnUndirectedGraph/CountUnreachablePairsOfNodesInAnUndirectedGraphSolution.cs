using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.CountUnreachablePairsOfNodesInAnUndirectedGraph;

// LeetCode 2316. Count Unreachable Pairs of Nodes in an Undirected Graph: how many
// unordered pairs of nodes have no path between them.
//
// Neither strategy searches for reachability pair-by-pair. Every pair is either
// inside one connected component (reachable) or split across two (unreachable), so
// once each component's SIZE is known the answer is C(n,2) minus the reachable
// pairs summed component-by-component. The two strategies differ only in how they
// discover those sizes: a flood fill over an adjacency list, or union-find.
internal static class CountUnreachablePairsOfNodesInAnUndirectedGraphSolution
{
    // The textbook answer: build an adjacency list, flood fill each unvisited node
    // with an explicit stack, and size the component as you go. Deliberately BCL
    // throughout - it is the arm the composed solution has to justify itself
    // against.
    public static long CountPairsByDepthFirstFloodFill(int n, int[][] edges)
    {
        var adjacency = BuildAdjacency(n, edges);
        var visited = new bool[n];
        var reachablePairs = 0L;

        for (var node = 0; node < n; node++)
        {
            if (visited[node])
            {
                continue;
            }

            var size = FloodFill(node, adjacency, visited);
            reachablePairs += PairsWithin(size);
        }

        return PairsWithin(n) - reachablePairs;
    }

    private static int[][] BuildAdjacency(int n, int[][] edges)
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

        var result = new int[n][];

        for (var node = 0; node < n; node++)
        {
            result[node] = [.. adjacency[node]];
        }

        return result;
    }

    private static long FloodFill(int start, int[][] adjacency, bool[] visited)
    {
        var stack = new Stack<int>();
        stack.Push(start);
        visited[start] = true;
        var size = 0L;

        while (stack.Count > 0)
        {
            var node = stack.Pop();
            size++;

            foreach (var next in adjacency[node])
            {
                if (!visited[next])
                {
                    visited[next] = true;
                    stack.Push(next);
                }
            }
        }

        return size;
    }

    // This repo's own answer: union every edge into DisjointSet (the same
    // composition NumberOfProvinces and NumberOfOperationsToMakeNetworkConnected
    // use), then tally each component's size in a HashMap keyed by root. Reading
    // the sizes back through HashMap.Values makes each distinct component
    // contribute exactly once, so no second pass over the nodes is needed.
    public static long CountPairsByDisjointSet(int n, int[][] edges)
    {
        var components = new DisjointSet(n);

        foreach (var edge in edges)
        {
            components.Union(edge[0], edge[1]);
        }

        var sizeByRoot = new HashMap<int, long>();

        for (var node = 0; node < n; node++)
        {
            var root = components.Find(node);
            sizeByRoot.TryGetValue(root, out var size);
            sizeByRoot.Set(root, size + 1);
        }

        var reachablePairs = 0L;

        foreach (var size in sizeByRoot.Values)
        {
            reachablePairs += PairsWithin(size);
        }

        return PairsWithin(n) - reachablePairs;
    }

    // C(size, 2) - the unordered pairs available inside a group of this size. n can
    // reach 1e5, so the total is well past int range and every count stays long.
    private static long PairsWithin(long size) => size * (size - 1) / 2;
}
