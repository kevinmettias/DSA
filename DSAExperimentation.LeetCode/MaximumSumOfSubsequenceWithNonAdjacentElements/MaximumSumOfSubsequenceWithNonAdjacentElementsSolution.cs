using DSAExperimentation.DataStructures.SegmentTree;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.MaximumSumOfSubsequenceWithNonAdjacentElements;

// LeetCode 3165. Maximum Sum of Subsequence With Non-adjacent Elements: apply
// each query as a point update to nums, and after every update sum up LC 198's
// own "max sum, no two adjacent" answer over the whole array, modulo 1e9+7.
// An empty subsequence (sum 0) is always a legal choice, so every per-query
// answer - and therefore the running total - is never negative.
internal static class MaximumSumOfSubsequenceWithNonAdjacentElementsSolution
{
    // Baseline: LC 198's own House Robber recurrence recomputed over the whole
    // array from scratch after every point update - deliberately no repo
    // primitive, O(n) per query, O(n*q) overall.
    public static int MaximumSumByRecomputeDP(int[] nums, int[][] queries)
    {
        var array = (int[])nums.Clone();
        var total = 0L;

        foreach (var query in queries)
        {
            array[query[0]] = query[1];
            total += MaxNonAdjacentSum(array);
        }

        return (int)(total % ModularArithmetic.Modulo);
    }

    private static long MaxNonAdjacentSum(int[] array)
    {
        var excluded = 0L;
        var included = NonAdjacentSumBound.NegativeInfinity;

        foreach (var value in array)
        {
            var nextExcluded = Math.Max(excluded, included);
            var nextIncluded = excluded + value;
            excluded = nextExcluded;
            included = nextIncluded;
        }

        return Math.Max(excluded, included);
    }

    // This repo's own point-update SegmentTree<Element,TOperation>, keyed by
    // NonAdjacentMergeOperation, which folds each range into the four
    // boundary-constrained best sums the House Robber recurrence needs to
    // merge safely across a range split - O(log n) per point update instead
    // of the O(n) baseline rescan, and O(1) to read the answer back since the
    // query below is always the tree's own full range.
    public static int MaximumSumBySegmentTreeMerge(int[] nums, int[][] queries)
    {
        var leaves = new NonAdjacentSumNode[nums.Length];

        for (var i = 0; i < nums.Length; i++)
        {
            leaves[i] = NonAdjacentSumNode.Leaf(nums[i]);
        }

        var tree = new SegmentTree<NonAdjacentSumNode, NonAdjacentMergeOperation>(leaves);
        var total = 0L;

        foreach (var query in queries)
        {
            tree.Update(query[0], NonAdjacentSumNode.Leaf(query[1]));
            total += tree.Query(0, nums.Length - 1).BestSum;
        }

        return (int)(total % ModularArithmetic.Modulo);
    }
}
