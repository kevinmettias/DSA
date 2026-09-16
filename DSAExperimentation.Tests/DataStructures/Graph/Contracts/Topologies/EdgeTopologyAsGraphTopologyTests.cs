using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;
using DSAExperimentation.DataStructures.Graph.Grids;

namespace DSAExperimentation.Tests.DataStructures.Graph.Contracts.Topologies;

public sealed class EdgeTopologyAsGraphTopologyTests
{
    private static WeightedGridNode Cell(int row, int col) => new(row, col);

    [Fact]
    public void GetChildren_ProjectsAnEdgeTopologyOntoItsTargetsAlone()
    {
        var nodes = WeightedGrid.Build(2, 2);

        var children = EdgeTopologyAsGraphTopology<
            WeightedGridNode, WeightedGridTopology, ListEdges<WeightedGridNode, int>, int>.GetChildren(nodes[(0, 0)]);

        Assert.Equal(2, children.Count);
        Assert.Equal(
            [(0, 1), (1, 0)],
            Enumerable.Range(0, children.Count).Select(i => (children.Get(i).Row, children.Get(i).Col)).OrderBy(t => t));
    }

    [Fact]
    public void GetChildren_CountMatchesTheUnderlyingEdgeCount()
    {
        var nodes = WeightedGrid.Build(3, 3);
        var node = nodes[(1, 1)];

        var children = EdgeTopologyAsGraphTopology<
            WeightedGridNode, WeightedGridTopology, ListEdges<WeightedGridNode, int>, int>.GetChildren(node);

        Assert.Equal(WeightedGridTopology.GetEdges(node).Count, children.Count);
    }

    [Fact]
    public void GetChildren_NodeWithNoEdges_ReturnsNone()
    {
        var node = Cell(9, 9);

        var children = EdgeTopologyAsGraphTopology<
            WeightedGridNode, WeightedGridTopology, ListEdges<WeightedGridNode, int>, int>.GetChildren(node);

        Assert.Equal(0, children.Count);
    }
}
