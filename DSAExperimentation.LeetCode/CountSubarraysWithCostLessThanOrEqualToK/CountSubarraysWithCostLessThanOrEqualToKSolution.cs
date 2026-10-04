using DSAExperimentation.DataStructures.MonotonicDeque;

namespace DSAExperimentation.LeetCode.CountSubarraysWithCostLessThanOrEqualToK;

// LeetCode 3835. Count Subarrays With Cost Less Than or Equal to K: cost(l, r) =
// (max(nums[l..r]) - min(nums[l..r])) * (r - l + 1). Extending a window in either
// direction can only grow both factors - widening never shrinks the running
// max-min gap or the length - so cost(l, r) is non-decreasing as the window
// widens on either side. That is the same monotonicity LC 1438's fixed-limit
// window exploits, just paired with a length term instead of a flat bound: for a
// fixed r, the smallest valid l only ever moves forward as r grows, and once
// [l, r] satisfies the cost bound, every [l'..r] with l' >= l does too, so all
// (r - l + 1) of them count at once.
internal static class CountSubarraysWithCostLessThanOrEqualToKSolution
{
    // The textbook O(n^2) scan: for every start index, extend the end one
    // element at a time, tracking the running max/min directly against the
    // growing window. Deliberately written without this repo's primitives - it
    // is the arm the monotonic-deque strategy below has to justify itself
    // against.
    public static long CountByBruteForce(int[] nums, long costLimit)
    {
        var count = 0L;

        for (var left = 0; left < nums.Length; left++)
        {
            var max = nums[left];
            var min = nums[left];

            for (var right = left; right < nums.Length; right++)
            {
                max = Math.Max(max, nums[right]);
                min = Math.Min(min, nums[right]);

                if ((long)(max - min) * (right - left + 1) <= costLimit)
                {
                    count++;
                }
            }
        }

        return count;
    }

    // This repo's MonotonicDeque, doubled up the same way LC 1438's coverage
    // already does: one under MaxWindowOrder for the running max, one under
    // MinWindowOrder for the running min, so both ends of the window's cost are
    // an O(1) front-peek instead of an O(window) rescan. Every index is pushed
    // and popped from each deque at most once, so the whole sweep is O(n)
    // amortized even though left never resets between iterations of r.
    public static long CountByMonotonicDeques(int[] nums, long costLimit)
    {
        var maxWindow = new MonotonicDeque<int, MaxWindowOrder<int>>();
        var minWindow = new MonotonicDeque<int, MinWindowOrder<int>>();
        var count = 0L;
        var left = 0;

        for (var right = 0; right < nums.Length; right++)
        {
            maxWindow.Push(right, nums[right]);
            minWindow.Push(right, nums[right]);
            left = ShrinkToCost((maxWindow, minWindow), (left, right), costLimit);
            count += right - left + 1;
        }

        return count;
    }

    // The window's two extremes travel together and so do its two bounds: each pair is
    // one argument, and the shrunk left edge still comes back as the result rather than
    // as a second out-of-band value.
    private static int ShrinkToCost(
        (MonotonicDeque<int, MaxWindowOrder<int>> MaxWindow, MonotonicDeque<int, MinWindowOrder<int>> MinWindow) extremes,
        (int Left, int Right) window,
        long costLimit)
    {
        var (maxWindow, minWindow) = extremes;
        var left = window.Left;

        while (TryPeekWindowExtremes(maxWindow, minWindow, out var maxFront, out var minFront) &&
               IsWindowCostOverBudget(maxFront.Key - minFront.Key, window.Right - left + 1, costLimit))
        {
            left++;
            maxWindow.EvictBefore(left);
            minWindow.EvictBefore(left);
        }

        return left;
    }

    // Both ends of the window are recorded at the deques' fronts, and the cost is only
    // worth asking about once both are there.
    private static bool TryPeekWindowExtremes(
        MonotonicDeque<int, MaxWindowOrder<int>> maxWindow,
        MonotonicDeque<int, MinWindowOrder<int>> minWindow,
        out (int Position, int Key) maxFront,
        out (int Position, int Key) minFront)
    {
        // Assigned on every path so the short-circuit below is still definite: the
        // caller only reads minFront when the result is true.
        minFront = default;

        return maxWindow.TryPeekFront(out maxFront) && minWindow.TryPeekFront(out minFront);
    }

    // The problem's cost formula: the gap between the window's largest and smallest
    // element, times how many elements the window holds.
    private static bool IsWindowCostOverBudget(int maxMinGap, int windowLength, long costLimit) =>
        (long)maxMinGap * windowLength > costLimit;
}
