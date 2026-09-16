using DSAExperimentation.DataStructures.Graph.Grids;

namespace DSAExperimentation.Tests.DataStructures.Graph.Grids;

public sealed partial class WeightedGridTopologyTests
{
    [Fact]
    public void GetEdges_ExposesTheNodesOwnWeightedEdges()
    {
        var nodes = WeightedGrid.Build(2, 2);

        var edges = WeightedGridTopology.GetEdges(nodes[(0, 0)]);

        Assert.Equal(2, edges.Count);
    }

    [Fact]
    public void GetEdges_CellWithNoNeighbours_ReturnsNone()
    {
        var nodes = WeightedGrid.Build(1, 1);

        Assert.Equal(0, WeightedGridTopology.GetEdges(nodes[(0, 0)]).Count);
    }
}
