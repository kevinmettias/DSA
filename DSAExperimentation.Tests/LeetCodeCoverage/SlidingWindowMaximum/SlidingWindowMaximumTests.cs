using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SlidingWindowMaximum;

// LeetCode 239. Sliding Window Maximum: this repo's own Deque<int> holds window
// indices in decreasing-value order (a monotonic deque) - PushBack after
// evicting smaller trailing values from the back, TryPeekFront/TryPopFront
// evicting indices that fell out of the window from the front. Every index is
// pushed and popped at most once, giving O(n) total instead of the O(n*k)
// per-window rescan.
public sealed class SlidingWindowMaximumTests
{
    [Fact]
    public void MaxSlidingWindow_ClassicExample_ReturnsPerWindowMaximums()
    {
        int[] nums = [1, 3, -1, -3, 5, 3, 6, 7];

        var result = MaxSlidingWindow(nums, k: 3);

        Assert.Equal([3, 3, 5, 5, 6, 7], result);
    }

    [Fact]
    public void MaxSlidingWindow_WindowSizeOne_ReturnsInputUnchanged()
    {
        int[] nums = [4, -2, 9];

        var result = MaxSlidingWindow(nums, k: 1);

        Assert.Equal([4, -2, 9], result);
    }

    private static int[] MaxSlidingWindow(int[] nums, int k)
    {
        var result = new int[nums.Length - k + 1];
        var window = new RepoDeque();
        var query = new WindowQuery(nums, k);

        for (var i = 0; i < nums.Length; i++)
        {
            SlideWindow(query, window, result, i);
        }

        return result;
    }

    private static void SlideWindow(WindowQuery query, RepoDeque window, int[] result, int i)
    {
        while (window.TryPeekBack(out var backIndex) && query.Nums[backIndex] <= query.Nums[i])
        {
            window.TryPopBack(out _);
        }

        window.PushBack(i);

        if (window.TryPeekFront(out var frontIndex) && frontIndex <= i - query.K)
        {
            window.TryPopFront(out _);
        }

        if (i >= query.K - 1)
        {
            window.TryPeekFront(out var maxIndex);
            result[i - query.K + 1] = query.Nums[maxIndex];
        }
    }

    private readonly record struct WindowQuery(int[] Nums, int K);
}
