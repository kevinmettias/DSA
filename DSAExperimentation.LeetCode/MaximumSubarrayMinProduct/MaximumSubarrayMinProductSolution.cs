using DSAExperimentation.Domain.Modular;
using RepoIndexStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

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

    // The Sum of Subarray Minimums (LC 907) contribution technique: two passes over
    // this repo's own Stack<int> of pending indices give each element the maximal span
    // over which it is the minimum, and a prefix-sum array turns that span's sum into
    // one subtraction - O(n) overall. Only the maximum is kept rather than a total, so
    // the symmetric strict rule on both sides is safe: LC 907's </<= asymmetry exists
    // to stop equal minima double-counting, and there is nothing to double-count here.
    public static int MaxSumMinProductByMonotonicStack(int[] nums)
    {
        var prefixSums = BuildPrefixSums(nums);
        var leftBound = NearestSmallerBounds(nums, direction: ScanDirection.Forward);
        var rightBound = NearestSmallerBounds(nums, direction: ScanDirection.Backward);

        var best = 0L;

        for (var i = 0; i < nums.Length; i++)
        {
            var spanSum = prefixSums[rightBound[i]] - prefixSums[leftBound[i]];
            best = Math.Max(best, nums[i] * spanSum);
        }

        return ReportedAnswer(best);
    }

    private static long[] BuildPrefixSums(int[] nums)
    {
        var prefixSums = new long[nums.Length + 1];

        for (var i = 0; i < nums.Length; i++)
        {
            prefixSums[i + 1] = prefixSums[i] + nums[i];
        }

        return prefixSums;
    }

    // For each index, the boundary of the maximal span in which nums[i] is the minimum,
    // found by discarding every pending index whose value is not smaller than nums[i] -
    // once passed by a smaller element, an index can never bound anything again.
    // Scanning forward yields the left bound, nudged past the blocking element into an
    // INCLUSIVE start (0 when nothing blocks); scanning backward yields the EXCLUSIVE
    // right bound (nums.Length when nothing blocks). Both feed the prefix-sum
    // subtraction above directly.
    private static int[] NearestSmallerBounds(int[] nums, ScanDirection direction)
    {
        var bounds = new int[nums.Length];
        var pending = new RepoIndexStack();
        var start = direction == ScanDirection.Forward ? 0 : LastIndex(nums);
        var step = direction == ScanDirection.Forward ? 1 : -1;
        var fallback = direction == ScanDirection.Forward ? 0 : nums.Length;
        var offset = direction == ScanDirection.Forward ? 1 : 0;

        for (var count = 0; count < nums.Length; count++)
        {
            var i = start + (count * step);

            while (pending.TryPeek(out var top) && nums[top] >= nums[i])
            {
                pending.TryPop(out _);
            }

            bounds[i] = pending.TryPeek(out var neighbor) ? AdjustedBound(neighbor, offset) : fallback;
            pending.Push(i);
        }

        return bounds;
    }

    private static int LastIndex(int[] nums) => nums.Length - 1;

    // The bound the blocking neighbour implies: one past it when scanning forward,
    // the neighbour itself when scanning backward.
    private static int AdjustedBound(int neighbor, int offset) => neighbor + offset;

    private static int ReportedAnswer(long best) => (int)(best % ModularArithmetic.Modulo);

    // Which way NearestSmallerBounds walks the array: forward to find each element's
    // inclusive left bound, backward to find its exclusive right bound - a state the
    // call site names, where a bare `true` said it only by position.
    private enum ScanDirection
    {
        Forward,
        Backward,
    }
}
