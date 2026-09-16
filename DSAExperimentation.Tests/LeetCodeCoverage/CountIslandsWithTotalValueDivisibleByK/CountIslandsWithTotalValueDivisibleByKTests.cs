using DSAExperimentation.LeetCode.CountIslandsWithTotalValueDivisibleByK;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountIslandsWithTotalValueDivisibleByK;

// Harness only. Both strategies are
// CountIslandsWithTotalValueDivisibleByKSolution's - this file just pins them to
// LeetCode's published examples.
public sealed partial class CountIslandsWithTotalValueDivisibleByKTests
{
    public static TheoryData<int[][], int, int> Examples =>
        new()
        {
            {
                [
                    [0, 2, 1, 0, 0],
                    [0, 5, 0, 0, 5],
                    [0, 0, 1, 0, 0],
                    [0, 1, 4, 7, 0],
                    [0, 2, 0, 0, 8],
                ],
                5,
                2
            },
            {
                [
                    [3, 0, 3, 0],
                    [0, 3, 0, 3],
                    [3, 0, 3, 0],
                ],
                3,
                6
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByFloodFillStack_LeetCodeExamples_ReturnsIslandsWithTotalValueDivisibleByK(
        int[][] grid, int divisor, int expected)
    {
        var actual = CountIslandsWithTotalValueDivisibleByKSolution.CountByFloodFillStack(grid, divisor);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByDepthFirstSearchTraverse_LeetCodeExamples_ReturnsIslandsWithTotalValueDivisibleByK(
        int[][] grid, int divisor, int expected)
    {
        var actual = CountIslandsWithTotalValueDivisibleByKSolution.CountByDepthFirstSearchTraverse(
            grid, divisor);
        Assert.Equal(expected, actual);
    }
}
