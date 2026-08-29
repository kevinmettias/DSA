namespace DSAExperimentation.Benchmarks.Fixtures;

// Mirrors DSAExperimentation.Tests' WeightedNode fixture: edges stored directly on
// the node so ListEdges<TNode,TEdgeData> can back IEdgeTopology.GetEdges with zero
// extra lookup structure.
internal sealed class WeightedGraphNode(int id)
{
    public int Id { get; } = id;

    public List<(int Weight, WeightedGraphNode Target)> Edges { get; } = [];
}
