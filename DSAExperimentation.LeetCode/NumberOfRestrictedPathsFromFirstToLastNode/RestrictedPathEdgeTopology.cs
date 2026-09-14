using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.NumberOfRestrictedPathsFromFirstToLastNode;

// The IEdgeTopology witness Algorithms.ShortestPaths.ShortestPath.Dijkstra needs to
// run over RestrictedPathNode's weighted adjacency, so RestrictedPathGraph can label
// every node with its distance to the last one.
internal readonly struct RestrictedPathEdgeTopology
    : IEdgeTopology<RestrictedPathNode, ListEdges<RestrictedPathNode, int>, int>
{
    public static ListEdges<RestrictedPathNode, int> GetEdges(RestrictedPathNode node) => new(node.Edges);
}
