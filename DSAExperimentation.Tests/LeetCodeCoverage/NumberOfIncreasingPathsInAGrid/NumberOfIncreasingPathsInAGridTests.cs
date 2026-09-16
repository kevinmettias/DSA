using DSAExperimentation.LeetCode.NumberOfIncreasingPathsInAGrid;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfIncreasingPathsInAGrid;

// Harness only. Both recurrences are NumberOfIncreasingPathsInAGridSolution's - this
// file just pins them to LeetCode's published examples, plus the degenerate grids the
// examples never reach: an all-equal grid where no step is ever increasing, and a
// strictly increasing column where every suffix of the chain counts.
public sealed partial class NumberOfIncreasingPathsInAGridTests
{
    public static TheoryData<int[,], int> Examples =>
        new()
        {
            {
                new int[,]
                {
                    { 1, 1 },
                    { 3, 4 },
                },
                8
            },
            {
                new int[,]
                {
                    { 1 },
                    { 2 },
                },
                3
            },
            { new int[,] { { 5 } }, 1 },
            {
                new int[,]
                {
                    { 7, 7 },
                    { 7, 7 },
                },
                4
            },
            {
                new int[,]
                {
                    { 1, 2 },
                    { 4, 3 },
                },
                11
            },
            {
                new int[,]
                {
                    { 1 },
                    { 2 },
                    { 3 },
                },
                6
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountPathsByNaiveRecursion_LeetCodeExamples_ReturnsIncreasingPathCount(
        int[,] grid, int expected) =>
        Assert.Equal(expected, NumberOfIncreasingPathsInAGridSolution.CountPathsByNaiveRecursion(grid));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountPathsByMemoizedRecurrence_LeetCodeExamples_ReturnsIncreasingPathCount(
        int[,] grid, int expected) =>
        Assert.Equal(expected, NumberOfIncreasingPathsInAGridSolution.CountPathsByMemoizedRecurrence(grid));
}
