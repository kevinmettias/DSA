using DSAExperimentation.LeetCode.Solutions.DiagonalTraverse;

namespace DSAExperimentation.LeetCode.Solutions.Tests.DiagonalTraverse;

// Harness only. Both zig-zag walks are DiagonalTraverseSolution's - this file
// pins them to a single-row matrix, whose every diagonal holds one cell, so the
// walk reads the row left to right. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class DiagonalTraverseSolutionTests
{
    public static TheoryData<int[][], int[]> Cases =>
        new()
        {
            { [[1, 2, 3, 4]], [1, 2, 3, 4] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindDiagonalOrderByDirectionToggle_Cases_ZigZagsAlongDiagonals(
        int[][] matrix, int[] expected) =>
        Assert.Equal(expected, DiagonalTraverseSolution.FindDiagonalOrderByDirectionToggle(matrix));

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindDiagonalOrderByStackReversal_Cases_ZigZagsAlongDiagonals(
        int[][] matrix, int[] expected) =>
        Assert.Equal(expected, DiagonalTraverseSolution.FindDiagonalOrderByStackReversal(matrix));
}
