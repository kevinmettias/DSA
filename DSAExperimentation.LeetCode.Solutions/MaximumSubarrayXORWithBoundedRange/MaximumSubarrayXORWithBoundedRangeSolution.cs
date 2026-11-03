using DSAExperimentation.DataStructures.CountedBitTrie;
using DSAExperimentation.DataStructures.ElementAlgebra;
using DSAExperimentation.DataStructures.MonotonicDeque;
using DSAExperimentation.DataStructures.PrefixSums;
using DSAExperimentation.LeetCode.Solutions.Conventions;

namespace DSAExperimentation.LeetCode.Solutions.MaximumSubarrayXORWithBoundedRange;

// LeetCode 3845. Maximum Subarray XOR with Bounded Range: among every non-empty
// subarray whose largest and smallest elements differ by at most k (maxSpread
// here), report the largest XOR of its elements. A single element always
// qualifies (k >= 0), so there is always an answer.
[LeetCodeQuestion("maximum-subarray-xor-with-bounded-range", LeetCodeAnswerOrder.AsWritten)]
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
    // in a MinMaxWindow. A subarray [l, r]'s XOR is TotalBefore(r + 1) XOR
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
        var window = new MinMaxWindow<int>();
        var left = 0;
        var best = 0;

        for (var right = 0; right < nums.Length; right++)
        {
            live.Insert(prefix.TotalBefore(right));
            window.Push(right, nums[right]);
            var newLeft = ShrinkToBound(window, maxSpread, left);

            for (; left < newLeft; left++)
            {
                live.TryRemove(prefix.TotalBefore(left));
            }

            live.TryMaxXor(prefix.TotalBefore(right + 1), out var bestEndingHere);
            best = Math.Max(best, bestEndingHere);
        }

        return best;
    }

    // Restores the spread bound on the MinMaxWindow, exactly as
    // LongestContinuousSubarrayWithAbsoluteDiffLessThanOrEqualToLimitSolution does - each
    // step past the left edge dropping whatever entries it leaves behind - and reports
    // the window's new left edge. Each index enters and leaves each of the window's two
    // deques at most once.
    private static int ShrinkToBound(MinMaxWindow<int> window, int maxSpread, int left)
    {
        while (IsSpreadOverBound(window, maxSpread))
        {
            left++;
            window.EvictBefore(left);
        }

        return left;
    }

    private static bool IsSpreadOverBound(MinMaxWindow<int> window, int maxSpread) =>
        window.TryPeekRange(out var minimum, out var maximum) && maximum - minimum > maxSpread;
}
