using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumCostToReachDestinationInTime.Fixtures;

internal readonly struct TimeCityEdgeTopology
    : IEdgeTopology<TimeCityNode, ListEdges<TimeCityNode, int>, int>
{
    public static ListEdges<TimeCityNode, int> GetEdges(TimeCityNode node) => new(node.Edges);
}
