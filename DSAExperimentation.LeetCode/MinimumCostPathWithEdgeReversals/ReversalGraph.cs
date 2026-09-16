namespace DSAExperimentation.LeetCode.MinimumCostPathWithEdgeReversals;

// The augmented digraph over the whole input node set, built once from LeetCode's
// own edges array - the domain model, not an answer to any one query about it
// (EdgeGraph's own framing for LC 3123). Every input edge (u, v, w) contributes
// both a forward hop u -> v at cost w and a reversed hop v -> u at cost 2w, so the
// puzzle's per-node switch is already folded into the graph's shape by the time a
// search runs over it.
internal sealed class ReversalGraph
{
    public ReversalGraphNode[] Nodes { get; }

    private ReversalGraph(ReversalGraphNode[] nodes) => Nodes = nodes;

    public static ReversalGraph Build(int nodeCount, int[][] edges)
    {
        var nodes = new ReversalGraphNode[nodeCount];

        for (var i = 0; i < nodeCount; i++)
        {
            nodes[i] = new ReversalGraphNode(i);
        }

        foreach (var edge in edges)
        {
            var (u, v, weight) = (edge[0], edge[1], (long)edge[2]);
            nodes[u].Edges.Add((weight, nodes[v]));
            nodes[v].Edges.Add((2 * weight, nodes[u]));
        }

        return new ReversalGraph(nodes);
    }
}
