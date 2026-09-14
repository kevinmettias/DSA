using DSAExperimentation.LeetCode.StoneGameVII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.StoneGameVII;

// Harness only. Both strategies are StoneGameVIISolution's -
// MaxScoreDifferenceByUnmemoizedRecursion (previously untested scaffolding inlined in
// the benchmark as its baseline arm) now gets the same examples as
// MaxScoreDifferenceByMemoizedRecursion (previously this file's own private helper),
// so a failure names the strategy that broke.
public sealed class StoneGameVIITests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [5, 3, 1, 4, 2], 6 },
            { [7, 90, 5, 1, 100, 10, 10, 2], 122 },
            { [1, 4], 4 },
            { [1, 1], 1 },
            { [1, 2, 3], 2 },
            { [4, 4, 4, 4], 8 },
            { [9], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxScoreDifferenceByUnmemoizedRecursion_LeetCodeExamples_ReturnsWinnerMinusLoserScore(
        int[] stones, int expected) =>
        Assert.Equal(expected, StoneGameVIISolution.MaxScoreDifferenceByUnmemoizedRecursion(stones));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxScoreDifferenceByMemoizedRecursion_LeetCodeExamples_ReturnsWinnerMinusLoserScore(
        int[] stones, int expected) =>
        Assert.Equal(expected, StoneGameVIISolution.MaxScoreDifferenceByMemoizedRecursion(stones));
}
