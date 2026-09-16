using DSAExperimentation.DataStructures.Graph.Grids;

namespace DSAExperimentation.Tests.DataStructures.Graph.Grids;

public sealed partial class GridChildrenTests
{
    private static Grid Open(int rows, int cols)
    {
        var passable = new bool[rows, cols];

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                passable[r, c] = true;
            }
        }

        return new Grid(passable);
    }

    // The top-left corner of an open 3x3 grid. Its up and left neighbours are off-grid, so
    // only down and right are passable and they take the first two entries of the index
    // space.
    private static GridChildren TopLeftCornerChildren()
    {
        var grid = Open(3, 3);

        return new GridChildren(new GridNode(0, 0, grid));
    }

    [Fact]
    public void Count_InteriorCell_HasFourNeighbours()
    {
        var grid = Open(3, 3);

        Assert.Equal(4, new GridChildren(new GridNode(1, 1, grid)).Count);
    }

    [Fact]
    public void Count_CornerCell_HasTwoNeighbours()
    {
        var children = TopLeftCornerChildren();

        Assert.Equal(2, children.Count);
    }

    [Fact]
    public void Count_ImpassableNeighboursAreExcluded()
    {
        var passable = new bool[1, 3];
        passable[0, 0] = true;
        passable[0, 1] = true;
        // (0,2) is a wall.

        Assert.Equal(1, new GridChildren(new GridNode(0, 1, new Grid(passable))).Count);
    }

    [Fact]
    public void Get_YieldsNeighboursInUpDownLeftRightOrder()
    {
        var grid = Open(3, 3);
        var children = new GridChildren(new GridNode(1, 1, grid));

        var coordinates = Enumerable.Range(0, children.Count)
            .Select(i => (children.Get(i).Row, children.Get(i).Col));

        Assert.Equal([(0, 1), (2, 1), (1, 0), (1, 2)], coordinates);
    }

    [Fact]
    public void Get_CompactsTheIndexSpaceOverBlockedDirections()
    {
        var children = TopLeftCornerChildren();

        // Up and left are off-grid, so down and right occupy indices 0 and 1.
        Assert.Equal((1, 0), (children.Get(0).Row, children.Get(0).Col));
        Assert.Equal((0, 1), (children.Get(1).Row, children.Get(1).Col));
    }

    [Fact]
    public void Get_PastTheLastPassableNeighbour_Throws()
    {
        var children = TopLeftCornerChildren();

        // Only down and right are passable here, so index 2 is past the end.
        Assert.Throws<IndexOutOfRangeException>(() => children.Get(2));
    }

    [Fact]
    public void Count_FullyWalledCell_IsZero()
    {
        var passable = new bool[1, 1];
        passable[0, 0] = true;

        Assert.Equal(0, new GridChildren(new GridNode(0, 0, new Grid(passable))).Count);
    }
}
