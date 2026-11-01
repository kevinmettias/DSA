using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountIslandsWithTotalValueDivisibleByKBenchmarks (ARCHITECTURE 17.9):
// BenchmarkArmsTests already holds the arms to agreeing on a rebuilt workload, so this asserts only
// the shape of the grid Setup draws through IslandValueGridWorkloads, which no arm's answer reveals.
public sealed partial class CountIslandsWithTotalValueDivisibleByKBenchmarksTests
{
    private const int SmallestGridSize = 50;

    // Mirrors the benchmark's own seed, so the workload asserted here is the one Setup builds.
    private const int GridSeed = 3619;

    [Fact]
    public void Setup_SmallestGrid_BuildsASquareGridOfWaterAndLand()
    {
        var grid = IslandValueGridWorkloads.BuildGrid(SmallestGridSize, SmallestGridSize, GridSeed);

        // The documented shape: a GridSize x GridSize grid of water (0) and positive-valued
        // land, so both strategies flood-fill many separate islands rather than one solid block.
        Assert.Equal(SmallestGridSize, grid.Length);
        Assert.All(grid, row => Assert.Equal(SmallestGridSize, row.Length));
        Assert.Contains(grid, row => row.Contains(0));
        Assert.Contains(grid, row => row.Any(value => value > 0));
    }
}
