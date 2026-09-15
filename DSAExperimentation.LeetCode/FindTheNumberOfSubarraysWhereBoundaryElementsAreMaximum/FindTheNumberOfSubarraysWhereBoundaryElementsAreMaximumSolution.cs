using ValueRunStack = DSAExperimentation.DataStructures.Stack.Stack<(int Value, long Count)>;

namespace DSAExperimentation.LeetCode.FindTheNumberOfSubarraysWhereBoundaryElementsAreMaximum;

// LeetCode 3113. Find the Number of Subarrays Where Boundary Elements Are Maximum:
// count subarrays whose first and last elements both equal the subarray's own
// maximum.
//
// A monotonic non-increasing stack of (value, count) runs answers this in one
// left-to-right pass. A stacked value smaller than the incoming element can never
// again be a valid left boundary - the incoming element is now a bigger "wall"
// standing between it and everything still to come - so it is popped for good. An
// incoming element equal to the (post-pop) top extends that run: every one of the
// run's earlier occurrences, plus the element itself, pairs with the current
// index as a newly valid [left, i] subarray, which is exactly what incrementing
// the run's count and adding the new count to the total counts. An incoming
// element strictly less than the top starts a fresh run of its own, contributing
// only itself.
//
// Repo Stack<T> is aliased (ValueRunStack) rather than used via a bare `using` +
// `new Stack<T>()`: this project's global `using System.Collections.Generic;`
// makes a bare `Stack<T>` ambiguous (CS0104) against the BCL's own Stack<T> - the
// same collision MaximumEleganceOfAKLengthSubsequenceSolution's RepoProfitStack
// works around.
internal static class FindTheNumberOfSubarraysWhereBoundaryElementsAreMaximumSolution
{
    // Baseline: extend every subarray's right endpoint in a plain nested loop
    // while tracking its running maximum - O(n^2), "what you'd write without this
    // repo".
    public static long CountByBruteForce(int[] nums)
    {
        var count = 0L;

        for (var left = 0; left < nums.Length; left++)
        {
            var max = nums[left];

            for (var right = left; right < nums.Length; right++)
            {
                max = Math.Max(max, nums[right]);

                if (nums[left] == nums[right] && nums[left] == max)
                {
                    count++;
                }
            }
        }

        return count;
    }

    // Composed: this repo's own Stack<T> as the monotonic run stack.
    public static long CountByMonotonicStack(int[] nums)
    {
        var stack = new ValueRunStack();
        var count = 0L;

        foreach (var value in nums)
        {
            count = FoldRun(count, stack, value);
        }

        return count;
    }

    // Folds one more element into the run stack and returns the updated total:
    // pop every run the incoming value dominates, extend the top run when it
    // equals the value, otherwise start a fresh run of one. Every occurrence in
    // the extended run pairs with the current index as a new valid subarray.
    private static long FoldRun(long count, ValueRunStack stack, int value)
    {
        while (stack.TryPeek(out var smaller) && smaller.Value < value)
        {
            stack.TryPop(out _);
        }

        long runCount;

        if (stack.TryPeek(out var run) && run.Value == value)
        {
            stack.TryPop(out _);
            runCount = run.Count + 1;
        }
        else
        {
            runCount = 1;
        }

        stack.Push((value, runCount));

        return count + runCount;
    }
}
