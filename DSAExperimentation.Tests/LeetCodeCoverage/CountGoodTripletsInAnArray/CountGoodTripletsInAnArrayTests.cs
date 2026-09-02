using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountGoodTripletsInAnArray;

// LeetCode 2179. Count Good Triplets in an Array: nums1/nums2 are both permutations
// of [0, n). Re-express nums1's sequence in terms of nums2's positions -
// a[i] = (index of nums1[i] within nums2) - so a good triplet becomes exactly "count
// increasing triples i<j<k with a[i]<a[j]<a[k]," a classic
// FenwickTree<int,SumOperation<int>> (this repo's own Binary Indexed Tree) sweep -
// the same coordinate-free counting shape CountOfSmallerNumbersAfterSelfTests uses,
// run twice: one forward pass counts, for each j, how many earlier positions hold a
// smaller a-value; one backward pass counts how many later positions hold a larger
// one; multiplying and summing those two counts per j gives the total.
public sealed class CountGoodTripletsInAnArrayTests
{
    [Fact]
    public void CountGoodTriplets_LeetCodeExampleOne_ReturnsOne()
    {
        int[] nums1 = [2, 0, 1, 3];
        int[] nums2 = [0, 1, 2, 3];

        var actual = CountGoodTriplets(nums1, nums2);

        Assert.Equal(1L, actual);
    }

    [Fact]
    public void CountGoodTriplets_LeetCodeExampleTwo_ReturnsFour()
    {
        int[] nums1 = [4, 0, 1, 3, 2];
        int[] nums2 = [4, 1, 0, 2, 3];

        var actual = CountGoodTriplets(nums1, nums2);

        Assert.Equal(4L, actual);
    }

    [Fact]
    public void CountGoodTriplets_TooFewElementsForATriplet_ReturnsZero()
    {
        int[] nums1 = [1, 0];
        int[] nums2 = [0, 1];

        var actual = CountGoodTriplets(nums1, nums2);

        Assert.Equal(0L, actual);
    }

    private static long CountGoodTriplets(int[] nums1, int[] nums2)
    {
        var a = BuildRankArray(nums1, nums2);
        var leftSmallerCount = ComputeLeftSmallerCounts(a);
        var rightLargerCount = ComputeRightLargerCounts(a);

        return SumTripletCounts(leftSmallerCount, rightLargerCount);
    }

    private static int[] ComputeLeftSmallerCounts(int[] a)
    {
        var n = a.Length;
        var leftSmallerCount = new int[n];
        var leftTree = new FenwickTree<int, SumOperation<int>>(n);
        for (var i = 0; i < n; i++)
        {
            leftSmallerCount[i] = a[i] == 0 ? 0 : leftTree.PrefixQuery(a[i] - 1);
            leftTree.Add(a[i], 1);
        }

        return leftSmallerCount;
    }

    private static int[] ComputeRightLargerCounts(int[] a)
    {
        var n = a.Length;
        var rightLargerCount = new int[n];
        var rightTree = new FenwickTree<int, SumOperation<int>>(n);
        for (var i = n - 1; i >= 0; i--)
        {
            var smallerToRight = a[i] == 0 ? 0 : rightTree.PrefixQuery(a[i] - 1);
            rightLargerCount[i] = (n - 1 - i) - smallerToRight;
            rightTree.Add(a[i], 1);
        }

        return rightLargerCount;
    }

    private static long SumTripletCounts(int[] leftSmallerCount, int[] rightLargerCount)
    {
        long total = 0;
        for (var j = 0; j < leftSmallerCount.Length; j++)
        {
            total += (long)leftSmallerCount[j] * rightLargerCount[j];
        }

        return total;
    }

    private static int[] BuildRankArray(int[] nums1, int[] nums2)
    {
        var n = nums1.Length;
        var pos2 = new int[n];
        for (var i = 0; i < n; i++)
        {
            pos2[nums2[i]] = i;
        }

        var ranks = new int[n];
        for (var i = 0; i < n; i++)
        {
            ranks[i] = pos2[nums1[i]];
        }

        return ranks;
    }
}
