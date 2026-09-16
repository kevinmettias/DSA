using DSAExperimentation.LeetCode.MinimumNumberOfCoinsForFruits;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumNumberOfCoinsForFruits;

// Harness only: both strategies live in MinimumNumberOfCoinsForFruitsSolution and
// are asserted against the same examples, including LeetCode's own note that
// re-purchasing an already-free fruit can still be worth it.
public sealed partial class MinimumNumberOfCoinsForFruitsTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [3, 1, 2], 4 },
            { [1, 10, 1, 1], 2 },
            { [1], 1 },
            { [1, 2], 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinCoinsByBruteForceDp_LeetCodeExamples_ReturnsMinimumCoins(int[] prices, int expected) =>
        Assert.Equal(expected, MinimumNumberOfCoinsForFruitsSolution.MinCoinsByBruteForceDp(prices));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinCoinsBySegmentTreeDp_LeetCodeExamples_ReturnsMinimumCoins(int[] prices, int expected) =>
        Assert.Equal(expected, MinimumNumberOfCoinsForFruitsSolution.MinCoinsBySegmentTreeDp(prices));
}
