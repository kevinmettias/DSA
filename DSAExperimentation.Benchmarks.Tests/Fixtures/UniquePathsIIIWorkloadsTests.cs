using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for UniquePathsIIIWorkloads (ARCHITECTURE 17.7). LC 980 promises exactly one
// starting cell and one ending cell, and its grid holds nothing but -1, 0, 1 and 2; the fixture's
// comment adds that the obstacles number exactly obstacleCount. All of that is asserted at LeetCode's
// 20-cell cap, since a grid that broke the promise would send both arms' searches somewhere LeetCode
// never tests.
public sealed partial class UniquePathsIIIWorkloadsTests
{
    private const int Rows = 4;
    private const int Columns = 5;
    private const int ObstacleCount = 3;
    private const int Seed = 980; // LC problem number
    private const int Open = 0;

    [Fact]
    public void BuildGrid_RowsAndColumns_ReturnsAGridOfThatShape()
    {
        var grid = Build();

        Assert.Equal(Rows, grid.Length);
        Assert.All(grid, row => Assert.Equal(Columns, row.Length));
    }

    [Fact]
    public void BuildGrid_Markers_PlaceOneStartOneEndAndTheAskedObstacles()
    {
        var cells = Build().SelectMany(row => row).ToList();

        Assert.Single(cells, cell => cell == UniquePathsIIIWorkloads.Start);
        Assert.Single(cells, cell => cell == UniquePathsIIIWorkloads.End);
        Assert.Equal(ObstacleCount, cells.Count(cell => cell == UniquePathsIIIWorkloads.Obstacle));
        Assert.All(
            cells,
            cell => Assert.Contains(cell, new[] { UniquePathsIIIWorkloads.Obstacle, Open, UniquePathsIIIWorkloads.Start, UniquePathsIIIWorkloads.End }));
    }

    [Fact]
    public void BuildGrid_SameSeed_ReturnsTheSameGrid() =>
        Assert.Equal(Build(), Build());

    private static int[][] Build() => UniquePathsIIIWorkloads.BuildGrid(Rows, Columns, ObstacleCount, new Random(Seed));
}
