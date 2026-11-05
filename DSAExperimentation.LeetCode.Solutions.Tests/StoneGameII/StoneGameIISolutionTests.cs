using DSAExperimentation.LeetCode.Solutions.StoneGameII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.StoneGameII;

// Harness only. Both strategies are StoneGameIISolution's -
// MaxStonesByUnmemoizedRecursion (previously untested scaffolding inlined in the
// benchmark as its baseline arm) now gets the same cases as
// MaxStonesByMemoizedRecursion (previously the test's own private helper), so a
// failure names the strategy that broke.
public sealed partial class StoneGameIISolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [7], 7 },
            { [1, 1], 2 },
            { [3, 3, 3], 6 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxStonesByUnmemoizedRecursion_Cases_ReturnsMostStonesAliceCanForce(
        int[] piles, int expected) =>
        Assert.Equal(expected, StoneGameIISolution.MaxStonesByUnmemoizedRecursion(piles));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxStonesByMemoizedRecursion_Cases_ReturnsMostStonesAliceCanForce(
        int[] piles, int expected) =>
        Assert.Equal(expected, StoneGameIISolution.MaxStonesByMemoizedRecursion(piles));
}
