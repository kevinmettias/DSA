using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for SelectCellsWorkloads (ARCHITECTURE 17.7). The LC 3276 reading depends on the
// values being drawn from a range tied to the grid's own size rather than the problem's full 1..100,
// which is what keeps most rows holding several duplicate values - the case the brute-force baseline
// has to re-derive row by row and the bitmask strategy collapses into one memoized state.
public sealed partial class SelectCellsWorkloadsTests
{
    private const int GridSize = 7;
    private const int Seed = 3276; // LC problem number
    private const int MinCellValue = 1;

    [Fact]
    public void BuildGrid_GridSize_ReturnsASquareGridOfThatSide()
    {
        var grid = SelectCellsWorkloads.BuildGrid(GridSize, Seed);

        Assert.Equal(GridSize, grid.Length);
        Assert.All(grid, row => Assert.Equal(GridSize, row.Length));
    }

    [Fact]
    public void BuildGrid_EveryCell_FallsInsideTheSizeTiedValueRange() =>
        Assert.All(
            SelectCellsWorkloads.BuildGrid(GridSize, Seed).SelectMany(row => row),
            cell => Assert.InRange(cell, MinCellValue, GridSize));

    // Pigeonhole: the grid holds GridSize times GridSize cells drawn from GridSize values, so some
    // value must repeat however the draws fall. That repeat is the whole reason the baseline pays.
    [Fact]
    public void BuildGrid_Grid_RepeatsValuesSoTheBaselineHasToReDeriveRows()
    {
        var cells = SelectCellsWorkloads.BuildGrid(GridSize, Seed).SelectMany(row => row).ToList();

        Assert.True(cells.Count > cells.Distinct().Count());
    }

    [Fact]
    public void BuildGrid_SameSeed_ReturnsTheSameGrid() =>
        Assert.Equal(
            AnswerText.Of(SelectCellsWorkloads.BuildGrid(GridSize, Seed)),
            AnswerText.Of(SelectCellsWorkloads.BuildGrid(GridSize, Seed)));
}
