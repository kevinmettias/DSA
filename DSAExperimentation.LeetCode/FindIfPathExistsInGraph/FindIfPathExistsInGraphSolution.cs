using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.LeetCode.FindIfPathExistsInGraph;

// LeetCode 1971. Find if Path Exists in Graph: given a bi-directional graph on n
// nodes described by an edge list, is there any path at all from source to
// destination?
//
// The question asks nothing about the path itself - only whether the two nodes sit
// in the same connected component - so the two strategies differ in whether they
// answer it by walking (a reachability search that must exhaust source's whole
// component before it can say "no") or by partitioning (union every edge once, then
// compare two roots).
internal static class FindIfPathExistsInGraphSolution
{
    // The endpoints' slots in LeetCode's own two-element edge array.
    private const int From = 0;
    private const int To = 1;

    // The textbook answer: materialize an adjacency list and run an iterative
    // depth-first reachability search from source, entirely with BCL collections -
    // iterative rather than recursive purely to stay overflow-safe on a deep chain.
    // It is the arm the union-find strategy below has to justify itself against.
    public static bool HasPathByDepthFirstSearch(int nodeCount, int[][] edges, int source, int destination)
    {
        var adjacency = BuildAdjacency(nodeCount, edges);
        var visited = new bool[nodeCount];
        var stack = new Stack<int>();

        stack.Push(source);
        visited[source] = true;
        TraverseDepthFirst(adjacency, visited, stack);

        return visited[destination];
    }

    private static List<int>[] BuildAdjacency(int nodeCount, int[][] edges)
    {
        var adjacency = new List<int>[nodeCount];

        for (var id = 0; id < nodeCount; id++)
        {
            adjacency[id] = [];
        }

        foreach (var edge in edges)
        {
            adjacency[edge[From]].Add(edge[To]);
            adjacency[edge[To]].Add(edge[From]);
        }

        return adjacency;
    }

    private static void TraverseDepthFirst(List<int>[] adjacency, bool[] visited, Stack<int> stack)
    {
        while (stack.Count > 0)
        {
            var node = stack.Pop();

            foreach (var next in adjacency[node])
            {
                if (!visited[next])
                {
                    visited[next] = true;
                    stack.Push(next);
                }
            }
        }
    }

    // This repo's own DisjointSet: path existence in an undirected graph is plain
    // connectivity, so unioning every edge and asking IsConnected answers the
    // question with no traversal at all - and, unlike the search above, without ever
    // materializing a second copy of the adjacency (the same union-per-edge shape
    // NumberOfProvinces and NumberOfOperationsToMakeNetworkConnected use).
    public static bool HasPathByDisjointSet(int nodeCount, int[][] edges, int source, int destination)
    {
        var components = new DisjointSet(nodeCount);

        foreach (var edge in edges)
        {
            components.Union(edge[From], edge[To]);
        }

        return components.IsConnected(source, destination);
    }
}
