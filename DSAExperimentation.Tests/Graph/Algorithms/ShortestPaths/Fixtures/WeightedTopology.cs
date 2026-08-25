using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Graph.Contracts.Topologies;

namespace DSAExperimentation.Tests.Graph.Algorithms.ShortestPaths.Fixtures;

internal readonly struct WeightedTopology : IEdgeTopology<WeightedNode, ListEdges<WeightedNode, int>, int>
{
    public static ListEdges<WeightedNode, int> GetEdges(WeightedNode node) => new(node.Edges);
}
