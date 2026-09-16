using DSAExperimentation.LeetCode.StoneGameII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.StoneGameII;

// Harness only. Both strategies are StoneGameIISolution's -
// MaxStonesByUnmemoizedRecursion (previously untested scaffolding inlined in the
// benchmark as its baseline arm) now gets the same examples as
// MaxStonesByMemoizedRecursion (previously the test's own private helper), so a
// failure names the strategy that broke.
public sealed partial class StoneGameIITests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [2, 7, 9, 4, 4], 10 },
            { [1, 2, 3, 4, 5, 100], 104 },
            { [7], 7 },
            { [1, 1], 2 },
            { [3, 3, 3], 6 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxStonesByUnmemoizedRecursion_LeetCodeExamples_ReturnsMostStonesAliceCanForce(
        int[] piles, int expected) =>
        Assert.Equal(expected, StoneGameIISolution.MaxStonesByUnmemoizedRecursion(piles));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxStonesByMemoizedRecursion_LeetCodeExamples_ReturnsMostStonesAliceCanForce(
        int[] piles, int expected) =>
        Assert.Equal(expected, StoneGameIISolution.MaxStonesByMemoizedRecursion(piles));
}
