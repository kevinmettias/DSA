using DSAExperimentation.LeetCode.Solutions.GameOfLife;

namespace DSAExperimentation.LeetCode.Solutions.Tests.GameOfLife;

// Harness only. Both strategies are GameOfLifeSolution's - this file pins them to
// a 2x2 block, which is a still life, and three 3x3 boards that between them decide
// survival, overcrowding and birth. Each row is cloned before advancing so the two
// theory methods never share a mutated board. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class GameOfLifeSolutionTests
{
    public static TheoryData<int[][], int[][]> Cases =>
        new()
        {
            { [[1, 1], [1, 1]], [[1, 1], [1, 1]] },

            // A full 3x3 board: each corner keeps its three neighbours and survives, and every
            // other cell has five or eight and dies, so overcrowding is pinned too.
            { [[1, 1, 1], [1, 1, 1], [1, 1, 1]], [[1, 0, 1], [0, 0, 0], [1, 0, 1]] },

            // A plus: its centre has exactly four live neighbours and dies, each arm has three
            // and survives, and each empty corner has three and is born.
            { [[0, 1, 0], [1, 1, 1], [0, 1, 0]], [[1, 1, 1], [1, 0, 1], [1, 1, 1]] },

            // The empty centre has six live neighbours and stays empty: Conway's rule is born on
            // three alone, where a Life-like rule such as HighLife (B36/S23) would bring it to life.
            { [[1, 1, 1], [1, 0, 1], [1, 0, 0]], [[1, 0, 1], [1, 0, 1], [0, 1, 0]] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void AdvanceByFullBoardCopy_Cases_MatchesExpectedNextGeneration(
        int[][] board, int[][] expected)
    {
        var working = Clone(board);

        GameOfLifeSolution.AdvanceByFullBoardCopy(working);

        Assert.Equal(expected, working);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void AdvanceBySetSnapshot_Cases_MatchesExpectedNextGeneration(
        int[][] board, int[][] expected)
    {
        var working = Clone(board);

        GameOfLifeSolution.AdvanceBySetSnapshot(working);

        Assert.Equal(expected, working);
    }

    private static int[][] Clone(int[][] matrix) =>
        matrix.Select(row => (int[])row.Clone()).ToArray();
}
