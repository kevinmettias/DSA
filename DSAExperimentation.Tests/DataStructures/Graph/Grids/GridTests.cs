using DSAExperimentation.DataStructures.Graph.Grids;

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
}
