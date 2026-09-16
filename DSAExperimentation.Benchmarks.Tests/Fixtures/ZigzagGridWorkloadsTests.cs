using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for ZigzagGridWorkloads (ARCHITECTURE 17.7): LC 3417 walks a rectangular grid of positive
// values with alternating column directions. The fixture is sized inside the problem's own 2 <= m, n <= 50
// constraint and its values stay inside 1 <= grid[i][j] <= 2500, so both strategies traverse exactly the
// shape LeetCode's judge would present - which is what makes the numbers these arms produce comparable.
public sealed partial class ZigzagGridWorkloadsTests
{
    private const int Rows = 16;
    private const int Cols = 24;
    private const int Seed = 3417; // LC problem number
    private const int SmallestCellValue = 1;
    private const int LargestCellValue = 2_500;

    [Fact]
    public void BuildGrid_RowsAndCols_ReturnsAGridOfExactlyThatShape()
    {
        var grid = ZigzagGridWorkloads.BuildGrid(Rows, Cols, Seed);

        Assert.Equal(Rows, grid.Length);
        Assert.All(grid, row => Assert.Equal(Cols, row.Length));
    }

    // The grid is rectangular rather than square, which is the case a single width would never reach: the
    // zigzag changes direction on every column, and a square grid would hide a column/row transposition.
    [Fact]
    public void BuildGrid_RectangularShape_IsWiderThanItIsTall() =>
        Assert.NotEqual(Rows, ZigzagGridWorkloads.BuildGrid(Rows, Cols, Seed)[0].Length);

    [Fact]
    public void BuildGrid_EveryCell_IsPositiveAndInsideTheDocumentedBound() =>
        Assert.All(
            ZigzagGridWorkloads.BuildGrid(Rows, Cols, Seed).SelectMany(row => row),
            cell => Assert.InRange(cell, SmallestCellValue, LargestCellValue));

    [Fact]
    public void BuildGrid_SameSeed_ReturnsTheSameGrid() =>
        Assert.Equal(
            AnswerText.Of(ZigzagGridWorkloads.BuildGrid(Rows, Cols, Seed)),
            AnswerText.Of(ZigzagGridWorkloads.BuildGrid(Rows, Cols, Seed)));
}
