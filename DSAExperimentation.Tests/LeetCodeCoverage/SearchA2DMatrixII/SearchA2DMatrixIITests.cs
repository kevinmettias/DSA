using DSAExperimentation.LeetCode.SearchA2DMatrixII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SearchA2DMatrixII;

// Harness only. All three strategies are SearchA2DMatrixIISolution's - this file
// pins them to LeetCode's published examples, including the empty-matrix edge
// case that forces the corner walk to guard its first index instead of assuming a
// non-empty row.
public sealed class SearchA2DMatrixIITests
{
    private static readonly int[][] Matrix =
    [
        [1, 4, 7, 11, 15],
        [2, 5, 8, 12, 19],
        [3, 6, 9, 16, 22],
        [10, 13, 14, 17, 24],
        [18, 21, 23, 26, 30],
    ];

    public static TheoryData<int[][], int, bool> Examples =>
        new()
        {
            { Matrix, 5, true },
            { Matrix, 20, false },
            { [], 1, false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SearchMatrixByFullScan_LeetCodeExamples_ReturnsWhetherTargetExists(
        int[][] matrix, int target, bool expected) =>
        Assert.Equal(expected, SearchA2DMatrixIISolution.SearchMatrixByFullScan(matrix, target));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SearchMatrixByPerRowBinarySearch_LeetCodeExamples_ReturnsWhetherTargetExists(
        int[][] matrix, int target, bool expected) =>
        Assert.Equal(expected, SearchA2DMatrixIISolution.SearchMatrixByPerRowBinarySearch(matrix, target));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SearchMatrixByStaircaseSearch_LeetCodeExamples_ReturnsWhetherTargetExists(
        int[][] matrix, int target, bool expected) =>
        Assert.Equal(expected, SearchA2DMatrixIISolution.SearchMatrixByStaircaseSearch(matrix, target));
}
