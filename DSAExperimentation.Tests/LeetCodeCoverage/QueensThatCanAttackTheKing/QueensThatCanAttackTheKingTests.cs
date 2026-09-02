using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.QueensThatCanAttackTheKing;

// LeetCode 1222. Queens That Can Attack the King: ray-walk each of the 8 queen-move
// directions outward from the king until the edge or the first queen is reached - the
// same Set<(int Row, int Col)> O(1)-membership primitive MinimumAreaRectangleTests
// uses for corner bookkeeping, applied here so each ray step is an O(1) lookup instead
// of a linear rescan of the queens array. No repo Topology primitive fits the ray-cast
// shape itself: Grid/GridChildren model unordered single-step 4-directional adjacency
// for graph walks, not an 8-directional stop-at-first-blocker ray cast - the same
// reasoning AvailableCapturesForRookTests already gives for its own 4-direction rook
// version of this shape, extended here to the queen's 8 directions.
public sealed class QueensThatCanAttackTheKingTests
{
    private static readonly (int DRow, int DCol)[] Directions =
    [
        (-1, -1), (-1, 0), (-1, 1),
        (0, -1), (0, 1),
        (1, -1), (1, 0), (1, 1),
    ];

    [Fact]
    public void QueensAttackTheKing_LeetCodeExampleOne_ReturnsThreeAttackingQueens()
    {
        int[][] queens = [[0, 1], [1, 0], [4, 0], [0, 4], [3, 3], [2, 4]];
        int[] king = [0, 0];

        var attackers = QueensAttackTheKing(queens, boardSize: 8, king);

        AssertSameCoordinates([[0, 1], [1, 0], [3, 3]], attackers);
    }

    [Fact]
    public void QueensAttackTheKing_LeetCodeExampleTwo_ReturnsThreeAttackingQueens()
    {
        int[][] queens = [[0, 0], [1, 1], [2, 2], [3, 4], [3, 5], [4, 4], [4, 5]];
        int[] king = [3, 3];

        var attackers = QueensAttackTheKing(queens, boardSize: 8, king);

        AssertSameCoordinates([[2, 2], [3, 4], [4, 4]], attackers);
    }

    [Fact]
    public void QueensAttackTheKing_NoQueenAlignedWithKing_ReturnsEmpty()
    {
        int[][] queens = [[7, 0], [0, 7]];
        int[] king = [3, 3];

        var attackers = QueensAttackTheKing(queens, boardSize: 8, king);

        Assert.Empty(attackers);
    }

    private static List<(int Row, int Col)> QueensAttackTheKing(int[][] queens, int boardSize, int[] king)
    {
        var occupied = new Set<(int Row, int Col)>();

        foreach (var queen in queens)
        {
            occupied.TryAdd((queen[0], queen[1]));
        }

        var attackers = new List<(int Row, int Col)>();

        foreach (var direction in Directions)
        {
            var attacker = FindAttackerAlongRay(king, direction, boardSize, occupied);

            if (attacker is not null)
            {
                attackers.Add(attacker.Value);
            }
        }

        return attackers;
    }

    private static (int Row, int Col)? FindAttackerAlongRay(
        int[] king, (int DRow, int DCol) direction, int boardSize, Set<(int Row, int Col)> occupied)
    {
        var row = king[0] + direction.DRow;
        var col = king[1] + direction.DCol;

        while (IsInBounds(row, col, boardSize) && !occupied.Has((row, col)))
        {
            row += direction.DRow;
            col += direction.DCol;
        }

        return IsInBounds(row, col, boardSize) ? (row, col) : null;
    }

    private static bool IsInBounds(int row, int col, int boardSize) =>
        row >= 0 && row < boardSize && col >= 0 && col < boardSize;

    private static void AssertSameCoordinates(int[][] expected, List<(int Row, int Col)> actual)
    {
        var expectedSorted = expected
            .Select(e => (Row: e[0], Col: e[1]))
            .OrderBy(p => p.Row).ThenBy(p => p.Col)
            .ToList();
        var actualSorted = actual.OrderBy(p => p.Row).ThenBy(p => p.Col).ToList();

        Assert.Equal(expectedSorted, actualSorted);
    }
}
