using DSAExperimentation.LeetCode.FindWinnerOnATicTacToeGame;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindWinnerOnATicTacToeGame;

// Harness only. Both the full-board rescan and the running-count bookkeeping are
// FindWinnerOnATicTacToeGameSolution's; this file pins them to LeetCode's
// published examples plus the column and anti-diagonal wins the examples never
// exercise, so a regression names the strategy that broke.
public sealed class FindWinnerOnATicTacToeGameTests
{
    public static TheoryData<int[][], string> Examples =>
        new()
        {
            { [[0, 0], [2, 0], [1, 1], [2, 1], [2, 2]], "A" },
            { [[0, 0], [1, 0], [0, 1], [1, 1], [2, 0], [1, 2]], "B" },
            { [[0, 0], [1, 1], [2, 0], [1, 0], [1, 2], [2, 1], [0, 1], [0, 2], [2, 2]], "Draw" },
            { [[0, 0], [1, 1]], "Pending" },
            { [[0, 0], [0, 1], [1, 0], [1, 1], [2, 0]], "A" },
            { [[0, 2], [0, 0], [1, 1], [0, 1], [2, 0]], "A" },
            { [], "Pending" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindWinnerByBoardRescan_LeetCodeExamples_ReturnsTheWinnerOrTheGameState(
        int[][] moves, string expected) =>
        Assert.Equal(expected, FindWinnerOnATicTacToeGameSolution.FindWinnerByBoardRescan(moves));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindWinnerByRunningCounts_LeetCodeExamples_ReturnsTheWinnerOrTheGameState(
        int[][] moves, string expected) =>
        Assert.Equal(expected, FindWinnerOnATicTacToeGameSolution.FindWinnerByRunningCounts(moves));
}
