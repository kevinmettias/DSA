using DSAExperimentation.LeetCode.Solutions.FinalPricesWithASpecialDiscountInAShop;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FinalPricesWithASpecialDiscountInAShop;

// Harness only: both strategies are FinalPricesWithASpecialDiscountInAShopSolution's
// - the forward-scanning baseline and the monotonic-stack pass - pinned here to
// the degenerate single-item shop.
public sealed partial class FinalPricesWithASpecialDiscountInAShopSolutionTests
{
    public static TheoryData<int[], int[]> Cases =>
        new()
        {
            { [5], [5] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void FinalPricesByBruteForce_Cases_AppliesNextNotGreaterDiscount(
        int[] prices, int[] expected) =>
        Assert.Equal(
            expected,
            FinalPricesWithASpecialDiscountInAShopSolution.FinalPricesByBruteForce(prices));

    [Theory]
    [MemberData(nameof(Cases))]
    public void FinalPricesByMonotonicStack_Cases_AppliesNextNotGreaterDiscount(
        int[] prices, int[] expected) =>
        Assert.Equal(
            expected,
            FinalPricesWithASpecialDiscountInAShopSolution.FinalPricesByMonotonicStack(prices));
}
