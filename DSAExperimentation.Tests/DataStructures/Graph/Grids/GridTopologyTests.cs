using DSAExperimentation.DataStructures.Graph.Grids;

namespace DSAExperimentation.Tests.DataStructures.Graph.Grids;

public sealed partial class GridTopologyTests
{
    [Fact]
    public void GetChildren_ComputesNeighboursFromTheNodesOwnGridReference()
    {
        var node = new GridNode(1, 1, new Grid(3, 3));

        Assert.Equal(4, GridTopology.GetChildren(node).Count);
    }

    [Fact]
    public void GetChildren_WalledOffCell_ReturnsNone()
    {
        var passable = new bool[1, 1];
        passable[0, 0] = true;

        Assert.Equal(0, GridTopology.GetChildren(new GridNode(0, 0, new Grid(passable))).Count);
    }
}
