using ValueStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.MinimumOperationsToConvertAllElementsToZero;

// LeetCode 3542. Minimum Operations to Convert All Elements to Zero: one operation
// picks any subarray and zeroes every occurrence of that subarray's minimum value.
// Repeatedly operating on the whole remaining range's minimum is always optimal, so
// the answer is the count of distinct nonzero "levels" that decomposition visits.
//
// Both strategies walk that same decomposition; they differ only in how they find
// each level's boundary - a fresh linear scan for the range minimum (the textbook
// divide-and-conquer reading of the operation itself) against this repo's own
// Stack<int>, which discovers the same boundaries in one left-to-right pass.
internal static class MinimumOperationsToConvertAllElementsToZeroSolution
{
    // Recursively finds the minimum of the current range, counts one operation for
    // it (unless it is already 0), and recurses on every maximal segment between
    // consecutive occurrences of that minimum. Each level does an O(range) scan, so
    // this is O(n^2) on a strictly monotonic array - the arm the monotonic-stack
    // strategy below has to beat.
    public static int MinOperationsByDivideAndConquer(int[] nums) =>
        MinOperationsByDivideAndConquer(nums, 0, nums.Length - 1);

    private static int MinOperationsByDivideAndConquer(int[] nums, int left, int right)
    {
        if (left > right)
        {
            return 0;
        }

        var min = RangeMinimum(nums, left, right);
        var operations = min == 0 ? 0 : 1;

        return operations + CountSplitOperations(nums, min, left, right);
    }

    // The smallest value in nums[left..right] - the level the whole range shares,
    // and therefore the one level this call's own operation may zero out.
    private static int RangeMinimum(int[] nums, int left, int right)
    {
        var min = nums[left];

        for (var i = left + 1; i <= right; i++)
        {
            min = Math.Min(min, nums[i]);
        }

        return min;
    }

    // The sub-problems one level creates: a recursive call on every maximal run of
    // values that are NOT that level - the gaps between its consecutive occurrences,
    // plus the runs before the first and after the last.
    private static int CountSplitOperations(int[] nums, int level, int left, int right)
    {
        var operations = 0;
        var segmentStart = left;

        for (var i = left; i <= right; i++)
        {
            if (nums[i] != level)
            {
                continue;
            }

            operations += MinOperationsByDivideAndConquer(nums, segmentStart, i - 1);
            segmentStart = i + 1;
        }

        return operations + MinOperationsByDivideAndConquer(nums, segmentStart, right);
    }

    // One left-to-right pass over this repo's own Stack<int>, kept non-decreasing
    // bottom-to-top: a value strictly greater than the incoming one has become
    // separated from it and can never share an operation with anything that
    // follows, so it is popped and charged one operation on the spot. Equal
    // top-of-stack values are deduplicated (they will still share one operation),
    // and 0 is never pushed - it needs no operation of its own. Whatever survives
    // on the stack at the end still needs one operation each.
    public static int MinOperationsByMonotonicStack(int[] nums)
    {
        var stack = new ValueStack();
        var operations = 0;

        foreach (var value in nums)
        {
            while (stack.TryPeek(out var top) && top > value)
            {
                stack.TryPop(out _);
                operations++;
            }

            if (NeedsPushing(stack, value))
            {
                stack.Push(value);
            }
        }

        return operations + stack.Count;
    }

    // A level is pushed only when it needs an operation of its own (it is nonzero) and
    // differs from whatever already sits on top - an empty stack holds nothing to differ
    // from.
    private static bool NeedsPushing(ValueStack stack, int value) =>
        value != 0 && (!stack.TryPeek(out var top) || top != value);
}
