using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.HouseRobberV;

// LeetCode 3840. House Robber V: nums[i] is the money in house i, colors[i] its
// color code. Adjacent houses i-1 and i may both be robbed only when their
// colors differ - two adjacent same-colored houses can never both be robbed.
// This collapses to the classic House Robber recurrence best(i) =
// max(best(i-1), nums[i] + best(i-2)) - the only twist is which predecessor
// state feeds the "take house i" branch: best(i-2) when colors[i] == colors[i-1]
// (house i-1 is excluded from that state entirely, so pairing it with house i
// can never violate the same-color rule), best(i-1) otherwise (no adjacency
// constraint exists between i-1 and i at all, so house i can be added to
// whatever was already optimal through i-1).
//
// Both strategies solve the same recurrence and differ only in evaluation order
// - the same DecodeWays precedent: bottom-up tabulation fills a long[] from the
// front, top-down memoization lets this repo's own Memoizer cache the identical
// recurrence written as ordinary recursion.
internal static class HouseRobberVSolution
{
    // The textbook answer: a BCL long[] filled front-to-back, dp[i] = the best
    // amount robbing among houses 0..i. Deliberately written without this
    // repo's primitives - it is the arm the memoized strategy below has to
    // justify itself against.
    public static long MaxAmountByTabulation(int[] nums, int[] colors)
    {
        var dp = new long[nums.Length];
        dp[0] = nums[0];

        for (var i = 1; i < nums.Length; i++)
        {
            var priorBest = colors[i] == colors[i - 1] ? PriorBest(dp, i) : dp[i - 1];
            dp[i] = Math.Max(dp[i - 1], nums[i] + priorBest);
        }

        return dp[^1];
    }

    private static long PriorBest(long[] dp, int i) => i >= 2 ? dp[i - 2] : 0;

    // This repo's own top-down engine: Memoizer.Memoize caches best(i) the first
    // time each index is reached, so the recurrence reads as ordinary recursion
    // with no hand-rolled cache dictionary.
    public static long MaxAmountByMemoization(int[] nums, int[] colors)
    {
        return Memoizer.Memoize<int, long>(nums.Length - 1, BestThrough);

        long BestThrough(int i, Func<int, long> best)
        {
            if (i < 0)
            {
                return 0;
            }

            if (i == 0)
            {
                return nums[0];
            }

            var priorBest = colors[i] == colors[i - 1] ? best(i - 2) : best(i - 1);
            return Math.Max(best(i - 1), nums[i] + priorBest);
        }
    }
}
