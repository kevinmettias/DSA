using DSAExperimentation.DataStructures.Deque;

namespace DSAExperimentation.LeetCode.CountNonDecreasingSubarraysAfterKOperations;

// LeetCode 3420. Count Non-Decreasing Subarrays After K Operations: since an
// operation can only increment (never decrement), the cheapest way to make
// nums[l..r] non-decreasing is to raise each element to the running maximum seen
// so far within the subarray - the pointwise-smallest non-decreasing sequence that
// still dominates it. So cost(l, r) = sum(runningMax(l, i) - nums[i]) for i in
// [l, r].
//
// Growing a window by extending its RIGHT end only ever appends one more
// non-negative term, so cost(l, r) is monotonic non-decreasing in r for fixed l -
// but shrinking from the LEFT is not a symmetric O(1) operation: dropping nums[l]
// can lower the running max for every later position that depended on it, which
// would force recomputing a whole plateau of the window's compressed profile.
// Shrinking from the RIGHT has no such problem - removing the last index never
// changes any earlier position's running max - so the composed strategy below
// walks `left` from n-1 down to 0 (growing the window by prepending, where a new
// leftmost element can only ever replace a SMALLER prefix of what's already
// there) and shrinks by dropping `right` instead, keeping every window update
// O(1) amortized.
internal static class CountNonDecreasingSubarraysAfterKOperationsSolution
{
    // The textbook approach: for every left endpoint, walk right recomputing the
    // running maximum and the accumulated cost from scratch, stopping as soon as
    // it exceeds maxOperations (cost only grows from there). O(n^2) worst case -
    // the arm the sliding-window strategy has to beat.
    public static long CountByPrefixMaxBruteForce(int[] nums, int maxOperations)
    {
        var count = 0L;

        for (var left = 0; left < nums.Length; left++)
        {
            var runningMax = 0L;
            var cost = 0L;

            for (var right = left; right < nums.Length; right++)
            {
                runningMax = Math.Max(runningMax, nums[right]);
                cost += runningMax - nums[right];

                if (cost > maxOperations)
                {
                    break;
                }

                count++;
            }
        }

        return count;
    }

    // Composed: a sliding window over this repo's own Deque<(Value, Count)>,
    // walked right-to-left. The window's running-max profile (relative to its
    // current `left`) is non-decreasing from `left` to `right`, so it compresses
    // into plateaus - a new leftmost element merges every plateau it dominates at
    // the FRONT (the ones nearest `left`, since those are exactly the positions a
    // bigger element prepended in front of them would replace) and the window
    // shrinks by peeling one element at a time off the BACK (nearest `right`)
    // whenever the total cost still exceeds maxOperations.
    public static long CountByMonotonicDequeWindow(int[] nums, int maxOperations)
    {
        var window = new Deque<(long Value, long Count)>();
        var count = 0L;
        var sumOfRunningMax = 0L;
        var sumOfValues = 0L;
        var right = nums.Length - 1;

        for (var left = nums.Length - 1; left >= 0; left--)
        {
            (sumOfRunningMax, sumOfValues, right) =
                ExtendLeft(window, nums, maxOperations, (left, right, sumOfRunningMax, sumOfValues));

            count += right - left + 1;
        }

        return count;
    }

    // Absorb nums[left] as the window's new leftmost element, then peel elements off
    // the back until the window's cost is back within maxOperations. Returns the
    // advanced sums and the window's new right end.
    private static (long SumOfRunningMax, long SumOfValues, int Right) ExtendLeft(
        Deque<(long Value, long Count)> window,
        int[] nums,
        long maxOperations,
        (int Left, int Right, long SumOfRunningMax, long SumOfValues) frame)
    {
        var value = (long)nums[frame.Left];
        var sumOfValues = frame.SumOfValues + value;
        var sumOfRunningMax = frame.SumOfRunningMax + MergeIntoFront(window, value);
        var right = frame.Right;

        while (sumOfRunningMax - sumOfValues > maxOperations)
        {
            sumOfRunningMax -= ShrinkBack(window);
            sumOfValues -= nums[right];
            right--;
        }

        return (sumOfRunningMax, sumOfValues, right);
    }

    // Pops every front plateau strictly smaller than the new leftmost value (they
    // collapse into it, since the running max from `left` onward through them is
    // now `value`), then pushes the merged plateau back onto the front. Returns
    // the NET change to sumOfRunningMax: the merged plateau's own contribution
    // minus the (smaller) contributions it just replaced.
    private static long MergeIntoFront(Deque<(long Value, long Count)> window, long value)
    {
        var mergedCount = 1L;
        var replaced = 0L;

        while (window.TryPeekFront(out var front) && front.Value < value)
        {
            window.TryPopFront(out _);
            mergedCount += front.Count;
            replaced += front.Value * front.Count;
        }

        window.PushFront((value, mergedCount));
        return value * mergedCount - replaced;
    }

    // Removes exactly one element - the window's current rightmost - from the back
    // plateau, popping it entirely only once its count reaches zero. Returns that
    // one element's running-max contribution being removed.
    private static long ShrinkBack(Deque<(long Value, long Count)> window)
    {
        window.TryPeekBack(out var back);
        window.TryPopBack(out _);

        if (back.Count > 1)
        {
            window.PushBack((back.Value, back.Count - 1));
        }

        return back.Value;
    }
}
