using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BestTimeToBuyAndSellStockWithTransactionFee;

// LeetCode 714. Best Time to Buy and Sell Stock with Transaction Fee: the same
// (day, holding) state-machine DP shape BestTimeToBuyAndSellStockWithCooldownTests
// (LC 309) already establishes, just with the "next buy skipped" cooldown rule
// swapped out for "subtract fee once per completed sale" - the sell transition pays
// fee instead of advancing an extra day. This repo's own Memoizer<TState,TResult>
// supplies the cache, keyed by that same pair.
public sealed partial class BestTimeToBuyAndSellStockWithTransactionFeeTests
{
    [Theory]
    [InlineData(new[] { 1, 3, 2, 8, 4, 9 }, 2, 8)]
    [InlineData(new[] { 1, 3, 7, 5, 10, 3 }, 3, 6)]
    public void MaxProfit_LeetCodeExamples_ReturnsBestProfitAfterFees(int[] prices, int fee, int expected)
        => Assert.Equal(expected, MaxProfit(prices, fee));

    [Fact]
    public void MaxProfit_FeeExceedsAnyGain_ReturnsZero()
        => Assert.Equal(0, MaxProfit([1, 2], fee: 5));

    private static int MaxProfit(int[] prices, int fee)
    {
        return Memoizer.Memoize<(int Day, bool Holding), int>((0, false), ProfitFrom);

        int ProfitFrom((int Day, bool Holding) state, Func<(int Day, bool Holding), int> profit)
        {
            var (day, holding) = state;
            if (day >= prices.Length)
            {
                return 0;
            }

            if (holding)
            {
                var sell = prices[day] - fee + profit((day + 1, false));
                var hold = profit((day + 1, true));
                return Math.Max(sell, hold);
            }

            var buy = -prices[day] + profit((day + 1, true));
            var rest = profit((day + 1, false));
            return Math.Max(buy, rest);
        }
    }
}
