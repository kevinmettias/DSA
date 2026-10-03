
// One node per ORIGINAL graph vertex - never per subdivision node, which is the whole point of
// LC 882's composed strategy: the subdivision chain along an edge is represented by that edge's
// weight (cnt + 1 unit moves), not by cnt extra objects.
using SubdividedGraphNode = DSAExperimentation.DataStructures.Graph.Adjacency.WeightedAdjacencyNode<int>;

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

    public static SubdividedGraph Build(int nodeCount, int[][] edges)
    {
        var nodes = new SubdividedGraphNode[nodeCount];

        for (var i = 0; i < nodeCount; i++)
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
