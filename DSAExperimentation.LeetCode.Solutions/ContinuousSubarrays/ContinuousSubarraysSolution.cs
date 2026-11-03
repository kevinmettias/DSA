using DSAExperimentation.DataStructures.MonotonicDeque;
using DSAExperimentation.LeetCode.Solutions.Conventions;

namespace DSAExperimentation.LeetCode.Solutions.ContinuousSubarrays;

// LeetCode 2762. Continuous Subarrays: count the contiguous subarrays whose largest
// and smallest values differ by at most 2.
//
// This is LC 1438's window with the limit pinned at 2 and the answer changed from
// "how long is the best window" to "how many subarrays are there", so both
// strategies answer it the same way that problem's do and differ only in how a
// window learns its max and min. Every window ending at right that satisfies the
// limit contributes (right - left + 1) subarrays, one per admissible starting point,
// so summing that across every right is the whole answer.
[LeetCodeQuestion("continuous-subarrays", LeetCodeAnswerOrder.AsWritten)]
internal static class ContinuousSubarraysSolution
{
    // The problem fixes the admissible spread; unlike LC 1438 it is not an argument.
    private const int Limit = 2;

    // The textbook answer: every starting index grows its own window from scratch,
    // tracking a running max/min and counting each endpoint until the spread exceeds
    // the limit. Plain BCL indexing and Math.Max/Min, nothing from this repo - it is
    // the O(n^2) arm the deque strategy below has to justify itself against, and
    // putting it here is what finally gets it asserted.
    public static long CountContinuousSubarraysByBruteForceWindows(int[] nums)
    {
        var total = 0L;

        for (var start = 0; start < nums.Length; start++)
        {
            var windowMax = int.MinValue;
            var windowMin = int.MaxValue;

            for (var end = start; end < nums.Length; end++)
            {
                windowMax = Math.Max(windowMax, nums[end]);
                windowMin = Math.Min(windowMin, nums[end]);

                if (windowMax - windowMin > Limit)
                {
                    break;
                }

                total++;
            }
        }

        return total;
    }

    // This repo's MinMaxWindow over the expanding/shrinking range - two
    // MonotonicDeques inside, one under each window order - the exact
    // LongestContinuousSubarrayWithAbsoluteDiffLessThanOrEqualToLimit precedent,
    // accumulating window lengths instead of taking the largest. Each index is pushed
    // and popped from each deque at most once, so the whole sweep is O(n) rather than
    // the baseline's O(n^2) rescan.
    public static long CountContinuousSubarraysByMonotonicDeques(int[] nums)
    {
        var window = new MinMaxWindow<int>();
        var left = 0;
        var total = 0L;

        for (var right = 0; right < nums.Length; right++)
        {
            window.Push(right, nums[right]);
            left = ShrinkToLimit(window, left);

            // One subarray per starting point from the left edge through right.
            total += right - left + 1;
        }

        return total;
    }

    // Each step past the left edge drops whatever entries it leaves behind; the
    // advanced left edge comes back as the result.
    private static int ShrinkToLimit(MinMaxWindow<int> window, int left)
    {
        while (IsSpreadBeyondLimit(window))
        {
            left++;
            window.EvictBefore(left);
        }

        return left;
    }

    // The live window's spread is its max minus its min; an empty window has
    // nothing left to measure, so nothing can exceed the limit.
    private static bool IsSpreadBeyondLimit(MinMaxWindow<int> window)
        => window.TryPeekRange(out var minimum, out var maximum) && maximum - minimum > Limit;
}
