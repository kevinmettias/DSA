using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.FindEventualSafeStates;

internal readonly struct SafeStateTopology : IGraphTopology<SafeStateNode, ListChildren<SafeStateNode>>
{
    public static ListChildren<SafeStateNode> GetChildren(SafeStateNode node) => new(node.Predecessors);
}
