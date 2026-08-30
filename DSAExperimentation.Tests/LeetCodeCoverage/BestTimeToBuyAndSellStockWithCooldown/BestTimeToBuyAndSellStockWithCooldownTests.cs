using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BestTimeToBuyAndSellStockWithCooldown;

// LeetCode 309. Best Time to Buy and Sell Stock with Cooldown: state-machine DP over
// (day, holding) - after a sell the very next buy is skipped by advancing two days
// instead of one, which is exactly the cooldown rule. This repo's own
// Memoizer<TState,TResult> supplies the cache, keyed by that pair, the same
// "recurrence takes a memoized recursive callback" shape EditDistanceBenchmarks/
// DecodeWaysTests already use for a 2-tuple state.
public sealed partial class BestTimeToBuyAndSellStockWithCooldownTests
{
    [Theory]
    [InlineData(new[] { 1, 2, 3, 0, 2 }, 3)]
    [InlineData(new[] { 1 }, 0)]
    [InlineData(new[] { 1, 2, 4 }, 3)]
    public void MaxProfit_LeetCodeExamples_ReturnsBestProfitWithCooldown(int[] prices, int expected)
        => Assert.Equal(expected, MaxProfit(prices));

    private static int MaxProfit(int[] prices)
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
                var sell = prices[day] + profit((day + 2, false));
                var hold = profit((day + 1, true));
                return Math.Max(sell, hold);
            }

            var buy = -prices[day] + profit((day + 1, true));
            var rest = profit((day + 1, false));
            return Math.Max(buy, rest);
        }
    }
}
