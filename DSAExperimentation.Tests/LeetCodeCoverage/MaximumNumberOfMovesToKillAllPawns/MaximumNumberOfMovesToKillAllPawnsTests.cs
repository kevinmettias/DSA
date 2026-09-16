using DSAExperimentation.LeetCode.MaximumNumberOfMovesToKillAllPawns;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumNumberOfMovesToKillAllPawns;

// Harness only: both strategies are MaximumNumberOfMovesToKillAllPawnsSolution's -
// this file just pins them to LeetCode's published examples.
public sealed class MaximumNumberOfMovesToKillAllPawnsTests
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
}
