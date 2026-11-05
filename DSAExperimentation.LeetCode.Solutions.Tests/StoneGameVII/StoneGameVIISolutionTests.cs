using DSAExperimentation.LeetCode.Solutions.StoneGameVII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.StoneGameVII;

// Harness only. Every strategy is StoneGameVIISolution's -
// MaxScoreDifferenceByUnmemoizedRecursion (previously untested scaffolding inlined in
// the benchmark as its baseline arm) now gets the same cases as
// MaxScoreDifferenceByMemoizedRecursion (previously this file's own private helper),
// so a failure names the strategy that broke.
public sealed partial class StoneGameVIISolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [1, 4], 4 },
            { [1, 1], 1 },
            { [1, 2, 3], 2 },
            { [4, 4, 4, 4], 8 },
            { [9], 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxScoreDifferenceByUnmemoizedRecursion_Cases_ReturnsWinnerMinusLoserScore(
        int[] stones, int expected) =>
        Assert.Equal(expected, StoneGameVIISolution.MaxScoreDifferenceByUnmemoizedRecursion(stones));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxScoreDifferenceByMemoizedRecursion_Cases_ReturnsWinnerMinusLoserScore(
        int[] stones, int expected) =>
        Assert.Equal(expected, StoneGameVIISolution.MaxScoreDifferenceByMemoizedRecursion(stones));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxScoreDifferenceByNegamax_Cases_ReturnsWinnerMinusLoserScore(
        int[] stones, int expected) =>
        Assert.Equal(expected, StoneGameVIISolution.MaxScoreDifferenceByNegamax(stones));
}
