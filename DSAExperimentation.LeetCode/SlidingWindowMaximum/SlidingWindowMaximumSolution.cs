using DSAExperimentation.DataStructures.MonotonicDeque;

namespace DSAExperimentation.LeetCode.SlidingWindowMaximum;

// LeetCode 239. Sliding Window Maximum: the maximum of every contiguous window
// of the given windowSize as it slides across nums, left to right.
//
// MaxSlidingWindowByBruteForceRescan is the textbook O(n*k) answer - rescan
// every window from scratch. MaxSlidingWindowByMonotonicDeque is this repo's
// MonotonicDeque under MaxWindowOrder: Push evicts the values the arriving one
// dominates from the back, EvictBefore drops the indices that fell out of the
// window from the front, and the front is the window's maximum. Every index is
// pushed and popped at most once, giving O(n) total instead of the O(n*k)
// per-window rescan.
internal static class SlidingWindowMaximumSolution
{
    public static int[] MaxSlidingWindowByBruteForceRescan(int[] nums, int windowSize)
    {
        var result = new int[nums.Length - windowSize + 1];

        for (var i = 0; i <= nums.Length - windowSize; i++)
        {
            var windowMax = int.MinValue;

            for (var j = i; j < i + windowSize; j++)
            {
                windowMax = Math.Max(windowMax, nums[j]);
            }

            result[i] = windowMax;
        }

        return result;
    }

    public static int[] MaxSlidingWindowByMonotonicDeque(int[] nums, int windowSize)
    {
        var result = new int[nums.Length - windowSize + 1];
        var sweep = (Window: new MonotonicDeque<int, MaxWindowOrder<int>>(), Result: result);

        for (var i = 0; i < nums.Length; i++)
        {
            SlideWindow(nums, windowSize, sweep, i);
        }

        return result;
    }

    // The sweep's two accumulators - the monotonic deque and the answers it fills -
    // are created together by the caller and mutated together on every step, so they
    // arrive as the one piece of state this pass carries.
    private static void SlideWindow(
        int[] nums,
        int windowSize,
        (MonotonicDeque<int, MaxWindowOrder<int>> Window, int[] Result) sweep,
        int index)
    {
        sweep.Window.Push(index, nums[index]);
        sweep.Window.EvictBefore(index - windowSize + 1);

        if (index >= windowSize - 1)
        {
            sweep.Window.TryPeekFront(out var maximum);
            sweep.Result[index - windowSize + 1] = maximum.Key;
        }
    }
}
