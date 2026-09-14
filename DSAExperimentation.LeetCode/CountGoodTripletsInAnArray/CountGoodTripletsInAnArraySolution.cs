using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.LeetCode.CountGoodTripletsInAnArray;

// LeetCode 2179. Count Good Triplets in an Array: nums1 and nums2 are both
// permutations of [0, n), and a triplet of values is good when the three appear in
// the same relative order in both. Re-expressing nums1's sequence in terms of
// nums2's positions - a[i] = the index of nums1[i] within nums2 - turns that into
// the classic "count increasing triples i < j < k with a[i] < a[j] < a[k]".
//
// Both strategies then count the same way: for every middle position j, multiply
// how many earlier positions hold a smaller a-value by how many later positions
// hold a larger one, and sum those products. They differ only in how the two
// per-position counts are obtained - direct inner loops, or two
// FenwickTree<int, SumOperation<int>> sweeps (this repo's own Binary Indexed
// Tree), the same coordinate-free counting shape
// CountOfSmallerNumbersAfterSelfSolution uses for LC 315.
internal static class CountGoodTripletsInAnArraySolution
{
    // The textbook answer: for each middle position, scan every earlier index and
    // every later index directly. Deliberately written with nothing but BCL arrays
    // - it is the O(n^2) arm the Fenwick sweep below has to justify itself
    // against.
    public static long CountGoodTripletsByPairwiseScan(int[] nums1, int[] nums2)
    {
        var ranks = BuildRankArray(nums1, nums2);
        long total = 0;

        for (var j = 0; j < ranks.Length; j++)
        {
            total += CountTripletsCenteredAt(ranks, j);
        }

        return total;
    }

    private static long CountTripletsCenteredAt(int[] ranks, int middleIndex)
    {
        var middleRank = ranks[middleIndex];
        var leftSmaller = 0;

        for (var i = 0; i < middleIndex; i++)
        {
            if (ranks[i] < middleRank)
            {
                leftSmaller++;
            }
        }

        var rightLarger = 0;

        for (var k = middleIndex + 1; k < ranks.Length; k++)
        {
            if (ranks[k] > middleRank)
            {
                rightLarger++;
            }
        }

        return (long)leftSmaller * rightLarger;
    }

    // Two Binary Indexed Tree sweeps over the rank array: one forward pass asks,
    // at each position, how many already-inserted ranks are smaller; one backward
    // pass asks how many already-inserted ranks are larger, obtained as "everything
    // to the right, minus the smaller ones". Both are O(n log n) overall.
    public static long CountGoodTripletsByFenwickTreeSweep(int[] nums1, int[] nums2)
    {
        var ranks = BuildRankArray(nums1, nums2);
        var leftSmallerCount = ComputeLeftSmallerCounts(ranks);
        var rightLargerCount = ComputeRightLargerCounts(ranks);

        return SumTripletCounts(leftSmallerCount, rightLargerCount);
    }

    private static int[] ComputeLeftSmallerCounts(int[] ranks)
    {
        var n = ranks.Length;
        var leftSmallerCount = new int[n];
        var leftTree = new FenwickTree<int, SumOperation<int>>(n);

        for (var i = 0; i < n; i++)
        {
            var hasSmallerRanks = ranks[i] > 0;
            leftSmallerCount[i] = hasSmallerRanks ? leftTree.PrefixQuery(ranks[i] - 1) : 0;
            leftTree.Add(ranks[i], 1);
        }

        return leftSmallerCount;
    }

    private static int[] ComputeRightLargerCounts(int[] ranks)
    {
        var n = ranks.Length;
        var rightLargerCount = new int[n];
        var rightTree = new FenwickTree<int, SumOperation<int>>(n);

        for (var i = n - 1; i >= 0; i--)
        {
            var hasSmallerRanks = ranks[i] > 0;
            var smallerToRight = hasSmallerRanks ? rightTree.PrefixQuery(ranks[i] - 1) : 0;
            rightLargerCount[i] = (n - 1 - i) - smallerToRight;
            rightTree.Add(ranks[i], 1);
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

    // a[i] = where nums1[i] sits inside nums2. Both inputs are permutations of
    // [0, n), so this is a plain inverse-permutation lookup and needs no sorting.
    private static int[] BuildRankArray(int[] nums1, int[] nums2)
    {
        var n = nums1.Length;
        var positionInNums2 = new int[n];

        for (var i = 0; i < n; i++)
        {
            positionInNums2[nums2[i]] = i;
        }

        var ranks = new int[n];

        for (var i = 0; i < n; i++)
        {
            ranks[i] = positionInNums2[nums1[i]];
        }

        return ranks;
    }
}
