using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FillASpecialGridBenchmarks (ARCHITECTURE 17.9): the generic arms check only
// compares the arms' grids with each other, so this pins the baseline's grid to the shape the
// problem fixes independently of any arm - a level-n grid is 2^n cells on a side. The class has no
// [GlobalSetup], so the [Params] level count is the whole workload.
public sealed partial class FillASpecialGridBenchmarksTests
{
    private const int SmallestLevelCount = 5;

    // A level-n special grid is 2^n x 2^n, one value per cell.
    private const int ExpectedSideLength = 1 << SmallestLevelCount;

    [Fact]
    public void RecursiveQuadrants_FifthLevelGrid_IsTwoToTheLevelCellsOnASide()
    {
        var grid = BuildHarness().RecursiveQuadrants();

        Assert.Equal(ExpectedSideLength, grid.Length);
        Assert.All(grid, row => Assert.Equal(ExpectedSideLength, row.Length));
    }

    private static FillASpecialGridBenchmarks BuildHarness() =>
        new() { LevelCount = SmallestLevelCount };
}
