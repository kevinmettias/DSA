using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.LeetCode.KthSmallestAmountWithSingleDenominationCombination;

// LeetCode 3116. Kth Smallest Amount With Single Denomination Combination:
// coins are never combined - an achievable amount is any positive multiple of
// exactly one denomination - so the answer is the k-th smallest value in the
// union of coins.Length arithmetic progressions.
//
// Both strategies enumerate that union correctly; they differ in whether they
// walk it value-by-value (cost bounded by k) or ask "how many achievable
// amounts are <= x?" directly via inclusion-exclusion and binary search on x
// - the only shape that stays fast once k reaches LeetCode's full 2*10^9,
// since the achievable amount itself can then exceed Int32 range.
internal static class KthSmallestAmountWithSingleDenominationCombinationSolution
{
    // The textbook k-way merge: this repo's own Heap<Element,TOrder> as a
    // priority queue over (amount, coinIndex), always popping the smallest
    // pending multiple and pushing that coin's next one. Duplicate amounts
    // (two coins landing on the same multiple) are collapsed by only
    // counting a popped amount once. O(k log coins.Length) - correct for
    // every input, but the arm the search strategy below has to beat once k
    // grows large.
    public static long KthSmallestAmountByHeapMerge(int[] coins, long k)
    {
        var frontier = new Heap<(long Amount, int CoinIndex), MinHeapOrder<(long, int)>>();

        for (var i = 0; i < coins.Length; i++)
        {
            frontier.Push((coins[i], i));
        }

        var lastAmount = 0L;
        var count = 0L;

        while (frontier.TryPop(out var next))
        {
            var (amount, coinIndex) = next;

            if (amount != lastAmount)
            {
                count++;
                lastAmount = amount;

                if (count == k)
                {
                    return amount;
                }
            }

            frontier.Push((amount + coins[coinIndex], coinIndex));
        }

        return LeetCodeAnswer.None;
    }

    // "How many achievable amounts are <= x?" is monotone in x, and is exactly
    // inclusion-exclusion over every non-empty subset of coins: a subset's
    // multiples of ALL its coins are exactly its multiples of lcm(subset),
    // added back for an odd-sized subset and subtracted for an even one so
    // every amount is counted exactly once regardless of how many coins
    // divide it. Binary searching that predicate for its smallest true finds
    // the k-th amount directly in O(log(k * min(coins)) * 2^coins.Length),
    // independent of k itself - coins.Length <= 15 keeps that subset walk
    // small.
    public static long KthSmallestAmountByInclusionExclusionSearch(int[] coins, long k)
    {
        var low = 1L;
        var high = k * coins.Min();

        while (low < high)
        {
            var mid = low + ((high - low) / 2);

            if (CountAchievableAtMost(mid, coins) >= k)
            {
                high = mid;
            }
            else
            {
                low = mid + 1;
            }
        }

        return low;
    }

    private static long CountAchievableAtMost(long x, int[] coins)
    {
        var subsetCount = 1 << coins.Length;
        var total = 0L;

        for (var mask = 1; mask < subsetCount; mask++)
        {
            var lcm = 1L;
            var coinsInSubset = 0;

            for (var i = 0; i < coins.Length; i++)
            {
                if ((mask & (1 << i)) == 0)
                {
                    continue;
                }

                coinsInSubset++;
                lcm = Lcm(lcm, coins[i]);

                if (lcm > x)
                {
                    break;
                }
            }

            if (lcm > x)
            {
                continue;
            }

            var multiplesOfLcm = x / lcm;
            total += coinsInSubset % 2 == 1 ? multiplesOfLcm : -multiplesOfLcm;
        }

        return total;
    }

    private static long Lcm(long a, long b) => a / Gcd(a, b) * b;

    private static long Gcd(long a, long b)
    {
        while (b != 0)
        {
            (a, b) = (b, a % b);
        }

        return a;
    }
}
