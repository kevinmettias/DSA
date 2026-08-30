using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.FenwickTree;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ReversePairs;

// LeetCode 493. Reverse Pairs: coordinate-compress nums (as long, since 2*value can
// overflow a 32-bit int) via BinarySearch.LowerBound/UpperBound over the sorted distinct
// values, then sweep left-to-right through a FenwickTree<int, SumOperation<int>> (this
// repo's own Binary Indexed Tree) - before inserting nums[j]'s own rank, UpperBound
// locates the first rank strictly greater than 2*nums[j], and a single Query over
// [rank, end] counts every earlier nums[i] that forms a reverse pair with it. Same
// coordinate-compression-plus-Fenwick-sweep shape CountOfSmallerNumbersAfterSelfTests/
// CountOfRangeSumTests already use, generalized to a value-dependent (not fixed) split
// point.
public sealed partial class ReversePairsTests
{
    [Fact]
    public void ReversePairs_ClassicExampleOne_ReturnsTwo()
    {
        int[] nums = [1, 3, 2, 3, 1];

        var count = CountReversePairs(nums);

        Assert.Equal(2, count);
    }

    [Fact]
    public void ReversePairs_ClassicExampleTwo_ReturnsThree()
    {
        int[] nums = [2, 4, 3, 5, 1];

        var count = CountReversePairs(nums);

        Assert.Equal(3, count);
    }

    [Fact]
    public void ReversePairs_NoReversePairs_ReturnsZero()
    {
        int[] nums = [1, 2, 3, 4];

        var count = CountReversePairs(nums);

        Assert.Equal(0, count);
    }

    [Fact]
    public void ReversePairs_ExtremeValues_DoesNotOverflow32Bits()
    {
        // 2 * int.MinValue overflows a 32-bit accumulator - a wrapped-to-zero
        // threshold would silently miss this pair, proving the algorithm's long
        // threshold is load-bearing, not incidental.
        int[] nums = [int.MinValue, int.MinValue];

        var count = CountReversePairs(nums);

        Assert.Equal(1, count);
    }

    private static int CountReversePairs(int[] nums)
    {
        var sortedDistinct = nums.Select(value => (long)value).Distinct().OrderBy(value => value).ToArray();
        var sequence = new ArraySequence<long>(sortedDistinct);
        var tree = new FenwickTree<int, SumOperation<int>>(sortedDistinct.Length);
        var count = 0;

        foreach (var value in nums)
        {
            var firstGreaterRank = BinarySearch.UpperBound(sequence, 2L * value);

            if (firstGreaterRank < sortedDistinct.Length)
            {
                count += tree.Query(firstGreaterRank, sortedDistinct.Length - 1);
            }

            tree.Add(BinarySearch.LowerBound(sequence, (long)value), 1);
        }

        return count;
    }
}
