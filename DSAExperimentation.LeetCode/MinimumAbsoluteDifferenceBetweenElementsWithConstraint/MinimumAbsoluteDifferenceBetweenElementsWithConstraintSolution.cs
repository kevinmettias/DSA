using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.MinimumAbsoluteDifferenceBetweenElementsWithConstraint;

// LeetCode 2817. Minimum Absolute Difference Between Elements With Constraint: the
// smallest |nums[i] - nums[j]| over pairs of indices at least minimumIndexDistance
// apart.
//
// Both strategies answer the same question and differ only in how the earlier,
// already-eligible partners of the current index are searched: a full rescan of
// them, or a nearest-value query against an ordered structure holding exactly
// those partners.
//
// The index constraint is what makes an ordered structure applicable at all. When
// the sweep reaches j, the partners it may pair with are precisely the indices
// 0..j-minimumIndexDistance, and that set grows by exactly one element per step -
// index j - minimumIndexDistance, which only just became eligible - so the structure
// is built incrementally by the same loop that queries it, never rebuilt.
internal static class MinimumAbsoluteDifferenceBetweenElementsWithConstraintSolution
{
    // The textbook answer: for every i, walk every partner at or beyond
    // i + minimumIndexDistance and keep the smallest gap seen. Plain BCL indexing and
    // Math.Abs/Math.Min, nothing from this repo - it is the O(n^2) arm the
    // sliding-window strategy below has to justify itself against, and stating it here
    // is what finally gets it asserted.
    public static int MinAbsoluteDifferenceByBruteForcePairScan(int[] nums, int minimumIndexDistance)
    {
        var minDifference = int.MaxValue;

        for (var i = 0; i < nums.Length; i++)
        {
            for (var j = i + minimumIndexDistance; j < nums.Length; j++)
            {
                minDifference = Math.Min(minDifference, Math.Abs(nums[i] - nums[j]));
            }
        }

        return minDifference;
    }

    // This repo's own BinarySearchTree<int> holding the eligible prefix, queried with
    // FindClosest.TryFind - the same insert-as-you-go plus nearest-value composition
    // ClosestNodesQueriesInABinarySearchTree already proves out, driven here by the
    // index gap rather than by a fixed query list. One insert and one root-to-leaf
    // descent per index, so O(n log n) on a tree that stays balanced, against the
    // baseline's O(n^2) rescan.
    public static int MinAbsoluteDifferenceByBstSlidingWindow(int[] nums, int minimumIndexDistance)
    {
        var eligiblePartners = new BinarySearchTree<int>();
        var minDifference = int.MaxValue;

        for (var j = 0; j < nums.Length; j++)
        {
            // Index j - minimumIndexDistance is the one partner that becomes eligible
            // at step j; every earlier one is already in the tree.
            if (j >= minimumIndexDistance)
            {
                eligiblePartners.Insert(nums[j - minimumIndexDistance]);
            }

            if (FindClosest.TryFind(eligiblePartners.Root, nums[j], out var closest))
            {
                minDifference = Math.Min(minDifference, Math.Abs(closest - nums[j]));
            }
        }

        return minDifference;
    }
}
