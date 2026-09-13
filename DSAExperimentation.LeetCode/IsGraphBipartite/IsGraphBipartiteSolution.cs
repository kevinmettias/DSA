using DSAExperimentation.Algorithms.Bipartiteness;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.IsGraphBipartite;

// LeetCode 785. Is Graph Bipartite?: can the vertices of an undirected graph,
// given as a symmetric adjacency list, be split into two sets so that every edge
// crosses between them?
//
// The question is 2-colorability, so both strategies are a greedy coloring walk
// that fails the moment an edge joins two same-colored vertices. The graph may be
// disconnected, so both restart from every still-uncolored vertex.
internal static class IsGraphBipartiteSolution
{
    // Uncolored; the two colors are +1 and -1 so flipping is a negation.
    private const sbyte Uncolored = 0;
    private const sbyte FirstColor = 1;

    // The textbook answer: an sbyte[] color array and an explicit Stack<int>
    // walking the problem's own int[][] adjacency iteratively, depth first.
    // Deliberately written without this repo's primitives - it is the arm the
    // composed solution below has to justify itself against. LeetCode's input is
    // already the prepared shape here, so this strategy needs no hoisted overload.
    public static bool IsBipartiteByColorArrayDfs(int[][] graph)
    {
        var color = new sbyte[graph.Length];
        var stack = new Stack<int>();

        for (var start = 0; start < graph.Length; start++)
        {
            if (color[start] != Uncolored)
            {
                continue;
            }

            color[start] = FirstColor;
            stack.Push(start);

            if (!TryColorComponent(graph, stack, color))
            {
                return false;
            }
        }

        return true;
    }

    private static bool TryColorComponent(int[][] graph, Stack<int> stack, sbyte[] color)
    {
        while (stack.Count > 0)
        {
            var node = stack.Pop();

            if (!TryColorNeighbors(graph, stack, color, node))
            {
                return false;
            }
        }

        return true;
    }

    private static bool TryColorNeighbors(int[][] graph, Stack<int> stack, sbyte[] color, int node)
    {
        foreach (var neighbor in graph[node])
        {
            if (color[neighbor] == color[node])
            {
                return false;
            }

            if (color[neighbor] == Uncolored)
            {
                color[neighbor] = (sbyte)-color[node];
                stack.Push(neighbor);
            }
        }

        return true;
    }

    // This repo's own answer: Algorithms.Bipartiteness.BipartiteCheck is already a
    // multi-root BFS 2-coloring over any IGraphTopology, so the problem reduces to
    // materializing the adjacency as a BipartiteNode graph and asking it - the same
    // "graph + repo traversal algorithm" composition
    // OpenTheLockSolution.MinTurnsByReduceGraph uses for LC 752.
    public static bool IsBipartiteByBipartiteCheck(int[][] graph)
    {
        var nodes = BipartiteGraph.Build(graph);

        return IsBipartiteByBipartiteCheck(nodes);
    }

    public static bool IsBipartiteByBipartiteCheck(BipartiteGraph graph) =>
        BipartiteCheck.IsBipartite<
            BipartiteNode, BipartiteTopology, ListChildren<BipartiteNode>,
            NaturalChildOrder<BipartiteNode, ListChildren<BipartiteNode>>, ListChildren<BipartiteNode>>(
            graph.Nodes);
}
