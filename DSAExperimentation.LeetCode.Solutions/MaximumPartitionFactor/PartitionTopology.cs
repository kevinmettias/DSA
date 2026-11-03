using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.Solutions.MaximumPartitionFactor;

// The "too close" conflict graph is undirected and, at some thresholds, cyclic -
// nothing here promises acyclicity, matching tier 1's StateTopology's own
// general (non-tree) IGraphTopology witness.
internal readonly struct PartitionTopology : IGraphTopology<PartitionNode, ListChildren<PartitionNode>>
{
    public static ListChildren<PartitionNode> GetChildren(PartitionNode node) => new(node.Neighbors);
}
