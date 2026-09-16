using DSAExperimentation.LeetCode.StoneGameVIII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.StoneGameVIII;

// Harness only. Both strategies are StoneGameVIIISolution's -
// MaxScoreDifferenceByUnmemoizedRecursion (previously untested scaffolding inlined
// in the benchmark as its baseline arm) now faces the same examples as
// MaxScoreDifferenceByMemoizedRecursion (previously this file's own private
// helper), so a failure names the strategy that broke. Beyond LeetCode's three
// published examples the cases pin the two branches of the recurrence apart: a
// board whose last boundary is ruinous, so ending the move early wins, and boards
// where taking everything is optimal.
public sealed partial class StoneGameVIIITests
{
    public static TheoryData<int[], long> Examples =>
        new()
        {
            { [-1, 2, -3, 4, -5], 5 },
            { [7, -6, 5, 10, 5, -2, -6], 13 },
            { [-10, -12], -22 },
            { [1, 2], 3 },
            { [1, 2, 3, 4], 10 },
            { [3, 4, -100], 100 },
            { [-1, -2, -3], 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxScoreDifferenceByUnmemoizedRecursion_LeetCodeExamples_ReturnsWinnerMinusLoserScore(
        int[] stones, long expected) =>
        Assert.Equal(expected, StoneGameVIIISolution.MaxScoreDifferenceByUnmemoizedRecursion(stones));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxScoreDifferenceByMemoizedRecursion_LeetCodeExamples_ReturnsWinnerMinusLoserScore(
        int[] stones, long expected) =>
        Assert.Equal(expected, StoneGameVIIISolution.MaxScoreDifferenceByMemoizedRecursion(stones));
}
