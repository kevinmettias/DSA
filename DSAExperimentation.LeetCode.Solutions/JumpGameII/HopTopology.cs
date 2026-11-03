using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.Solutions.JumpGameII;

internal readonly struct HopTopology : IGraphTopology<HopNode, ListChildren<HopNode>>
{
    public static ListChildren<HopNode> GetChildren(HopNode node) => new(node.Targets);
}
