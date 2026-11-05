using DSAExperimentation.LeetCode.Solutions.SurroundedRegions;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SurroundedRegions;

// Harness only: both border strategies live in SurroundedRegionsSolution - this file
// pins each to boards whose every 'O' touches the border, so nothing is captured.
// Solve mutates its board in place, so each case carries the input and the expected
// post-mutation board, and each arm is handed its own clone of the input. LeetCode's
// published examples run in CatalogCaseTests.
public sealed partial class SurroundedRegionsSolutionTests
{
    public static TheoryData<char[][], char[][]> Cases =>
        new()
        {
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
    [MemberData(nameof(Cases))]
    public void SolveByBorderDepthFirstSearch_Cases_CapturesInteriorRegions(
        char[][] board, char[][] expected)
    {
        SurroundedRegionsSolution.SolveByBorderDepthFirstSearch(board);

        Assert.Equal(expected, board);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void SolveByBorderBreadthFirstSearch_Cases_CapturesInteriorRegions(
        char[][] board, char[][] expected)
    {
        SurroundedRegionsSolution.SolveByBorderBreadthFirstSearch(board);

        Assert.Equal(expected, board);
    }

    private static char[][] Clone(char[][] board) =>
        board.Select(row => (char[])row.Clone()).ToArray();
}
