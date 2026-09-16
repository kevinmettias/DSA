using DSAExperimentation.LeetCode.RotateImage;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RotateImage;

// Harness only. Both strategies are RotateImageSolution's - this file just pins
// them to LeetCode's published examples. Each row is cloned before rotating so
// the two theory methods (and repeated data rows) never share a mutated matrix.
public sealed partial class RotateImageTests
{
    public static TheoryData<int[][], int[][]> Examples =>
        new()
        {
            {
                [[1, 2, 3], [4, 5, 6], [7, 8, 9]],
                [[7, 4, 1], [8, 5, 2], [9, 6, 3]]
            },
            {
                [[5, 1, 9, 11], [2, 4, 8, 10], [13, 3, 6, 7], [15, 14, 12, 16]],
                [[15, 13, 2, 5], [14, 3, 4, 1], [12, 6, 8, 9], [16, 7, 10, 11]]
            },
            {
                [[1]],
                [[1]]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RotateByArrayReverse_LeetCodeExamples_RotatesClockwiseInPlace(
        int[][] matrix, int[][] expected)
    {
        var working = Clone(matrix);

        RotateImageSolution.RotateByArrayReverse(working);

        Assert.Equal(expected, working);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void RotateByStackReverse_LeetCodeExamples_RotatesClockwiseInPlace(
        int[][] matrix, int[][] expected)
    {
        var working = Clone(matrix);

        RotateImageSolution.RotateByStackReverse(working);

        Assert.Equal(expected, working);
    }

    private static int[][] Clone(int[][] matrix)
    {
        var copy = new int[matrix.Length][];

        for (var r = 0; r < matrix.Length; r++)
        {
            copy[r] = (int[])matrix[r].Clone();
        }

        return copy;
    }
}
