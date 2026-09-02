using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.FenwickTree;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfPairsSatisfyingInequality;

// LeetCode 2426. Number of Pairs Satisfying Inequality: nums1[i]-nums1[j] <=
// nums2[i]-nums2[j]+diff rearranges to diff[i] <= diff[j]+diff where diff[i] =
// nums1[i]-nums2[i]. That is exactly CountOfSmallerNumbersAfterSelfTests's
// coordinate-compress-and-sweep shape, generalized from a strict "smaller" count to
// a "<=" count with a per-query offset: BinarySearch.UpperBound over the sorted
// distinct diff values finds how many distinct values are <= diff[j]+diff, and
// FenwickTree<int,SumOperation<int>>.PrefixQuery sums how many of the diff[i]'s
// already swept in (i < j) sit at or below that rank - an O(n log n) left-to-right
// sweep instead of the O(n^2) pairwise scan.
public sealed partial class NumberOfPairsSatisfyingInequalityTests
{
    [Fact]
    public void CountPairs_ClassicExampleOne_ReturnsThree()
    {
        int[] nums1 = [3, 2, 5];
        int[] nums2 = [2, 2, 1];

        var count = CountPairs(nums1, nums2, diff: 1);

        Assert.Equal(3, count);
    }

    [Fact]
    public void CountPairs_ClassicExampleTwo_ReturnsZero()
    {
        int[] nums1 = [3, -1];
        int[] nums2 = [-2, 2];

        var count = CountPairs(nums1, nums2, diff: -1);

        Assert.Equal(0, count);
    }

    [Fact]
    public void CountPairs_AllElementsEqualWithZeroDiff_CountsEveryPair()
    {
        int[] nums1 = [1, 1, 1];
        int[] nums2 = [1, 1, 1];

        var count = CountPairs(nums1, nums2, diff: 0);

        Assert.Equal(3, count);
    }

    private static long CountPairs(int[] nums1, int[] nums2, int diff)
    {
        var n = nums1.Length;
        var differences = new int[n];
        for (var i = 0; i < n; i++)
        {
            differences[i] = nums1[i] - nums2[i];
        }

        var sortedDistinct = differences.Distinct().OrderBy(value => value).ToArray();
        var sequence = new ArraySequence<int>(sortedDistinct);
        var tree = new FenwickTree<int, SumOperation<int>>(sortedDistinct.Length);

        var count = 0L;

        foreach (var value in differences)
        {
            var upperRank = BinarySearch.UpperBound(sequence, value + diff);
            count += upperRank == 0 ? 0 : tree.PrefixQuery(upperRank - 1);

            var ownRank = BinarySearch.LowerBound(sequence, value);
            tree.Add(ownRank, 1);
        }

        return count;
    }
}
