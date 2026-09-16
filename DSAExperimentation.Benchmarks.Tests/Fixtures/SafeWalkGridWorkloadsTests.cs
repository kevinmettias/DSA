using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for SafeWalkGridWorkloads (ARCHITECTURE 17.7). The LC 3286 reading depends on
// roughly a third of the cells being unsafe: dense enough that a "fewest unsafe cells" path has to
// route around real obstacles rather than walk a mostly-zero grid in a straight line, and sparse
// enough that a path still exists to be found.
public sealed partial class SafeWalkGridWorkloadsTests
{
    private const int GridSize = 32;
    private const int Seed = 3286; // LC problem number
    private const int SafeCell = 0;
    private const int UnsafeCell = 1;
    private const int FewestUnsafeCells = 1;
    private const int MinUnsafePercent = 15;
    private const int MaxUnsafePercent = 55;
    private const int PercentScale = 100;

    [Fact]
    public void BuildGrid_GridSize_ReturnsASquareGridOfThatSide()
    {
        var grid = SafeWalkGridWorkloads.BuildGrid(GridSize, Seed);

        Assert.Equal(GridSize, grid.Length);
        Assert.All(grid, row => Assert.Equal(GridSize, row.Length));
    }

    [Fact]
    public void BuildGrid_EveryCell_IsEitherSafeOrUnsafe() =>
        Assert.All(
            SafeWalkGridWorkloads.BuildGrid(GridSize, Seed).SelectMany(row => row),
            cell => Assert.True(cell == SafeCell || cell == UnsafeCell));

    [Fact]
    public void BuildGrid_UnsafeDensity_StaysInsideTheBandTheRoutingNeeds()
    {
        var cells = SafeWalkGridWorkloads.BuildGrid(GridSize, Seed).SelectMany(row => row).ToList();
        var unsafeCount = cells.Count(cell => cell == UnsafeCell);

        Assert.InRange(unsafeCount, FewestUnsafeCells, cells.Count - FewestUnsafeCells);
        Assert.InRange(
            unsafeCount * PercentScale / cells.Count, MinUnsafePercent, MaxUnsafePercent);
    }

    [Fact]
    public void BuildGrid_SameSeed_ReturnsTheSameGrid() =>
        Assert.Equal(
            AnswerText.Of(SafeWalkGridWorkloads.BuildGrid(GridSize, Seed)),
            AnswerText.Of(SafeWalkGridWorkloads.BuildGrid(GridSize, Seed)));
}
