using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.MinimumTimeToReachTargetWithLimitedPower;

// The IEdgeTopology witness Algorithms.ShortestPaths.ShortestPath.Dijkstra needs
// to run over PowerStateNode - the same one-line shape RecoveryTopology already
// uses for its own weighted, eagerly-wired nodes.
internal readonly struct PowerStateTopology
    : IEdgeTopology<PowerStateNode, ListEdges<PowerStateNode, long>, long>
{
    public static ListEdges<PowerStateNode, long> GetEdges(PowerStateNode node) => new(node.Edges);
}
