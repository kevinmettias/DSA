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
        var window = new RepoDeque();

        for (var i = 0; i < nums.Length; i++)
        {
            SlideWindow(nums, k, window, result, i);
        }

        return result;
    }

    private static void SlideWindow(int[] nums, int k, RepoDeque window, int[] result, int i)
    {
        while (window.TryPeekBack(out var backIndex) && nums[backIndex] <= nums[i])
        {
            window.TryPopBack(out _);
        }

        window.PushBack(i);

        if (window.TryPeekFront(out var frontIndex) && frontIndex <= i - k)
        {
            window.TryPopFront(out _);
        }

        if (i >= k - 1)
        {
            window.TryPeekFront(out var maxIndex);
            result[i - k + 1] = nums[maxIndex];
        }
    }
}
