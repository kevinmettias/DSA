
// One node per graph vertex; Edges holds both directions of every undirected edge from the
// input, weighted by long since a path's total cost can exceed int range (n and w are each up
// to 5*10^4/1e5).
using EdgeGraphNode = DSAExperimentation.DataStructures.Graph.Adjacency.WeightedAdjacencyNode<long>;

namespace DSAExperimentation.LeetCode.FindEdgesInShortestPaths;

// The full weighted graph over nodeCount nodes, built once from LeetCode's own
// edges array - the domain model, not an answer to any one query about it
// (BranchNetwork's own framing for LC 2959). Edges is kept alongside Nodes so the
// solution can walk the input's own edge order when it builds the per-edge answer
// array, without a second pass over the raw input.
internal sealed class EdgeGraph
{
    public EdgeGraphNode[] Nodes { get; }

    public int[][] Edges { get; }

    private EdgeGraph(EdgeGraphNode[] nodes, int[][] edges)
    {
        Nodes = nodes;
        Edges = edges;
    }

    // An EdgeGraphNode per vertex id, then both directions of every edge, each beside
    // the edge's weight widened to long - LeetCodeAdjacency's layout, with the weight
    // read from the edge's third value.
    public static EdgeGraph Build(int nodeCount, int[][] edges)
    {
        var nodes = LeetCodeAdjacency.ZeroBased<EdgeGraphNode>(
            nodeCount, edges, id => new EdgeGraphNode(id), (node, _, farNode, edgeIndex) => node.Edges.Add(((long)edges[edgeIndex][2], farNode)));

        return new EdgeGraph(nodes, edges);
    }
}
