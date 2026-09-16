using DSAExperimentation.LeetCode.ReshapeTheMatrix;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ReshapeTheMatrix;

// LeetCode 566. Reshape the Matrix: both strategies must produce the same
// row-major reshape (or the original matrix back, when the cell counts don't
// match) - see ReshapeTheMatrixSolution for the strategies themselves.
public sealed partial class ReshapeTheMatrixTests
{
    public static TheoryData<int[][], int, int, int[][]> Examples => new()
    {
        { [[1, 2], [3, 4]], 1, 4, [[1, 2, 3, 4]] },
        { [[1, 2], [3, 4]], 2, 4, [[1, 2], [3, 4]] },
    };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ReshapeByLinearIndexDivMod_Example_ReturnsRowMajorReshape(
        int[][] mat, int targetRows, int targetCols, int[][] expected)
    {
        var actual = ReshapeTheMatrixSolution.ReshapeByLinearIndexDivMod(mat, targetRows, targetCols);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void ReshapeByCursorWalk_Example_ReturnsRowMajorReshape(
        int[][] mat, int targetRows, int targetCols, int[][] expected)
    {
        var actual = ReshapeTheMatrixSolution.ReshapeByCursorWalk(mat, targetRows, targetCols);

        Assert.Equal(expected, actual);
    }
}
