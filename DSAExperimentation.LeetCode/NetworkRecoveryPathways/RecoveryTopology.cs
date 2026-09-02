using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.NetworkRecoveryPathways;

// The IEdgeTopology witness Algorithms.ShortestPaths.ShortestPath.Dijkstra needs
// to run over RecoveryNode - the same one-line shape WeightedGridTopology and
// TransportTopology already use for their own weighted nodes.
internal readonly struct RecoveryTopology : IEdgeTopology<RecoveryNode, ListEdges<RecoveryNode, long>, long>
{
    public static ListEdges<RecoveryNode, long> GetEdges(RecoveryNode node) => new(node.Edges);
}
