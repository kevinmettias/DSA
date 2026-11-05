using DSAExperimentation.LeetCode.Solutions.SpiralMatrix;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SpiralMatrix;

// Harness only: both strategies live in SpiralMatrixSolution and are asserted
// against the same cases - the four-boundary-pointer shrink this file's
// original helper computed, and the visited-grid simulation that used to be
// untested benchmark scaffolding.
public sealed partial class SpiralMatrixSolutionTests
{
    public static TheoryData<int[][], int[]> Cases =>
        new()
        {
            { [[1, 2, 3]], [1, 2, 3] },
            { [[1], [2], [3]], [1, 2, 3] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void SpiralOrderByBoundaryPointerShrink_Cases_ReturnsClockwiseOrder(
        int[][] matrix, int[] expected) =>
        Assert.Equal(expected, SpiralMatrixSolution.SpiralOrderByBoundaryPointerShrink(matrix));

    [Theory]
    [MemberData(nameof(Cases))]
    public void SpiralOrderByVisitedGridWalk_Cases_ReturnsClockwiseOrder(
        int[][] matrix, int[] expected) =>
        Assert.Equal(expected, SpiralMatrixSolution.SpiralOrderByVisitedGridWalk(matrix));
}
