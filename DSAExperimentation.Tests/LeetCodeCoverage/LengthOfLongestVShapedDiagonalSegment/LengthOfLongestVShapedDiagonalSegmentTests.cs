using DSAExperimentation.LeetCode.LengthOfLongestVShapedDiagonalSegment;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LengthOfLongestVShapedDiagonalSegment;

// Harness only. Both the turn-aware walk and the directional DP tables live in
// LengthOfLongestVShapedDiagonalSegmentSolution - this file just pins both
// strategies to LeetCode's published examples, including the one-turn, no-turn and
// single-cell cases.
public sealed partial class LengthOfLongestVShapedDiagonalSegmentTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            {
                [
                    [2, 2, 1, 2, 2],
                    [2, 0, 2, 2, 0],
                    [2, 0, 1, 1, 0],
                    [1, 0, 2, 2, 2],
                    [2, 0, 0, 2, 2],
                ],
                5
            },
            {
                [
                    [2, 2, 2, 2, 2],
                    [2, 0, 2, 2, 0],
                    [2, 0, 1, 1, 0],
                    [1, 0, 2, 2, 2],
                    [2, 0, 0, 2, 2],
                ],
                4
            },
            {
                [
                    [1, 2, 2, 2, 2],
                    [2, 2, 2, 2, 0],
                    [2, 0, 0, 0, 0],
                    [0, 0, 2, 2, 2],
                    [2, 0, 0, 2, 0],
                ],
                5
            },
            { [[1]], 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestLengthByBruteForceWalk_LeetCodeExamples_ReturnsLongestVShapeLength(
        int[][] grid, int expected) =>
        Assert.Equal(expected, LengthOfLongestVShapedDiagonalSegmentSolution.LongestLengthByBruteForceWalk(grid));

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestLengthByDirectionalDp_LeetCodeExamples_ReturnsLongestVShapeLength(
        int[][] grid, int expected) =>
        Assert.Equal(expected, LengthOfLongestVShapedDiagonalSegmentSolution.LongestLengthByDirectionalDp(grid));
}
