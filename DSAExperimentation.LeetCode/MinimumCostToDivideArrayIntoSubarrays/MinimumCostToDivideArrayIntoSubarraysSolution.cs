using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.MinimumCostToDivideArrayIntoSubarrays;

// LeetCode 3500. Minimum Cost to Divide Array Into Subarrays: partition nums
// (with a parallel cost array) into contiguous subarrays; the i-th subarray
// (1-indexed, spanning nums[l..r]) costs
// (nums[0] + ... + nums[r] + k * i) * (cost[l] + ... + cost[r]).
// Minimize the total cost over every partition.
//
// The k * i term is what makes this harder than a plain partition DP: i is the
// SEGMENT'S OWN INDEX, which depends on how many segments precede it, so it looks
// like state would need to track (position, segment count) as a pair. It doesn't:
// summing k * i * costSum(segment_i) over segments 1..m and swapping the order of
// summation (Σ_i i*x_i = Σ_t Σ_{i>=t} x_i) turns it into Σ_t SuffixCostSum(b_t),
// one term per segment START position b_t, where SuffixCostSum(b_t) is the TRUE
// cost sum from b_t to the array's own end - independent of which segment index
// b_t turns out to be. That makes the k-term additive per segment start, exactly
// like the (nums[0..r]) term is additive per segment end, so state collapses back
// to plain position: MinCost(index) = min over end >= index of
// PrefixNumsSum[end] * SegmentCostSum(index, end) + k * SuffixCostSum[index] +
// MinCost(end + 1). Both strategies share this one recurrence (PartitionStep) and
// differ only in how repeated states get cached - the same "Dictionary memo vs.
// this repo's Memoizer" contrast MinimumSumOfValuesByDividingArraySolution draws.
internal static class MinimumCostToDivideArrayIntoSubarraysSolution
{
    // The textbook form: recursion over a hand-rolled Dictionary memo table, its
    // lookup/store written out at the call site. The arm the repo's own
    // Memoizer-based strategy below has to beat.
    public static long MinimumCostByDictionaryMemo(int[] nums, int[] cost, int k)
    {
        var run = new DictionaryMemoRun(BuildContext(nums, cost, k));

        return run.Replay(0, run);
    }

    // Same recurrence, routed through this repo's own Memoizer so each distinct
    // index is solved once without hand-writing the cache lookup/store around it.
    public static long MinimumCostByMemoizedPartition(int[] nums, int[] cost, int k) =>
        Memoizer.Memoize(0, new CostFromSegmentStart(BuildContext(nums, cost, k)));

    // index == nums.Length means every element has been assigned to a segment -
    // 0 more cost to add. Otherwise, try every possible end for the segment
    // STARTING at index and let solveRest handle everything after it.
    private static long PartitionStep(int index, PartitionContext context, IRecurrence<int, long> solveRest)
    {
        var n = context.Nums.Length;

        if (index == n)
        {
            return 0L;
        }

        var segmentStartTerm = context.K * context.CostSuffixSum[index];
        var priorCostSum = index == 0 ? 0L : CostBefore(context, index);
        var best = long.MaxValue;

        for (var end = index; end < n; end++)
        {
            var candidate = SegmentCostTo(context, end, priorCostSum, segmentStartTerm)
                + solveRest.Replay(end + 1, solveRest);
            best = Math.Min(best, candidate);
        }

        return best;
    }

    private static long CostBefore(PartitionContext context, int index) => context.CostPrefixSum[index - 1];

    // One candidate segment [index..end]: its own (nums, cost) cost, before the cost of
    // everything after it is added on.
    private static long SegmentCostTo(
        PartitionContext context, int end, long priorCostSum, long segmentStartTerm)
    {
        var segmentCostSum = context.CostPrefixSum[end] - priorCostSum;

        return context.NumsPrefixSum[end] * segmentCostSum + segmentStartTerm;
    }

    private static PartitionContext BuildContext(int[] nums, int[] cost, int k) =>
        new(nums, k, PrefixSum(nums), PrefixSum(cost), SuffixSum(cost));

    private readonly record struct PartitionContext(
        int[] Nums, int K, long[] NumsPrefixSum, long[] CostPrefixSum, long[] CostSuffixSum);

    // The next segment's start index is the whole state: everything the step needs about
    // the rest of the array is already in the context the two strategies share.
    private sealed class CostFromSegmentStart(PartitionContext context) : IRecurrence<int, long>
    {
        public long Replay(int index, IRecurrence<int, long> rest) => PartitionStep(index, context, rest);
    }

    // The hand-rolled memo run: it keeps the table and hands itself to the recurrence as
    // the recursion, exactly as Memoizer's own run does - the lookup and the store are
    // just written out here instead of being supplied by the library.
    private sealed class DictionaryMemoRun(PartitionContext context) : IRecurrence<int, long>
    {
        private readonly CostFromSegmentStart step = new(context);
        private readonly Dictionary<int, long> memo = [];

        public long Replay(int index, IRecurrence<int, long> rest)
        {
            if (memo.TryGetValue(index, out var cached))
            {
                return cached;
            }

            var result = step.Replay(index, this);
            memo[index] = result;
            return result;
        }
    }

    private static long[] PrefixSum(int[] values)
    {
        var prefix = new long[values.Length];
        var running = 0L;

        for (var i = 0; i < values.Length; i++)
        {
            running += values[i];
            prefix[i] = running;
        }

        return prefix;
    }

    private static long[] SuffixSum(int[] values)
    {
        var suffix = new long[values.Length];
        var running = 0L;

        for (var i = values.Length - 1; i >= 0; i--)
        {
            running += values[i];
            suffix[i] = running;
        }

        return suffix;
    }
}
