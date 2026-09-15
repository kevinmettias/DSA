using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.LeetCode.SlidingWindowMaximum;

// LeetCode 239. Sliding Window Maximum: the maximum of every contiguous window
// of size k as it slides across nums, left to right.
//
// MaxSlidingWindowByBruteForceRescan is the textbook O(n*k) answer - rescan
// every window from scratch. MaxSlidingWindowByMonotonicDeque is this repo's
// own Deque<int> used as a monotonic decreasing-value index window: PushBack
// after evicting smaller trailing values from the back, TryPeekFront /
// TryPopFront evicting indices that fell out of the window from the front.
// Every index is pushed and popped at most once, giving O(n) total instead of
// the O(n*k) per-window rescan.
internal static class SlidingWindowMaximumSolution
{
    public static int[] MaxSlidingWindowByBruteForceRescan(int[] nums, int k)
    {
        var result = new int[nums.Length - k + 1];

        for (var i = 0; i <= nums.Length - k; i++)
        {
            var windowMax = int.MinValue;

            for (var j = i; j < i + k; j++)
            {
                windowMax = Math.Max(windowMax, nums[j]);
            }

            result[i] = windowMax;
        }

        return result;
    }

    public static int[] MaxSlidingWindowByMonotonicDeque(int[] nums, int k)
    {
        var result = new int[nums.Length - k + 1];
        var sweep = (Window: new RepoDeque(), Result: result);

        for (var i = 0; i < nums.Length; i++)
        {
            SlideWindow(nums, k, sweep, i);
        }

        return result;
    }

    // The sweep's two accumulators - the monotonic deque and the answers it fills -
    // are created together by the caller and mutated together on every step, so they
    // arrive as the one piece of state this pass carries.
    private static void SlideWindow(int[] nums, int k, (RepoDeque Window, int[] Result) sweep, int i)
    {
        while (sweep.Window.TryPeekBack(out var backIndex) && nums[backIndex] <= nums[i])
        {
            sweep.Window.TryPopBack(out _);
        }

        sweep.Window.PushBack(i);

        if (sweep.Window.TryPeekFront(out var frontIndex) && frontIndex <= i - k)
        {
            sweep.Window.TryPopFront(out _);
        }

        if (i >= k - 1)
        {
            sweep.Window.TryPeekFront(out var maxIndex);
            sweep.Result[i - k + 1] = nums[maxIndex];
        }
    }
}
