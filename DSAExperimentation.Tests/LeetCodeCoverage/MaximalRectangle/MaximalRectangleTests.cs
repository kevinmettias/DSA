using DSAExperimentation.LeetCode.MaximalRectangle;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximalRectangle;

// Harness only. Both strategies live in MaximalRectangleSolution - this file
// just pins them to LeetCode's published examples plus an all-zeros matrix,
// where neither strategy finds any rectangle at all.
public sealed partial class MaximalRectangleTests
{
    public static TheoryData<char[][], int> Examples =>
        new()
        {
            {
                [
                    ['1', '0', '1', '0', '0'],
                    ['1', '0', '1', '1', '1'],
                    ['1', '1', '1', '1', '1'],
                    ['1', '0', '0', '1', '0'],
                ],
                6
            },
            { [['0']], 0 },
            { [['1']], 1 },
            { [['0', '0'], ['0', '0']], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximalRectangleAreaByRowPairScan_LeetCodeExamples_ReturnsLargestAllOnesRectangle(
        char[][] matrix, int expected) =>
        Assert.Equal(expected, MaximalRectangleSolution.MaximalRectangleAreaByRowPairScan(matrix));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximalRectangleAreaByRowHistogramStack_LeetCodeExamples_ReturnsLargestAllOnesRectangle(
        char[][] matrix, int expected) =>
        Assert.Equal(expected, MaximalRectangleSolution.MaximalRectangleAreaByRowHistogramStack(matrix));
}
