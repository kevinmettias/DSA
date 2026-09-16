namespace DSAExperimentation.LeetCode.FindEdgesInShortestPaths;

// The full weighted graph over nodeCount nodes, built once from LeetCode's own
// edges array - the domain model, not an answer to any one query about it
// (BranchNetwork's own framing for LC 2959). Edges is kept alongside Nodes so the
// solution can walk the input's own edge order when it builds the per-edge answer
// array, without a second pass over the raw input.
// without a second pass over the raw input.
internal sealed class EdgeGraph
{
    public EdgeGraphNode[] Nodes { get; }

    public int[][] Edges { get; }

    private EdgeGraph(EdgeGraphNode[] nodes, int[][] edges)
    {
        Nodes = nodes;
        Edges = edges;
    }

    public static EdgeGraph Build(int nodeCount, int[][] edges)
    {
        var nodes = new EdgeGraphNode[nodeCount];

        for (var i = 0; i < nodeCount; i++)
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
