using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ShortestPathVisitingAllNodes.Fixtures;

internal readonly struct VisitStateTopology : IGraphTopology<VisitStateNode, ListChildren<VisitStateNode>>
{
    public static ListChildren<VisitStateNode> GetChildren(VisitStateNode node) => new(node.Neighbors);
}
