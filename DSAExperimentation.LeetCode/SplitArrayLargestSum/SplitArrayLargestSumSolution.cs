using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures;

namespace DSAExperimentation.LeetCode.SplitArrayLargestSum;

// LeetCode 410. Split Array Largest Sum: "binary search on the answer" - the
// feasibility of a candidate limit ("can nums be split into <= subarrayCount
// contiguous subarrays each summing to at most limit?") is monotone non-decreasing in
// limit, so the minimum feasible limit is the leftmost "true" in an implicit
// [false...false, true...true] sequence over limit in [max(nums), sum(nums)].
//
// ManualBinarySearch is a hand-rolled int lo/hi bisection loop - what you
// would write without this repo. PredicateSearch states the same feasibility
// rule as SplitsWithinLimit, an IMonotonePredicate<int>, so this repo's own
// MonotonePredicateSearch.FirstTrue can locate its boundary over the limit range
// directly, instead of hand-rolling a second bisection loop. Both arms
// binary-search the same monotone predicate in
// O(nums.Length * log(sum - max)).
internal static class SplitArrayLargestSumSolution
{

    public static int MinimizedLargestSumByManualBinarySearch(int[] nums, int subarrayCount)
    {
        var low = nums.Max();
        var high = nums.Sum();

        while (low < high)
        {
            var mid = low + ((high - low) / AlgorithmConstants.HalvingFactor);
            if (CanSplitWithinLimit(nums, subarrayCount, mid))
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

    private static bool CanSplitWithinLimit(int[] nums, int subarrayCount, int limit)
    {
        var subarrays = 1;
        var currentSum = 0;

        foreach (var num in nums)
        {
            if (currentSum + num > limit)
            {
                subarrays++;
                currentSum = 0;
            }

            currentSum += num;
        }

        return subarrays <= subarrayCount;
    }

    public static int MinimizedLargestSumByPredicateSearch(int[] nums, int subarrayCount) =>
        MonotonePredicateSearch.FirstTrue(nums.Max(), nums.Sum(), new SplitsWithinLimit(nums, subarrayCount));

    // IsSatisfiedBy(limit) is "nums splits into at most k subarrays, none summing past
    // limit". Meaningless outside this one problem's feasibility check - stays
    // beside the solution rather than in DataStructures/ or Algorithms/ (§17.3's
    // CountWaysToBuildRoomsInAnAntColony precedent).
    private readonly struct SplitsWithinLimit(int[] nums, int k) : IMonotonePredicate<int>
    {
        public bool IsSatisfiedBy(int limit) => CanSplitWithinLimit(nums, k, limit);
    }
}
