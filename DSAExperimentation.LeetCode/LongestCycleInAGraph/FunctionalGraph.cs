namespace DSAExperimentation.LeetCode.LongestCycleInAGraph;

// LC 2360's edges[] reshaped once into the node form the Tarjan strategy walks.
// Being a named type rather than a bare List<FunctionalGraphNode> is what keeps
// the hoisted overload unambiguous against LeetCode's own int[] shape
// (ARCHITECTURE.md #17.4) - it is not an IEnumerable, so the two overloads can
// never both bind - and it lets a benchmark charge construction to [GlobalSetup]
// instead of to the search being measured. Same role EmployeeGraph plays for LC
// 2127.
internal readonly record struct FunctionalGraph(List<FunctionalGraphNode> Nodes)
{
    public static FunctionalGraph Build(int[] edges)
    {
        var nodes = Enumerable.Range(0, edges.Length).Select(id => new FunctionalGraphNode(id)).ToList();

        for (var i = 0; i < edges.Length; i++)
        {
            if (edges[i] != FunctionalGraphEdges.NoOutgoingEdge)
            {
                nodes[i].Successors.Add(nodes[edges[i]]);
            }
        }

        return new FunctionalGraph(nodes);
    }
}
