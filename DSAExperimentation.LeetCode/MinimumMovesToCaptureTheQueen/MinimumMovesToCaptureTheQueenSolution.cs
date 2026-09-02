namespace DSAExperimentation.LeetCode.MinimumMovesToCaptureTheQueen;

// LeetCode 3001. Minimum Moves to Capture The Queen: a 1-indexed 8x8 board holds a
// white rook (a, b), a white bishop (c, d), and a stationary black queen (e, f).
// Rooks slide any distance orthogonally, bishops slide any distance diagonally,
// neither can jump the other piece. The answer is always 1 (either piece captures
// immediately) or 2 (move the blocker, or the other piece, first).
//
// Both strategies answer the same question with the same signature, so the test
// harness can assert they agree and the benchmark harness can time them against
// each other without either restating the algorithm. Neither needs a repo
// container - the whole domain is six bounded coordinates, the same "plain
// arithmetic, no shared type to reuse" shape TwoSum's own two arms already have.
internal static class MinimumMovesToCaptureTheQueenSolution
{
    private static readonly (int DeltaRow, int DeltaCol)[] RookDirections = [(1, 0), (-1, 0), (0, 1), (0, -1)];
    private static readonly (int DeltaRow, int DeltaCol)[] BishopDirections = [(1, 1), (1, -1), (-1, 1), (-1, -1)];

    // The textbook baseline: simulate every square each sliding piece could
    // actually land on - walking outward in all 4 legal directions until the
    // board edge or the other piece blocks the ray - and check whether the queen
    // is among them. The arm the direct line-of-sight check below has to beat.
    public static int MinMovesByDestinationEnumeration(int a, int b, int c, int d, int e, int f)
    {
        if (CanReachAnySquare(a, b, RookDirections, c, d, e, f))
        {
            return 1;
        }

        if (CanReachAnySquare(c, d, BishopDirections, a, b, e, f))
        {
            return 1;
        }

        return 2;
    }

    private static bool CanReachAnySquare(
        int fromRow, int fromCol, (int DeltaRow, int DeltaCol)[] directions,
        int blockerRow, int blockerCol, int targetRow, int targetCol)
    {
        foreach (var (deltaRow, deltaCol) in directions)
        {
            var row = fromRow + deltaRow;
            var col = fromCol + deltaCol;

            while (row is >= 1 and <= 8 && col is >= 1 and <= 8)
            {
                if (row == targetRow && col == targetCol)
                {
                    return true;
                }

                if (row == blockerRow && col == blockerCol)
                {
                    break;
                }

                row += deltaRow;
                col += deltaCol;
            }
        }

        return false;
    }

    // Rather than enumerate every reachable square, walk only the one segment
    // that matters - straight from the piece to the queen - and check whether the
    // other piece sits strictly between them.
    public static int MinMovesByLineOfSight(int a, int b, int c, int d, int e, int f)
    {
        if (CanCaptureDirectly(a, b, c, d, e, f, diagonal: false))
        {
            return 1;
        }

        if (CanCaptureDirectly(c, d, a, b, e, f, diagonal: true))
        {
            return 1;
        }

        return 2;
    }

    private static bool CanCaptureDirectly(
        int pieceRow, int pieceCol, int blockerRow, int blockerCol, int queenRow, int queenCol, bool diagonal)
    {
        var deltaRow = queenRow - pieceRow;
        var deltaCol = queenCol - pieceCol;

        var onLine = diagonal
            ? deltaRow != 0 && Math.Abs(deltaRow) == Math.Abs(deltaCol)
            : (deltaRow == 0) ^ (deltaCol == 0);

        if (!onLine)
        {
            return false;
        }

        var stepRow = Math.Sign(deltaRow);
        var stepCol = Math.Sign(deltaCol);
        var row = pieceRow + stepRow;
        var col = pieceCol + stepCol;

        while (row != queenRow || col != queenCol)
        {
            if (row == blockerRow && col == blockerCol)
            {
                return false;
            }

            row += stepRow;
            col += stepCol;
        }

        return true;
    }
}
