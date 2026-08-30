using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.FenwickTree;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountOfSmallerNumbersAfterSelf;

// LeetCode 315. Count of Smaller Numbers After Self: coordinate-compress nums via
// BinarySearch.LowerBound over the sorted distinct values, then sweep right-to-left
// through a FenwickTree<int, SumOperation<int>> (this repo's own Binary Indexed
// Tree) - PrefixQuery(rank-1) counts every smaller value already added on the way
// in, and Add(rank, 1) records the current one before moving further left.
public sealed partial class CountOfSmallerNumbersAfterSelfTests
{
    [Fact]
    public void CountSmaller_ClassicExample_ReturnsCountsToTheRight()
    {
        int[] nums = [5, 2, 6, 1];

        var counts = CountSmaller(nums);

        Assert.Equal([2, 1, 1, 0], counts);
    }

    [Fact]
    public void CountSmaller_AllEqualValues_ReturnsAllZeros()
    {
        int[] nums = [1, 1, 1];

        var counts = CountSmaller(nums);

        Assert.Equal([0, 0, 0], counts);
    }

    [Fact]
    public void CountSmaller_StrictlyDescending_ReturnsDecreasingCounts()
    {
        int[] nums = [3, 2, 1];

        var counts = CountSmaller(nums);

        Assert.Equal([2, 1, 0], counts);
    }

    private static int[] CountSmaller(int[] nums)
    {
        var sortedDistinct = nums.Distinct().OrderBy(value => value).ToArray();
        var sequence = new ArraySequence<int>(sortedDistinct);
        var tree = new FenwickTree<int, SumOperation<int>>(sortedDistinct.Length);
        var counts = new int[nums.Length];

        for (var i = nums.Length - 1; i >= 0; i--)
        {
            var rank = BinarySearch.LowerBound(sequence, nums[i]);
            counts[i] = rank == 0 ? 0 : tree.PrefixQuery(rank - 1);
            tree.Add(rank, 1);
        }

        return counts;
    }
}
