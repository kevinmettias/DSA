namespace DSAExperimentation.LeetCode.ReachableNodesInSubdividedGraph;

// LC 882's ORIGINAL n-node graph, weighted so that one edge's weight is the number
// of unit moves its subdivision chain actually costs (cnt subdivision nodes plus the
// final hop onto the far endpoint = cnt + 1). The subdivided graph itself is never
// materialized. Edges is kept alongside Nodes so the per-edge subdivision arithmetic
// can walk the input's own edge list without a second pass over the raw input - the
// same framing EdgeGraph uses for LC 3123.
internal sealed class SubdividedGraph
{
    public SubdividedGraphNode[] Nodes { get; }

    public int[][] Edges { get; }

    private SubdividedGraph(SubdividedGraphNode[] nodes, int[][] edges)
    {
        Nodes = nodes;
        Edges = edges;
    }

    public static SubdividedGraph Build(int n, int[][] edges)
    {
        var nodes = new SubdividedGraphNode[n];

        for (var i = 0; i < n; i++)
        {
            nodes[i] = new SubdividedGraphNode(i);
        }

        foreach (var edge in edges)
        {
            var (u, v, cnt) = (edge[0], edge[1], edge[2]);
            var weight = cnt + 1;
            nodes[u].Edges.Add((weight, nodes[v]));
            nodes[v].Edges.Add((weight, nodes[u]));
        }

        return new SubdividedGraph(nodes, edges);
    }
}
