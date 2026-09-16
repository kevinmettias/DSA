using DSAExperimentation.LeetCode.EscapeALargeMaze;

namespace DSAExperimentation.Tests.LeetCodeCoverage.EscapeALargeMaze;

// Harness only. Both strategies are EscapeALargeMazeSolution's.
//
// Two example sets, because the two strategies do not fit on the same board: the
// capped traversal runs on LC 1036's real 10^6 x 10^6 board, while the full-board
// flood fill allocates boardSize^2 cells and can only be asserted on a reduced
// board. The reduced set is the same five scenarios, so the capped traversal is
// pinned to it as well and the two strategies are shown to agree cell for cell.
public sealed partial class EscapeALargeMazeTests
{
    private const int ReducedBoardSize = 500;
    private const int ReducedBoardMaxCoordinate = ReducedBoardSize - 1;
    private const int LeetCodeBoardMaxCoordinate = EscapeALargeMazeBoard.Size - 1;

    // blocked, source, target, canEscape - on LC 1036's own 10^6 x 10^6 board.
    public static TheoryData<EscapeExample> Examples =>
        new()
        {
            // LC example 1: the source is walled into its corner by two cells.
            { new EscapeExample(Blocked: [[0, 1], [1, 0]], Source: [0, 0], Target: [0, 2], CanEscape: false) },

            // LC example 2: nothing blocked, so opposite corners connect.
            {
                new EscapeExample(
                    Blocked: [],
                    Source: [0, 0],
                    Target: [LeetCodeBoardMaxCoordinate, LeetCodeBoardMaxCoordinate],
                    CanEscape: true)
            },

            // A three-cell wall one row below the source with a gap at column 5, so
            // the source slips through into open board rather than being sealed in.
            { new EscapeExample(Blocked: [[1, 3], [1, 4], [1, 6]], Source: [0, 5], Target: [50, 50], CanEscape: true) },

            // The far corner is the sealed one this time: the search from the source
            // escapes, and only the second, target-side search rejects the pair.
            {
                new EscapeExample(
                    Blocked: [[LeetCodeBoardMaxCoordinate - 1, LeetCodeBoardMaxCoordinate], [LeetCodeBoardMaxCoordinate, LeetCodeBoardMaxCoordinate - 1]],
                    Source: [0, 0],
                    Target: [LeetCodeBoardMaxCoordinate, LeetCodeBoardMaxCoordinate],
                    CanEscape: false)
            },

            // One blocked cell can never wall anything off.
            { new EscapeExample(Blocked: [[0, 1]], Source: [0, 0], Target: [5, 5], CanEscape: true) },
        };

    // The same scenarios on a board small enough to materialize.
    public static TheoryData<EscapeExample> ReducedBoardExamples =>
        new()
        {
            { new EscapeExample(Blocked: [[0, 1], [1, 0]], Source: [0, 0], Target: [0, 2], CanEscape: false) },
            {
                new EscapeExample(
                    Blocked: [],
                    Source: [0, 0],
                    Target: [ReducedBoardMaxCoordinate, ReducedBoardMaxCoordinate],
                    CanEscape: true)
            },
            { new EscapeExample(Blocked: [[1, 3], [1, 4], [1, 6]], Source: [0, 5], Target: [50, 50], CanEscape: true) },
            {
                new EscapeExample(
                    Blocked: [[ReducedBoardMaxCoordinate - 1, ReducedBoardMaxCoordinate], [ReducedBoardMaxCoordinate, ReducedBoardMaxCoordinate - 1]],
                    Source: [0, 0],
                    Target: [ReducedBoardMaxCoordinate, ReducedBoardMaxCoordinate],
                    CanEscape: false)
            },
            { new EscapeExample(Blocked: [[0, 1]], Source: [0, 0], Target: [5, 5], CanEscape: true) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanEscapeByCappedTraversal_LeetCodeExamples_ReturnsWhetherSourceReachesTarget(
        EscapeExample example)
    {
        var canEscape = EscapeALargeMazeSolution.CanEscapeByCappedTraversal(
            example.Blocked, example.Source, example.Target);

        Assert.Equal(example.CanEscape, canEscape);
    }

    [Theory]
    [MemberData(nameof(ReducedBoardExamples))]
    public void CanEscapeByCappedTraversal_ReducedBoard_AgreesWithTheFullBoardFloodFill(
        EscapeExample example)
    {
        var canEscape = EscapeALargeMazeSolution.CanEscapeByCappedTraversal(
            example.Blocked, example.Source, example.Target, ReducedBoardSize);

        Assert.Equal(example.CanEscape, canEscape);
    }

    [Theory]
    [MemberData(nameof(ReducedBoardExamples))]
    public void CanEscapeByFullBoardFloodFill_ReducedBoard_ReturnsWhetherSourceReachesTarget(
        EscapeExample example)
    {
        var canEscape = EscapeALargeMazeSolution.CanEscapeByFullBoardFloodFill(
            example.Blocked, example.Source, example.Target, ReducedBoardSize);

        Assert.Equal(example.CanEscape, canEscape);
    }

    // One scenario on a maze: the blocked cells, the coordinates the search runs between,
    // and whether it gets there. The escape flag is named at the row that states it, so a
    // reader of the two example sets never has to remember which position it sits in.
    public readonly record struct EscapeExample(int[][] Blocked, int[] Source, int[] Target, bool CanEscape);
}
