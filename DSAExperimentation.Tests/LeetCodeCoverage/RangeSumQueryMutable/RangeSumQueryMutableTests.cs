using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RangeSumQueryMutable;

// LeetCode 307. Range Sum Query - Mutable: this repo's own SegmentTree<int,SumOperation<int>>
// (the point-update/range-query family ARCHITECTURE.md's SegmentTree worked example describes).
// SegmentTree.Update takes the new value directly, the same shape as LeetCode's update(index,
// val), so - unlike FenwickTree.Add, a pure delta increment - no "track the old value to compute
// a delta" bookkeeping is needed here.
public sealed class RangeSumQueryMutableTests
{
    [Fact]
    public void UpdateSumRange_LeetCodeExample_ReflectsMutation()
    {
        var numArray = new NumArrayOperations([1, 3, 5]);

        Assert.Equal(9, numArray.SumRange(0, 2));

        numArray.Update(1, 2);

        Assert.Equal(8, numArray.SumRange(0, 2));
    }

    [Fact]
    public void Update_RepeatedOnSameIndex_UsesLatestValue()
    {
        var numArray = new NumArrayOperations([0, 0, 0, 0]);

        numArray.Update(2, 10);
        numArray.Update(2, 4);

        Assert.Equal(4, numArray.SumRange(0, 3));
    }

    private sealed class NumArrayOperations
    {
        private readonly SegmentTree<int, SumOperation<int>> _tree;

        public NumArrayOperations(int[] nums) => _tree = new SegmentTree<int, SumOperation<int>>(nums);

        public void Update(int index, int val) => _tree.Update(index, val);

        public int SumRange(int left, int right) => _tree.Query(left, right);
    }
}
