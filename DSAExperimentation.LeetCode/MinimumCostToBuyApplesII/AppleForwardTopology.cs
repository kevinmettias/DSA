using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.MinimumCostToBuyApplesII;

// The IEdgeTopology witness for the outbound (empty-handed) leg of a trip -
// Algorithms.ShortestPaths.ShortestPath.Dijkstra runs over this to find, from a
// source shop, the cheapest way to REACH every other shop before buying.
internal readonly struct AppleForwardTopology : IEdgeTopology<AppleNode, ListEdges<AppleNode, long>, long>
{
    public static ListEdges<AppleNode, long> GetEdges(AppleNode node) => new(node.ForwardEdges);
}
