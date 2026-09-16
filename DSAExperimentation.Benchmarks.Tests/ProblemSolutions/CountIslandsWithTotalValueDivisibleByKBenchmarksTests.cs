using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountIslandsWithTotalValueDivisibleByKBenchmarks (ARCHITECTURE 17.9): its
// two arms are competing flood-fill strategies for the same question, so a harness whose arms
// disagree is timing two different problems. Setup draws the grid from a fixed seed through
// IslandValueGridWorkloads, so the same GridSize must rebuild the same grid.
public sealed partial class CountIslandsWithTotalValueDivisibleByKBenchmarksTests
{
    private const int SmallestGridSize = 50;

    // Mirrors the benchmark's own seed, so the workload asserted here is the one Setup builds.
    private const int GridSeed = 3619;

    [Fact]
    public void Setup_SmallestGrid_RebuildsTheSameWorkload()
    {
        var grid = IslandValueGridWorkloads.BuildGrid(SmallestGridSize, SmallestGridSize, GridSeed);

        // The documented shape: a GridSize x GridSize grid of water (0) and positive-valued
        // land, so both strategies flood-fill many separate islands rather than one solid block.
        Assert.Equal(SmallestGridSize, grid.Length);
        Assert.All(grid, row => Assert.Equal(SmallestGridSize, row.Length));
        Assert.Contains(grid, row => row.Contains(0));
        Assert.Contains(grid, row => row.Any(value => value > 0));
        Assert.Equal(BuildHarness().FloodFillStack(), BuildHarness().FloodFillStack());
    }

    [Fact]
    public void FloodFillStack_ScatteredIslandGrid_AgreesWithDepthFirstSearchTraverse()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DepthFirstSearchTraverse(), harness.FloodFillStack());
    }

    [Fact]
    public void DepthFirstSearchTraverse_ScatteredIslandGrid_AgreesWithFloodFillStack()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.FloodFillStack(), harness.DepthFirstSearchTraverse());
    }

    private static CountIslandsWithTotalValueDivisibleByKBenchmarks BuildHarness()
    {
        var harness = new CountIslandsWithTotalValueDivisibleByKBenchmarks { GridSize = SmallestGridSize };
        harness.Setup();

        return harness;
    }
}
