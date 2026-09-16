using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for IslandValueGridWorkloads (ARCHITECTURE 17.7). The reading depends on LC 3619's
// grid holding both water and land with positive island values, so both strategies flood-fill many
// separate islands instead of walking one solid block.
public sealed partial class IslandValueGridWorkloadsTests
{
    private const int Rows = 32;
    private const int Cols = 32;
    private const int Seed = 3619; // LC problem number
    private const int Water = 0;
    private const int LandValueUpperBound = 1_000_000; // exclusive
    private const int MinLandValue = 1;
    private const int FewestWaterCells = 1;

    [Fact]
    public void BuildGrid_RowsAndCols_ReturnAGridOfExactlyThatShape()
    {
        var grid = IslandValueGridWorkloads.BuildGrid(Rows, Cols, Seed);

        Assert.Equal(Rows, grid.Length);
        Assert.All(grid, row => Assert.Equal(Cols, row.Length));
    }

    [Fact]
    public void BuildGrid_EveryCell_IsEitherWaterOrLandWithinTheDocumentedBand()
    {
        var grid = IslandValueGridWorkloads.BuildGrid(Rows, Cols, Seed);

        Assert.All(
            grid.SelectMany(row => row),
            cell => Assert.True(cell == Water || (cell >= MinLandValue && cell < LandValueUpperBound)));
    }

    // Whether a cell is water is a probabilistic draw, so the structural fact is what is asserted:
    // the grid leaves both some water and some land, which is the implication the flood-fill
    // reading needs - separate islands with sea between them, not one all-land or all-water block.
    [Fact]
    public void BuildGrid_Grid_LeavesBothWaterAndLandToFloodFill()
    {
        var grid = IslandValueGridWorkloads.BuildGrid(Rows, Cols, Seed);
        var cells = grid.SelectMany(row => row).ToList();
        var waterCount = cells.Count(cell => cell == Water);

        Assert.InRange(waterCount, FewestWaterCells, cells.Count - FewestWaterCells);
    }

    [Fact]
    public void BuildGrid_SameSeed_ReturnsTheSameGrid() =>
        Assert.Equal(
            AnswerText.Of(IslandValueGridWorkloads.BuildGrid(Rows, Cols, Seed)),
            AnswerText.Of(IslandValueGridWorkloads.BuildGrid(Rows, Cols, Seed)));
}
