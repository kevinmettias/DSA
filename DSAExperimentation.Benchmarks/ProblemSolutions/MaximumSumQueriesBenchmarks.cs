using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.SegmentTree;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximum Sum Queries (LC 2736): BruteForceScan answers each query independently by
// rescanning every index for nums1[j] >= x && nums2[j] >= y - O(n*q). SweepWithSegmentTree
// is MaximumSumQueriesTests' approach: MergeSort both indices and queries by descending
// nums1/x, admit indices into a SegmentTree<long,MaxOperation<long>> keyed by nums2's
// coordinate-compressed rank as their nums1 threshold is met, and answer each query
// with one range-max query over BinarySearch.LowerBound(y)..lastRank -
// O((n + q) log n) total.
[MemoryDiagnoser]
public class MaximumSumQueriesBenchmarks
{
    private const int RandomSeed = 2736; // LC problem number
    private const int ValueRange = 1_000_000;

    [Params(200, 2_000)]
    public int Length;

    [Params(200, 2_000)]
    public int QueryCount;

    private int[] _nums1 = null!;
    private int[] _nums2 = null!;
    private int[][] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums1 = Enumerable.Range(0, Length).Select(_ => random.Next(1, ValueRange)).ToArray();
        _nums2 = Enumerable.Range(0, Length).Select(_ => random.Next(1, ValueRange)).ToArray();
        _queries = Enumerable.Range(0, QueryCount)
            .Select(_ => new[] { random.Next(1, ValueRange), random.Next(1, ValueRange) })
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForceScan()
    {
        var results = new int[_queries.Length];

        for (var q = 0; q < _queries.Length; q++)
        {
            results[q] = BestSumForQuery(_queries[q][0], _queries[q][1]);
        }

        return results;
    }

    private int BestSumForQuery(int x, int y)
    {
        var best = -1;

        for (var j = 0; j < _nums1.Length; j++)
        {
            if (_nums1[j] >= x && _nums2[j] >= y)
            {
                best = Math.Max(best, _nums1[j] + _nums2[j]);
            }
        }

        return best;
    }

    [Benchmark]
    public int[] SweepWithSegmentTree()
    {
        var distinctNums2 = SortedDistinct(_nums2);
        var tree = BuildEmptyMaxTree(distinctNums2.Length);
        var pairs = BuildPairsDescendingByNums1(distinctNums2);
        var sortedQueries = BuildQueriesDescendingByX();

        var results = new int[_queries.Length];
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

    private (int Nums1, int Rank, long Sum)[] BuildPairsDescendingByNums1(int[] distinctNums2)
    {
        var pairs = new (int Nums1, int Rank, long Sum)[_nums1.Length];
        var ranks = new ArraySequence<int>(distinctNums2);

        for (var j = 0; j < _nums1.Length; j++)
        {
            var rank = BinarySearch.LowerBound(ranks, _nums2[j]);
            pairs[j] = (_nums1[j], rank, (long)_nums1[j] + _nums2[j]);
        }

        var byNums1Descending = Comparer<(int Nums1, int Rank, long Sum)>.Create((a, b) => b.Nums1.CompareTo(a.Nums1));
        MergeSort.Sort<(int Nums1, int Rank, long Sum), ArrayIndexedSequence<(int Nums1, int Rank, long Sum)>>(
            new ArrayIndexedSequence<(int Nums1, int Rank, long Sum)>(pairs), byNums1Descending);

        return pairs;
    }

    private (int X, int Y, int OriginalIndex)[] BuildQueriesDescendingByX()
    {
        var sorted = new (int X, int Y, int OriginalIndex)[_queries.Length];

        for (var i = 0; i < _queries.Length; i++)
        {
            sorted[i] = (_queries[i][0], _queries[i][1], i);
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
