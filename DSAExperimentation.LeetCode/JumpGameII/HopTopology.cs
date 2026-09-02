using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.JumpGameII;

internal readonly struct HopTopology : IEdgeTopology<HopNode, ListEdges<HopNode, int>, int>
{
    public static ListEdges<HopNode, int> GetEdges(HopNode node) => new(node.Edges);
}
