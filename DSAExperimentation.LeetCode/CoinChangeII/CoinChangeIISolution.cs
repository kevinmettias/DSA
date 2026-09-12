using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.CoinChangeII;

// LeetCode 518. Coin Change II: count combinations (not permutations) of coins that
// sum to amount.
//
// CountCombinationsByTabulation is the standard bottom-up trick - outer loop over
// coins, inner loop over amount ascending - that counts each combination exactly
// once; it is the textbook baseline CountCombinationsByMemoizedTopDown is measured
// against. The memoized arm walks the order-independent recurrence
// f(index, remaining) = f(index+1, remaining) [skip the coin at index] +
//                        f(index, remaining - coins[index]) [reuse the coin at index]
// via this repo's own Memoizer (CombinationSumIV precedent for the counting shape,
// DistinctSubsequences precedent for the 2-D tuple state). Walking the coin index
// forward-only (a skipped coin is never revisited) is what keeps this a COMBINATION
// count - CombinationSumIV's near-identical recurrence tries every num at every
// remaining amount instead, which is what makes IT an order-sensitive PERMUTATION
// count for the same style of problem.
//
// Both arms' running counts can overflow a 32-bit int well before a benchmarked
// Amount's upper bound, same as CombinationSumIVSolution - harmless since both
// strategies overflow identically and a benchmark measures wall-clock time, not the
// returned value.
internal static class CoinChangeIISolution
{
    // The textbook bottom-up tabulation, deliberately written without this repo's
    // primitives - the arm CountCombinationsByMemoizedTopDown is measured against.
    public static int CountCombinationsByTabulation(int amount, int[] coins)
    {
        var dp = new int[amount + 1];
        dp[0] = 1;

        foreach (var coin in coins)
        {
            for (var a = coin; a <= amount; a++)
            {
                dp[a] += dp[a - coin];
            }
        }

        return dp[amount];
    }

    public static int CountCombinationsByMemoizedTopDown(int amount, int[] coins)
    {
        return Memoizer.Memoize<(int Index, int Remaining), int>((0, amount), WaysFor);

        int WaysFor((int Index, int Remaining) state, Func<(int Index, int Remaining), int> ways)
        {
            var (index, remaining) = state;

            if (remaining == 0)
            {
                return 1;
            }

            if (index == coins.Length)
            {
                return 0;
            }

            var total = ways((index + 1, remaining));
            if (coins[index] <= remaining)
            {
                total += ways((index, remaining - coins[index]));
            }

            return total;
        }
    }
}
