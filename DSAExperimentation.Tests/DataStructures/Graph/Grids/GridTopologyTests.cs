using DSAExperimentation.DataStructures.Graph.Grids;

namespace DSAExperimentation.Tests.DataStructures.Graph.Grids;

public sealed partial class GridTopologyTests
{
    private static Grid OpenThreeByThree()
    {
        var passable = new bool[3, 3];

        for (var r = 0; r < 3; r++)
        {
            for (var c = 0; c < 3; c++)
            {
                passable[r, c] = true;
            }
        }

        return new Grid(passable);
    }

    [Fact]
    public void GetChildren_ComputesNeighboursFromTheNodesOwnGridReference()
    {
        var node = new GridNode(1, 1, OpenThreeByThree());

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
