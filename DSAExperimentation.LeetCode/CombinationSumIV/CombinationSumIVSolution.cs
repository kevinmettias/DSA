using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.CombinationSumIV;

// LeetCode 377. Combination Sum IV: counting recurrence
// f(remaining) = sum over nums <= remaining of f(remaining - num). Despite the
// problem's own "combination" wording, order matters (LeetCode counts [1,3] and
// [3,1] as two different combinations for target 4) - the recurrence already
// reflects that by trying every num at every remaining target, not just nums
// at-or-after the previous choice the way a true (order-independent) combination
// count would restrict the inner loop.
//
// Both strategies walk the same recurrence from opposite directions: the textbook
// bottom-up array (indexed 0..target) versus this repo's own Memoizer walking the
// recursion top-down (CoinChange precedent, same unbounded-knapsack shape with a
// sum-reduction instead of a min-reduction).
internal static class CombinationSumIVSolution
{
    // The textbook bottom-up tabulation. Deliberately written without this repo's
    // primitives - it is the arm the memoized strategy below has to justify itself
    // against.
    public static int CountCombinationsByTabulation(int[] nums, int target)
    {
        var dp = new int[target + 1];
        dp[0] = 1;

        for (var t = 1; t <= target; t++)
        {
            var total = 0;
            foreach (var num in nums)
            {
                if (num <= t)
                {
                    total += dp[t - num];
                }
            }

            dp[t] = total;
        }

        return dp[target];
    }

    public static int CountCombinationsByMemoizedRecursion(int[] nums, int target)
    {
        return Memoizer.Memoize<int, int>(target, WaysFor);

        int WaysFor(int remaining, Func<int, int> ways)
        {
            if (remaining == 0)
            {
                return 1;
            }

            var total = 0;
            foreach (var num in nums)
            {
                if (num <= remaining)
                {
                    total += ways(remaining - num);
                }
            }

            return total;
        }
    }
}
