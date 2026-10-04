using DSAExperimentation.DataStructures.MonotonicDeque;

namespace DSAExperimentation.LeetCode.ConstrainedSubsequenceSum;

// LeetCode 1425. Constrained Subsequence Sum: the largest sum of a non-empty
// subsequence of nums in which consecutive chosen indices are at most maxGap apart.
//
// Both strategies run the same recurrence - best[i] = nums[i] + max(0, the largest
// best value in the trailing window [i - maxGap, i - 1]), answer = max best - and
// differ only in how that trailing window maximum is obtained: rescanning the window
// at every position, or maintaining it in a monotonic deque.
internal static class ConstrainedSubsequenceSumSolution
{
    // The textbook answer: at each position, walk the maxGap preceding best values
    // looking for the largest one. O(n*k), BCL array only - the arm the deque strategy
    // has to justify itself against.
    public static int MaxSumByWindowRescan(int[] nums, int maxGap)
    {
        var bestSums = new int[nums.Length];
        var answer = int.MinValue;

        for (var i = 0; i < nums.Length; i++)
        {
            var windowMaximum = 0;

            for (var j = Math.Max(0, i - maxGap); j < i; j++)
            {
                windowMaximum = Math.Max(windowMaximum, bestSums[j]);
            }

            bestSums[i] = nums[i] + windowMaximum;
            answer = Math.Max(answer, bestSums[i]);
        }

        return answer;
    }

    // This repo's MonotonicDeque under MaxWindowOrder: indices are held with their
    // best sums in decreasing order, so the front is always the window's maximum -
    // exactly the technique SlidingWindowMaximum uses, except the values being
    // compared are computed by the same loop that maintains the window rather than
    // read from a fixed input array, so the deque's stored key is the only copy the
    // walk needs. Every index is pushed and popped at most once, so the whole walk is
    // O(n) instead of O(n*k).
    public static int MaxSumByMonotonicDeque(int[] nums, int maxGap)
    {
        var window = new BestSumWindow(nums, maxGap);
        var answer = int.MinValue;

        for (var i = 0; i < nums.Length; i++)
        {
            var value = window.ComputeBestSumAt(i);
            answer = Math.Max(answer, value);
        }

        return answer;
    }

    private sealed class BestSumWindow(int[] nums, int maxGap)
    {
        private readonly MonotonicDeque<int, MaxWindowOrder<int>> _window = new();

        // The front index leaves the window once it is further than maxGap behind the
        // current one. Pushing the new best sum evicts every index whose best sum it
        // matches or beats: those can never be the window maximum again, since they
        // also leave the window no later than this one.
        public int ComputeBestSumAt(int index)
        {
            _window.EvictBefore(index - maxGap);

            var value = nums[index] + WindowMaximum();

            _window.Push(index, value);

            return value;
        }

        // The front holds the window's largest best-sum by construction; a negative
        // one is clamped away, which is how "start a fresh subsequence here" is said.
        private int WindowMaximum() =>
            _window.TryPeekFront(out var maximum) ? Math.Max(0, maximum.Key) : 0;
    }
}
