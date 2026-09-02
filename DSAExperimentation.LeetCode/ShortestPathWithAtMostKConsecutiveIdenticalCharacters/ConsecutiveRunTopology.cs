using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.ShortestPathWithAtMostKConsecutiveIdenticalCharacters;

// The IEdgeTopology witness Algorithms.ShortestPaths.ShortestPath.Dijkstra needs
// to run over ConsecutiveRunNode - the same one-line shape RecoveryTopology
// already uses for its own weighted, eagerly-wired nodes.
internal readonly struct ConsecutiveRunTopology
    : IEdgeTopology<ConsecutiveRunNode, ListEdges<ConsecutiveRunNode, long>, long>
{
    public static ListEdges<ConsecutiveRunNode, long> GetEdges(ConsecutiveRunNode node) => new(node.Edges);
}
