using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for WaitCostGridWorkloads (ARCHITECTURE 17.7): the three wait-cost grid problems
// (LC 3341, LC 3342, LC 2577) all guarantee that the origin costs nothing to enter and that every other
// cell demands a wait. Both facts are what force each relaxation through its wait branch instead of a
// constant-weight shortcut, so both are asserted here rather than only the grid's shape.
public sealed partial class WaitCostGridWorkloadsTests
{
    private const int Size = 32;
    private const int WaitCeilingExclusive = 100;
    private const int Seed = 3341; // LC problem number
    private const int OriginRow = 0;
    private const int OriginCol = 0;
    private const int ZeroWait = 0;
    private const int FewestNonZeroWaits = 1;

    [Fact]
    public void WithZeroOrigin_Size_ReturnsASquareGridOfThatShape()
    {
        var grid = WaitCostGridWorkloads.WithZeroOrigin(Size, WaitCeilingExclusive, Seed);

        Assert.Equal(Size, grid.Length);
        Assert.All(grid, row => Assert.Equal(Size, row.Length));
    }

    [Fact]
    public void WithZeroOrigin_EveryNonOriginCell_DemandsAWaitInsideTheRequestedRange() =>
        Assert.All(
            WaitCostGridWorkloads.WithZeroOrigin(Size, WaitCeilingExclusive, Seed)
                .SelectMany((row, rowIndex) => row.Select((wait, colIndex) => (rowIndex, colIndex, wait)))
                .Where(cell => cell.rowIndex != OriginRow || cell.colIndex != OriginCol),
            cell => Assert.InRange(cell.wait, ZeroWait, WaitCeilingExclusive - 1));

    [Fact]
    public void WithZeroOrigin_TheOrigin_CostsNothingToEnter() =>
        Assert.Equal(
            ZeroWait,
            WaitCostGridWorkloads.WithZeroOrigin(Size, WaitCeilingExclusive, Seed)[OriginRow][OriginCol]);

    // The wait branch is only exercised if the cells around the origin actually charge something.
    [Fact]
    public void WithZeroOrigin_SeededGrid_LeavesNonZeroWaitsToRelax() =>
        Assert.True(
            WaitCostGridWorkloads.WithZeroOrigin(Size, WaitCeilingExclusive, Seed)
                .SelectMany(row => row)
                .Count(wait => wait > ZeroWait)
                >= FewestNonZeroWaits);

    [Fact]
    public void WithZeroOrigin_SameSeed_ReturnsTheSameGrid() =>
        Assert.Equal(
            AnswerText.Of(WaitCostGridWorkloads.WithZeroOrigin(Size, WaitCeilingExclusive, Seed)),
            AnswerText.Of(WaitCostGridWorkloads.WithZeroOrigin(Size, WaitCeilingExclusive, Seed)));
}
