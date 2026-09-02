using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.SegmentTree;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumSumQueries;

// LeetCode 2736. Maximum Sum Queries: coordinate-compress nums2's distinct values (the
// same BinarySearch.LowerBound-over-a-sorted-array idiom BookingConcertTicketsInGroupsTests
// uses to turn a threshold into an index), then sweep indices and queries together in
// decreasing nums1/x order with this repo's own MergeSort. Every index whose nums1 has
// just become >= the current query's x is folded into a
// SegmentTree<long,MaxOperation<long>> keyed by its nums2's compressed rank
// (point-update = the max sum seen at that rank so far); a query then answers "max sum
// among nums2 >= y" with one range-max query over the compressed-rank suffix
// [LowerBound(y), lastRank]. Unset ranks carry MaxOperation's own Identity
// (long.MinValue) as the "no candidate" sentinel, which is what makes -1 fall out with
// no extra bookkeeping.
public sealed partial class MaximumSumQueriesTests
{
    [Fact]
    public void MaximumSumQueries_LeetCodeExampleOne_ReturnsExpectedAnswers()
    {
        int[] nums1 = [4, 3, 1, 2];
        int[] nums2 = [2, 4, 9, 5];
        int[][] queries = [[4, 1], [1, 3], [2, 5]];

        Assert.Equal([6, 10, 7], MaximumSumQueries(nums1, nums2, queries));
    }

    [Fact]
    public void MaximumSumQueries_ThresholdBeyondEveryPair_ReturnsNegativeOne()
    {
        int[] nums1 = [1, 1];
        int[] nums2 = [1, 1];
        int[][] queries = [[5, 5]];

        Assert.Equal([-1], MaximumSumQueries(nums1, nums2, queries));
    }

    [Fact]
    public void MaximumSumQueries_ExactBoundaryVersusJustPastIt_OnlyExactMatches()
    {
        int[] nums1 = [5];
        int[] nums2 = [5];
        int[][] queries = [[5, 5], [6, 5], [5, 6]];

        Assert.Equal([10, -1, -1], MaximumSumQueries(nums1, nums2, queries));
    }

    private static int[] MaximumSumQueries(int[] nums1, int[] nums2, int[][] queries)
    {
        var distinctNums2 = SortedDistinct(nums2);
        var tree = BuildEmptyMaxTree(distinctNums2.Length);
        var pairs = BuildPairsDescendingByNums1(nums1, nums2, distinctNums2);
        var sortedQueries = BuildQueriesDescendingByX(queries);

        var results = new int[queries.Length];
        var pairIndex = 0;

        foreach (var query in sortedQueries)
        {
            pairIndex = AdmitPairsUpTo(pairs, pairIndex, query.X, tree);
            results[query.OriginalIndex] = BestSumAtLeast(tree, distinctNums2, query.Y);
        }

        return results;
    }

    private static int[] SortedDistinct(int[] nums2)
    {
        var distinct = nums2.Distinct().ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(distinct));
        return distinct;
    }

    private static SegmentTree<long, MaxOperation<long>> BuildEmptyMaxTree(int rankCount)
    {
        var initial = Enumerable.Repeat(long.MinValue, rankCount).ToArray();
        return new SegmentTree<long, MaxOperation<long>>(initial);
    }

    private static (int Nums1, int Rank, long Sum)[] BuildPairsDescendingByNums1(int[] nums1, int[] nums2, int[] distinctNums2)
    {
        var pairs = new (int Nums1, int Rank, long Sum)[nums1.Length];
        var ranks = new ArraySequence<int>(distinctNums2);

        for (var j = 0; j < nums1.Length; j++)
        {
            var rank = BinarySearch.LowerBound(ranks, nums2[j]);
            pairs[j] = (nums1[j], rank, (long)nums1[j] + nums2[j]);
        }

        var byNums1Descending = Comparer<(int Nums1, int Rank, long Sum)>.Create((a, b) => b.Nums1.CompareTo(a.Nums1));
        MergeSort.Sort<(int Nums1, int Rank, long Sum), ArrayIndexedSequence<(int Nums1, int Rank, long Sum)>>(
            new ArrayIndexedSequence<(int Nums1, int Rank, long Sum)>(pairs), byNums1Descending);

        return pairs;
    }

    private static (int X, int Y, int OriginalIndex)[] BuildQueriesDescendingByX(int[][] queries)
    {
        var sorted = new (int X, int Y, int OriginalIndex)[queries.Length];

        for (var i = 0; i < queries.Length; i++)
        {
            sorted[i] = (queries[i][0], queries[i][1], i);
        }

        var byXDescending = Comparer<(int X, int Y, int OriginalIndex)>.Create((a, b) => b.X.CompareTo(a.X));
        MergeSort.Sort<(int X, int Y, int OriginalIndex), ArrayIndexedSequence<(int X, int Y, int OriginalIndex)>>(
            new ArrayIndexedSequence<(int X, int Y, int OriginalIndex)>(sorted), byXDescending);

        return sorted;
    }

    private static int AdmitPairsUpTo(
        (int Nums1, int Rank, long Sum)[] pairs, int pairIndex, int x, SegmentTree<long, MaxOperation<long>> tree)
    {
        while (pairIndex < pairs.Length && pairs[pairIndex].Nums1 >= x)
        {
            var (_, rank, sum) = pairs[pairIndex];
            tree.Update(rank, Math.Max(tree.Query(rank, rank), sum));
            pairIndex++;
        }

        return pairIndex;
    }

    private static int BestSumAtLeast(SegmentTree<long, MaxOperation<long>> tree, int[] distinctNums2, int y)
    {
        var lowerRank = BinarySearch.LowerBound(new ArraySequence<int>(distinctNums2), y);

        if (lowerRank >= distinctNums2.Length)
        {
            return -1;
        }

        var best = tree.Query(lowerRank, distinctNums2.Length - 1);
        return best == long.MinValue ? -1 : (int)best;
    }
}
