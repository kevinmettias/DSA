
// One node of LC 2360's graph, holding the single successor edges[i] names - or no successor at
// all when edges[i] is -1, which is the one way this differs from LC 2127's
// out-degree-exactly-one EmployeeNode. Neighbors is a list rather than a dedicated
// at-most-one-child contract because ListChildren already reads a List<T>, and the out-degree
// bound is a caller discipline FunctionalGraph.Build enforces rather than something the type
// itself states.
using FunctionalGraphNode = DSAExperimentation.DataStructures.Graph.Adjacency.AdjacencyNode;

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
                nodes[i].Neighbors.Add(nodes[edges[i]]);
            }
        }

        return new FunctionalGraph(nodes);
    }
}
