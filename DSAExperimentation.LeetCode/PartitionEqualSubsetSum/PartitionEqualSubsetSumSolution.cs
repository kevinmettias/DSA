using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.PartitionEqualSubsetSum;

// LeetCode 416. Partition Equal Subset Sum: does some subset of nums sum to exactly
// half the total? An odd total makes an equal split impossible outright; otherwise
// this is the classic 0/1-knapsack subset-sum reachability question - can some
// subset reach `half` - solved two ways below.
//
// Both strategies take a second overload that skips the total/parity check and
// starts straight from an already-known `half`, so a caller that has hoisted that
// computation (a benchmark's [GlobalSetup], say) is not charged for it again.
internal static class PartitionEqualSubsetSumSolution
{
    private const int SubsetSumDivisor = 2;

    // The textbook answer: bottom-up bool[] tabulation, walking each candidate sum
    // downward so an element is never reused within the same pass.
    public static bool CanPartitionByTabulation(int[] nums)
    {
        return TryGetHalf(nums, out var half) && CanPartitionByTabulation(nums, half);
    }

    public static bool CanPartitionByTabulation(int[] nums, int half)
    {
        var dp = new bool[half + 1];
        dp[0] = true;

        foreach (var num in nums)
        {
            for (var remaining = half; remaining >= num; remaining--)
            {
                dp[remaining] = dp[remaining] || dp[remaining - num];
            }
        }

        return dp[half];
    }

    // This repo's own top-down Memoizer (CoinChange/WordBreak precedent) solving
    // f(index, remaining) = remaining == 0 || (index < nums.Length &&
    // (f(index+1, remaining-nums[index]) || f(index+1, remaining))) - each
    // (index, remaining) state is solved once and reused across every branch that
    // lands back on it.
    public static bool CanPartitionByMemoization(int[] nums)
    {
        return TryGetHalf(nums, out var half) && CanPartitionByMemoization(nums, half);
    }

    public static bool CanPartitionByMemoization(int[] nums, int half)
    {
        return Memoizer.Memoize<(int Index, int Remaining), bool>(
            (0, half), (state, canReach) => CanReach(state, canReach, nums));
    }

    private static bool CanReach(
        (int Index, int Remaining) state, Func<(int Index, int Remaining), bool> canReach, int[] nums)
    {
        if (state.Remaining == 0)
        {
            return true;
        }

        if (state.Remaining < 0 || state.Index == nums.Length)
        {
            return false;
        }

        return canReach((state.Index + 1, state.Remaining - nums[state.Index]))
            || canReach((state.Index + 1, state.Remaining));
    }

    private static bool TryGetHalf(int[] nums, out int half)
    {
        var total = nums.Sum();
        if (total % SubsetSumDivisor != 0)
        {
            half = 0;
            return false;
        }

        half = total / SubsetSumDivisor;
        return true;
    }
}
