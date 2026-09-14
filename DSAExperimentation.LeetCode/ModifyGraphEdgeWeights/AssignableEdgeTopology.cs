using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.ModifyGraphEdgeWeights;

// The IEdgeTopology witness ShortestPath.Dijkstra needs in order to walk
// AssignableEdgeNode. It reads the node's edge list live rather than snapshotting
// it, so every reassignment made between two searches is visible to the next one.
internal readonly struct AssignableEdgeTopology
    : IEdgeTopology<AssignableEdgeNode, ListEdges<AssignableEdgeNode, int>, int>
{
    public static ListEdges<AssignableEdgeNode, int> GetEdges(AssignableEdgeNode node) => new(node.Edges);
}
