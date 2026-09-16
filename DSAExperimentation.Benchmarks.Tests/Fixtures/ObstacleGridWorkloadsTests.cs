using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for ObstacleGridWorkloads (ARCHITECTURE 17.7). The reading depends on LC 2290's square
// 0/1 grid carrying enough obstacles that the cheapest route has to weave, with both corners left clear
// because the problem guarantees them clear.
public sealed partial class ObstacleGridWorkloadsTests
{
    private const int GridSize = 15;
    private const int Seed = 2290; // LC problem number
    private const int Clear = 0;
    private const int Obstacle = 1;
    private const int FewestCells = 1;

    // One cell in five is an obstacle, so a grid this size lands near a fifth - this divisor only pins
    // the count away from an all-obstacle or obstacle-free block.
    private const int ObstacleBandDivisor = 10;

    [Fact]
    public void BuildGrid_GridSize_ReturnsASquareGridOfExactlyThatShape()
    {
        var grid = ObstacleGridWorkloads.BuildGrid(GridSize, Seed);

        Assert.Equal(GridSize, grid.Length);
        Assert.All(grid, row => Assert.Equal(GridSize, row.Length));
    }

    [Fact]
    public void BuildGrid_EveryCell_IsEitherAnObstacleOrClear()
    {
        var grid = ObstacleGridWorkloads.BuildGrid(GridSize, Seed);

        Assert.All(
            grid.SelectMany(row => row),
            cell => Assert.True(cell == Obstacle || cell == Clear));
    }

    // Both corners are cleared by construction, and LC 2290 guarantees them - a grid that left an
    // obstacle on either corner would make both arms' answer unreachable, not merely slower.
    [Fact]
    public void BuildGrid_BothCorners_AreLeftClear()
    {
        var grid = ObstacleGridWorkloads.BuildGrid(GridSize, Seed);

        Assert.Equal(Clear, grid[0][0]);
        Assert.Equal(Clear, grid[GridSize - 1][GridSize - 1]);
    }

    [Fact]
    public void BuildGrid_Grid_LeavesObstaclesToWeaveAround()
    {
        var cells = ObstacleGridWorkloads.BuildGrid(GridSize, Seed).SelectMany(row => row).ToList();
        var obstacleCount = cells.Count(cell => cell == Obstacle);
        var band = cells.Count / ObstacleBandDivisor;

        Assert.InRange(obstacleCount, FewestCells, cells.Count - FewestCells);
        Assert.InRange(obstacleCount, band, cells.Count - band);
    }

    [Fact]
    public void BuildGrid_SameSeed_ReturnsTheSameGrid() =>
        Assert.Equal(
            AnswerText.Of(ObstacleGridWorkloads.BuildGrid(GridSize, Seed)),
            AnswerText.Of(ObstacleGridWorkloads.BuildGrid(GridSize, Seed)));
}
