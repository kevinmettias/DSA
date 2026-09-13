using DSAExperimentation.LeetCode.QueensThatCanAttackTheKing;

namespace DSAExperimentation.Tests.LeetCodeCoverage.QueensThatCanAttackTheKing;

// Harness only. Both strategies are QueensThatCanAttackTheKingSolution's - this file
// pins them to LeetCode's published examples on the problem's own 8x8 board.
// LeetCode does not fix the order of the returned coordinates, so both arms are
// compared after sorting by row then column.
public sealed class QueensThatCanAttackTheKingTests
{
    public static TheoryData<int[][], int[], int[][]> Examples =>
        new()
        {
            // LeetCode example 1: the king in the corner, attacked along the rank,
            // the file and the diagonal - the queens at [0,4] and [2,4] are shadowed.
            {
                [[0, 1], [1, 0], [4, 0], [0, 4], [3, 3], [2, 4]],
                [0, 0],
                [[0, 1], [1, 0], [3, 3]]
            },

            // LeetCode example 2: [1,1] is shadowed by [2,2] on the diagonal, and
            // [3,5]/[4,5] by [3,4]/[4,4].
            {
                [[0, 0], [1, 1], [2, 2], [3, 4], [3, 5], [4, 4], [4, 5]],
                [3, 3],
                [[2, 2], [3, 4], [4, 4]]
            },

            // No queen shares a rank, file or diagonal with the king: every ray runs
            // off the edge.
            {
                [[7, 0], [0, 7]],
                [3, 3],
                []
            },

            // A single adjacent queen, reached on the first step of one ray.
            {
                [[0, 0]],
                [0, 1],
                [[0, 0]]
            },

            // Every direction answered at once: the king boxed in by eight queens.
            {
                [[2, 2], [2, 3], [2, 4], [3, 2], [3, 4], [4, 2], [4, 3], [4, 4]],
                [3, 3],
                [[2, 2], [2, 3], [2, 4], [3, 2], [3, 4], [4, 2], [4, 3], [4, 4]]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void QueensAttackTheKingByLinearScan_LeetCodeExamples_ReturnsAttackingQueens(
        int[][] queens, int[] king, int[][] expected) =>
        AssertSameCoordinates(
            expected,
            QueensThatCanAttackTheKingSolution.QueensAttackTheKingByLinearScan(queens, king));

    [Theory]
    [MemberData(nameof(Examples))]
    public void QueensAttackTheKingBySetLookup_LeetCodeExamples_ReturnsAttackingQueens(
        int[][] queens, int[] king, int[][] expected) =>
        AssertSameCoordinates(
            expected,
            QueensThatCanAttackTheKingSolution.QueensAttackTheKingBySetLookup(queens, king));

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
