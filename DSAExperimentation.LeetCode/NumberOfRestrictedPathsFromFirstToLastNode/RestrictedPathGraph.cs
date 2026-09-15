using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.NumberOfRestrictedPathsFromFirstToLastNode;

// The undirected weighted graph LeetCode's own (n, edges) input describes, with
// every node already labelled by its distance to the last node - the one Dijkstra
// run both counting strategies share (EdgeGraph's own "build the model once, answer
// queries about it afterwards" framing for LC 3123).
//
// Labelling is part of building the model rather than part of either count: the
// "restricted" rule is stated in terms of these distances, so a node without its
// Dist is not yet this problem's graph.
internal sealed class RestrictedPathGraph
{
    // The endpoints' and the weight's slots in LeetCode's own three-element edge array.
    private const int From = 0;
    private const int To = 1;
    private const int Weight = 2;

    // Node 1, where every restricted path starts.
    public RestrictedPathNode First { get; }

    private RestrictedPathGraph(RestrictedPathNode first) => First = first;

    public static RestrictedPathGraph Build(int n, int[][] edges)
    {
        var nodes = BuildNodes(n, edges);

        AssignDistancesFromLastNode(nodes);

        return new RestrictedPathGraph(nodes[0]);
    }

    // LeetCode labels the nodes 1..n; slot i holds node i + 1.
    private static RestrictedPathNode[] BuildNodes(int n, int[][] edges)
    {
        var nodes = new RestrictedPathNode[n];

        for (var id = 1; id <= n; id++)
        {
            nodes[id - 1] = new RestrictedPathNode(id);
        }

        foreach (var edge in edges)
        {
            var (from, to, weight) = (edge[From], edge[To], edge[Weight]);
            nodes[from - 1].Edges.Add((weight, nodes[to - 1]));
            nodes[to - 1].Edges.Add((weight, nodes[from - 1]));
        }

        return nodes;
    }

    // Every edge is undirected, so a single Dijkstra sourced at the last node gives
    // every other node its distance *to* that node in one pass. LeetCode guarantees
    // the graph is connected, so every node is reached.
    private static void AssignDistancesFromLastNode(RestrictedPathNode[] nodes)
    {
        var distances = ShortestPath.Dijkstra<
            RestrictedPathNode, RestrictedPathEdgeTopology, ListEdges<RestrictedPathNode, int>, int>(nodes[^1]);

        foreach (var node in nodes)
        {
            node.Dist = distances[node];
        }
    }
}
