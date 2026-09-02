using DSAExperimentation.LeetCode.SpiralMatrix;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SpiralMatrix;

// Harness only: both strategies live in SpiralMatrixSolution and are asserted
// against the same examples - the four-boundary-pointer shrink this file's
// original helper computed, and the visited-grid simulation that used to be
// untested benchmark scaffolding.
public sealed class SpiralMatrixTests
{
    public static TheoryData<int[][], int[]> Examples =>
        new()
        {
            { [[1, 2, 3], [4, 5, 6], [7, 8, 9]], [1, 2, 3, 6, 9, 8, 7, 4, 5] },
            { [[1, 2, 3, 4], [5, 6, 7, 8], [9, 10, 11, 12]], [1, 2, 3, 4, 8, 12, 11, 10, 9, 5, 6, 7] },
            { [[1]], [1] },
            { [[1, 2, 3]], [1, 2, 3] },
            { [[1], [2], [3]], [1, 2, 3] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SpiralOrderByBoundaryPointerShrink_LeetCodeExamples_ReturnsClockwiseOrder(
        int[][] matrix, int[] expected) =>
        Assert.Equal(expected, SpiralMatrixSolution.SpiralOrderByBoundaryPointerShrink(matrix));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SpiralOrderByVisitedGridWalk_LeetCodeExamples_ReturnsClockwiseOrder(
        int[][] matrix, int[] expected) =>
        Assert.Equal(expected, SpiralMatrixSolution.SpiralOrderByVisitedGridWalk(matrix));
}
