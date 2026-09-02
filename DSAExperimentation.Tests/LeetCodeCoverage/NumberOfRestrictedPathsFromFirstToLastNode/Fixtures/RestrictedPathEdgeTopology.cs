using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfRestrictedPathsFromFirstToLastNode.Fixtures;

internal readonly struct RestrictedPathEdgeTopology
    : IEdgeTopology<RestrictedPathNode, ListEdges<RestrictedPathNode, int>, int>
{
    public static ListEdges<RestrictedPathNode, int> GetEdges(RestrictedPathNode node) => new(node.Edges);
}
