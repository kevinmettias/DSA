using DSAExperimentation.Graph.Contracts.Ordering;
using DSAExperimentation.Graph.Contracts.Topologies;

namespace DSAExperimentation.Tests.Algorithms.ShortestPaths.Fixtures;

internal readonly struct WeightedGridTopology : IEdgeTopology<WeightedGridNode, ListEdges<WeightedGridNode, int>, int>
{
    public static ListEdges<WeightedGridNode, int> GetEdges(WeightedGridNode node) => new(node.Edges);
}
