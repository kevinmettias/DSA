using DSAExperimentation.DataStructures.Graph.Grids;

namespace DSAExperimentation.Tests.DataStructures.Graph.Grids;

public sealed partial class GridSizeTests
{
    // The four corners of TwoByThree.
    public static TheoryData<int, int> Corners =>
        new()
        {
            { 0, 0 },
            { 0, 2 },
            { 1, 0 },
            { 1, 2 },
        };

    // One step outside each edge of TwoByThree.
    public static TheoryData<int, int> OneStepOffEachEdge =>
        new()
        {
            { -1, 0 },
            { 2, 0 },
            { 0, -1 },
            { 0, 3 },
        };

    // Two rows by three columns - deliberately not square, so a Rows/Cols swap shows up.
    private static readonly GridSize TwoByThree = new(2, 3);

    [Theory]
    [MemberData(nameof(Corners))]
    public void HasCell_CellOnTheGrid_ReturnsTrue(int row, int col) => Assert.True(TwoByThree.HasCell(row, col));

    [Theory]
    [MemberData(nameof(OneStepOffEachEdge))]
    public void HasCell_OneStepOffAnEdge_ReturnsFalse(int row, int col) => Assert.False(TwoByThree.HasCell(row, col));

    [Fact]
    public void HasCell_EmptySize_HasNoCells() => Assert.False(new GridSize(0, 0).HasCell(0, 0));

    [Fact]
    public void Of_JaggedGrid_RowsFromTheOuterArrayColsFromTheFirstRow()
        => Assert.Equal(TwoByThree, GridSize.Of(new[] { new[] { 1, 0, 1 }, new[] { 0, 1, 0 } }));
}
