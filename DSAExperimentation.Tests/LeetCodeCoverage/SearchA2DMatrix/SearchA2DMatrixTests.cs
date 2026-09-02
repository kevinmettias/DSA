using DSAExperimentation.LeetCode.SearchA2DMatrix;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SearchA2DMatrix;

// Harness only. Both strategies are SearchA2DMatrixSolution's - this file just
// pins them to LeetCode's published examples.
public sealed class SearchA2DMatrixTests
{
    public static TheoryData<int[][], int, bool> Examples =>
        new()
        {
            { [[1, 3, 5, 7], [10, 11, 16, 20], [23, 30, 34, 60]], 3, true },
            { [[1, 3, 5, 7], [10, 11, 16, 20], [23, 30, 34, 60]], 13, false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SearchMatrixByLinearScan_LeetCodeExamples_ReturnsWhetherTargetExists(
        int[][] matrix, int target, bool expected) =>
        Assert.Equal(expected, SearchA2DMatrixSolution.SearchMatrixByLinearScan(matrix, target));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SearchMatrixByBinarySearch_LeetCodeExamples_ReturnsWhetherTargetExists(
        int[][] matrix, int target, bool expected) =>
        Assert.Equal(expected, SearchA2DMatrixSolution.SearchMatrixByBinarySearch(matrix, target));
}
