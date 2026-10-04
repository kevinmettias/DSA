using DSAExperimentation.LeetCode.FindTheCountOfMonotonicPairsII;

namespace DSAExperimentation.LeetCode.FindTheCountOfMonotonicPairsI;

// LeetCode 3250. Find the Count of Monotonic Pairs I: count pairs of non-negative
// integer arrays (arr1, arr2), both length n, with arr1 non-decreasing, arr2
// non-increasing, and arr1[i] + arr2[i] == nums[i] for every i - modulo 1e9+7.
//
// It is Part II (LeetCode 3251) with nums[i] <= 50 instead of 1000, so LC 3251's
// class owns both arms and its doc comment derives the row-by-row DP they share
// (ARCHITECTURE 17.3). Each arm here calls through; both parts answer a long, so
// nothing narrows. This problem's own test and benchmark still run them at its own
// bound.
internal static class FindTheCountOfMonotonicPairsISolution
{
    // The O(n * maxValue^2) row rescan, which Part I's small bound keeps tractable.
    public static long CountPairsByBruteForceDP(int[] nums) =>
        FindTheCountOfMonotonicPairsIISolution.CountPairsByBruteForceDP(nums);

    // The O(n * maxValue) running-prefix form.
    public static long CountPairsByPrefixSumDP(int[] nums) =>
        FindTheCountOfMonotonicPairsIISolution.CountPairsByPrefixSumDP(nums);
}
