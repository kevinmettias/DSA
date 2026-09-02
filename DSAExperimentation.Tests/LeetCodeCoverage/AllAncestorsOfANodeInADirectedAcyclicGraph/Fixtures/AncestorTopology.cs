using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Tests.LeetCodeCoverage.AllAncestorsOfANodeInADirectedAcyclicGraph.Fixtures;

internal readonly struct AncestorTopology : IGraphTopology<AncestorNode, ListChildren<AncestorNode>>
{
    public static ListChildren<AncestorNode> GetChildren(AncestorNode node) => new(node.Children);
}
