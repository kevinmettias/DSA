namespace DSAExperimentation.LeetCode.AvailableCapturesForRook;

// LeetCode 999. Available Captures for Rook: how many pawns the single white rook
// can capture in one move - walking outward until the edge, a bishop ('B', which
// blocks), or a pawn ('p', which is captured and stops the walk).
//
// No repo Representation/Operations primitive is composed here, and that is a
// judgement rather than an omission: Grid/GridNode/GridChildren model unordered
// single-step orthogonal adjacency for graph walks, whereas this problem is a
// directional ray cast that stops at the first non-empty square, so routing it
// through Grid/** would not be a genuine fit - the same shape TransposeMatrix and
// SpiralMatrix already establish for fixed-shape grid index arithmetic.
//
// The two strategies are therefore both plain board walks, and the whole point of
// the pair is the cost difference: scan every square looking for a pawn that
// shares the rook's row or column and then path-check it (O(rows*cols)), versus
// four rays out of the rook that only ever touch squares it could reach
// (O(rows+cols)).
internal static class AvailableCapturesForRookSolution
{
    private const char Empty = '.';
    private const char Pawn = 'p';
    private const char Rook = 'R';

    private static readonly (int DRow, int DCol)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    // The textbook baseline: look at every square on the board, and for each pawn
    // that happens to share a line with the rook, confirm nothing stands between
    // them. Deliberately pays for the whole board even though only two lines of it
    // can ever matter - it is the arm the ray walk has to justify itself against.
    public static int NumRookCapturesByFullBoardScan(char[][] board) =>
        NumRookCapturesByFullBoardScan(board, FindRook(board));

    public static int NumRookCapturesByFullBoardScan(char[][] board, RookSquare rook)
    {
        var captures = 0;

        for (var row = 0; row < board.Length; row++)
        {
            for (var col = 0; col < board[0].Length; col++)
            {
                if (!IsCaptureCandidate(board, row, col, rook))
                {
                    continue;
                }

                if (IsPathClear(board, new PathSegment(rook.Row, rook.Col, row, col)))
                {
                    captures++;
                }
            }
        }

        return captures;
    }

    // One ray per direction: advance while the square is empty, then capture if the
    // square that stopped the walk is a pawn. A bishop stops the walk without
    // scoring, and running off the edge stops it too.
    public static int NumRookCapturesByRayWalk(char[][] board) =>
        NumRookCapturesByRayWalk(board, FindRook(board));

    public static int NumRookCapturesByRayWalk(char[][] board, RookSquare rook)
    {
        var captures = 0;

        foreach (var (dRow, dCol) in Directions)
        {
            var row = rook.Row + dRow;
            var col = rook.Col + dCol;

            while (IsInBounds(board, row, col) && board[row][col] == Empty)
            {
                row += dRow;
                col += dCol;
            }

            if (IsInBounds(board, row, col) && board[row][col] == Pawn)
            {
                captures++;
            }
        }

        return captures;
    }

    private readonly record struct PathSegment(int FromRow, int FromCol, int ToRow, int ToCol);

    private static bool IsPathClear(char[][] board, PathSegment segment)
    {
        var dRow = Math.Sign(segment.ToRow - segment.FromRow);
        var dCol = Math.Sign(segment.ToCol - segment.FromCol);
        var row = segment.FromRow + dRow;
        var col = segment.FromCol + dCol;

        while (row != segment.ToRow || col != segment.ToCol)
        {
            if (board[row][col] != Empty)
            {
                return false;
            }

            row += dRow;
            col += dCol;
        }

        return true;
    }

    private static bool IsInBounds(char[][] board, int row, int col) =>
        row >= 0 && row < board.Length && col >= 0 && col < board[0].Length;

    // Only a pawn sharing the rook's row or column can ever be captured, so the full
    // board scan drops every other square before it pays for the path check.
    private static bool IsCaptureCandidate(char[][] board, int row, int col, RookSquare rook) =>
        board[row][col] == Pawn && (row == rook.Row || col == rook.Col);

    // LeetCode guarantees exactly one rook, so a board without one is a caller
    // error rather than an answerable input.
    private static RookSquare FindRook(char[][] board)
    {
        for (var row = 0; row < board.Length; row++)
        {
            for (var col = 0; col < board[0].Length; col++)
            {
                if (board[row][col] == Rook)
                {
                    return new RookSquare(row, col);
                }
            }
        }

        throw new InvalidOperationException("Board has no rook.");
    }
}
