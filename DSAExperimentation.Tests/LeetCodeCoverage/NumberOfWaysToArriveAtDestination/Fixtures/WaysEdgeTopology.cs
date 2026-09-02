using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfWaysToArriveAtDestination.Fixtures;

internal readonly struct WaysEdgeTopology : IEdgeTopology<WaysNode, ListEdges<WaysNode, long>, long>
{
    public static ListEdges<WaysNode, long> GetEdges(WaysNode node) => new(node.Edges);
}
