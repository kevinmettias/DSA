using RangeIndexStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.SumOfSubarrayRanges;

// LeetCode 2104. Sum of Subarray Ranges: sum (max(b) - min(b)) over every
// contiguous subarray b.
//
// SubarrayRangesByBruteForce is the textbook O(n^2): fix a start index, extend a
// running minimum and maximum rightwards, and add their difference at every step.
//
// SubarrayRangesByMonotonicStack splits the answer into "sum of every subarray's
// maximum" minus "sum of every subarray's minimum" and computes each in one O(n)
// sweep, flipping the question from "what is this subarray's extreme" to "how many
// subarrays is this element the extreme of" - the same contribution trick
// SumOfSubarrayMinimumsSolution uses for LC 907 and
// LargestRectangleInHistogramSolution for LC 84, over this repo's own Stack<int>
// holding indices rather than values.
internal static class SumOfSubarrayRangesSolution
{
    // The textbook answer: every subarray's range, computed directly from a running
    // min/max per start index. Deliberately written without this repo's primitives -
    // it is the arm the composed solution below has to justify itself against, and
    // until this migration it existed only as an unasserted benchmark baseline.
    public static long SubarrayRangesByBruteForce(int[] nums)
    {
        var sum = 0L;

        for (var i = 0; i < nums.Length; i++)
        {
            var min = nums[i];
            var max = nums[i];

            for (var j = i; j < nums.Length; j++)
            {
                min = Math.Min(min, nums[j]);
                max = Math.Max(max, nums[j]);
                sum += max - min;
            }
        }

        return sum;
    }

    public static long SubarrayRangesByMonotonicStack(int[] nums) =>
        SumOfSubarrayMaximums(nums) - SumOfSubarrayMinimums(nums);

    // Pop condition is strict (<) so a run of equal maximums only ever gets
    // attributed once, to the leftmost occurrence - the same duplicate-safe shape
    // SumOfSubarrayMinimums mirrors with the opposite strict comparison.
    private static long SumOfSubarrayMaximums(int[] nums) =>
        SumOfSubarrayContribution(nums, int.MaxValue, static (value, current) => value < current);

    private static long SumOfSubarrayMinimums(int[] nums) =>
        SumOfSubarrayContribution(nums, int.MinValue, static (value, current) => value > current);

    // Shared monotonic-stack shape behind SumOfSubarrayMaximums/Minimums: only the
    // "no more elements" sentinel and the pop condition differ between max and min.
    // A popped index's run is bounded on the right by the current index and on the
    // left by whatever index is left below it on the stack, so it is the extreme of
    // exactly (top - left) * (i - top) subarrays.
    private static long SumOfSubarrayContribution(int[] nums, int sentinel, Func<int, int, bool> shouldPop)
    {
        var sum = 0L;
        var indices = new RangeIndexStack();

        for (var i = 0; i <= nums.Length; i++)
        {
            var current = i == nums.Length ? sentinel : nums[i];

            while (indices.TryPeek(out var top) && shouldPop(nums[top], current))
            {
                indices.TryPop(out _);
                var left = indices.TryPeek(out var previous) ? previous : -1;
                sum += (long)nums[top] * (top - left) * (i - top);
            }

            indices.Push(i);
        }

        return sum;
    }
}
