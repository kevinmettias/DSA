using DSAExperimentation.LeetCode.Solutions.PaintingAGridWithThreeDifferentColors;

namespace DSAExperimentation.LeetCode.Solutions.Tests.PaintingAGridWithThreeDifferentColors;

// Harness only. Both strategies live in
// PaintingAGridWithThreeDifferentColorsSolution; this file pins them to the same
// cases, so a failure names the strategy that broke. The brute force was
// previously only a [Benchmark(Baseline = true)] arm with nothing asserting it -
// these are its first assertions. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class PaintingAGridWithThreeDifferentColorsSolutionTests
{
    // A grid counted by hand: one column of two cells, so n=1 has no horizontal
    // adjacency at all and only the vertical neighbour constrains it (3 colors for
    // the top cell, 2 for the bottom, 3*2 = 6).
    public static TheoryData<int, int, int> Cases =>
        new()
        {
            { 2, 1, 6 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void ColorTheGridByBruteForce_Cases_ReturnsColoringCount(
        int rows, int columns, int expected)
    {
        var actual = PaintingAGridWithThreeDifferentColorsSolution.ColorTheGridByBruteForce(rows, columns);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void ColorTheGridByColumnPatternDynamicProgramming_Cases_ReturnsColoringCount(
        int rows, int columns, int expected)
    {
        var actual = PaintingAGridWithThreeDifferentColorsSolution.ColorTheGridByColumnPatternDynamicProgramming(
            rows, columns);

        Assert.Equal(expected, actual);
    }
}
