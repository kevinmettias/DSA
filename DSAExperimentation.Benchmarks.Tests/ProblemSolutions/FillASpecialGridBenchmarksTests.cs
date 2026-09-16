using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FillASpecialGridBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same grid - recursing into the four quadrants against computing each cell's
// value directly from its row/column bits - so a harness whose arms disagree is filling two
// different grids. Both arms return the whole int[][] grid, and every row and cell position is the
// answer rather than an incidental order, so the two grids are compared as ordered sequences. A
// level-n grid is 2^n cells on a side, which is the shape the comparison has to hold over; the
// class has no [GlobalSetup], so the [Params] level count is the whole workload.
public sealed partial class FillASpecialGridBenchmarksTests
{
    private const int SmallestLevelCount = 5;

    // A level-n special grid is 2^n x 2^n, one value per cell.
    private const int ExpectedSideLength = 1 << SmallestLevelCount;

    [Fact]
    public void RecursiveQuadrants_FifthLevelGrid_AgreesWithBitQuadrantDigits()
    {
        var harness = BuildHarness();
        var grid = harness.RecursiveQuadrants();

        Assert.Equal(ExpectedSideLength, grid.Length);
        Assert.All(grid, row => Assert.Equal(ExpectedSideLength, row.Length));

        Assert.Equal(AnswerText.Of(harness.BitQuadrantDigits()), AnswerText.Of(grid));
    }

    [Fact]
    public void BitQuadrantDigits_FifthLevelGrid_AgreesWithRecursiveQuadrants()
    {
        var harness = BuildHarness();
        var grid = harness.BitQuadrantDigits();

        Assert.Equal(ExpectedSideLength, grid.Length);
        Assert.All(grid, row => Assert.Equal(ExpectedSideLength, row.Length));

        Assert.Equal(AnswerText.Of(harness.RecursiveQuadrants()), AnswerText.Of(grid));
    }

    private static FillASpecialGridBenchmarks BuildHarness() =>
        new() { LevelCount = SmallestLevelCount };
}
