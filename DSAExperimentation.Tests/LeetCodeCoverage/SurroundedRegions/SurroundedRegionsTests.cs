using DSAExperimentation.LeetCode.SurroundedRegions;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SurroundedRegions;

// Harness only: the border-DFS strategy lives in
// SurroundedRegionsSolution - this file just pins it to LeetCode's published
// examples. Solve mutates its board in place, so each example carries the
// input and the expected post-mutation board.
public sealed partial class SurroundedRegionsTests
{
    public static TheoryData<char[][], char[][]> Examples =>
        new()
        {
            {
                [['X', 'X', 'X', 'X'], ['X', 'O', 'O', 'X'], ['X', 'X', 'O', 'X'], ['X', 'O', 'X', 'X']],
                [['X', 'X', 'X', 'X'], ['X', 'X', 'X', 'X'], ['X', 'X', 'X', 'X'], ['X', 'O', 'X', 'X']]
            },
            {
                [['X']],
                [['X']]
            },
            {
                [['O']],
                [['O']]
            },
            {
                [['O', 'O'], ['O', 'O']],
                [['O', 'O'], ['O', 'O']]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SolveByBorderDepthFirstSearch_LeetCodeExamples_CapturesInteriorRegions(
        char[][] board, char[][] expected)
    {
        SurroundedRegionsSolution.SolveByBorderDepthFirstSearch(board);

        Assert.Equal(expected, board);
    }
}
