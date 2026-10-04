using DSAExperimentation.DataStructures.MonotonicDeque;

namespace DSAExperimentation.LeetCode.JumpGameVI;

// LeetCode 1696. Jump Game VI: start at index 0, each move jumps forward at most
// jumpLimit steps, and the score is the sum of the values landed on - maximize it at
// index n-1.
//
// Both strategies run the same recurrence - best[i] = nums[i] + the largest best value
// in the trailing window [i - jumpLimit, i - 1], answer = best[n-1] - and differ only in how that
// trailing window maximum is obtained: rescanning the window at every position, or
// maintaining it in a monotonic deque.
//
// Unlike LC 1425 (ConstrainedSubsequenceSumSolution), every step here is a mandatory
// jump rather than an optional "skip this index", so the window maximum has no
// floor-at-zero and the answer is the last cell rather than the best cell.
internal static class JumpGameVISolution
{
    // The textbook answer: at each position, walk the jumpLimit preceding best values
    // looking for the largest one. O(n*k), BCL array only - the arm the deque strategy
    // has to justify itself against.
    public static int MaxResultByWindowRescan(int[] nums, int jumpLimit)
    {
        var bestResults = new int[nums.Length];
        bestResults[0] = nums[0];

        for (var i = 1; i < nums.Length; i++)
        {
            var windowMaximum = int.MinValue;

            for (var j = Math.Max(0, i - jumpLimit); j < i; j++)
            {
                windowMaximum = Math.Max(windowMaximum, bestResults[j]);
            }

            bestResults[i] = nums[i] + windowMaximum;
        }

        return bestResults[^1];
    }

    // This repo's MonotonicDeque under MaxWindowOrder: indices are held with their best
    // results in decreasing order, so the front is always the window's maximum - the same
    // technique ConstrainedSubsequenceSumSolution uses for LC 1425, where the compared
    // values are computed by the very loop that maintains the window rather than read
    // from a fixed input array. Every index is pushed and popped at most once, so the
    // whole walk is O(n) instead of O(n*k).
    public static int MaxResultByMonotonicDeque(int[] nums, int jumpLimit)
    {
        var window = new BestResultWindow(nums, jumpLimit);

        for (var i = 1; i < nums.Length; i++)
        {
            window.ExtendTo(i);
        }

        return window.BestAtLastIndex;
    }

    private sealed class BestResultWindow
    {
        private readonly int[] _nums;
        private readonly int _jumpLimit;
        private readonly int[] _bestResults;
        private readonly MonotonicDeque<int, MaxWindowOrder<int>> _window = new();

        public int BestAtLastIndex => _bestResults[^1];

        public BestResultWindow(int[] nums, int jumpLimit)
        {
            _nums = nums;
            _jumpLimit = jumpLimit;
            _bestResults = new int[nums.Length];
            _bestResults[0] = nums[0];
            _window.Push(0, nums[0]);
        }

        // The front index leaves the window once it is further than jumpLimit behind
        // the current one. Pushing the new best result evicts every index whose best
        // result it matches or beats: those can never be the window maximum again,
        // since they also leave the window no later than this one.
        public void ExtendTo(int index)
        {
            _window.EvictBefore(index - _jumpLimit);

            var value = _nums[index] + WindowMaximum();
            _bestResults[index] = value;

            _window.Push(index, value);
        }

        // The front holds the window's largest best-result by construction. The window
        // is never empty when this is called: index - 1 was pushed by the previous step
        // and cannot have expired, since the jump limit is at least 1. Every jump is mandatory, so
        // unlike LC 1425 there is no clamp at zero here.
        private int WindowMaximum()
            => _window.TryPeekFront(out var maximum) ? maximum.Key : 0;
    }
}
