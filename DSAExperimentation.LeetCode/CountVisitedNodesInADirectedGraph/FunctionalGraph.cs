namespace DSAExperimentation.LeetCode.CountVisitedNodesInADirectedGraph;

// LC 2876's edges[] reshaped once into the node form the Tarjan strategy walks,
// keeping the original array alongside it because the reverse-BFS half of that
// strategy reads successors backwards and an int[] is the cheapest way to invert
// them.
//
// Being a named type rather than a bare List<FunctionalGraphNode> is what keeps the
// hoisted overload unambiguous against LeetCode's own int[] shape
// (ARCHITECTURE.md #17.4) - it is not an IEnumerable, so the two overloads can
// never both bind - and it lets a benchmark charge construction to [GlobalSetup]
// instead of to the search being measured.
internal readonly record struct FunctionalGraph(int[] Edges, List<FunctionalGraphNode> Nodes)
{
    public static FunctionalGraph Build(int[] edges)
    {
        var nodes = new List<FunctionalGraphNode>(edges.Length);

        for (var id = 0; id < edges.Length; id++)
        {
            nodes.Add(new FunctionalGraphNode(id));
        }

        for (var id = 0; id < edges.Length; id++)
        {
            nodes[id].Successors.Add(nodes[edges[id]]);
        }

        return new FunctionalGraph(edges, nodes);
    }
}
