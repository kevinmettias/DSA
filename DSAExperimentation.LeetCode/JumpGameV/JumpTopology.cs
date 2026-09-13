using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.JumpGameV;

// The IGraphTopology witness TopologicalSort.TrySort needs in order to walk
// JumpNode's reachability edges.
internal readonly struct JumpTopology : IGraphTopology<JumpNode, ListChildren<JumpNode>>
{
    public static ListChildren<JumpNode> GetChildren(JumpNode node) => new(node.ReachableIndices);
}
