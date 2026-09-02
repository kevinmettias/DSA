using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.FenwickTree;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Number of Pairs Satisfying Inequality (LC 2426): the textbook O(n^2) pairwise scan
// vs. a left-to-right sweep through this repo's own FenwickTree<int,SumOperation<int>>
// (a Binary Indexed Tree of counts) over the coordinate-compressed diff[i] =
// nums1[i]-nums2[i] values, with each query's "<=" cutoff found via
// BinarySearch.UpperBound - O(n log n) overall, the same approach
// NumberOfPairsSatisfyingInequalityTests uses.
[MemoryDiagnoser]
public class NumberOfPairsSatisfyingInequalityBenchmarks
{
    private const int RandomSeed = 2426; // LeetCode problem number
    private const int ValueBound = 10_000;

    [Params(200, 5_000)]
    public int Length;

    private int[] _nums1 = null!;
    private int[] _nums2 = null!;
    private int _diff;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums1 = Enumerable.Range(0, Length).Select(_ => random.Next(-ValueBound, ValueBound)).ToArray();
        _nums2 = Enumerable.Range(0, Length).Select(_ => random.Next(-ValueBound, ValueBound)).ToArray();
        _diff = random.Next(0, ValueBound);
    }

    [Benchmark(Baseline = true)]
    public long PairwiseScan()
    {
        var count = 0L;

        for (var i = 0; i < _nums1.Length; i++)
        {
            for (var j = i + 1; j < _nums1.Length; j++)
            {
                if (_nums1[i] - _nums1[j] <= _nums2[i] - _nums2[j] + _diff)
                {
                    count++;
                }
            }
        }

        return count;
    }

    [Benchmark]
    public long FenwickTreeSweep()
    {
        var n = _nums1.Length;
        var differences = new int[n];
        for (var i = 0; i < n; i++)
        {
            differences[i] = _nums1[i] - _nums2[i];
        }

        var sortedDistinct = differences.Distinct().OrderBy(value => value).ToArray();
        var sequence = new ArraySequence<int>(sortedDistinct);
        var tree = new FenwickTree<int, SumOperation<int>>(sortedDistinct.Length);

        var count = 0L;

        foreach (var value in differences)
        {
            var upperRank = BinarySearch.UpperBound(sequence, value + _diff);
            count += upperRank == 0 ? 0 : tree.PrefixQuery(upperRank - 1);

            var ownRank = BinarySearch.LowerBound(sequence, value);
            tree.Add(ownRank, 1);
        }

        return count;
    }
}
