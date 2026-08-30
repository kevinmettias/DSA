namespace DSAExperimentation.Tests.LeetCodeCoverage.AvailableCapturesForRook;

// LeetCode 999. Available Captures for Rook: walk outward from the rook in each
// of the four orthogonal directions until the edge, a bishop ('B', blocks - no
// capture), or a pawn ('p', captured, stop) is reached. The same "no repo
// Representation/Operations primitive to compose" shape TransposeMatrix/
// SpiralMatrix already establish for fixed-shape grid index arithmetic -
// Grid/GridChildren model unordered single-step orthogonal adjacency for graph
// walks, not a directional ray cast that stops at the first blocker, so forcing
// this through Grid/** would not be a genuine fit.
public sealed class AvailableCapturesForRookTests
{
    private static readonly (int DRow, int DCol)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    [Fact]
    public void NumRookCaptures_TwoReachablePawnsAndOnePastTheEdge_ReturnsThree()
    {
        var board = ParseBoard([
            "........",
            "...p....",
            "...R...p",
            "........",
            "........",
            "...p....",
            "........",
            "........",
        ]);

        Assert.Equal(3, NumRookCaptures(board));
    }

    [Fact]
    public void NumRookCaptures_RookBoxedInByBishopsOnAllFourSides_ReturnsZero()
    {
        var board = ParseBoard([
            "........",
            ".ppppp..",
            ".ppBpp..",
            ".pBRBp..",
            ".ppBpp..",
            ".ppppp..",
            "........",
            "........",
        ]);

        Assert.Equal(0, NumRookCaptures(board));
    }

    private static int NumRookCaptures(char[][] board)
    {
        var (rookRow, rookCol) = FindRook(board);
        var captures = 0;

        foreach (var (dRow, dCol) in Directions)
        {
            var row = rookRow + dRow;
            var col = rookCol + dCol;

            while (IsInBounds(board, row, col) && board[row][col] == '.')
            {
                row += dRow;
                col += dCol;
            }

            if (IsInBounds(board, row, col) && board[row][col] == 'p')
            {
                captures++;
            }
        }

        return captures;
    }

    private static bool IsInBounds(char[][] board, int row, int col) =>
        row >= 0 && row < board.Length && col >= 0 && col < board[0].Length;

    private static (int Row, int Col) FindRook(char[][] board)
    {
        for (var row = 0; row < board.Length; row++)
        {
            for (var col = 0; col < board[0].Length; col++)
            {
                if (board[row][col] == 'R')
                {
                    return (row, col);
                }
            }
        }

        throw new InvalidOperationException("Board has no rook.");
    }

    private static char[][] ParseBoard(string[] rows) => rows.Select(r => r.ToCharArray()).ToArray();
}
