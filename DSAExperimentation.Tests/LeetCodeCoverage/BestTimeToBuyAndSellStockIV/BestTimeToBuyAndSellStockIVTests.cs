using DSAExperimentation.LeetCode.BestTimeToBuyAndSellStockIV;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BestTimeToBuyAndSellStockIV;

// Harness only: both strategies live in BestTimeToBuyAndSellStockIVSolution and
// are asserted against the same examples.
public sealed class BestTimeToBuyAndSellStockIVTests
{
    public static TheoryData<int, int[], int> Examples =>
        new()
        {
            { 2, [2, 4, 1], 2 },
            { 2, [3, 2, 6, 5, 0, 3], 7 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxProfitByBruteForce_LeetCodeExamples_ReturnsBestKTransactions(
        int transactionBudget, int[] prices, int expected)
    {
        var actual = BestTimeToBuyAndSellStockIVSolution.MaxProfitByBruteForce(prices, transactionBudget);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxProfitByTransactionMemoization_LeetCodeExamples_ReturnsBestKTransactions(
        int transactionBudget, int[] prices, int expected)
    {
        var actual = BestTimeToBuyAndSellStockIVSolution.MaxProfitByTransactionMemoization(
            prices, transactionBudget);

        Assert.Equal(expected, actual);
    }
}
