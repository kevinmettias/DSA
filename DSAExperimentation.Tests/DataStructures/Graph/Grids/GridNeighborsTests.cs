using DSAExperimentation.DataStructures.Graph.Grids;

namespace DSAExperimentation.Tests.DataStructures.Graph.Grids;

public sealed partial class GridNeighborsTests
{
    private static readonly GridSize ThreeByThree = new(3, 3);

    [Fact]
    public void Of_InteriorCell_YieldsEveryNeighbourInTheTablesOrder() =>
        Assert.Equal(
            [(0, 1), (2, 1), (1, 0), (1, 2)],
            GridNeighbors.Of((1, 1), ThreeByThree, GridDirections.Orthogonal, default(EveryCell)));

    [Fact]
    public void Of_CornerCell_LeavesOutTheCellsOffTheGrid() =>
        Assert.Equal(
            [(1, 0), (0, 1)],
            GridNeighbors.Of((0, 0), ThreeByThree, GridDirections.Orthogonal, default(EveryCell)));

    [Fact]
    public void Of_CellTheFilterRefuses_IsLeftOut() =>
        Assert.Equal(
            [(0, 1), (2, 1), (1, 2)],
            GridNeighbors.Of((1, 1), ThreeByThree, GridDirections.Orthogonal, new EveryCellBut((1, 0))));

    [Fact]
    public void Of_KingTable_YieldsTheEightNeighboursRowMajor() =>
        Assert.Equal(
            [(0, 0), (0, 1), (0, 2), (1, 0), (1, 2), (2, 0), (2, 1), (2, 2)],
            GridNeighbors.Of((1, 1), ThreeByThree, GridDirections.King, default(EveryCell)));

    // A one-row board: only left and right exist, so a Rows/Cols swap in the bounds check shows up.
    [Fact]
    public void Of_NonSquareGrid_RespectsBothDimensions() =>
        Assert.Equal(
            [(0, 0), (0, 2)],
            GridNeighbors.Of((0, 1), new GridSize(1, 3), GridDirections.Orthogonal, default(EveryCell)));

    // The guarantee that lets a filter index its grid unguarded: a corner probes four offsets,
    // and only the two that land on the board reach the filter.
    [Fact]
    public void Of_CanEnter_IsAskedOnlyAboutCellsOnTheGrid()
    {
        var asked = new List<(int Row, int Col)>();

        _ = GridNeighbors.Of((0, 0), ThreeByThree, GridDirections.Orthogonal, new RecordingFilter(asked)).ToList();

        Assert.Equal([(1, 0), (0, 1)], asked);
    }

    private readonly struct EveryCell : IGridCellFilter
    {
        public bool CanEnter(int row, int col) => true;
    }

    private readonly struct EveryCellBut((int Row, int Col) refused) : IGridCellFilter
    {
        public bool CanEnter(int row, int col) => (row, col) != refused;
    }

    // Holds a reference, so what it records survives the scan's copy of it.
    private readonly struct RecordingFilter(List<(int Row, int Col)> asked) : IGridCellFilter
    {
        public bool CanEnter(int row, int col)
        {
            asked.Add((row, col));
            return true;
        }
    }
}
