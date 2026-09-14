using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.AllAncestorsOfANodeInADirectedAcyclicGraph;

// Bare IGraphTopology rather than IDagTopology: the problem promises acyclicity,
// but TopologicalSort.TrySort is the arm that would catch a violation anyway, and
// nothing here needs the stronger tier's fold guarantees.
internal readonly struct AncestorTopology : IGraphTopology<AncestorNode, ListChildren<AncestorNode>>
{
    public static ListChildren<AncestorNode> GetChildren(AncestorNode node) => new(node.Children);
}
