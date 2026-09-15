using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.LeetCode.MinimumOperationsToEqualizeSubarrays;

// LeetCode 3762. Minimum Operations to Equalize Subarrays: one operation
// raises or lowers a single element by exactly k. For each query [l, r],
// report the fewest operations to make nums[l..r] all equal, or -1 when it
// cannot be done.
//
// An element can only ever reach values congruent to itself mod k, so
// nums[l..r] is equalizable at all only when every element in it shares one
// remainder mod k - equivalently, l and r must fall in the same maximal run
// of equal-remainder positions. Given that, the target minimizing
// sum(|x_i - target|) / k is any median of the range (itself one of the
// range's own elements, so it automatically sits on the shared remainder) -
// the classic "sum of absolute deviations is minimized at the median" fact.
// Answering that per query is a range order-statistic query, composed here
// as a merge-sort tree: DataStructures.SegmentTree.SegmentTree generic over
// SortedMergeOperation (this folder's own ICombineOperation<int[]> witness),
// so Query(l, r) returns the queried range already sorted and the median /
// deviation sum falls out directly.
internal static class MinimumOperationsToEqualizeSubarraysSolution
{
    // The textbook answer: for each query, rescan the range for a shared
    // remainder, then BCL Array.Sort the copied window directly - no repo
    // primitive, O(q * range log range). Deliberately written this way; it is
    // the arm the composed solution below has to justify itself against.
    public static long[] MinOperationsByBruteForce(int[] nums, int[][] queries, int k)
    {
        var answers = new long[queries.Length];

        for (var q = 0; q < queries.Length; q++)
        {
            answers[q] = SolveRangeByBruteForce(nums, queries[q][0], queries[q][1], k);
        }

        return answers;
    }

    private static long SolveRangeByBruteForce(int[] nums, int left, int right, int k)
    {
        var remainder = Modulo(nums[left], k);

        for (var i = left + 1; i <= right; i++)
        {
            if (Modulo(nums[i], k) != remainder)
            {
                return LeetCodeAnswer.None;
            }
        }

        var window = new int[right - left + 1];
        Array.Copy(nums, left, window, 0, window.Length);
        Array.Sort(window);

        return SumOfAbsoluteDeviationsFromMedian(window) / k;
    }

    // Builds the merge-sort tree and the run-id index once, then answers
    // every query against them - the same "hoist construction, keep the
    // search measured" split OpenTheLockSolution.MinTurnsByReduceGraph uses
    // for LockGraph.Build.
    public static long[] MinOperationsByMergeSortTree(int[] nums, int[][] queries, int k)
    {
        var (tree, runId) = BuildIndex(nums, k);

        return MinOperationsByMergeSortTree(tree, runId, queries, k);
    }

    public static long[] MinOperationsByMergeSortTree(
        SegmentTree<int[], SortedMergeOperation> tree, int[] runId, int[][] queries, int k)
    {
        var answers = new long[queries.Length];

        for (var q = 0; q < queries.Length; q++)
        {
            var left = queries[q][0];
            var right = queries[q][1];

            var isSameRun = runId[left] == runId[right];

            answers[q] = isSameRun
                ? EqualizationCost(tree, left, right, k)
                : LeetCodeAnswer.None;
        }

        return answers;
    }

    // The run's answer: the range's sum of absolute deviations from its median, one
    // operation per k of it, read straight off the tree's sorted query result. Only an
    // all-one-run range has one - the other arm is the -1.
    private static long EqualizationCost(
        SegmentTree<int[], SortedMergeOperation> tree, int left, int right, int k)
    {
        var sortedRange = tree.Query(left, right);

        return SumOfAbsoluteDeviationsFromMedian(sortedRange) / k;
    }

    // The prepared input MinOperationsByMergeSortTree's hoisted overload
    // takes: one merge-sort tree over the whole array plus the run-id every
    // query's [l, r] gets checked against, built once regardless of how many
    // queries follow.
    public static EqualizeIndex BuildIndex(int[] nums, int k)
    {
        var runId = BuildRunIds(nums, k);
        var leaves = new int[nums.Length][];

        for (var i = 0; i < nums.Length; i++)
        {
            leaves[i] = [nums[i]];
        }

        return new(new SegmentTree<int[], SortedMergeOperation>(leaves), runId);
    }

    // runId[i] increments every time the remainder mod k changes from the
    // previous position, so two indices share a run iff every position
    // between them (inclusive) shares one remainder.
    private static int[] BuildRunIds(int[] nums, int k)
    {
        var runId = new int[nums.Length];

        for (var i = 1; i < nums.Length; i++)
        {
            var sameRemainder = Modulo(nums[i], k) == Modulo(nums[i - 1], k);
            runId[i] = sameRemainder ? RunIdAt(runId, i - 1) : IncrementedRunIdAt(runId, i - 1);
        }

        return runId;
    }

    private static int Modulo(int value, int k) => ((value % k) + k) % k;

    private static long SumOfAbsoluteDeviationsFromMedian(int[] sorted)
    {
        var median = sorted[sorted.Length / 2];
        var sum = 0L;

        foreach (var value in sorted)
        {
            sum += Math.Abs((long)value - median);
        }

        return sum;
    }

    // The previous position's run id, and that same id one run later.
    private static int RunIdAt(int[] runId, int index) => runId[index];

    private static int IncrementedRunIdAt(int[] runId, int index) => runId[index] + 1;
}
