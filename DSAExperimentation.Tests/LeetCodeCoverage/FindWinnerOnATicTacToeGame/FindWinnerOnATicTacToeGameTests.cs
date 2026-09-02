namespace DSAExperimentation.Tests.LeetCodeCoverage.FindWinnerOnATicTacToeGame;

// LeetCode 1275. Find Winner on a Tic Tac Toe Game: plain fixed-size (3-row,
// 3-column, 2-diagonal) running-count arrays, updated by +/-1 per move and checked
// against the board size - the same "no stronger reusable repo primitive beyond
// ordinary array bookkeeping" shape JumpGameTests already documents. This problem's
// optimal solution is O(1) extra space by nature (Grid/GridNode model pathfinding
// adjacency for graph walks, not score tracking, so reaching for them here would not
// be a genuine fit), so there is no data structure to compose - only the algorithm.
public sealed class FindWinnerOnATicTacToeGameTests
{
    [Fact]
    public void FindWinner_DiagonalWinForFirstPlayer_ReturnsA()
    {
        int[][] moves = [[0, 0], [2, 0], [1, 1], [2, 1], [2, 2]];

        Assert.Equal("A", FindWinner(moves));
    }

    [Fact]
    public void FindWinner_FullRowForSecondPlayer_ReturnsB()
    {
        int[][] moves = [[0, 0], [1, 0], [0, 1], [1, 1], [2, 0], [1, 2]];

        Assert.Equal("B", FindWinner(moves));
    }

    [Fact]
    public void FindWinner_BoardFillsWithNoWinner_ReturnsDraw()
    {
        int[][] moves = [[0, 0], [1, 1], [2, 0], [1, 0], [1, 2], [2, 1], [0, 1], [0, 2], [2, 2]];

        Assert.Equal("Draw", FindWinner(moves));
    }

    [Fact]
    public void FindWinner_FewerThanNineMovesAndNoWinnerYet_ReturnsPending()
    {
        int[][] moves = [[0, 0], [1, 1]];

        Assert.Equal("Pending", FindWinner(moves));
    }

    private static string FindWinner(int[][] moves)
    {
        var board = new TicTacToeBoard();

        for (var i = 0; i < moves.Length; i++)
        {
            var row = moves[i][0];
            var col = moves[i][1];
            var delta = i % 2 == 0 ? 1 : -1;

            if (board.ApplyMove(row, col, delta))
            {
                return delta == 1 ? "A" : "B";
            }
        }

        return moves.Length == 9 ? "Draw" : "Pending";
    }

    private sealed class TicTacToeBoard
    {
        private readonly int[] _rowCount = new int[3];
        private readonly int[] _colCount = new int[3];
        private int _diag;
        private int _antiDiag;

        public bool ApplyMove(int row, int col, int delta)
        {
            _rowCount[row] += delta;
            _colCount[col] += delta;
            _diag += row == col ? delta : 0;
            _antiDiag += row + col == 2 ? delta : 0;

            return Math.Abs(_rowCount[row]) == 3 || Math.Abs(_colCount[col]) == 3
                || Math.Abs(_diag) == 3 || Math.Abs(_antiDiag) == 3;
        }
    }
}
