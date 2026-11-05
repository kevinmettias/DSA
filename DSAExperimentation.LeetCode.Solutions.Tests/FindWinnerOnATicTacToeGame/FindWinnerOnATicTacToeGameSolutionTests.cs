using DSAExperimentation.LeetCode.Solutions.FindWinnerOnATicTacToeGame;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindWinnerOnATicTacToeGame;

// Harness only. Both the full-board rescan and the running-count bookkeeping are
// FindWinnerOnATicTacToeGameSolution's; this file pins them to the row and column
// wins LeetCode's examples never exercise, an anti-diagonal win for A, two pending
// games (one with no moves at all), and a win on the ninth move, which must beat
// the full board, so a regression names the strategy that broke. LeetCode's
// published examples run in CatalogCaseTests.
public sealed partial class FindWinnerOnATicTacToeGameSolutionTests
{
    public static TheoryData<int[][], string> Cases =>
        new()
        {
            { [[0, 0], [1, 0], [0, 1], [1, 1], [2, 0], [1, 2]], "B" },
            { [[0, 0], [1, 1]], "Pending" },
            { [[0, 0], [0, 1], [1, 0], [1, 1], [2, 0]], "A" },
            { [[0, 2], [0, 0], [1, 1], [0, 1], [2, 0]], "A" },
            { [], "Pending" },
            { [[0, 0], [0, 1], [0, 2], [1, 0], [1, 1], [1, 2], [2, 1], [2, 0], [2, 2]], "A" },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindWinnerByBoardRescan_Cases_ReturnsTheWinnerOrTheGameState(
        int[][] moves, string expected) =>
        Assert.Equal(expected, FindWinnerOnATicTacToeGameSolution.FindWinnerByBoardRescan(moves));

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindWinnerByRunningCounts_Cases_ReturnsTheWinnerOrTheGameState(
        int[][] moves, string expected) =>
        Assert.Equal(expected, FindWinnerOnATicTacToeGameSolution.FindWinnerByRunningCounts(moves));
}
