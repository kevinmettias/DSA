using DSAExperimentation.DataStructures.CountedBitTrie;
using DSAExperimentation.DataStructures.ElementAlgebra;
using DSAExperimentation.DataStructures.MonotonicDeque;
using DSAExperimentation.DataStructures.PrefixSums;

namespace DSAExperimentation.LeetCode.MaximumSubarrayXORWithBoundedRange;

// LeetCode 3845. Maximum Subarray XOR with Bounded Range: among every non-empty
// subarray whose largest and smallest elements differ by at most k (maxSpread
// here), report the largest XOR of its elements. A single element always
// qualifies (k >= 0), so there is always an answer.
internal static class MaximumSubarrayXORWithBoundedRangeSolution
{
    // The textbook O(n^2) scan: from every start index, extend the end one element
    // at a time, keeping the running max, min and XOR, until the spread passes
    // maxSpread. Deliberately written without this repo's primitives - the arm the
    // composed strategy below has to beat.
    public static int MaxSubarrayXorByBruteForce(int[] nums, int maxSpread)
    {
        var best = 0;

        for (var start = 0; start < nums.Length; start++)
        {
            var bestFromStart = BestXorStartingAt(nums, start, maxSpread);
            best = Math.Max(best, bestFromStart);
        }

        return best;
    }

    private static int BestXorStartingAt(int[] nums, int start, int maxSpread)
    {
        var (max, min) = (nums[start], nums[start]);
        var runningXor = 0;
        var best = 0;

        for (var end = start; end < nums.Length; end++)
        {
            max = Math.Max(max, nums[end]);
            min = Math.Min(min, nums[end]);

            if (max - min > maxSpread)
            {
                break;
            }

            runningXor ^= nums[end];
            best = Math.Max(best, runningXor);
        }

        return best;
    }

    // Composed: dropping an element from a subarray can only shrink its spread, so
    // for each right edge the valid left edges form one run [left, right] whose
    // left end only ever moves right - the window
    // LongestContinuousSubarrayWithAbsoluteDiffLessThanOrEqualToLimitSolution keeps
    // with two MonotonicDeques. A subarray [l, r]'s XOR is TotalBefore(r + 1) XOR
    // TotalBefore(l) of this repo's PrefixSums under XorOperation, so the best
    // subarray ending at r is the best XOR of TotalBefore(r + 1) against the live
    // totals TotalBefore(left..r) - one greedy walk down this repo's CountedBitTrie,
    // which takes back each total the left edge passes. The window always holds
    // TotalBefore(right), so that walk always finds a live value.
    // O(n * 32) against the scan's O(n * window).
    public static int MaxSubarrayXorBySlidingWindowBitTrie(int[] nums, int maxSpread)
    {
        var prefix = new PrefixSums<int, XorOperation<int>>(nums);
        var live = new CountedBitTrie();
        var window = new MinMaxWindow(nums, maxSpread);
        var left = 0;
        var best = 0;

        for (var right = 0; right < nums.Length; right++)
        {
            live.Insert(prefix.TotalBefore(right));
            var newLeft = window.Advance(right);

            for (; left < newLeft; left++)
            {
                live.TryRemove(prefix.TotalBefore(left));
            }

            live.TryMaxXor(prefix.TotalBefore(right + 1), out var bestEndingHere);
            best = Math.Max(best, bestEndingHere);
        }

        return best;
    }

    // Two MonotonicDeques over the same window - one under MaxWindowOrder (its front
    // is the window's max), one under MinWindowOrder (its front is the min) - exactly
    // as LongestContinuousSubarrayWithAbsoluteDiffLessThanOrEqualToLimitSolution keeps
    // them. Each index enters and leaves each deque at most once.
    private sealed class MinMaxWindow(int[] nums, int maxSpread)
    {
        private readonly MonotonicDeque<int, MaxWindowOrder<int>> _maxWindow = new();
        private readonly MonotonicDeque<int, MinWindowOrder<int>> _minWindow = new();
        private int _left;

        // Admits index right, restores the spread bound - each step past the left
        // edge dropping whatever front entries it leaves behind in both deques - and
        // reports the window's new left edge.
        public int Advance(int right)
        {
            _maxWindow.Push(right, nums[right]);
            _minWindow.Push(right, nums[right]);

            while (IsSpreadOverBound())
            {
                _left++;
                _maxWindow.EvictBefore(_left);
                _minWindow.EvictBefore(_left);
            }

            return _left;
        }

        private bool IsSpreadOverBound() =>
            _maxWindow.TryPeekFront(out var maximum) && _minWindow.TryPeekFront(out var minimum) &&
            maximum.Key - minimum.Key > maxSpread;
    }
}
