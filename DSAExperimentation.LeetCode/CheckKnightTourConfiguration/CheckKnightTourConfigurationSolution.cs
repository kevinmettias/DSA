namespace DSAExperimentation.LeetCode.CheckKnightTourConfiguration;

// LeetCode 2596. Check Knight Tour Configuration: grid[row][col] is the move on
// which the knight reached that cell, and the configuration is valid when the
// knight starts at the top-left cell and every consecutive pair of moves is a
// single knight's move apart.
//
// Both strategies answer that by walking the moves in order; they differ only in
// how they find the cell a given move number sits on - rescan the board for it, or
// invert the board into a move -> cell lookup once up front.
//
// No repo Representation or Operations primitive applies to either arm. Grid /
// GridChildren models unordered single-step ORTHOGONAL adjacency for graph walks,
// not the two-over-one-across offsets a knight makes, and nothing here is a search
// in the first place: the move order is given, so this is fixed-shape index
// arithmetic - the same call AvailableCapturesForRook and TransposeMatrix make.
// The knight's own offsets stay beside this solution for the same reason
// MaximumNumberOfMovesToKillAllPawns keeps its KnightChildren witness beside its.
internal static class CheckKnightTourConfigurationSolution
{
    // A valid configuration starts at the top-left cell, so the cell holding move
    // zero is fixed by the problem rather than found.
    private const int FirstMove = 0;

    private static readonly (int DRow, int DCol)[] KnightOffsets =
        [(1, 2), (1, -2), (-1, 2), (-1, -2), (2, 1), (2, -1), (-2, 1), (-2, -1)];

    // The textbook answer: for every consecutive pair of move numbers, sweep the
    // whole board to find each one's cell. Deliberately BCL-only, and deliberately
    // paying O(n^2) per move for O(n^4) overall - it is the arm the single-pass
    // inversion below has to justify itself against.
    public static bool CheckValidGridByBoardRescan(int[][] grid)
    {
        if (!StartsAtTopLeft(grid))
        {
            return false;
        }

        var moves = grid.Length * grid.Length;

        for (var move = 0; move < moves - 1; move++)
        {
            var from = FindMove(grid, move);
            var to = FindMove(grid, move + 1);

            if (!IsKnightMove(from, to))
            {
                return false;
            }
        }

        return true;
    }

    private static (int Row, int Col) FindMove(int[][] grid, int move)
    {
        for (var row = 0; row < grid.Length; row++)
        {
            for (var col = 0; col < grid.Length; col++)
            {
                if (grid[row][col] == move)
                {
                    return (row, col);
                }
            }
        }

        throw new InvalidOperationException($"Move {move} is absent - LC 2596 guarantees a permutation of 0..n*n-1.");
    }

    // The board already IS the map from cell to move number; inverting it in one
    // O(n^2) pass gives the map from move number to cell, after which the walk is
    // a single linear scan over consecutive entries.
    public static bool CheckValidGridByPositionLookup(int[][] grid)
    {
        if (!StartsAtTopLeft(grid))
        {
            return false;
        }

        var positionByMove = InvertToPositionByMove(grid);

        for (var move = 0; move < positionByMove.Length - 1; move++)
        {
            if (!IsKnightMove(positionByMove[move], positionByMove[move + 1]))
            {
                return false;
            }
        }

        return true;
    }

    // The board already IS the map from cell to move number; this single O(n^2) pass
    // inverts it into the map from move number to cell the walk above reads.
    private static (int Row, int Col)[] InvertToPositionByMove(int[][] grid)
    {
        var n = grid.Length;
        var positionByMove = new (int Row, int Col)[n * n];

        for (var row = 0; row < n; row++)
        {
            for (var col = 0; col < n; col++)
            {
                positionByMove[grid[row][col]] = (row, col);
            }
        }

        return positionByMove;
    }

    private static bool StartsAtTopLeft(int[][] grid) => grid[0][0] == FirstMove;

    private static bool IsKnightMove((int Row, int Col) from, (int Row, int Col) to)
    {
        foreach (var (dRow, dCol) in KnightOffsets)
        {
            if (from.Row + dRow == to.Row && from.Col + dCol == to.Col)
            {
                return true;
            }
        }

        return false;
    }
}
