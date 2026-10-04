using DSAExperimentation.LeetCode.MaximumNumberOfMovesToKillAllPawns;

namespace DSAExperimentation.LeetCode.Tests.MaximumNumberOfMovesToKillAllPawns;

// Harness only: both strategies are MaximumNumberOfMovesToKillAllPawnsSolution's -
// this file pins them to LeetCode's published examples, and the knight-distance
// matrix the Reduce.Graph strategy is handed to distances counted by hand.
public sealed partial class MaximumNumberOfMovesToKillAllPawnsSolutionTests
{
    public static TheoryData<int, int, int[][], int> Examples =>
        new()
        {
            { 1, 1, new[] { new[] { 0, 0 } }, 4 },
            { 0, 2, new[] { new[] { 1, 1 }, new[] { 2, 2 }, new[] { 3, 3 } }, 8 },
            { 0, 0, new[] { new[] { 1, 2 }, new[] { 2, 4 } }, 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxMovesByBruteForceMinimax_LeetCodeExamples_ReturnsOptimalAlternatingCaptureMoveTotal(
        int kx, int ky, int[][] positions, int expected)
    {
        var actual = MaximumNumberOfMovesToKillAllPawnsSolution.MaxMovesByBruteForceMinimax(kx, ky, positions);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxMovesByReduceGraphMinimax_LeetCodeExamples_ReturnsOptimalAlternatingCaptureMoveTotal(
        int kx, int ky, int[][] positions, int expected)
    {
        var actual = MaximumNumberOfMovesToKillAllPawnsSolution.MaxMovesByReduceGraphMinimax(kx, ky, positions);

        Assert.Equal(expected, actual);
    }

    // LeetCode's third example: the knight at (0,0) is index 0, the pawns (1,2) and (2,4)
    // indices 1 and 2. (0,0) -> (1,2) and (1,2) -> (2,4) are one knight move each; (0,0) ->
    // (2,4) is not a knight offset, so it takes two, through (1,2).
    [Fact]
    public void BuildKnightDistances_LeetCodeThirdExample_CountsKnightMovesBetweenEveryPairOfPoints()
    {
        var knightDistances = MaximumNumberOfMovesToKillAllPawnsSolution.BuildKnightDistances(0, 0, [[1, 2], [2, 4]]);
        var rows = RowsOf(knightDistances.Distances);

        Assert.Equal(2, knightDistances.PawnCount);
        Assert.Equal([[0, 1, 2], [1, 0, 1], [2, 1, 0]], rows);
    }

    // LeetCode's first example: from (1,1) the knight needs 4 moves to reach the pawn in
    // the corner at (0,0), as LeetCode's own explanation counts them - the board's edge
    // rules out the two-move route an unbounded board would allow.
    [Fact]
    public void BuildKnightDistances_LeetCodeFirstExample_StaysOnTheBoard()
    {
        var knightDistances = MaximumNumberOfMovesToKillAllPawnsSolution.BuildKnightDistances(1, 1, [[0, 0]]);
        var rows = RowsOf(knightDistances.Distances);

        Assert.Equal(1, knightDistances.PawnCount);
        Assert.Equal([[0, 4], [4, 0]], rows);
    }

    private static int[][] RowsOf(int[,] matrix) =>
        [.. Enumerable.Range(0, matrix.GetLength(0)).Select(row => RowOf(matrix, row))];

    private static int[] RowOf(int[,] matrix, int row) =>
        [.. Enumerable.Range(0, matrix.GetLength(1)).Select(col => matrix[row, col])];
}
