using RepoIndexStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.MaximumWidthRamp;

// LeetCode 962. Maximum Width Ramp: the widest (i, j) with i < j and nums[i] <= nums[j],
// reported as j - i, or 0 when no such pair exists.
//
// Both strategies answer the same question with the same signature: the O(n^2) pairwise
// scan over every candidate pair, and the O(n) two-pass candidate stack that only ever
// keeps left endpoints no later index can improve on.
internal static class MaximumWidthRampSolution
{
    // The textbook O(n^2) baseline: widen the answer at every (i, j) pair that qualifies.
    // BCL-only by design - this is what you would write without this repo.
    public static int MaxWidthRampByPairwiseScan(int[] nums)
    {
        var maxWidth = 0;

        for (var i = 0; i < nums.Length; i++)
        {
            for (var j = i + 1; j < nums.Length; j++)
            {
                if (nums[i] <= nums[j])
                {
                    maxWidth = Math.Max(maxWidth, j - i);
                }
            }
        }

        return maxWidth;
    }

    // Build a monotonically-decreasing candidate stack of left-endpoint indices over
    // this repo's own Stack<int> (the CarFleet/DailyTemperatures precedent for this
    // repo's Stack rather than the CLR's), then walk right-to-left popping every
    // candidate whose value is <= the current right endpoint. Each pop is that
    // candidate's widest ramp, because the scan only ever visits right endpoints in
    // decreasing order, so the first j that clears a candidate is its furthest one.
    public static int MaxWidthRampByCandidateStack(int[] nums)
    {
        var candidates = new RepoIndexStack();

        for (var i = 0; i < nums.Length; i++)
        {
            if (!candidates.TryPeek(out var topIndex) || nums[topIndex] > nums[i])
            {
                candidates.Push(i);
            }
        }

        var maxWidth = 0;

        for (var j = nums.Length - 1; j >= 0; j--)
        {
            while (candidates.TryPeek(out var topIndex) && nums[topIndex] <= nums[j])
            {
                candidates.TryPop(out _);
                maxWidth = Math.Max(maxWidth, j - topIndex);
            }
        }

        return maxWidth;
    }
}
