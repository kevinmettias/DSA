using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.QueensThatCanAttackTheKing;

// LeetCode 1222. Queens That Can Attack the King: which queens can reach the king
// in one move - the first queen met walking outward along each of the 8 queen-move
// directions, since any queen behind that one is blocked by it.
//
// No repo Topology primitive fits the ray-cast shape itself: Grid/GridChildren
// model unordered single-step 4-directional adjacency for graph walks, not an
// 8-directional stop-at-first-blocker ray cast - the same judgement
// AvailableCapturesForRookSolution already records for its own 4-direction rook
// version of this shape, extended here to the queen's 8 directions.
//
// The two strategies walk identical rays and differ only in how "is this square
// occupied" is answered per step: rescan the raw queens array (O(queens) a step),
// or build this repo's own Set<(int Row, int Col)> once up front and answer each
// step in O(1) - the same "swap a linear rescan for a hash lookup" move
// MinimumAreaRectangleSolution and TwoSumSolution both make. Building that set is
// deliberately inside the measured strategy rather than hoisted, because paying
// O(queens) once is precisely what the arm has to earn back.
internal static class QueensThatCanAttackTheKingSolution
{
    private static readonly (int DRow, int DCol)[] Directions =
    [
        (-1, -1), (-1, 0), (-1, 1),
        (0, -1), (0, 1),
        (1, -1), (1, 0), (1, 1),
    ];

    // The textbook baseline: no auxiliary index at all - every step of every ray
    // asks the queens array directly whether some queen sits on this square.
    // Deliberately written without this repo's primitives; it is the arm the set
    // lookup below has to justify itself against.
    public static List<(int Row, int Col)> QueensAttackTheKingByLinearScan(int[][] queens, int[] king) =>
        QueensAttackTheKingByLinearScan(queens, KingBoard.Standard(king));

    public static List<(int Row, int Col)> QueensAttackTheKingByLinearScan(int[][] queens, KingBoard board)
    {
        var attackers = new List<(int Row, int Col)>();

        foreach (var direction in Directions)
        {
            var row = board.KingRow + direction.DRow;
            var col = board.KingCol + direction.DCol;

            while (board.IsInBounds(row, col) && !IsQueen(queens, row, col))
            {
                row += direction.DRow;
                col += direction.DCol;
            }

            if (board.IsInBounds(row, col))
            {
                attackers.Add((row, col));
            }
        }

        return attackers;
    }

    private static bool IsQueen(int[][] queens, int row, int col)
    {
        foreach (var queen in queens)
        {
            if (queen[0] == row && queen[1] == col)
            {
                return true;
            }
        }

        return false;
    }

    // One pass over the queens into a Set<(int Row, int Col)>, after which each of
    // the at most 8*boardSize ray steps is an O(1) membership test.
    public static List<(int Row, int Col)> QueensAttackTheKingBySetLookup(int[][] queens, int[] king) =>
        QueensAttackTheKingBySetLookup(queens, KingBoard.Standard(king));

    public static List<(int Row, int Col)> QueensAttackTheKingBySetLookup(int[][] queens, KingBoard board)
    {
        var occupied = new Set<(int Row, int Col)>();

        foreach (var queen in queens)
        {
            occupied.TryAdd((queen[0], queen[1]));
        }

        var attackers = new List<(int Row, int Col)>();

        foreach (var direction in Directions)
        {
            var attacker = FindAttackerAlongRay(occupied, board, direction);

            if (attacker is not null)
            {
                attackers.Add(attacker.Value);
            }
        }

        return attackers;
    }

    private static (int Row, int Col)? FindAttackerAlongRay(
        Set<(int Row, int Col)> occupied, KingBoard board, (int DRow, int DCol) direction)
    {
        var row = board.KingRow + direction.DRow;
        var col = board.KingCol + direction.DCol;

        while (board.IsInBounds(row, col) && !occupied.Has((row, col)))
        {
            row += direction.DRow;
            col += direction.DCol;
        }

        return board.IsInBounds(row, col) ? (row, col) : null;
    }
}
