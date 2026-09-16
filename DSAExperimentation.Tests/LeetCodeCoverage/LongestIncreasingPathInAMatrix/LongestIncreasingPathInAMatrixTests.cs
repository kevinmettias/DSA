using DSAExperimentation.LeetCode.LongestIncreasingPathInAMatrix;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestIncreasingPathInAMatrix;

// Harness only. Both recurrences are LongestIncreasingPathInAMatrixSolution's - this
// file just pins them to LeetCode's published examples.
public sealed partial class LongestIncreasingPathInAMatrixTests
{
    public static TheoryData<int[,], int> Examples =>
        new()
        {
            {
                new int[,]
                {
                    { 9, 9, 4 },
                    { 6, 6, 8 },
                    { 2, 1, 1 },
                },
                4
            },
            {
                new int[,]
                {
                    { 3, 4, 5 },
                    { 3, 2, 6 },
                    { 2, 2, 1 },
                },
                4
            },
            { new int[,] { { 7 } }, 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestPathByNaiveRecursion_LeetCodeExamples_ReturnsLongestStrictlyIncreasingRun(
        int[,] matrix, int expected) =>
        Assert.Equal(expected, LongestIncreasingPathInAMatrixSolution.LongestPathByNaiveRecursion(matrix));

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestPathByMemoizedRecurrence_LeetCodeExamples_ReturnsLongestStrictlyIncreasingRun(
        int[,] matrix, int expected) =>
        Assert.Equal(expected, LongestIncreasingPathInAMatrixSolution.LongestPathByMemoizedRecurrence(matrix));
}
