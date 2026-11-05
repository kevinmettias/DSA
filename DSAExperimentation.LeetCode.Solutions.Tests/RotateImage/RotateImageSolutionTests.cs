using DSAExperimentation.LeetCode.Solutions.RotateImage;

namespace DSAExperimentation.LeetCode.Solutions.Tests.RotateImage;

// Harness only. Both strategies are RotateImageSolution's - this file just pins
// them to a one-by-one matrix, which rotates to itself. LeetCode's published
// examples run in CatalogCaseTests. Each row is cloned before rotating so the two
// theory methods (and repeated data rows) never share a mutated matrix.
public sealed partial class RotateImageSolutionTests
{
    public static TheoryData<int[][], int[][]> Cases =>
        new()
        {
            {
                [[1]],
                [[1]]
            },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void RotateByArrayReverse_Cases_RotatesClockwiseInPlace(
        int[][] matrix, int[][] expected)
    {
        var working = Clone(matrix);

        RotateImageSolution.RotateByArrayReverse(working);

        Assert.Equal(expected, working);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void RotateByStackReverse_Cases_RotatesClockwiseInPlace(
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
