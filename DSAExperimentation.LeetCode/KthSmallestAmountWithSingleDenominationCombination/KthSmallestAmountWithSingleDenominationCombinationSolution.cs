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
        var frontier = SeedFrontier(coins);

        return PopUntilKth(frontier, coins, k);
    }

    // Every denomination's own first multiple: the frontier the k-way merge starts from.
    private static Heap<(long Amount, int CoinIndex), MinHeapOrder<(long, int)>> SeedFrontier(int[] coins)
    {
        var frontier = new Heap<(long Amount, int CoinIndex), MinHeapOrder<(long, int)>>();

        for (var i = 0; i < coins.Length; i++)
        {
            frontier.Push((coins[i], i));
        }

        return frontier;
    }

    // The merge itself: always pop the frontier's smallest pending multiple, counting an
    // amount only the first time it is popped (two coins landing on the same multiple
    // are one achievable amount), and push that coin's next multiple back. The rank-th
    // distinct amount popped is the answer.
    private static long PopUntilKth(
        Heap<(long Amount, int CoinIndex), MinHeapOrder<(long, int)>> frontier, int[] coins, long rank)
    {
        var lastAmount = 0L;
        var count = 0L;

        while (frontier.TryPop(out var next))
        {
            var (amount, coinIndex) = next;

            if (amount != lastAmount)
            {
                count++;
                lastAmount = amount;

                if (count == rank)
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
            total += SubsetContribution(mask, coins, x);
        }

        return total;
    }

    // One non-empty subset of coins, named by its bit mask: it covers exactly its
    // multiples of lcm(subset), added back for an odd-sized subset and subtracted for an
    // even one so no amount is counted twice. The walk stops as soon as the running lcm
    // passes the limit, because such a subset covers no amount <= limit at all.
    private static long SubsetContribution(int mask, int[] coins, long limit)
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

            if (lcm > limit)
            {
                return 0;
            }
        }
        var multiplesOfLcm = limit / lcm;
        var subsetSizeIsOdd = coinsInSubset % 2 == 1;

        return subsetSizeIsOdd ? multiplesOfLcm : -multiplesOfLcm;
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
