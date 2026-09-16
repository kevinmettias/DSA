using DSAExperimentation.Algorithms.Searching;

namespace DSAExperimentation.LeetCode.MaximumScoreOfAGoodSubarray;

// LeetCode 1793. Maximum Score of a Good Subarray: among the subarrays that
// contain index k, maximize min(nums[i..j]) * (j - i + 1).
//
// MaximumScoreByBruteForceExpand is the textbook O(n^2): fix a left end at or
// before k, take the running minimum out to k, then extend the right end past k
// one step at a time, scoring every window as it goes.
//
// MaximumScoreByMonotonicStackBoundaries flips the question from "what is each
// window's minimum" to "how wide is the window each element is the minimum of".
// Two NearestBoundary sweeps (the DailyTemperatures/SumOfSubarrayMinimums precedent,
// narrowed by the strict relation both sweeps here ask for) give, per index, the
// nearest strictly-smaller element to its left and to its right - the widest window
// nums[i] dominates. The optimal good subarray's minimum is one of those defining
// indices, so the answer is the best nums[i] * width among the indices whose
// window actually straddles k. Every index is pushed and popped at most once per
// sweep, so both are O(n).
internal static class MaximumScoreOfAGoodSubarraySolution
{
    // What the leftward sweep reports when nothing to an index's left is
    // smaller: one position before the array, so the "window straddles k" test
    // below reads the same way at the boundary as it does anywhere else.
    private const int BeforeFirstIndex = -1;

    // The textbook answer: re-derive every window's minimum from scratch.
    // Deliberately written without this repo's primitives - it is the arm the
    // composed solution below has to justify itself against.
    public static int MaximumScoreByBruteForceExpand(int[] nums, int k)
    {
        var best = 0;

        for (var left = 0; left <= k; left++)
        {
            var min = int.MaxValue;

            for (var i = left; i <= k; i++)
            {
                min = Math.Min(min, nums[i]);
            }

            for (var right = k; right < nums.Length; right++)
            {
                min = Math.Min(min, nums[right]);
                best = Math.Max(best, min * (right - left + 1));
            }
        }

        return best;
    }

    public static int MaximumScoreByMonotonicStackBoundaries(int[] nums, int k)
    {
        // Both sweeps take the strict relation: an equal neighbour is not a boundary, so a
        // run of equal minimums is claimed by its widest member rather than split between
        // them - which is what lets the width below be the whole window nums[i] dominates.
        var previousSmaller = NearestBoundary.SmallerToTheLeft(nums, BeforeFirstIndex);
        var nextSmaller = NearestBoundary.SmallerToTheRight(nums, nums.Length);

        var best = 0;

        for (var i = 0; i < nums.Length; i++)
        {
            // nums[i] is the minimum of everything strictly between its two
            // boundaries; that window is a good subarray only if it contains k.
            if (previousSmaller[i] < k && k < nextSmaller[i])
            {
                var width = nextSmaller[i] - previousSmaller[i] - 1;
                best = Math.Max(best, nums[i] * width);
            }
        }

        return best;
    }
}
