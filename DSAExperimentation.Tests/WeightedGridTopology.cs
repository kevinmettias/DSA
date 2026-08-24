using DSAExperimentation.Graph;

namespace DSAExperimentation.Tests;

public readonly struct WeightedGridTopology : IEdgeTopology<WeightedGridNode, ListEdges<WeightedGridNode, int>, int>
{
    public static ListEdges<WeightedGridNode, int> GetEdges(WeightedGridNode node) => new(node.Edges);
}
