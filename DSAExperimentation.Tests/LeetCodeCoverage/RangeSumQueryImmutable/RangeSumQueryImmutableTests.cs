using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RangeSumQueryImmutable;

// LeetCode 303. Range Sum Query - Immutable: this repo's own FenwickTree<int,SumOperation<int>>
// (the point-update/range-query family ARCHITECTURE.md's FenwickTree worked example describes),
// built once from the input array and never mutated again - SumRange is then an O(log n)
// FenwickTree.Query instead of a fresh O(n) rescan per call.
public sealed class RangeSumQueryImmutableTests
{
    [Fact]
    public void SumRange_LeetCodeExample_ReturnsExpectedSums()
    {
        var numArray = new NumArrayOperations([-2, 0, 3, -5, 2, -1]);

        Assert.Equal(1, numArray.SumRange(0, 2));
        Assert.Equal(-1, numArray.SumRange(2, 5));
        Assert.Equal(-3, numArray.SumRange(0, 5));
    }

    [Fact]
    public void SumRange_SingleIndex_ReturnsThatElement()
    {
        var numArray = new NumArrayOperations([7, -3, 4]);

        Assert.Equal(-3, numArray.SumRange(1, 1));
    }

    private sealed class NumArrayOperations
    {
        private readonly FenwickTree<int, SumOperation<int>> _tree;

        public NumArrayOperations(int[] nums) => _tree = new FenwickTree<int, SumOperation<int>>(nums);

        public int SumRange(int left, int right) => _tree.Query(left, right);
    }
}
