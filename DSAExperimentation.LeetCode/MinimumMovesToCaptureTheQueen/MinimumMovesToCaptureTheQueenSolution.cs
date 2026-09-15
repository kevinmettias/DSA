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
// container - ChessSquare is the whole domain, and it lives beside this solution
// the way RookSquare already does for LC 999.
internal static class MinimumMovesToCaptureTheQueenSolution
{
    private static readonly (int DeltaRow, int DeltaCol)[] RookDirections = [(1, 0), (-1, 0), (0, 1), (0, -1)];
    private static readonly (int DeltaRow, int DeltaCol)[] BishopDirections = [(1, 1), (1, -1), (-1, 1), (-1, -1)];

    // The textbook baseline: simulate every square each sliding piece could
    // actually land on - walking outward in all 4 legal directions until the
    // board edge or the other piece blocks the ray - and check whether the queen
    // is among them. The arm the direct line-of-sight check below has to beat.
    public static int MinMovesByDestinationEnumeration(ChessSquare rook, ChessSquare bishop, ChessSquare queen)
    {
        if (CanReachAnySquare(rook, RookDirections, bishop, queen))
        {
            return 1;
        }

        if (CanReachAnySquare(bishop, BishopDirections, rook, queen))
        {
            return 1;
        }

        return 2;
    }

    private static bool CanReachAnySquare(
        ChessSquare from, (int DeltaRow, int DeltaCol)[] directions, ChessSquare blocker, ChessSquare target)
    {
        foreach (var direction in directions)
        {
            if (RayReachesTarget(from, blocker, target, direction))
            {
                return true;
            }
        }

        return false;
    }

    // One ray outward from the piece, stepping until the board edge or the blocking
    // piece stops it: does it stand on the target before either of those happens?
    private static bool RayReachesTarget(
        ChessSquare from, ChessSquare blocker, ChessSquare target, (int DeltaRow, int DeltaCol) direction)
    {
        var row = from.Row + direction.DeltaRow;
        var col = from.Col + direction.DeltaCol;

        while (row is >= 1 and <= 8 && col is >= 1 and <= 8)
        {
            if (row == target.Row && col == target.Col)
            {
                return true;
            }

            if (row == blocker.Row && col == blocker.Col)
            {
                break;
            }

            row += direction.DeltaRow;
            col += direction.DeltaCol;
        }

        return false;
    }

    // Rather than enumerate every reachable square, walk only the one segment
    // that matters - straight from the piece to the queen - and check whether the
    // other piece sits strictly between them.
    public static int MinMovesByLineOfSight(ChessSquare rook, ChessSquare bishop, ChessSquare queen)
    {
        if (CanCaptureDirectly(rook, bishop, queen, SlidingLine.Orthogonal))
        {
            return 1;
        }

        if (CanCaptureDirectly(bishop, rook, queen, SlidingLine.Diagonal))
        {
            return 1;
        }

        return 2;
    }

    private static bool CanCaptureDirectly(
        ChessSquare piece, ChessSquare blocker, ChessSquare queen, SlidingLine line)
    {
        var offset = (DeltaRow: queen.Row - piece.Row, DeltaCol: queen.Col - piece.Col);

        if (!IsOnSlidingLine(offset.DeltaRow, offset.DeltaCol, line))
        {
            return false;
        }

        return PathIsClear(piece, blocker, queen, offset);
    }

    // Whether a queen this far away sits on the line the piece slides along: a
    // diagonal line needs equal, non-zero row and column distances, and an
    // orthogonal one exactly one of them zero.
    private static bool IsOnSlidingLine(int deltaRow, int deltaCol, SlidingLine line)
    {
        if (line == SlidingLine.Diagonal)
        {
            return deltaRow != 0 && Math.Abs(deltaRow) == Math.Abs(deltaCol);
        }

        return (deltaRow == 0) ^ (deltaCol == 0);
    }

    // The queen is already known to sit on the piece's own line, so the only question
    // left is whether the other piece stands strictly between them: step the one
    // segment from the piece toward the queen, and stop at the first thing that is not
    // the queen herself.
    private static bool PathIsClear(
        ChessSquare piece, ChessSquare blocker, ChessSquare queen, (int DeltaRow, int DeltaCol) offset)
    {
        var (deltaRow, deltaCol) = offset;
        var stepRow = Math.Sign(deltaRow);
        var stepCol = Math.Sign(deltaCol);
        var row = piece.Row + stepRow;
        var col = piece.Col + stepCol;

        while (row != queen.Row || col != queen.Col)
        {
            if (row == blocker.Row && col == blocker.Col)
            {
                return false;
            }

            row += stepRow;
            col += stepCol;
        }

        return true;
    }

    // Which lines a piece can slide along: a rook only ever travels orthogonally, a
    // bishop only diagonally, and the caller naming its piece's line says which is
    // meant - where a bare true/false at the call site said it only by position.
    private enum SlidingLine
    {
        Orthogonal,
        Diagonal,
    }
}
