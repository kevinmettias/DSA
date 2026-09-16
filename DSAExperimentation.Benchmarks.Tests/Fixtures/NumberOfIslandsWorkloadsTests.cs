using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for NumberOfIslandsWorkloads (ARCHITECTURE 17.7). The reading depends on LC 200's
// grid being a square of land and water cells whose land is scattered, so the flood fill starts many
// separate islands instead of walking one solid block - which is the whole cost being measured.
public sealed partial class NumberOfIslandsWorkloadsTests
{
    private const int GridSize = 50;
    private const int Seed = 200; // LC problem number
    private const char Land = '1';
    private const char Water = '0';
    private const int FewestCells = 1;

    // The generator draws one land/water coin per cell, so the documented "roughly half the cells are
    // land" is a band, not a count: a quarter of the grid sits many standard deviations below the
    // coin-flip mean, far enough that the band only rejects an all-land or all-water block.
    private const int LandCountDivisor = 4;

    [Fact]
    public void BuildGrid_GridSize_ReturnsASquareGridOfExactlyThatShape()
    {
        var grid = NumberOfIslandsWorkloads.BuildGrid(GridSize, GridSize, Seed);

        Assert.Equal(GridSize, grid.Length);
        Assert.All(grid, row => Assert.Equal(GridSize, row.Length));
    }

    [Fact]
    public void BuildGrid_EveryCell_IsEitherLandOrWater()
    {
        var cells = NumberOfIslandsWorkloads.BuildGrid(GridSize, GridSize, Seed).SelectMany(row => row);

        Assert.All(cells, cell => Assert.True(cell == Land || cell == Water));
    }

    // Whether a cell is land is a probabilistic draw, so the structural fact is what is asserted: the
    // grid holds both land and water, so there are islands to start on and sea between them.
    [Fact]
    public void BuildGrid_Grid_LeavesBothLandAndWaterToFloodFill()
    {
        var cells = NumberOfIslandsWorkloads.BuildGrid(GridSize, GridSize, Seed).SelectMany(row => row).ToList();
        var landCount = cells.Count(cell => cell == Land);

        Assert.InRange(landCount, FewestCells, cells.Count - FewestCells);
    }

    [Fact]
    public void BuildGrid_LandCount_StaysNearTheDocumentedCoinFlipRate()
    {
        var cells = NumberOfIslandsWorkloads.BuildGrid(GridSize, GridSize, Seed).SelectMany(row => row).ToList();
        var landCount = cells.Count(cell => cell == Land);
        var band = cells.Count / LandCountDivisor;

        Assert.InRange(landCount, band, cells.Count - band);
    }

    [Fact]
    public void BuildGrid_SameSeed_ReturnsTheSameGrid() =>
        Assert.Equal(
            AnswerText.Of(NumberOfIslandsWorkloads.BuildGrid(GridSize, GridSize, Seed)),
            AnswerText.Of(NumberOfIslandsWorkloads.BuildGrid(GridSize, GridSize, Seed)));
}
