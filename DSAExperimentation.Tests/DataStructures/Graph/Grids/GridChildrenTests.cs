using DSAExperimentation.DataStructures.Graph.Grids;
using DSAExperimentation.Domain.Mathematics.Lattices;

namespace DSAExperimentation.Tests.DataStructures.Graph.Grids;

public sealed partial class GridChildrenTests
{
    // The top-left corner of an open 3x3 grid. Its up and left neighbours are off-grid, so
    // only down and right are passable and they take the first two entries of the index
    // space.
    private static GridChildren<NodePerStep> TopLeftCornerChildren()
    {
        var grid = new Grid(3, 3);

        return new GridChildren<NodePerStep>(new GridNode(0, 0, grid));
    }

    [Fact]
    public void Count_InteriorCell_HasFourNeighbours()
    {
        var grid = new Grid(3, 3);

        Assert.Equal(4, new GridChildren<NodePerStep>(new GridNode(1, 1, grid)).Count);
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

        Assert.Equal(1, new GridChildren<NodePerStep>(new GridNode(0, 1, new Grid(passable))).Count);
    }

    [Fact]
    public void Get_YieldsNeighboursInUpDownLeftRightOrder()
    {
        var grid = new Grid(3, 3);
        var children = new GridChildren<NodePerStep>(new GridNode(1, 1, grid));

        var coordinates = Enumerable.Range(0, children.Count)
            .Select(i => (children.Get(i).Row, children.Get(i).Col));

        Assert.Equal([(0, 1), (2, 1), (1, 0), (1, 2)], coordinates);
    }

    // A neighbour reached from two cells is the board's one node for it, not a copy per step.
    [Fact]
    public void Get_NodePerCell_IsTheBoardsOwnNodeForTheNeighbour()
    {
        var grid = new Grid(3, 3);
        var (fromLeft, fromAbove) = CentreReachedTwoWays<NodePerCell>(grid);

        Assert.Same(grid.NodeAt(1, 1), fromLeft);
        Assert.Same(fromLeft, fromAbove);
    }

    // The centre of a 3 x 3 board reached two ways: as (1, 0)'s third open neighbour (up, down,
    // right) and as (0, 1)'s first (down).
    private static (GridNode FromLeft, GridNode FromAbove) CentreReachedTwoWays<TNodes>(Grid grid)
        where TNodes : struct, IGridNodeSource
        => (new GridChildren<TNodes>(grid.NodeAt(1, 0)).Get(2), new GridChildren<TNodes>(grid.NodeAt(0, 1)).Get(0));

    // Each step makes its own node for the centre: equal to the other step's, and a different object.
    [Fact]
    public void Get_NodePerStep_IsANewEqualNodeEachStep()
    {
        var (fromLeft, fromAbove) = CentreReachedTwoWays<NodePerStep>(new Grid(3, 3));

        Assert.Equal(fromLeft, fromAbove);
        Assert.NotSame(fromLeft, fromAbove);
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

        Assert.Equal(0, new GridChildren<NodePerStep>(new GridNode(0, 0, new Grid(passable))).Count);
    }

    [Fact]
    public void Get_OctilinearBoard_YieldsEveryNeighbourInTheTablesOrder()
    {
        var grid = new Grid(new bool[3, 3] { { true, true, true }, { true, true, true }, { true, true, true } }, GridDirections.Octilinear);
        var children = new GridChildren<NodePerStep>(new GridNode(1, 1, grid));
        var neighbours = Enumerable.Range(0, children.Count).Select(index => children.Get(index)).Select(node => (node.Row - 1, node.Col - 1));

        Assert.Equal(GridDirections.Octilinear.Select(step => (step.DeltaRow, step.DeltaCol)), neighbours);
    }

    [Fact]
    public void Count_OctilinearBoardCorner_HasThreeNeighbours()
    {
        var grid = new Grid(new bool[3, 3] { { true, true, true }, { true, true, true }, { true, true, true } }, GridDirections.Octilinear);

        Assert.Equal(3, new GridChildren<NodePerStep>(new GridNode(0, 0, grid)).Count);
    }
}
