using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.MinimumCostOfAPathWithSpecialRoads;

internal readonly struct SpecialRoadTopology
    : IEdgeTopology<SpecialRoadNode, ListEdges<SpecialRoadNode, int>, int>
{
    public static ListEdges<SpecialRoadNode, int> GetEdges(SpecialRoadNode node) => new(node.Edges);
}
