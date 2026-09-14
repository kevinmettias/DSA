using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.MinimumCostToReachDestinationInTime;

// The IEdgeTopology witness Algorithms.ShortestPaths.ShortestPath.Dijkstra needs to
// run over TimeCityNode - the same one-line shape FlightStateTopology and
// ProbabilityTopology already use for their own weighted nodes.
internal readonly struct TimeCityEdgeTopology
    : IEdgeTopology<TimeCityNode, ListEdges<TimeCityNode, int>, int>
{
    public static ListEdges<TimeCityNode, int> GetEdges(TimeCityNode node) => new(node.Edges);
}
