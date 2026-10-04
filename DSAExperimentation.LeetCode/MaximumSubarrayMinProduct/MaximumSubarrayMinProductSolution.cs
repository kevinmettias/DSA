using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.ElementAlgebra;
using DSAExperimentation.DataStructures.PrefixSums;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.MaximumSubarrayMinProduct;

// LeetCode 1856. Maximum Subarray Min-Product: the min-product of a subarray is its
// minimum element times its sum; report the largest such product over all non-empty
// subarrays, modulo 1e9+7.
//
// Both strategies maximize the same quantity over the same subarrays. They differ in
// how many subarrays they have to look at: every one of them, or only the single
// maximal span in which each element is the minimum - which is enough, because the
// best subarray for a given minimum is always that element's whole span.
internal static class MaximumSubarrayMinProductSolution
{
    private const int NoSmallerElementToTheLeft = -1;

    // The textbook answer: extend every start index one element at a time, carrying a
    // running minimum and running sum so each subarray costs O(1) - O(n^2) subarrays.
    // Deliberately plain arrays and loops; it is the arm the sweep below has to
    // justify itself against.
    public static int MaxSumMinProductByBruteForce(int[] nums)
    {
        var best = 0L;

        for (var start = 0; start < nums.Length; start++)
        {
            long min = nums[start];
            var sum = 0L;

            for (var end = start; end < nums.Length; end++)
            {
                min = Math.Min(min, nums[end]);
                sum += nums[end];
                best = Math.Max(best, min * sum);
            }
        }

        return ReportedAnswer(best);
    }

    // The Sum of Subarray Minimums (LC 907) contribution technique: two NearestBoundary
    // sweeps give each element the maximal span over which it is the minimum - the
    // nearest strictly smaller element on each side blocks it, since once passed by a
    // smaller element an index can never bound anything again - and PrefixSums turns
    // that span's sum into one inclusive Query - O(n) overall. Only the maximum is kept
    // rather than a total, so the symmetric strict rule on both sides is safe: LC 907's
    // </<= asymmetry exists to stop equal minima double-counting, and there is nothing
    // to double-count here.
    public static int MaxSumMinProductByMonotonicStack(int[] nums)
    {
        // Both sentinels are one step outside the array, so the span between them,
        // exclusive at both ends, is the whole array when nothing blocks.
        var prefixSums = new PrefixSums<long, SumOperation<long>>(nums.Select(value => (long)value).ToArray());
        var previousSmaller = NearestBoundary.SmallerToTheLeft(nums, NoSmallerElementToTheLeft);
        var nextSmaller = NearestBoundary.SmallerToTheRight(nums, nums.Length);

        var best = 0L;

        for (var i = 0; i < nums.Length; i++)
        {
            var spanSum = prefixSums.Query(previousSmaller[i] + 1, nextSmaller[i] - 1);
            best = Math.Max(best, nums[i] * spanSum);
        }

        return ReportedAnswer(best);
    }

    private static int ReportedAnswer(long best) => (int)(best % ModularArithmetic.Modulo);
}
