using DSAExperimentation.LeetCode.BestTimeToBuyAndSellStockWithTransactionFee;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BestTimeToBuyAndSellStockWithTransactionFee;

// Harness only. Both strategies are
// BestTimeToBuyAndSellStockWithTransactionFeeSolution's - this file just pins them
// to LeetCode's published examples.
public sealed class BestTimeToBuyAndSellStockWithTransactionFeeTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            { [1, 3, 2, 8, 4, 9], 2, 8 },
            { [1, 3, 7, 5, 10, 3], 3, 6 },
            { [1, 2], 5, 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxProfitByUnmemoizedRecursion_LeetCodeExamples_ReturnsBestProfitAfterFees(
        int[] prices, int fee, int expected) =>
        Assert.Equal(
            expected,
            BestTimeToBuyAndSellStockWithTransactionFeeSolution.MaxProfitByUnmemoizedRecursion(prices, fee));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxProfitByMemoizedRecursion_LeetCodeExamples_ReturnsBestProfitAfterFees(
        int[] prices, int fee, int expected) =>
        Assert.Equal(
            expected,
            BestTimeToBuyAndSellStockWithTransactionFeeSolution.MaxProfitByMemoizedRecursion(prices, fee));
}
