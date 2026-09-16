using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.SplitArrayWithSameAverage;

// LeetCode 805. Split Array With Same Average: can the array be split into two
// non-empty parts with the same average?
//
// avg(A) == avg(B) holds exactly when avg(A) == avg(whole), so the question
// reduces to "does some proper non-empty subset of size k (1 <= k <= n/2, by
// symmetry with its complement) sum to total*k/n". The baseline enumerates all
// 2^n subsets as bit masks; the composed strategy runs that as a
// 0/1-knapsack-with-a-required-count recurrence over (index, countNeeded,
// sumNeeded) through this repo's own Memoizer - the PartitionEqualSubsetSum
// precedent with one extra dimension (a required subset SIZE, not just a
// required subset SUM).
internal static class SplitArrayWithSameAverageSolution
{
    // A subset and its complement give the same split, so only sizes up to half
    // the array need to be tried.
    private const int MaxSubsetSizeDivisor = 2;

    // The textbook answer: every proper non-empty subset as a bit mask, checked
    // with the division-free test sum * n == total * count. BCL arithmetic only -
    // it is the arm the memoized DP has to justify itself against, and at n = 30
    // it is the 2^n wall LeetCode's constraints exist to force.
    public static bool CanSplitBySubsetMasks(int[] nums)
    {
        var n = nums.Length;
        var total = nums.Sum();

        for (var mask = 1; mask < (1 << n) - 1; mask++)
        {
            if (IsSameAverageSplitForMask(nums, mask, n, total))
            {
                return true;
            }
        }

        return false;
    }

    // Splits nums by `mask` into the selected subset (bits set) and the rest, and
    // checks whether the selected subset's average equals the whole array's.
    private static bool IsSameAverageSplitForMask(int[] nums, int mask, int elementCount, int total)
    {
        var count = 0;
        var sum = 0;

        for (var i = 0; i < elementCount; i++)
        {
            if ((mask & (1 << i)) != 0)
            {
                count++;
                sum += nums[i];
            }
        }

        return sum * elementCount == total * count;
    }

    // This repo's own Memoizer over (index, countNeeded, sumNeeded): one candidate
    // subset size at a time, each a subset-sum search that also has to land on an
    // exact element count. O(n * (n/2) * sum) instead of O(2^n).
    public static bool CanSplitByMemoizedSubsetSum(int[] nums)
    {
        var n = nums.Length;
        var total = nums.Sum();

        for (var k = 1; k <= n / MaxSubsetSizeDivisor; k++)
        {
            if (HasSubsetOfSizeWithTargetSum(nums, n, total, k))
            {
                return true;
            }
        }

        return false;
    }

    // For one candidate subset size `subsetSize`, checks whether some subset of that
    // size sums to the exact target that would make its average equal the whole
    // array's. A target that is not an integer rules the size out with no search at all.
    private static bool HasSubsetOfSizeWithTargetSum(int[] nums, int elementCount, int total, int subsetSize)
    {
        if (total * subsetSize % elementCount != 0)
        {
            return false;
        }

        var targetSum = total * subsetSize / elementCount;

        return Memoizer.Memoize<(int Index, int Count, int Sum), bool>(
            (0, subsetSize, targetSum), new SubsetSizeSearch(nums));
    }

    // The rule, named: a subset of the required size and sum exists when the element at
    // hand is either taken (one fewer needed of both count and sum) or left alone. The
    // elements are the whole of what the rule needs from its caller, so they are the
    // constructor's only input.
    private sealed class SubsetSizeSearch(int[] nums) : IRecurrence<(int Index, int Count, int Sum), bool>
    {
        public bool Replay(
            (int Index, int Count, int Sum) state,
            IRecurrence<(int Index, int Count, int Sum), bool> rest)
        {
            if (state.Count == 0)
            {
                return state.Sum == 0;
            }

            if (state.Index == nums.Length || state.Sum < 0)
            {
                return false;
            }

            return rest.Replay((state.Index + 1, state.Count, state.Sum), rest)
                || rest.Replay((state.Index + 1, state.Count - 1, state.Sum - nums[state.Index]), rest);
        }
    }
}
