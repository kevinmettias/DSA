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
    // Both pop conditions are stateless, so one instance each serves every call and
    // the benchmark arms that measure these two methods allocate nothing to pick one.
    private static readonly IStackPopCondition MaximumPopCondition = new PopWhenTopIsSmaller();
    private static readonly IStackPopCondition MinimumPopCondition = new PopWhenTopIsLarger();

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

    // Pop condition is strict so a run of equal maximums only ever gets attributed
    // once, to the leftmost occurrence - the same duplicate-safe shape
    // SumOfSubarrayMinimums mirrors with the opposite strict comparison.
    private static long SumOfSubarrayMaximums(int[] nums) =>
        SumOfSubarrayContribution(nums, int.MaxValue, MaximumPopCondition);

    private static long SumOfSubarrayMinimums(int[] nums) =>
        SumOfSubarrayContribution(nums, int.MinValue, MinimumPopCondition);

    // The one question the two sweeps answer differently: whether the element now
    // arriving has just settled the index on top of the stack. The comparison is the
    // whole contract, and the direction it must run in - strict, so a run of equal
    // extremes is attributed to the leftmost of them exactly once - is the thing a bare
    // `Func<int, int, bool>` had nowhere to write down.
    private interface IStackPopCondition
    {
        // `topValue` is the value the stack's top index holds, `current` the value now
        // arriving; nothing later can re-bound that index, so a true settles it for
        // good. The sentinel the sweep runs one step past the array's end compares like
        // any other value, which is what empties the stack on the final iteration.
        bool ShouldPop(int topValue, int current);
    }

    // Shared monotonic-stack shape behind SumOfSubarrayMaximums/Minimums: only the
    // "no more elements" sentinel and the pop condition differ between max and min.
    // A popped index's run is bounded on the right by the current index and on the
    // left by whatever index is left below it on the stack, so it is the extreme of
    // exactly (top - left) * (i - top) subarrays.
    private static long SumOfSubarrayContribution(int[] nums, int sentinel, IStackPopCondition popCondition)
    {
        var sum = 0L;
        var indices = new RangeIndexStack();

        for (var i = 0; i <= nums.Length; i++)
        {
            var current = i == nums.Length ? sentinel : ValueAt(nums, i);

            while (indices.TryPeek(out var top) && popCondition.ShouldPop(nums[top], current))
            {
                indices.TryPop(out _);
                var left = indices.TryPeek(out var previous) ? previous : -1;
                sum += (long)nums[top] * (top - left) * (i - top);
            }

            indices.Push(i);
        }

        return sum;
    }

    // The maximum sweep's condition: the arriving element is strictly larger, so it
    // supersedes the top as the extreme of every subarray reaching past it.
    private sealed class PopWhenTopIsSmaller : IStackPopCondition
    {
        public bool ShouldPop(int topValue, int current) => topValue < current;
    }

    // The minimum sweep's condition, mirrored: the arriving element is strictly
    // smaller, so the top can no longer be what a subarray reaching past it minimizes.
    private sealed class PopWhenTopIsLarger : IStackPopCondition
    {
        public bool ShouldPop(int topValue, int current) => topValue > current;
    }

    // The array's own value at this index, read only where the caller has already
    // established that one exists there.
    private static int ValueAt(int[] nums, int i) => nums[i];
}
