using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.LeetCode.CountSubarraysWithFixedBounds;

// LeetCode 2444. Count Subarrays With Fixed Bounds: how many subarrays have minimum
// exactly minK and maximum exactly maxK.
//
// A subarray qualifies if and only if those two equalities hold - min == minK already
// forces every element >= minK and max == maxK already forces every element <= maxK,
// so no separate "every element lies within [minK, maxK]" check is needed on top of
// them. Both strategies therefore enumerate the same (start, end) pairs and differ
// only in how each pair's min and max are answered.
//
// The count is a long: with n up to 1e5 the number of qualifying subarrays can exceed
// int.MaxValue even though neither strategy here is fast enough to reach that size.
internal static class CountSubarraysWithFixedBoundsSolution
{
    // The textbook answer: for each (start, end) pair, rescan the subarray from
    // scratch to recompute its own min and max, paying O(end - start) per pair from
    // zero every time. Deliberately written without this repo's primitives - it is
    // the arm the composed strategy below has to justify itself against.
    public static long CountFixedBoundSubarraysByRescan(int[] nums, int minK, int maxK)
    {
        long count = 0;

        for (var start = 0; start < nums.Length; start++)
        {
            for (var end = start; end < nums.Length; end++)
            {
                var min = int.MaxValue;
                var max = int.MinValue;

                for (var i = start; i <= end; i++)
                {
                    min = Math.Min(min, nums[i]);
                    max = Math.Max(max, nums[i]);
                }

                if (min == minK && max == maxK)
                {
                    count++;
                }
            }
        }

        return count;
    }

    // This repo's own SegmentTree pair - one folded by MinOperation<int>, one by
    // MaxOperation<int> - answers each subarray's min and max in O(log n) after an
    // O(n) build, so the innermost rescan disappears entirely: the same "fold the
    // array once, then query ranges" composition LC 2286 uses over its own
    // MaxOperation tree, here with the Min and Max folds side by side and one query
    // pair per (start, end).
    public static long CountFixedBoundSubarraysBySegmentTreeQueries(int[] nums, int minK, int maxK)
    {
        var minTree = new SegmentTree<int, MinOperation<int>>(nums);
        var maxTree = new SegmentTree<int, MaxOperation<int>>(nums);
        long count = 0;

        for (var start = 0; start < nums.Length; start++)
        {
            for (var end = start; end < nums.Length; end++)
            {
                if (minTree.Query(start, end) == minK && maxTree.Query(start, end) == maxK)
                {
                    count++;
                }
            }
        }

        return count;
    }
}
