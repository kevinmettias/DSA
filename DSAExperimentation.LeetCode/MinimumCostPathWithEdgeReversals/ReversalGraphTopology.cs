using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.MinimumCostPathWithEdgeReversals;

// The IEdgeTopology witness Algorithms.ShortestPaths.ShortestPath.Dijkstra needs to
// run over ReversalGraphNode - the same one-line shape EdgeGraphTopology already
// uses for LC 3123's own weighted node.
internal readonly struct ReversalGraphTopology
    : IEdgeTopology<ReversalGraphNode, ListEdges<ReversalGraphNode, long>, long>
{
    public static ListEdges<ReversalGraphNode, long> GetEdges(ReversalGraphNode node) => new(node.Edges);
}
