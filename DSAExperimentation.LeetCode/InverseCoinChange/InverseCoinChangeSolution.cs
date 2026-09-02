using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.InverseCoinChange;

// LeetCode 3592. Inverse Coin Change: numWays[v] (1-indexed) is how many ways
// an unlimited supply of some lost denomination set makes amount v, via LC
// 518's own unbounded-combinations recurrence. Amounts are examined in
// increasing order 1..numWays.Length; by the time v is reached, the count
// using only denominations < v is already fixed, and confirming v as a
// denomination can only ever add exactly one brand-new way to make v itself
// (the recurrence's own "ways(0) = 1" base case, using v alone) - so
// numWays[v] must equal that fixed count exactly (v is not a coin) or that
// count plus one (v is a coin); anything else means no denomination set could
// have produced this array.
internal static class InverseCoinChangeSolution
{
    // Textbook baseline: the BCL array tabulation LC 518's own combinations-
    // count DP uses - "for i = coin..n: ways[i] += ways[i - coin]" - applied
    // once per confirmed coin, mutating one running array in place.
    // Deliberately no repo primitive - the arm the composed strategy below
    // has to justify itself against.
    public static int[] FindDenominationsByArrayTabulation(int[] numWays)
    {
        var n = numWays.Length;
        var ways = new int[n + 1];
        ways[0] = 1;
        var denominations = new List<int>();

        for (var v = 1; v <= n; v++)
        {
            var target = numWays[v - 1];

            if (target == ways[v] + 1)
            {
                denominations.Add(v);

                for (var i = v; i <= n; i++)
                {
                    ways[i] += ways[i - v];
                }
            }
            else if (target != ways[v])
            {
                return [];
            }
        }

        return denominations.ToArray();
    }

    // This repo's own Memoizer: rather than mutate a running tabulation
    // array, re-derives "ways to make v using only the denominations
    // confirmed so far" fresh from LC 518's own recursive formula each time -
    // ways(amount, coinIndex) = skip coins[coinIndex] entirely, or use it at
    // least once - Memoizer's Y-combinator recurrence shape
    // (MinimumTimeToBreakLocksI's FindMinimumTimeByBitmaskMemo composes it
    // the same way over a different state).
    public static int[] FindDenominationsByMemoizedRecurrence(int[] numWays)
    {
        var n = numWays.Length;
        var denominations = new List<int>();

        for (var v = 1; v <= n; v++)
        {
            var target = numWays[v - 1];
            var currentWays = WaysToMake(v, denominations);

            if (target == currentWays + 1)
            {
                denominations.Add(v);
            }
            else if (target != currentWays)
            {
                return [];
            }
        }

        return denominations.ToArray();
    }

    private static int WaysToMake(int amount, List<int> coins) =>
        Memoizer.Memoize<(int Remaining, int CoinIndex), int>(
            (amount, coins.Count - 1),
            (state, waysFor) =>
            {
                var (remaining, coinIndex) = state;

                if (remaining == 0)
                {
                    return 1;
                }

                if (coinIndex < 0)
                {
                    return 0;
                }

                var skip = waysFor((remaining, coinIndex - 1));
                var use = remaining >= coins[coinIndex] ? waysFor((remaining - coins[coinIndex], coinIndex)) : 0;

                return skip + use;
            });
}
