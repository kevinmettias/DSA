using DSAExperimentation.LeetCode.EscapeALargeMaze;

namespace DSAExperimentation.Tests.LeetCodeCoverage.EscapeALargeMaze;

// Harness only. Both strategies are EscapeALargeMazeSolution's.
//
// Two example sets, because the two strategies do not fit on the same board: the
// capped traversal runs on LC 1036's real 10^6 x 10^6 board, while the full-board
// flood fill allocates boardSize^2 cells and can only be asserted on a reduced
// board. The reduced set is the same five scenarios, so the capped traversal is
// pinned to it as well and the two strategies are shown to agree cell for cell.
public sealed class EscapeALargeMazeTests
{
    private const int ReducedBoardSize = 500;
    private const int ReducedBoardMaxCoordinate = ReducedBoardSize - 1;
    private const int LeetCodeBoardMaxCoordinate = EscapeALargeMazeSolution.LeetCodeBoardSize - 1;

    // blocked, source, target, canEscape - on LC 1036's own 10^6 x 10^6 board.
    public static TheoryData<int[][], int[], int[], bool> Examples =>
        new()
        {
            // LC example 1: the source is walled into its corner by two cells.
            { [[0, 1], [1, 0]], [0, 0], [0, 2], false },

            // LC example 2: nothing blocked, so opposite corners connect.
            { [], [0, 0], [LeetCodeBoardMaxCoordinate, LeetCodeBoardMaxCoordinate], true },

            // A three-cell wall one row below the source with a gap at column 5, so
            // the source slips through into open board rather than being sealed in.
            { [[1, 3], [1, 4], [1, 6]], [0, 5], [50, 50], true },

            // The far corner is the sealed one this time: the search from the source
            // escapes, and only the second, target-side search rejects the pair.
            {
                [[LeetCodeBoardMaxCoordinate - 1, LeetCodeBoardMaxCoordinate], [LeetCodeBoardMaxCoordinate, LeetCodeBoardMaxCoordinate - 1]],
                [0, 0],
                [LeetCodeBoardMaxCoordinate, LeetCodeBoardMaxCoordinate],
                false
            },

            // One blocked cell can never wall anything off.
            { [[0, 1]], [0, 0], [5, 5], true },
        };

    // The same scenarios on a board small enough to materialize.
    public static TheoryData<int[][], int[], int[], bool> ReducedBoardExamples =>
        new()
        {
            { [[0, 1], [1, 0]], [0, 0], [0, 2], false },
            { [], [0, 0], [ReducedBoardMaxCoordinate, ReducedBoardMaxCoordinate], true },
            { [[1, 3], [1, 4], [1, 6]], [0, 5], [50, 50], true },
            {
                [[ReducedBoardMaxCoordinate - 1, ReducedBoardMaxCoordinate], [ReducedBoardMaxCoordinate, ReducedBoardMaxCoordinate - 1]],
                [0, 0],
                [ReducedBoardMaxCoordinate, ReducedBoardMaxCoordinate],
                false
            },
            { [[0, 1]], [0, 0], [5, 5], true },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanEscapeByCappedTraversal_LeetCodeExamples_ReturnsWhetherSourceReachesTarget(
        int[][] blocked, int[] source, int[] target, bool expected) =>
        Assert.Equal(expected, EscapeALargeMazeSolution.CanEscapeByCappedTraversal(blocked, source, target));

    [Theory]
    [MemberData(nameof(ReducedBoardExamples))]
    public void CanEscapeByCappedTraversal_ReducedBoard_AgreesWithTheFullBoardFloodFill(
        int[][] blocked, int[] source, int[] target, bool expected) =>
        Assert.Equal(
            expected,
            EscapeALargeMazeSolution.CanEscapeByCappedTraversal(blocked, source, target, ReducedBoardSize));

    [Theory]
    [MemberData(nameof(ReducedBoardExamples))]
    public void CanEscapeByFullBoardFloodFill_ReducedBoard_ReturnsWhetherSourceReachesTarget(
        int[][] blocked, int[] source, int[] target, bool expected) =>
        Assert.Equal(
            expected,
            EscapeALargeMazeSolution.CanEscapeByFullBoardFloodFill(blocked, source, target, ReducedBoardSize));
}
