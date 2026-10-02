using DSAExperimentation.LeetCode.SurroundedRegions;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SurroundedRegions;

// Harness only: both border strategies live in SurroundedRegionsSolution - this file
// pins them to LeetCode's published examples and to each other. Solve mutates its
// board in place, so each example carries the input and the expected post-mutation
// board, and the agreement check hands each arm its own clone of the input.
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

    [Theory]
    [MemberData(nameof(Examples))]
    public void SolveByBorderBreadthFirstSearch_LeetCodeExamples_CapturesInteriorRegions(
        char[][] board, char[][] expected)
    {
        SurroundedRegionsSolution.SolveByBorderBreadthFirstSearch(board);

        Assert.Equal(expected, board);
    }

    // Both arms solve the board in place, so each gets its own clone of the input; the
    // two clones must then agree.
    [Theory]
    [MemberData(nameof(Examples))]
    public void Solve_AgreeOnEveryExample(char[][] board, char[][] expected)
    {
        var byDepthFirst = Clone(board);
        var byBreadthFirst = Clone(board);

        SurroundedRegionsSolution.SolveByBorderDepthFirstSearch(byDepthFirst);
        SurroundedRegionsSolution.SolveByBorderBreadthFirstSearch(byBreadthFirst);

        Assert.Equal(byDepthFirst, byBreadthFirst);
    }

    private static char[][] Clone(char[][] board) =>
        board.Select(row => (char[])row.Clone()).ToArray();
}
