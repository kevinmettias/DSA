using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.LeetCode.FindTheCityWithTheSmallestNumberOfNeighborsAtAThresholdDistance;

// The IEdgeTopology witness both Algorithms.ShortestPaths entry points this
// problem uses - ShortestPath.Dijkstra and AllPairsShortestPaths.TryComputeDistances
// - need in order to walk CityNode, the same one-line shape EdgeGraphTopology
// already uses for LC 3123's own weighted node.
internal readonly struct CityTopology : IEdgeTopology<CityNode, ListEdges<CityNode, int>, int>
{
    public static ListEdges<CityNode, int> GetEdges(CityNode node) => new(node.Edges);
}
