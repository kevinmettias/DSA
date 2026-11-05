using DSAExperimentation.LeetCode.Solutions.EscapeALargeMaze;

namespace DSAExperimentation.LeetCode.Solutions.Tests.EscapeALargeMaze;

// Harness only. Both strategies are EscapeALargeMazeSolution's, run on LC 1036's own
// 10^6 x 10^6 board over three scenarios that each decide the answer a different way.
// LeetCode's published examples run in CatalogCaseTests.
public sealed partial class EscapeALargeMazeSolutionTests
{
    private const int LeetCodeBoardMaxCoordinate = EscapeALargeMazeBoard.Size - 1;

    // blocked, source, target, canEscape - on LC 1036's own 10^6 x 10^6 board.
    public static TheoryData<EscapeCase> Cases =>
        new()
        {
            // A three-cell wall one row below the source with a gap at column 5, so
            // the source slips through into open board rather than being sealed in.
            { new EscapeCase(Blocked: [[1, 3], [1, 4], [1, 6]], Source: [0, 5], Target: [50, 50], CanEscape: true) },

            // The far corner is the sealed one this time: the search from the source
            // escapes, and only the second, target-side search rejects the pair.
            {
                new EscapeCase(
                    Blocked: [[LeetCodeBoardMaxCoordinate - 1, LeetCodeBoardMaxCoordinate], [LeetCodeBoardMaxCoordinate, LeetCodeBoardMaxCoordinate - 1]],
                    Source: [0, 0],
                    Target: [LeetCodeBoardMaxCoordinate, LeetCodeBoardMaxCoordinate],
                    CanEscape: false)
            },

            // One blocked cell can never wall anything off.
            { new EscapeCase(Blocked: [[0, 1]], Source: [0, 0], Target: [5, 5], CanEscape: true) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanEscapeByCappedBreadthFirstSearch_Cases_ReturnsWhetherSourceReachesTarget(
        EscapeCase testCase)
    {
        var canEscape = EscapeALargeMazeSolution.CanEscapeByCappedBreadthFirstSearch(
            testCase.Blocked, testCase.Source, testCase.Target);

        Assert.Equal(testCase.CanEscape, canEscape);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanEscapeByCappedTraversal_Cases_ReturnsWhetherSourceReachesTarget(
        EscapeCase testCase)
    {
        var canEscape = EscapeALargeMazeSolution.CanEscapeByCappedTraversal(
            testCase.Blocked, testCase.Source, testCase.Target);

        Assert.Equal(testCase.CanEscape, canEscape);
    }

    // One scenario on a maze: the blocked cells, the coordinates the search runs between,
    // and whether it gets there. The escape flag is named at the row that states it, so a
    // reader of the rows never has to remember which position it sits in.
    public readonly record struct EscapeCase(int[][] Blocked, int[] Source, int[] Target, bool CanEscape);
}
