using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.LeetCode.RangeSumQueryMutable;

// LeetCode 307. Range Sum Query - Mutable: given a mutable array, support Update(index, val) and
// SumRange(left, right) efficiently.
//
// This is a design problem - LeetCode's own shape is a stateful object (a constructor plus Update
// and SumRange operations), not a single return value - the same shape RangeSumQuery2DImmutableSolution
// uses for its own design problem. The composed strategy needs no new data structure: this repo's
// own SegmentTree<int, SumOperation<int>>. SegmentTree.Update takes the new value directly, the same
// shape as LeetCode's update(index, val), so - unlike FenwickTree.Add, a pure delta increment - no
// "track the old value to compute a delta" bookkeeping is needed here.
internal static class RangeSumQueryMutableSolution
{
    // The textbook baseline this composition has to justify itself against: a raw array, O(1)
    // writes on Update, an O(n) rescan on every SumRange.
    public static INumArray CreateByArrayRescan(int[] nums) => new ArrayRescanNumArray(nums);

    // The composed answer: one SegmentTree built once (O(n)), so every Update/SumRange afterward is
    // O(log n) instead of an O(n) rescan.
    public static INumArray CreateBySegmentTreeQuery(int[] nums) => new SegmentTreeNumArray(nums);

    private sealed class ArrayRescanNumArray : INumArray
    {
        private readonly int[] _nums;

        public ArrayRescanNumArray(int[] nums) => _nums = (int[])nums.Clone();

        public void Update(int index, int val) => _nums[index] = val;

        public int SumRange(int left, int right)
        {
            var total = 0;

            for (var i = left; i <= right; i++)
            {
                total += _nums[i];
            }

            return total;
        }
    }

    private sealed class SegmentTreeNumArray : INumArray
    {
        private readonly SegmentTree<int, SumOperation<int>> _tree;

        public SegmentTreeNumArray(int[] nums) => _tree = new SegmentTree<int, SumOperation<int>>(nums);

        public void Update(int index, int val) => _tree.Update(index, val);

        public int SumRange(int left, int right) => _tree.Query(left, right);
    }
}
