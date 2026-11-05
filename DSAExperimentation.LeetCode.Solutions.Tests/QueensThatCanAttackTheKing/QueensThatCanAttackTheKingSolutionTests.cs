using DSAExperimentation.LeetCode.Solutions.QueensThatCanAttackTheKing;

namespace DSAExperimentation.LeetCode.Solutions.Tests.QueensThatCanAttackTheKing;

// Harness only. Both strategies are QueensThatCanAttackTheKingSolution's - this file
// pins them to positions on the problem's own 8x8 board: no queen in line with the
// king, a single adjacent queen, and a king boxed in by eight queens. LeetCode does
// not fix the order of the returned coordinates, so both arms are compared after
// sorting by row then column. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class QueensThatCanAttackTheKingSolutionTests
{
    public static TheoryData<int[][], int[], int[][]> Cases =>
        new()
        {
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
    [MemberData(nameof(Cases))]
    public void QueensAttackTheKingByLinearScan_Cases_ReturnsAttackingQueens(
        int[][] queens, int[] king, int[][] expected)
    {
        var attacking = QueensThatCanAttackTheKingSolution.QueensAttackTheKingByLinearScan(queens, king);

        AssertSameCoordinates(expected, attacking);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void QueensAttackTheKingBySetLookup_Cases_ReturnsAttackingQueens(
        int[][] queens, int[] king, int[][] expected)
    {
        var attacking = QueensThatCanAttackTheKingSolution.QueensAttackTheKingBySetLookup(queens, king);

        AssertSameCoordinates(expected, attacking);
    }

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
