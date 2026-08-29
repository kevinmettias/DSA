using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BestTimeToBuyAndSellStockIII;

public sealed partial class BestTimeToBuyAndSellStockIIITests
{
    [Theory]
    [InlineData(new[] { 3, 3, 5, 0, 0, 3, 1, 4 }, 6)]
    [InlineData(new[] { 1, 2, 3, 4, 5 }, 4)]
    public void MaxProfit_Examples_ReturnsBestTwoTransactions(int[] prices, int expected) => Assert.Equal(expected, MaxProfit(prices));
    private static int MaxProfit(int[] prices) { return Memoizer.Memoize<(int Day, int Holding, int Transactions), int>((0, 0, 0), Best); int Best((int Day, int Holding, int Transactions) s, Func<(int Day, int Holding, int Transactions), int> best) { if (s.Day == prices.Length || s.Transactions == 2) return 0; var skip = best((s.Day + 1, s.Holding, s.Transactions)); return s.Holding == 1 ? Math.Max(skip, prices[s.Day] + best((s.Day + 1, 0, s.Transactions + 1))) : Math.Max(skip, -prices[s.Day] + best((s.Day + 1, 1, s.Transactions))); } }
}
