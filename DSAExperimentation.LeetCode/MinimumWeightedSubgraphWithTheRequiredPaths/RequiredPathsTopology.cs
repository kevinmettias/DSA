using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.MinimumWeightedSubgraphWithTheRequiredPaths;

// The IEdgeTopology witness Algorithms.ShortestPaths.ShortestPath.Dijkstra needs to
// run over RequiredPathsNode - the same one-line shape EdgeGraphTopology already
// uses for LC 3123's own weighted node. One witness serves both orientations of the
// graph, since a reversed edge is still just an entry in some node's Edges list.
internal readonly struct RequiredPathsTopology
    : IEdgeTopology<RequiredPathsNode, ListEdges<RequiredPathsNode, long>, long>
{
    public static ListEdges<RequiredPathsNode, long> GetEdges(RequiredPathsNode node) => new(node.Edges);
}
