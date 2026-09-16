using DSAExperimentation.LeetCode.DiagonalTraverse;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DiagonalTraverse;

// Harness only. Both zig-zag walks are DiagonalTraverseSolution's - this file
// just pins them to LeetCode's published examples.
public sealed partial class DiagonalTraverseTests
{
    public static TheoryData<int[][], int[]> Examples =>
        new()
        {
            { [[1, 2, 3], [4, 5, 6], [7, 8, 9]], [1, 2, 4, 7, 5, 3, 6, 8, 9] },
            { [[1, 2], [3, 4]], [1, 2, 3, 4] },
            { [[1, 2, 3, 4]], [1, 2, 3, 4] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindDiagonalOrderByDirectionToggle_LeetCodeExamples_ZigZagsAlongDiagonals(
        int[][] matrix, int[] expected) =>
        Assert.Equal(expected, DiagonalTraverseSolution.FindDiagonalOrderByDirectionToggle(matrix));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindDiagonalOrderByStackReversal_LeetCodeExamples_ZigZagsAlongDiagonals(
        int[][] matrix, int[] expected) =>
        Assert.Equal(expected, DiagonalTraverseSolution.FindDiagonalOrderByStackReversal(matrix));
}
