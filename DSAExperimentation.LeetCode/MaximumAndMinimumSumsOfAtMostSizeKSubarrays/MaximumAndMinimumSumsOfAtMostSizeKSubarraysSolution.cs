using RepoIntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.MaximumAndMinimumSumsOfAtMostSizeKSubarrays;

// LeetCode 3430. Maximum and Minimum Sums of at Most Size K Subarrays: over
// every contiguous subarray of length 1..maxSubarrayLength, sum its maximum plus
// its minimum. No modulo here (unlike its subsequence sibling, LC 3428) - values
// and lengths are small enough that the true sum fits a long outright.
//
// Every subarray's extreme is attributed to exactly one of its indices - the
// leftmost occurrence of that value inside it - by breaking left boundaries
// strictly and right boundaries non-strictly (the same SumOfSubarrayMinimums/LC
// 907 tie-break, so duplicate values are neither double- nor under-counted).
// That index then dominates a contiguous index range; counting only the
// subarrays inside it whose length is also <= maxSubarrayLength is closed-form
// arithmetic, not a walk.
internal static class MaximumAndMinimumSumsOfAtMostSizeKSubarraysSolution
{
    // The textbook answer: slide the window end forward from every start,
    // tracking the running max/min in O(1) per step - O(n*k) overall, no repo
    // primitives. The baseline SumByMonotonicStackContribution is measured
    // against.
    public static long SumByBruteForceWindow(int[] nums, int maxSubarrayLength)
    {
        var n = nums.Length;
        var total = 0L;

        for (var start = 0; start < n; start++)
        {
            var max = nums[start];
            var min = nums[start];
            var end = Math.Min(n, start + maxSubarrayLength);

            for (var i = start; i < end; i++)
            {
                max = Math.Max(max, nums[i]);
                min = Math.Min(min, nums[i]);
                total += max + min;
            }
        }

        return total;
    }

    public static long SumByMonotonicStackContribution(int[] nums, int maxSubarrayLength) =>
        ExtremeSum(nums, maxSubarrayLength, Extreme.Maximum)
            + ExtremeSum(nums, maxSubarrayLength, Extreme.Minimum);

    private static long ExtremeSum(int[] nums, int maxSubarrayLength, Extreme extreme)
    {
        var n = nums.Length;
        var left = PreviousDominantIndex(nums, extreme);
        var right = NextDominantIndex(nums, extreme);
        var total = 0L;

        for (var i = 0; i < n; i++)
        {
            var count = CountDominatedSubarrays(left[i] + 1, i, right[i] - 1, maxSubarrayLength);
            total += (long)nums[i] * count;
        }

        return total;
    }

    // Index of the nearest strictly-dominant element to the left (nums[left] >
    // nums[i] for Extreme.Maximum, nums[left] < nums[i] for Extreme.Minimum), or
    // -1. This repo's own Stack<int> holds candidate indices in the classic
    // monotonic order, each popped the moment it can no longer be any later
    // element's nearest dominant neighbor.
    private static int[] PreviousDominantIndex(int[] nums, Extreme extreme)
    {
        var n = nums.Length;
        var boundary = new int[n];
        var stack = new RepoIntStack();

        for (var i = 0; i < n; i++)
        {
            while (stack.TryPeek(out var top) && !IsDominantOver(nums[top], nums[i], extreme, DominanceRule.Strict))
            {
                stack.TryPop(out _);
            }

            boundary[i] = stack.TryPeek(out var kept) ? kept : -1;
            stack.Push(i);
        }

        return boundary;
    }

    // Index of the nearest non-strictly-dominant element to the right, or n.
    // DominanceRule.Inclusive here (paired with DominanceRule.Strict on the left)
    // is what gives every duplicate value exactly one owning index instead of
    // over- or under-counting it.
    private static int[] NextDominantIndex(int[] nums, Extreme extreme)
    {
        var n = nums.Length;
        var boundary = new int[n];
        var stack = new RepoIntStack();

        for (var i = n - 1; i >= 0; i--)
        {
            while (stack.TryPeek(out var top) && !IsDominantOver(nums[top], nums[i], extreme, DominanceRule.Inclusive))
            {
                stack.TryPop(out _);
            }

            boundary[i] = stack.TryPeek(out var kept) ? kept : n;
            stack.Push(i);
        }

        return boundary;
    }

    // Subarrays [s, e] with left <= s <= dominantIndex <= e <= right and
    // e-s+1 <= maxSubarrayLength, counted directly: for each start s the valid
    // ends run from dominantIndex up to min(right, s+maxSubarrayLength-1), a range
    // whose size is either a linear function of s (s early enough that the length
    // cap binds) or constant (s late enough that right binds first instead) -
    // summed in closed form over those two pieces instead of enumerated.
    private static long CountDominatedSubarrays(int left, int dominantIndex, int right, int maxSubarrayLength)
    {
        var startLow = Math.Max(left, dominantIndex - maxSubarrayLength + 1);
        var startHigh = dominantIndex;
        var pivot = right - maxSubarrayLength + 1;
        var mid = Math.Min(startHigh, pivot);
        var total = 0L;

        if (mid >= startLow)
        {
            var count = mid - startLow + 1;
            var indexSum = (long)(mid + startLow) * count / 2;
            total += indexSum + (long)(maxSubarrayLength - dominantIndex) * count;
        }

        var constantStart = Math.Max(startLow, mid + 1);

        if (constantStart <= startHigh)
        {
            total += (long)(right - dominantIndex + 1) * (startHigh - constantStart + 1);
        }

        return total;
    }

    // The nearest-dominant test as a decision table: Extreme names which side of
    // the value counts as beyond it, and DominanceRule says whether landing level
    // with it counts too - only the left boundary is DominanceRule.Strict, which
    // is what gives every duplicate value exactly one owning index.
    private static bool IsDominantOver(int candidate, int value, Extreme extreme, DominanceRule rule) =>
        (extreme, rule) switch
    {
        (Extreme.Maximum, DominanceRule.Strict) => candidate > value,
        (Extreme.Maximum, DominanceRule.Inclusive) => candidate >= value,
        (Extreme.Minimum, DominanceRule.Strict) => candidate < value,
        _ => candidate <= value,
    };

    // Which of a subarray's two extremes a pass over the array is attributing:
    // the largest element or the smallest.
    private enum Extreme
    {
        Maximum,
        Minimum,
    }

    // Whether an element level with the candidate counts as dominated by it, or
    // only one strictly beyond it does. Named where a rewritten `true`/`false` at
    // the call site said it only by position.
    private enum DominanceRule
    {
        Strict,
        Inclusive,
    }
}
