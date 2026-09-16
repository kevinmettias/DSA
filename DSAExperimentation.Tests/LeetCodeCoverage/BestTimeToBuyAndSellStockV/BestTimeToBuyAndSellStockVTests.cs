using DSAExperimentation.LeetCode.BestTimeToBuyAndSellStockV;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BestTimeToBuyAndSellStockV;

// Harness only. Both strategies are BestTimeToBuyAndSellStockVSolution's - this
// file just pins them to LeetCode's published examples.
public sealed partial class BestTimeToBuyAndSellStockVTests
{
    public static TheoryData<int[], int, long> Examples =>
        new()
        {
            { [1, 7, 9, 8, 2], 2, 14L },
            { [12, 16, 19, 19, 8, 1, 19, 13, 9], 3, 36L },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxProfitByBruteForce_LeetCodeExamples_ReturnsMaximumProfit(
        int[] prices, int transactionBudget, long expected)
    {
        var actual = BestTimeToBuyAndSellStockVSolution.MaxProfitByBruteForce(prices, transactionBudget);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxProfitByTransactionMemoization_LeetCodeExamples_ReturnsMaximumProfit(
        int[] prices, int transactionBudget, long expected)
    {
        var actual = BestTimeToBuyAndSellStockVSolution.MaxProfitByTransactionMemoization(
            prices, transactionBudget);

        Assert.Equal(expected, actual);
    }
}
