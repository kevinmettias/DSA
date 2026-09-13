using DSAExperimentation.LeetCode.FinalPricesWithASpecialDiscountInAShop;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FinalPricesWithASpecialDiscountInAShop;

// Harness only: both strategies are FinalPricesWithASpecialDiscountInAShopSolution's
// - the forward-scanning baseline and the monotonic-stack pass - pinned here to
// LeetCode's three published examples plus the degenerate single-item shop.
public sealed class FinalPricesWithASpecialDiscountInAShopTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            { [8, 4, 6, 2, 3], [4, 2, 4, 2, 3] },
            { [1, 2, 3, 4, 5], [1, 2, 3, 4, 5] },
            { [10, 1, 1, 6], [9, 0, 1, 6] },
            { [5], [5] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FinalPricesByBruteForce_LeetCodeExamples_AppliesNextNotGreaterDiscount(
        int[] prices, int[] expected) =>
        Assert.Equal(
            expected,
            FinalPricesWithASpecialDiscountInAShopSolution.FinalPricesByBruteForce(prices));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FinalPricesByMonotonicStack_LeetCodeExamples_AppliesNextNotGreaterDiscount(
        int[] prices, int[] expected) =>
        Assert.Equal(
            expected,
            FinalPricesWithASpecialDiscountInAShopSolution.FinalPricesByMonotonicStack(prices));
}
