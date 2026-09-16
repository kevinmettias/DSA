using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for WeightedGraphTopology (ARCHITECTURE 17.7): the pattern-matching witness that lets the
// shortest-path strategies walk this fixture's WeightedGraphNode through IEdgeTopology. Its one job is to
// present a node's own edge list unchanged - same edges, same weights, same targets, same order - so the
// strategies measure the graph the fixture built rather than a reordering of it.
public sealed partial class WeightedGraphTopologyTests
{
    private const int SourceId = 1;
    private const int FirstTargetId = 9;
    private const int SecondTargetId = 4;
    private const int FirstWeight = 3;
    private const int SecondWeight = 7;
    private const int NoEdges = 0;

    [Fact]
    public void GetEdges_NodeWithTwoEdges_PresentsBothWithTheirOwnWeightAndTarget()
    {
        var firstTarget = new WeightedGraphNode(FirstTargetId);
        var secondTarget = new WeightedGraphNode(SecondTargetId);
        var node = new WeightedGraphNode(SourceId);
        node.Edges.Add((FirstWeight, firstTarget));
        node.Edges.Add((SecondWeight, secondTarget));

        var edges = WeightedGraphTopology.GetEdges(node);

        Assert.Equal(node.Edges.Count, edges.Count);
        Assert.Equal((FirstWeight, firstTarget), edges.Get(0));
        Assert.Equal((SecondWeight, secondTarget), edges.Get(1));
    }

    [Fact]
    public void GetEdges_NodeWithNoEdges_PresentsNoEdges() =>
        Assert.Equal(NoEdges, WeightedGraphTopology.GetEdges(new WeightedGraphNode(SourceId)).Count);
}
