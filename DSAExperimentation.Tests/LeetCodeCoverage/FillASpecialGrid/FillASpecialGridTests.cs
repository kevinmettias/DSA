using DSAExperimentation.LeetCode.FillASpecialGrid;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FillASpecialGrid;

// Harness only: both strategies live in FillASpecialGridSolution and are asserted
// against the same examples, so a failure names the strategy that broke.
public sealed class FillASpecialGridTests
{
    public static TheoryData<int, int[][]> Examples =>
        new()
        {
            { 0, [[0]] },
            { 1, [[3, 0], [2, 1]] },
            { 2, [[15, 12, 3, 0], [14, 13, 2, 1], [11, 8, 7, 4], [10, 9, 6, 5]] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SpecialGridByRecursiveQuadrants_LeetCodeExamples_FillsQuadrantsInDescendingOrder(
        int levelCount, int[][] expected) =>
        Assert.Equal(expected, FillASpecialGridSolution.SpecialGridByRecursiveQuadrants(levelCount));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SpecialGridByBitQuadrantDigits_LeetCodeExamples_FillsQuadrantsInDescendingOrder(
        int levelCount, int[][] expected) =>
        Assert.Equal(expected, FillASpecialGridSolution.SpecialGridByBitQuadrantDigits(levelCount));
}
