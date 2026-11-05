using DSAExperimentation.DataStructures.Graph.Grids;
using DSAExperimentation.Domain.Mathematics.Lattices;

namespace DSAExperimentation.Tests.DataStructures.Graph.Grids;

public sealed partial class GridTests
{
    // Open cells of TwoByThree, one per row plus the far corner of the first.
    public static TheoryData<int, int> OpenCells =>
        new()
        {
            { 0, 0 },
            { 0, 2 },
            { 1, 1 },
        };

    // One step outside each edge of TwoByThree. The bounds check runs before the array is
    // indexed, so these answer false rather than throwing - which is what lets a neighbour
    // scan probe past the border without guarding each step itself.
    public static TheoryData<int, int> OutOfBoundsCells =>
        new()
        {
            { -1, 0 },
            { 2, 0 },
            { 0, -1 },
            { 0, 3 },
        };

    // Two rows by three columns - deliberately not square, so a Rows/Cols swap or a
    // transposed index shows up. The only blocked cell is (1, 2).
    private static Grid TwoByThree() => new(new[,]
    {
        { true, true, true },
        { true, true, false },
    });

    [Fact]
    public void Rows_IsTheFirstDimensionOfThePassableMap() => Assert.Equal(2, TwoByThree().Rows);

    [Fact]
    public void Cols_IsTheSecondDimensionOfThePassableMap() => Assert.Equal(3, TwoByThree().Cols);

    [Theory]
    [MemberData(nameof(OpenCells))]
    public void IsPassable_OpenCellInBounds_ReturnsTrue(int row, int col)
        => Assert.True(TwoByThree().IsPassable(row, col));

    [Fact]
    public void IsPassable_BlockedCell_ReturnsFalse() => Assert.False(TwoByThree().IsPassable(1, 2));

    [Theory]
    [MemberData(nameof(OutOfBoundsCells))]
    public void IsPassable_OutOfBounds_ReturnsFalse(int row, int col)
        => Assert.False(TwoByThree().IsPassable(row, col));

    [Fact]
    public void Size_IsRowsByColsOfThePassableMap() => Assert.Equal(new GridSize(2, 3), TwoByThree().Size);

    [Fact]
    public void Size_AllOpenGrid_IsTheRowsAndColsItWasGiven() => Assert.Equal(new GridSize(2, 3), new Grid(2, 3).Size);

    [Fact]
    public void IsPassable_AllOpenGrid_EveryCellIsOpen()
    {
        var grid = new Grid(2, 3);

        Assert.All(
            Enumerable.Range(0, 2).SelectMany(row => Enumerable.Range(0, 3).Select(col => (Row: row, Col: col))),
            cell => Assert.True(grid.IsPassable(cell.Row, cell.Col)));
    }

    [Theory]
    [MemberData(nameof(OutOfBoundsCells))]
    public void IsPassable_AllOpenGridOutOfBounds_ReturnsFalse(int row, int col)
        => Assert.False(new Grid(2, 3).IsPassable(row, col));

    // The filter opens a cell exactly when its column is even: (0,0), (0,2), (1,0) and (1,2).
    [Fact]
    public void Where_Filter_OpensExactlyTheCellsItAccepts()
    {
        var grid = Grid.Where(new GridSize(2, 3), new EvenColumns());

        Assert.Equal(
            [true, false, true, true, false, true],
            Enumerable.Range(0, 2).SelectMany(row => Enumerable.Range(0, 3).Select(col => grid.IsPassable(row, col))));
    }

    [Fact]
    public void Where_Filter_SizesTheGridAsGiven() => Assert.Equal(new GridSize(2, 3), Grid.Where(new GridSize(2, 3), new EvenColumns()).Size);

    [Fact]
    public void PassableNodes_ListsTheOpenCellsRowByRow()
        => Assert.Equal([(0, 0), (0, 1), (0, 2), (1, 0), (1, 1)], TwoByThree().PassableNodes<NodePerStep>().Select(node => (node.Row, node.Col)));

    [Fact]
    public void PassableNodes_EachNodeIsOfThisGrid()
    {
        var grid = TwoByThree();

        Assert.All(grid.PassableNodes<NodePerStep>(), node => Assert.Same(grid, node.Grid));
    }

    [Fact]
    public void PassableNodes_NoOpenCell_IsEmpty() => Assert.Empty(new Grid(new bool[2, 3]).PassableNodes<NodePerStep>());

    [Fact]
    public void PassableNodes_NodePerCell_AreTheBoardsOwnNodes()
    {
        var grid = TwoByThree();

        Assert.All(grid.PassableNodes<NodePerCell>(), node => Assert.Same(grid.NodeAt(node.Row, node.Col), node));
    }

    [Fact]
    public void PassableNodes_NodePerStep_AreNotTheBoardsOwnNodes()
    {
        var grid = TwoByThree();

        Assert.All(grid.PassableNodes<NodePerStep>(), node => Assert.NotSame(grid.NodeAt(node.Row, node.Col), node));
    }

    [Fact]
    public void NodeAt_SameCellTwice_IsTheSameNode()
    {
        var grid = TwoByThree();

        Assert.Same(grid.NodeAt(1, 2), grid.NodeAt(1, 2));
    }

    // Every cell of one board asked for in turn, so a table slot shared by two cells would hand the
    // second cell the first one's node.
    [Fact]
    public void NodeAt_EveryCellOfOneBoard_IsThatCellsNodeOnThisBoard()
    {
        var grid = new Grid(2, 3);
        var cells = Enumerable.Range(0, grid.Rows * grid.Cols).Select(i => (Row: i / grid.Cols, Col: i % grid.Cols)).ToList();

        Assert.Equal(cells.Select(cell => new GridNode(cell.Row, cell.Col, grid)), cells.Select(cell => grid.NodeAt(cell.Row, cell.Col)));
    }

    [Theory]
    [MemberData(nameof(OutOfBoundsCells))]
    public void NodeAt_OffTheBoard_Throws(int row, int col)
    {
        var grid = TwoByThree();

        Assert.Throws<ArgumentOutOfRangeException>(() => grid.NodeAt(row, col));
    }

    private readonly struct EvenColumns : IGridCellFilter
    {
        public bool CanEnter(int row, int col) => col % 2 == 0;
    }

    [Fact]
    public void Steps_ABoardGivenNone_AreTheOrthogonalSteps() =>
        Assert.Equal(GridDirections.Orthogonal.ToArray(), new Grid(2, 3).Steps.ToArray());

    [Fact]
    public void Steps_ABoardBuiltWithTheOctilinearSteps_KeepsThem() =>
        Assert.Equal(GridDirections.Octilinear.ToArray(), new Grid(new bool[2, 3], GridDirections.Octilinear).Steps.ToArray());

    // GridChildren marks each step in one bit of a 64-bit mask, so a board of more steps is refused.
    [Fact]
    public void Steps_MoreThanAMaskHolds_AreRefused() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new Grid(new bool[1, 1], GridDirections.WithinChebyshev(4)));
}
