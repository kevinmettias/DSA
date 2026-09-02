using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CoinChange;

// LeetCode 322. Coin Change: unbounded-knapsack recurrence
// f(remaining) = 1 + min over coins <= remaining of f(remaining - coin), via this
// repo's own Memoizer (HouseRobber/DecodeWays/ClimbingStairs precedent) instead of
// the textbook un-memoized exponential recursion - each remaining amount is solved
// once and reused across every coin choice that lands back on it.
public sealed partial class CoinChangeTests
{
    private const int Unreachable = int.MaxValue / 2;

    [Theory]
    [InlineData(new[] { 1, 2, 5 }, 11, 3)]
    [InlineData(new[] { 2 }, 3, -1)]
    [InlineData(new[] { 1 }, 0, 0)]
    public void FewestCoins_LeetCodeExamples_ReturnsMinimumCoinCount(int[] coins, int amount, int expected)
    {
        var actual = FewestCoins(coins, amount);
        Assert.Equal(expected, actual);
    }

    private static int FewestCoins(int[] coins, int amount)
    {
        var result = Memoizer.Memoize<int, int>(amount, MinCoinsFor);
        return result >= Unreachable ? -1 : result;

        int MinCoinsFor(int remaining, Func<int, int> minCoins)
        {
            if (remaining == 0)
            {
                return 0;
            }

            if (remaining < 0)
            {
                return Unreachable;
            }

            var best = Unreachable;
            foreach (var coin in coins)
            {
                var sub = minCoins(remaining - coin);
                if (sub < Unreachable)
                {
                    best = Math.Min(best, sub + 1);
                }
            }

            return best;
        }
    }
}
