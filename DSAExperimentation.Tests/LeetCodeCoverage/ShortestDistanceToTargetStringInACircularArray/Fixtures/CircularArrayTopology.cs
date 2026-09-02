using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ShortestDistanceToTargetStringInACircularArray.Fixtures;

internal readonly struct CircularArrayTopology : IGraphTopology<CircularArrayNode, ListChildren<CircularArrayNode>>
{
    public static ListChildren<CircularArrayNode> GetChildren(CircularArrayNode node) => new(node.Neighbors);
}
