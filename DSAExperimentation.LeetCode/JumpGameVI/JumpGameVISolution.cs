using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.LeetCode.JumpGameVI;

// LeetCode 1696. Jump Game VI: start at index 0, each move jumps forward at most k
// steps, and the score is the sum of the values landed on - maximize it at index n-1.
//
// Both strategies run the same recurrence - best[i] = nums[i] + the largest best value
// in the trailing window [i-k, i-1], answer = best[n-1] - and differ only in how that
// trailing window maximum is obtained: rescanning the window at every position, or
// maintaining it in a monotonic deque.
//
// Unlike LC 1425 (ConstrainedSubsequenceSumSolution), every step here is a mandatory
// jump rather than an optional "skip this index", so the window maximum has no
// floor-at-zero and the answer is the last cell rather than the best cell.
internal static class JumpGameVISolution
{
    // The textbook answer: at each position, walk the k preceding best values looking
    // for the largest one. O(n*k), BCL array only - the arm the deque strategy has to
    // justify itself against.
    public static int MaxResultByWindowRescan(int[] nums, int k)
    {
        var bestResults = new int[nums.Length];
        bestResults[0] = nums[0];

        for (var i = 1; i < nums.Length; i++)
        {
            var windowMaximum = int.MinValue;

            for (var j = Math.Max(0, i - k); j < i; j++)
            {
                windowMaximum = Math.Max(windowMaximum, bestResults[j]);
            }

            bestResults[i] = nums[i] + windowMaximum;
        }

        return bestResults[^1];
    }

    // This repo's own Deque<int> as a monotonic window: indices are held in decreasing
    // best-result order, so the front is always the window's maximum - the same
    // technique ConstrainedSubsequenceSumSolution uses for LC 1425, where the compared
    // values are computed by the very loop that maintains the window rather than read
    // from a fixed input array. Every index is pushed and popped at most once, so the
    // whole walk is O(n) instead of O(n*k).
    public static int MaxResultByMonotonicDeque(int[] nums, int k)
    {
        var window = new BestResultWindow(nums, k);

        for (var i = 1; i < nums.Length; i++)
        {
            window.ExtendTo(i);
        }

        return window.BestAtLastIndex;
    }

    private sealed class BestResultWindow
    {
        private readonly int[] _nums;
        private readonly int _k;
        private readonly int[] _bestResults;
        private readonly RepoDeque _indices = new();

        public BestResultWindow(int[] nums, int k)
        {
            _nums = nums;
            _k = k;
            _bestResults = new int[nums.Length];
            _bestResults[0] = nums[0];
            _indices.PushBack(0);
        }

        public int BestAtLastIndex => _bestResults[^1];

        public void ExtendTo(int index)
        {
            DropExpiredFront(index);

            var value = _nums[index] + WindowMaximum();
            _bestResults[index] = value;

            DropDominatedBack(value);
            _indices.PushBack(index);
        }

        // The front index leaves the window once it is further than k behind the
        // current one; at most one expires per step, because the index advances by one.
        private void DropExpiredFront(int index)
        {
            while (_indices.TryPeekFront(out var frontIndex) && frontIndex < index - _k)
            {
                _indices.TryPopFront(out _);
            }
        }

        // The front holds the window's largest best-result by construction. The window
        // is never empty when this is called: index - 1 was pushed by the previous step
        // and cannot have expired, since k is at least 1. Every jump is mandatory, so
        // unlike LC 1425 there is no clamp at zero here.
        private int WindowMaximum()
            => _indices.TryPeekFront(out var maximumIndex) ? _bestResults[maximumIndex] : 0;

        // Indices whose best-result this one matches or beats can never be the window
        // maximum again, since they also leave the window no later than this one.
        private void DropDominatedBack(int value)
        {
            while (_indices.TryPeekBack(out var backIndex) && _bestResults[backIndex] <= value)
            {
                _indices.TryPopBack(out _);
            }
        }
    }
}
