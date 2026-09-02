using DSAExperimentation.LeetCode.BestTimeToBuyAndSellStockIII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BestTimeToBuyAndSellStockIII;

// Harness only: both strategies live in BestTimeToBuyAndSellStockIIISolution and
// are asserted against the same examples.
public sealed class BestTimeToBuyAndSellStockIIITests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [3, 3, 5, 0, 0, 3, 1, 4], 6 },
            { [1, 2, 3, 4, 5], 4 },
            { [7, 6, 4, 3, 1], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxProfitByBruteForce_LeetCodeExamples_ReturnsBestTwoTransactions(int[] prices, int expected) =>
        Assert.Equal(expected, BestTimeToBuyAndSellStockIIISolution.MaxProfitByBruteForce(prices));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxProfitByTransactionMemoization_LeetCodeExamples_ReturnsBestTwoTransactions(
        int[] prices, int expected) =>
        Assert.Equal(expected, BestTimeToBuyAndSellStockIIISolution.MaxProfitByTransactionMemoization(prices));
}
