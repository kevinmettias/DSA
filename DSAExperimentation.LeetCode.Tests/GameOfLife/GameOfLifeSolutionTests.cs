using DSAExperimentation.LeetCode.GameOfLife;

namespace DSAExperimentation.LeetCode.Tests.GameOfLife;

// Harness only. Both strategies are GameOfLifeSolution's - this file just pins
// them to LeetCode's published examples. Each row is cloned before advancing
// so the two theory methods never share a mutated board.
public sealed partial class GameOfLifeSolutionTests
{
    public static TheoryData<int[][], int[][]> Examples =>
        new()
        {
            {
                [[0, 1, 0], [0, 0, 1], [1, 1, 1], [0, 0, 0]],
                [[0, 0, 0], [1, 0, 1], [0, 1, 1], [0, 1, 0]]
            },
            { [[1, 1], [1, 1]], [[1, 1], [1, 1]] },

            // A full 3x3 board: each corner keeps its three neighbours and survives, and every
            // other cell has five or eight and dies, so overcrowding is pinned too.
            { [[1, 1, 1], [1, 1, 1], [1, 1, 1]], [[1, 0, 1], [0, 0, 0], [1, 0, 1]] },

            // A plus: its centre has exactly four live neighbours and dies, each arm has three
            // and survives, and each empty corner has three and is born.
            { [[0, 1, 0], [1, 1, 1], [0, 1, 0]], [[1, 1, 1], [1, 0, 1], [1, 1, 1]] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void AdvanceByFullBoardCopy_LeetCodeExamples_MatchesExpectedNextGeneration(
        int[][] board, int[][] expected)
    {
        var working = Clone(board);

        GameOfLifeSolution.AdvanceByFullBoardCopy(working);

        Assert.Equal(expected, working);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void AdvanceBySetSnapshot_LeetCodeExamples_MatchesExpectedNextGeneration(
        int[][] board, int[][] expected)
    {
        var working = Clone(board);

        GameOfLifeSolution.AdvanceBySetSnapshot(working);

        Assert.Equal(expected, working);
    }

    private static int[][] Clone(int[][] matrix) =>
        matrix.Select(row => (int[])row.Clone()).ToArray();
}
