using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.NumberOfOperationsToMakeNetworkConnected;

// LeetCode 1319. Number of Operations to Make Network Connected: how many cables
// have to be moved before every computer can reach every other one.
//
// Both strategies answer that the same way - count the connected components and
// report componentCount - 1, since one spare cable buys exactly one merge - and
// differ only in how the components are counted: a textbook DFS flood fill over a
// freshly built adjacency list, or this repo's own DisjointSet unioning every
// connection. Neither can do anything at all when there are fewer than n-1 cables
// for n computers, which is checked before either counting pass begins.
internal static class NumberOfOperationsToMakeNetworkConnectedSolution
{
    private const int From = 0;
    private const int To = 1;

    // The textbook answer: build the adjacency list, then flood fill from every
    // unvisited computer and count how many fills it took. Deliberately written
    // without this repo's primitives - it is the arm the DisjointSet below has to
    // justify itself against.
    public static int MakeConnectedByDepthFirstFloodFill(int n, int[][] connections)
    {
        if (!HasEnoughCables(n, connections))
        {
            return LeetCodeAnswer.None;
        }

        var adjacency = BuildAdjacency(n, connections);
        var visited = new bool[n];
        var components = 0;

        for (var computer = 0; computer < n; computer++)
        {
            if (visited[computer])
            {
                continue;
            }

            Visit(computer, adjacency, visited);
            components++;
        }

        return ComponentsToOperations(components);
    }

    private static int[][] BuildAdjacency(int n, int[][] connections)
    {
        var neighbors = new List<int>[n];

        for (var computer = 0; computer < n; computer++)
        {
            neighbors[computer] = [];
        }

        foreach (var connection in connections)
        {
            neighbors[connection[From]].Add(connection[To]);
            neighbors[connection[To]].Add(connection[From]);
        }

        var adjacency = new int[n][];

        for (var computer = 0; computer < n; computer++)
        {
            adjacency[computer] = [.. neighbors[computer]];
        }

        return adjacency;
    }

    private static void Visit(int computer, int[][] adjacency, bool[] visited)
    {
        visited[computer] = true;

        foreach (var neighbor in adjacency[computer])
        {
            if (!visited[neighbor])
            {
                Visit(neighbor, adjacency, visited);
            }
        }
    }

    // This repo's own DisjointSet: every connection is a Union, so the components
    // are already tracked by the time the walk over the connections ends and the
    // count is just how many distinct roots the n computers report - collected in
    // this repo's own Set<int>, the same composition NumberOfProvinces uses.
    public static int MakeConnectedByDisjointSet(int n, int[][] connections)
    {
        if (!HasEnoughCables(n, connections))
        {
            return LeetCodeAnswer.None;
        }

        var components = new DisjointSet(n);

        foreach (var connection in connections)
        {
            components.Union(connection[From], connection[To]);
        }

        var roots = new Set<int>();

        for (var computer = 0; computer < n; computer++)
        {
            roots.TryAdd(components.Find(computer));
        }

        return ComponentsToOperations(roots.Count);
    }

    // n computers need at least n-1 cables to be connected at all, however they
    // are currently arranged - no amount of moving creates a cable.
    private static bool HasEnoughCables(int n, int[][] connections) =>
        connections.Length >= n - 1;

    // One spare cable merges exactly two components, so joining c components takes
    // c-1 moves - and the cable count above already guarantees the spares exist.
    private static int ComponentsToOperations(int components) => components - 1;
}
