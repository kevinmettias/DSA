namespace DSAExperimentation.LeetCode.FindEdgesInShortestPaths;

// The full n-node weighted graph, built once from LeetCode's own edges array -
// the domain model, not an answer to any one query about it (BranchNetwork's
// own framing for LC 2959). Edges is kept alongside Nodes so the solution can
// walk the input's own edge order when it builds the per-edge answer array,
// without a second pass over the raw input.
internal sealed class EdgeGraph
{
    private EdgeGraph(EdgeGraphNode[] nodes, int[][] edges)
    {
        Nodes = nodes;
        Edges = edges;
    }

    public EdgeGraphNode[] Nodes { get; }

    public int[][] Edges { get; }

    public static EdgeGraph Build(int n, int[][] edges)
    {
        var nodes = new EdgeGraphNode[n];

        for (var i = 0; i < n; i++)
        {
            nodes[i] = new EdgeGraphNode(i);
        }

        foreach (var edge in edges)
        {
            var (a, b, weight) = (edge[0], edge[1], (long)edge[2]);
            nodes[a].Edges.Add((weight, nodes[b]));
            nodes[b].Edges.Add((weight, nodes[a]));
        }

        return new EdgeGraph(nodes, edges);
    }
}
