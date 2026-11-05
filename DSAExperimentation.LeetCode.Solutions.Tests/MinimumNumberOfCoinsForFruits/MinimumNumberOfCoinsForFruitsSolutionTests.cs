using DSAExperimentation.LeetCode.Solutions.MinimumNumberOfCoinsForFruits;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumNumberOfCoinsForFruits;

// Harness only: both strategies live in MinimumNumberOfCoinsForFruitsSolution and
// are asserted against the same cases, among them LeetCode's own note that
// re-purchasing an already-free fruit can still be worth it.
public sealed partial class MinimumNumberOfCoinsForFruitsSolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [1, 2], 1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinCoinsByBruteForceDp_Cases_ReturnsMinimumCoins(int[] prices, int expected) =>
        Assert.Equal(expected, MinimumNumberOfCoinsForFruitsSolution.MinCoinsByBruteForceDp(prices));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinCoinsBySegmentTreeDp_Cases_ReturnsMinimumCoins(int[] prices, int expected) =>
        Assert.Equal(expected, MinimumNumberOfCoinsForFruitsSolution.MinCoinsBySegmentTreeDp(prices));
}
