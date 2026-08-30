using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.FenwickTree;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountOfRangeSum;

// LeetCode 327. Count of Range Sum: build the prefix-sum array, coordinate-compress
// it via BinarySearch.LowerBound/UpperBound over its sorted distinct values, then
// sweep left-to-right through a FenwickTree<int, SumOperation<int>> (this repo's own
// Binary Indexed Tree) counting, for each prefix[j], how many earlier prefix[i]
// already inserted fall in [prefix[j]-upper, prefix[j]-lower] before inserting
// prefix[j] itself. Same coordinate-compression-plus-Fenwick-sweep shape
// CountOfSmallerNumbersAfterSelfTests already uses for LC 315, generalized from a
// single one-sided PrefixQuery to a two-sided Query range.
public sealed partial class CountOfRangeSumTests
{
    [Fact]
    public void CountRangeSum_ClassicExample_ReturnsThree()
    {
        int[] nums = [-2, 5, -1];

        var count = CountRangeSum(nums, lower: -2, upper: 2);

        Assert.Equal(3, count);
    }

    [Fact]
    public void CountRangeSum_SingleZeroInRange_ReturnsOne()
    {
        int[] nums = [0];

        var count = CountRangeSum(nums, lower: 0, upper: 0);

        Assert.Equal(1, count);
    }

    [Fact]
    public void CountRangeSum_NoRangeInBounds_ReturnsZero()
    {
        int[] nums = [1, 2, 3];

        var count = CountRangeSum(nums, lower: 100, upper: 200);

        Assert.Equal(0, count);
    }

    [Fact]
    public void CountRangeSum_LargeMagnitudeValues_DoesNotOverflow32Bits()
    {
        // Prefix sums here reach 2 * int.MaxValue, which overflows a 32-bit
        // accumulator - proves the algorithm's long[] prefix array is load-bearing,
        // not incidental.
        int[] nums = [int.MaxValue, int.MaxValue, -1, -1];

        var count = CountRangeSum(nums, lower: 0, upper: int.MaxValue);

        Assert.Equal(4, count);
    }

    private static int CountRangeSum(int[] nums, int lower, int upper)
    {
        var prefix = new long[nums.Length + 1];

        for (var i = 0; i < nums.Length; i++)
        {
            prefix[i + 1] = prefix[i] + nums[i];
        }

        var sortedDistinct = prefix.Distinct().OrderBy(value => value).ToArray();
        var sequence = new ArraySequence<long>(sortedDistinct);
        var tree = new FenwickTree<int, SumOperation<int>>(sortedDistinct.Length);
        var count = 0;

        foreach (var prefixSum in prefix)
        {
            var loRank = BinarySearch.LowerBound(sequence, prefixSum - upper);
            var hiRank = BinarySearch.UpperBound(sequence, prefixSum - lower) - 1;

            if (loRank <= hiRank)
            {
                count += tree.Query(loRank, hiRank);
            }

            tree.Add(BinarySearch.LowerBound(sequence, prefixSum), 1);
        }

        return count;
    }
}
