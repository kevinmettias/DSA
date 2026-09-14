using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.NumberOfWaysToArriveAtDestination;

// The IEdgeTopology witness Algorithms.ShortestPaths.ShortestPath.Dijkstra needs to
// run over WaysNode's weighted adjacency, so WaysGraph can label every intersection
// with its shortest travel time to the destination.
internal readonly struct WaysEdgeTopology : IEdgeTopology<WaysNode, ListEdges<WaysNode, long>, long>
{
    public static ListEdges<WaysNode, long> GetEdges(WaysNode node) => new(node.Edges);
}
