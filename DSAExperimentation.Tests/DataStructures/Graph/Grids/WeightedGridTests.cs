using DSAExperimentation.DataStructures.Graph.Grids;

namespace DSAExperimentation.Tests.DataStructures.Graph.Grids;

public sealed class WeightedGridTests
{
    [Fact]
    public void Build_ProducesOneNodePerCellKeyedByItsCoordinate()
    {
        var nodes = WeightedGrid.Build(3, 4);

        Assert.Equal(12, nodes.Count);
        Assert.All(nodes, entry => Assert.Equal(entry.Key, (entry.Value.Row, entry.Value.Col)));
    }

    [Fact]
    public void Build_GivesInteriorCellsFourEdgesAndCornersTwo()
    {
        var nodes = WeightedGrid.Build(3, 3);

        Assert.Equal(4, nodes[(1, 1)].Edges.Count);
        Assert.Equal(2, nodes[(0, 0)].Edges.Count);
        Assert.Equal(3, nodes[(0, 1)].Edges.Count);
    }

    [Fact]
    public void Build_ConnectsOrthogonallyOnlyNeverDiagonally()
    {
        var nodes = WeightedGrid.Build(2, 2);

        var targets = nodes[(0, 0)].Edges.Select(e => (e.Target.Row, e.Target.Col)).OrderBy(t => t);

        Assert.Equal([(0, 1), (1, 0)], targets);
    }

    [Fact]
    public void Build_UsesUnitWeightsThroughout()
    {
        var nodes = WeightedGrid.Build(3, 3);

        Assert.All(nodes.Values, node => Assert.All(node.Edges, edge => Assert.Equal(1, edge.Weight)));
    }

    [Fact]
    public void Build_OmitsWalledCellsEntirelyRatherThanIsolatingThem()
    {
        var nodes = WeightedGrid.Build(3, 3, new HashSet<(int Row, int Col)> { (1, 1) });

        Assert.Equal(8, nodes.Count);
        Assert.False(nodes.ContainsKey((1, 1)));
    }

    [Fact]
    public void Build_NoEdgeEverPointsAtAWalledCell()
    {
        var nodes = WeightedGrid.Build(3, 3, new HashSet<(int Row, int Col)> { (1, 1) });

        Assert.All(nodes.Values, node => Assert.All(node.Edges,
            edge => Assert.NotEqual((1, 1), (edge.Target.Row, edge.Target.Col))));

        Assert.Equal(2, nodes[(1, 0)].Edges.Count);
    }

    [Fact]
    public void Build_SingleCell_HasNoEdges()
    {
        var nodes = WeightedGrid.Build(1, 1);

        Assert.Single(nodes);
        Assert.Empty(nodes[(0, 0)].Edges);
    }
}
