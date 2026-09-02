using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.CoinChange;

// LeetCode 322. Coin Change: unbounded-knapsack recurrence
// f(remaining) = 1 + min over coins <= remaining of f(remaining - coin).
//
// The two strategies walk the same recurrence from opposite directions: the
// textbook bottom-up array (indexed 0..amount) versus this repo's own Memoizer
// walking the recursion top-down (HouseRobber/DecodeWays/ClimbingStairs
// precedent) - each remaining amount solved once and reused across every coin
// choice that lands back on it.
internal static class CoinChangeSolution
{
    // Larger than any reachable dp value (LC bounds amount well below int range)
    // but small enough that Unreachable + 1 cannot overflow.
    private const int Unreachable = int.MaxValue / 2;

    // The textbook bottom-up tabulation. Deliberately written without this repo's
    // primitives - it is the arm the memoized strategy has to justify itself
    // against.
    public static int FewestCoinsByTabulation(int[] coins, int amount)
    {
        var dp = new int[amount + 1];

        for (var a = 1; a <= amount; a++)
        {
            var best = Unreachable;
            foreach (var coin in coins)
            {
                if (coin <= a && dp[a - coin] < Unreachable)
                {
                    best = Math.Min(best, dp[a - coin] + 1);
                }
            }

            dp[a] = best;
        }

        return dp[amount] >= Unreachable ? LeetCodeAnswer.None : dp[amount];
    }

    public static int FewestCoinsByMemoization(int[] coins, int amount)
    {
        var result = Memoizer.Memoize<int, int>(amount, MinCoinsFor);
        return result >= Unreachable ? LeetCodeAnswer.None : result;

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
