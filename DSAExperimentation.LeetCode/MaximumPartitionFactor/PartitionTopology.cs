using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.MaximumPartitionFactor;

// The "too close" conflict graph is undirected and, at some thresholds, cyclic -
// nothing here promises acyclicity, matching Domain.Locks.LockTopology's own
// general (non-tree) IGraphTopology witness.
internal readonly struct PartitionTopology : IGraphTopology<PartitionNode, ListChildren<PartitionNode>>
{
    public static ListChildren<PartitionNode> GetChildren(PartitionNode node) => new(node.Neighbors);
}
