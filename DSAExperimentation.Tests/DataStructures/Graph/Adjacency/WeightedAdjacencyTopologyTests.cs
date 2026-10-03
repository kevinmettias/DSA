using DSAExperimentation.DataStructures.Graph.Adjacency;

namespace DSAExperimentation.Tests.DataStructures.Graph.Adjacency;

public sealed partial class WeightedAdjacencyTopologyTests
{
    private const int LighterWeight = 3;
    private const long HeavierWeight = 5_000_000_000;

    [Fact]
    public void GetEdges_ExposesEachEdgesWeightAndTargetInOrder()
    {
        var (source, near, far) = (new WeightedAdjacencyNode<int>(0), new WeightedAdjacencyNode<int>(1), new WeightedAdjacencyNode<int>(2));
        source.Edges.Add((LighterWeight, near));
        source.Edges.Add((LighterWeight + 1, far));

        var edges = WeightedAdjacencyTopology<int>.GetEdges(source);

        Assert.Equal([(LighterWeight, near), (LighterWeight + 1, far)], Enumerable.Range(0, edges.Count).Select(edges.Get));
    }

    // The weight type is the caller's: a long weight past int's range survives unchanged.
    [Fact]
    public void GetEdges_LongWeight_KeepsTheFullValue()
    {
        var (source, target) = (new WeightedAdjacencyNode<long>(0), new WeightedAdjacencyNode<long>(1));
        source.Edges.Add((HeavierWeight, target));

        Assert.Equal(HeavierWeight, WeightedAdjacencyTopology<long>.GetEdges(source).Get(0).Data);
    }

    [Fact]
    public void GetEdges_NodeWithNoOutEdges_ReturnsNoEdges() =>
        Assert.Equal(0, WeightedAdjacencyTopology<int>.GetEdges(new WeightedAdjacencyNode<int>(0)).Count);
}
