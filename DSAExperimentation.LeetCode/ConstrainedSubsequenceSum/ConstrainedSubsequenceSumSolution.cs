using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.LeetCode.ConstrainedSubsequenceSum;

// LeetCode 1425. Constrained Subsequence Sum: the largest sum of a non-empty
// subsequence of nums in which consecutive chosen indices are at most k apart.
//
// Both strategies run the same recurrence - best[i] = nums[i] + max(0, the largest
// best value in the trailing window [i-k, i-1]), answer = max best - and differ only
// in how that trailing window maximum is obtained: rescanning the window at every
// position, or maintaining it in a monotonic deque.
internal static class ConstrainedSubsequenceSumSolution
{
    // The textbook answer: at each position, walk the k preceding best values looking
    // for the largest one. O(n*k), BCL array only - the arm the deque strategy has to
    // justify itself against.
    public static int MaxSumByWindowRescan(int[] nums, int k)
    {
        var bestSums = new int[nums.Length];
        var answer = int.MinValue;

        for (var i = 0; i < nums.Length; i++)
        {
            var windowMaximum = 0;

            for (var j = Math.Max(0, i - k); j < i; j++)
            {
                windowMaximum = Math.Max(windowMaximum, bestSums[j]);
            }

            bestSums[i] = nums[i] + windowMaximum;
            answer = Math.Max(answer, bestSums[i]);
        }

        return answer;
    }

    // This repo's own Deque<int> as a monotonic window: indices are held in
    // decreasing best-sum order, so the front is always the window's maximum -
    // exactly the technique SlidingWindowMaximum uses, except the values being
    // compared are computed by the same loop that maintains the window rather than
    // read from a fixed input array. Every index is pushed and popped at most once,
    // so the whole walk is O(n) instead of O(n*k).
    public static int MaxSumByMonotonicDeque(int[] nums, int k)
    {
        var window = new BestSumWindow(nums, k);
        var answer = int.MinValue;

        for (var i = 0; i < nums.Length; i++)
        {
            var value = window.ComputeBestSumAt(i);
            answer = Math.Max(answer, value);
        }

        return answer;
    }

    private sealed class BestSumWindow(int[] nums, int k)
    {
        private readonly int[] _bestSums = new int[nums.Length];
        private readonly RepoDeque _indices = new();

        public int ComputeBestSumAt(int index)
        {
            DropExpiredFront(index);

            var value = nums[index] + WindowMaximum();

            DropDominatedBack(value);
            _indices.PushBack(index);
            _bestSums[index] = value;

            return value;
        }

        // The front index leaves the window once it is further than k behind the
        // current one; at most one expires per step, because the index advances by one.
        private void DropExpiredFront(int index)
        {
            if (_indices.TryPeekFront(out var frontIndex) && frontIndex < index - k)
            {
                _indices.TryPopFront(out _);
            }
        }

        // The front holds the window's largest best-sum by construction; a negative
        // one is clamped away, which is how "start a fresh subsequence here" is said.
        private int WindowMaximum() =>
            _indices.TryPeekFront(out var maximumIndex) ? Math.Max(0, _bestSums[maximumIndex]) : 0;

        // Indices whose best-sum this one matches or beats can never be the window
        // maximum again, since they also leave the window no later than this one.
        private void DropDominatedBack(int value)
        {
            while (_indices.TryPeekBack(out var backIndex) && _bestSums[backIndex] <= value)
            {
                _indices.TryPopBack(out _);
            }
        }
    }
}
