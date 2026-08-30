using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;
using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Number of Longest Increasing Subsequence (LC 673): the textbook O(n^2) DP (for
// each i, rescan every earlier j tracking both the longest length ending at i and
// how many subsequences reach it) vs. this repo's own SegmentTree<Element,
// ICombineOperation<Element>> keyed by a BinarySearch.LowerBound-compressed rank
// (the same LongestIncreasingSubsequenceBenchmarks precedent, extended from
// length-only to a (Length,Count) pair) - O(n log n).
[MemoryDiagnoser]
public class NumberOfLongestIncreasingSubsequenceBenchmarks
{
    [Params(3_000, 8_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(673);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(0, Length)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int DynamicProgramming()
    {
        var length = new int[_values.Length];
        var count = new int[_values.Length];
        var best = 0;

        for (var i = 0; i < _values.Length; i++)
        {
            length[i] = 1;
            count[i] = 1;

            for (var j = 0; j < i; j++)
            {
                if (_values[j] >= _values[i])
                {
                    continue;
                }

                if (length[j] + 1 > length[i])
                {
                    length[i] = length[j] + 1;
                    count[i] = count[j];
                }
                else if (length[j] + 1 == length[i])
                {
                    count[i] += count[j];
                }
            }

            best = Math.Max(best, length[i]);
        }

        var total = 0;

        for (var i = 0; i < _values.Length; i++)
        {
            if (length[i] == best)
            {
                total += count[i];
            }
        }

        return total;
    }

    [Benchmark]
    public int SegmentTreeCoordinateCompression()
    {
        var sortedDistinct = _values.Distinct().Order().ToArray();
        var ranks = new ArraySequence<int>(sortedDistinct);
        var tree = new SegmentTree<(int Length, int Count), LisAggregate>(new (int, int)[sortedDistinct.Length]);

        foreach (var num in _values)
        {
            var rank = BinarySearch.LowerBound<int, ArraySequence<int>>(ranks, num);
            var best = rank == 0 ? (Length: 0, Count: 0) : tree.Query(0, rank - 1);
            var candidate = best.Length == 0 ? (Length: 1, Count: 1) : (Length: best.Length + 1, best.Count);
            var existing = tree.Query(rank, rank);

            tree.Update(rank, LisAggregate.Combine(existing, candidate));
        }

        return tree.Query(0, sortedDistinct.Length - 1).Count;
    }

    // See NumberOfLongestIncreasingSubsequenceTests.LisAggregate for the full
    // explanation - repeated here rather than shared because TwoSumBenchmarks/
    // MedianOfTwoSortedArraysBenchmarks establish this project keeps its own copy of
    // the solution rather than depending on the Tests project.
    private readonly struct LisAggregate : ICombineOperation<(int Length, int Count)>
    {
        public static (int Length, int Count) Identity => (0, 0);

        public static (int Length, int Count) Combine((int Length, int Count) left, (int Length, int Count) right)
        {
            if (left.Length != right.Length)
            {
                return left.Length > right.Length ? left : right;
            }

            return (left.Length, left.Count + right.Count);
        }
    }
}
