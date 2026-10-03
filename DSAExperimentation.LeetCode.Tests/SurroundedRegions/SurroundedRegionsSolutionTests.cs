using DSAExperimentation.LeetCode.SurroundedRegions;

namespace DSAExperimentation.LeetCode.Tests.SurroundedRegions;

// Harness only: both border strategies live in SurroundedRegionsSolution - this file
// pins each to LeetCode's published examples. Solve mutates its board in place, so
// each example carries the input and the expected post-mutation board, and each arm
// is handed its own clone of the input.
public sealed partial class SurroundedRegionsSolutionTests
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

    [Theory]
    [MemberData(nameof(Examples))]
    public void SolveByBorderBreadthFirstSearch_LeetCodeExamples_CapturesInteriorRegions(
        char[][] board, char[][] expected)
    {
        SurroundedRegionsSolution.SolveByBorderBreadthFirstSearch(board);

        Assert.Equal(expected, board);
    }

    private static char[][] Clone(char[][] board) =>
        board.Select(row => (char[])row.Clone()).ToArray();
}
