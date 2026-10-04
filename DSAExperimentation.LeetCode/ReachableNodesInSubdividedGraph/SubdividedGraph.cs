
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
        var nodes = LeetCodeAdjacency.ZeroBased<SubdividedGraphNode>(
            nodeCount, edges, id => new SubdividedGraphNode(id), (node, _, farNode, edgeIndex) => node.Edges.Add((ChainMoves(edges[edgeIndex]), farNode)));

        return new SubdividedGraph(nodes, edges);
    }

    // An edge's cnt subdivision nodes, plus the final hop onto its far endpoint.
    private static int ChainMoves(int[] edge) => edge[2] + 1;
}
