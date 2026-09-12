using DSAExperimentation.LeetCode.ValidSquare;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ValidSquare;

// Harness only. Both strategies are ValidSquareSolution's; this file pins them to
// LeetCode's published examples.
public sealed class ValidSquareTests
{
    public static TheoryData<int[], int[], int[], int[], bool> Examples =>
        new()
        {
            { [0, 0], [1, 1], [1, 0], [0, 1], true },
            { [0, 0], [1, 1], [2, 0], [1, -1], true },
            { [0, 0], [1, 1], [1, 0], [0, 12], false },
            { [0, 0], [1, 1], [2, 2], [3, 3], false },
            { [5, 5], [5, 5], [5, 5], [5, 5], false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsValidSquareByMergeSort_LeetCodeExamples_ReturnsWhetherFourPointsFormASquare(
        int[] p1, int[] p2, int[] p3, int[] p4, bool expected) =>
        Assert.Equal(expected, ValidSquareSolution.IsValidSquareByMergeSort(p1, p2, p3, p4));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsValidSquareByMinMaxScan_LeetCodeExamples_ReturnsWhetherFourPointsFormASquare(
        int[] p1, int[] p2, int[] p3, int[] p4, bool expected) =>
        Assert.Equal(expected, ValidSquareSolution.IsValidSquareByMinMaxScan(p1, p2, p3, p4));
}
