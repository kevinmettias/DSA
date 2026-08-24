using DSAExperimentation.Graph;

namespace DSAExperimentation.Tests;

public readonly struct WeightedTopology : IEdgeTopology<WeightedNode, ListEdges<WeightedNode, int>, int>
{
    public static ListEdges<WeightedNode, int> GetEdges(WeightedNode node) => new(node.Edges);
}
