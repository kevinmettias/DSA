using DSAExperimentation.LeetCode.Solutions.BestTimeToBuyAndSellStockWithTransactionFee;

namespace DSAExperimentation.LeetCode.Solutions.Tests.BestTimeToBuyAndSellStockWithTransactionFee;

// Harness only. Both strategies are
// BestTimeToBuyAndSellStockWithTransactionFeeSolution's - this file just pins them
// to a fee larger than any price rise, where the best profit is never to trade.
// LeetCode's published examples run in CatalogCaseTests.
public sealed partial class BestTimeToBuyAndSellStockWithTransactionFeeSolutionTests
{
    public static TheoryData<int[], int, int> Cases =>
        new()
        {
            { [1, 2], 5, 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxProfitByUnmemoizedRecursion_Cases_ReturnsBestProfitAfterFees(
        int[] prices, int fee, int expected)
    {
        var actual = BestTimeToBuyAndSellStockWithTransactionFeeSolution.MaxProfitByUnmemoizedRecursion(
            prices, fee);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxProfitByMemoizedRecursion_Cases_ReturnsBestProfitAfterFees(
        int[] prices, int fee, int expected)
    {
        var actual = BestTimeToBuyAndSellStockWithTransactionFeeSolution.MaxProfitByMemoizedRecursion(
            prices, fee);

        Assert.Equal(expected, actual);
    }
}
