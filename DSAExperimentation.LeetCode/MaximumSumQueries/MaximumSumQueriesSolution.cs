using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

using RepoSegmentTree = DSAExperimentation.DataStructures.SegmentTree.SegmentTree<long, DSAExperimentation.DataStructures.SegmentTree.MaxOperation<long>>;

namespace DSAExperimentation.LeetCode.MaximumSumQueries;

// LeetCode 2736. Maximum Sum Queries: each query (x, y) asks for the largest
// nums1[j] + nums2[j] over the indices j with nums1[j] >= x and nums2[j] >= y, or -1
// when no index qualifies.
//
// Both strategies answer the same question with the same signature, so the test
// harness can pin them to the same examples and the benchmark can time them against
// each other without either restating the algorithm.
internal static class MaximumSumQueriesSolution
{
    // Textbook O(n*q): answer each query independently by rescanning every index for
    // the two thresholds. Deliberately written with nothing but the input arrays - it
    // is what you would write without this repo, and it is the arm the sweep below has
    // to justify itself against.
    public static int[] MaxSumsByBruteForceScan(int[] nums1, int[] nums2, int[][] queries)
    {
        var results = new int[queries.Length];

        for (var q = 0; q < queries.Length; q++)
        {
            results[q] = BestSumForQuery(nums1, nums2, queries[q][0], queries[q][1]);
        }

        return results;
    }

    private static int BestSumForQuery(int[] nums1, int[] nums2, int x, int y)
    {
        var best = LeetCodeAnswer.None;

        for (var j = 0; j < nums1.Length; j++)
        {
            if (nums1[j] >= x && nums2[j] >= y)
            {
                best = Math.Max(best, nums1[j] + nums2[j]);
            }
        }

        return best;
    }

    // Composed: coordinate-compress nums2's distinct values (the same
    // BinarySearch.LowerBound-over-a-sorted-array idiom BookingConcertTicketsInGroups
    // and MaximumBalancedSubsequenceSum use to turn a threshold into a rank), then sweep
    // indices and queries together in decreasing nums1/x order with this repo's own
    // MergeSort. Every index whose nums1 has just become >= the current query's x is
    // folded into a SegmentTree<long, MaxOperation<long>> keyed by its nums2's compressed
    // rank (point-update = the best sum seen at that rank so far); a query then answers
    // "max sum among nums2 >= y" with one range-max query over the compressed-rank suffix
    // [LowerBound(y), lastRank]. Unset ranks carry MaxOperation's own identity
    // (long.MinValue) as the "no candidate" sentinel, which is what makes -1 fall out with
    // no extra bookkeeping. O((n + q) log n) in total, against the baseline's O(n*q).
    public static int[] MaxSumsBySweepWithSegmentTree(int[] nums1, int[] nums2, int[][] queries)
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

    private static RepoSegmentTree BuildEmptyMaxTree(int rankCount)
    {
        var initial = new long[rankCount];
        Array.Fill(initial, long.MinValue);

        return new RepoSegmentTree(initial);
    }

    private static (int Nums1, int Rank, long Sum)[] BuildPairsDescendingByNums1(
        int[] nums1, int[] nums2, int[] distinctNums2)
    {
        var pairs = BuildRankedPairs(nums1, nums2, distinctNums2);
        SortPairsByNums1Descending(pairs);

        return pairs;
    }

    // Each index as the (nums1, compressed nums2 rank, nums1 + nums2) triple the sweep
    // admits, before the descending-nums1 order that pass needs.
    private static (int Nums1, int Rank, long Sum)[] BuildRankedPairs(
        int[] nums1, int[] nums2, int[] distinctNums2)
    {
        var pairs = new (int Nums1, int Rank, long Sum)[nums1.Length];
        var ranks = new ArraySequence<int>(distinctNums2);

        for (var j = 0; j < nums1.Length; j++)
        {
            var rank = BinarySearch.LowerBound(ranks, nums2[j]);
            pairs[j] = (nums1[j], rank, (long)nums1[j] + nums2[j]);
        }

        return pairs;
    }

    private static void SortPairsByNums1Descending((int Nums1, int Rank, long Sum)[] pairs)
    {
        var byNums1Descending = Comparer<(int Nums1, int Rank, long Sum)>.Create((a, b) => b.Nums1.CompareTo(a.Nums1));
        var sequence = new ArrayIndexedSequence<(int Nums1, int Rank, long Sum)>(pairs);

        MergeSort.Sort<(int Nums1, int Rank, long Sum), ArrayIndexedSequence<(int Nums1, int Rank, long Sum)>>(
            sequence, byNums1Descending);
    }

    private static (int X, int Y, int OriginalIndex)[] BuildQueriesDescendingByX(int[][] queries)
    {
        var sorted = BuildIndexedQueries(queries);
        SortQueriesByXDescending(sorted);

        return sorted;
    }

    // Each query as the (x, y, original position) triple, before the descending-x order
    // the sweep visits them in.
    private static (int X, int Y, int OriginalIndex)[] BuildIndexedQueries(int[][] queries)
    {
        var sorted = new (int X, int Y, int OriginalIndex)[queries.Length];

        for (var i = 0; i < queries.Length; i++)
        {
            sorted[i] = (queries[i][0], queries[i][1], i);
        }

        return sorted;
    }

    private static void SortQueriesByXDescending((int X, int Y, int OriginalIndex)[] sorted)
    {
        var byXDescending = Comparer<(int X, int Y, int OriginalIndex)>.Create((a, b) => b.X.CompareTo(a.X));
        var sequence = new ArrayIndexedSequence<(int X, int Y, int OriginalIndex)>(sorted);

        MergeSort.Sort<(int X, int Y, int OriginalIndex), ArrayIndexedSequence<(int X, int Y, int OriginalIndex)>>(
            sequence, byXDescending);
    }

    private static int AdmitPairsUpTo(
        (int Nums1, int Rank, long Sum)[] pairs, int pairIndex, int x, RepoSegmentTree tree)
    {
        while (pairIndex < pairs.Length && pairs[pairIndex].Nums1 >= x)
        {
            var (_, rank, sum) = pairs[pairIndex];
            var current = tree.Query(rank, rank);
            var updated = Math.Max(current, sum);
            tree.Update(rank, updated);
            pairIndex++;
        }

        return pairIndex;
    }

    private static int BestSumAtLeast(RepoSegmentTree tree, int[] distinctNums2, int y)
    {
        var lowerRank = BinarySearch.LowerBound(new ArraySequence<int>(distinctNums2), y);

        if (lowerRank >= distinctNums2.Length)
        {
            return LeetCodeAnswer.None;
        }

        var best = tree.Query(lowerRank, distinctNums2.Length - 1);

        return best == long.MinValue ? LeetCodeAnswer.None : (int)best;
    }
}
