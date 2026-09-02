using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.FenwickTree;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Reverse Pairs (LC 493): the textbook O(n^2) pairwise scan vs. a left-to-right sweep
// through this repo's own FenwickTree<int,SumOperation<int>> (a Binary Indexed Tree of
// counts), with each value's compressed rank found via BinarySearch.LowerBound/UpperBound
// over the sorted distinct values (as long, since 2*value can overflow a 32-bit int) -
// O(n log n) overall, the same approach ReversePairsTests uses and the same
// coordinate-compression-plus-Fenwick shape CountOfSmallerNumbersAfterSelfBenchmarks
// already establishes.
[MemoryDiagnoser]
public class ReversePairsBenchmarks
{
    private const int RandomSeed = 493; // LC problem number
    private const int ValueBound = 10_000;
    private const long ReversePairMultiplier = 2L;

    [Params(200, 5_000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(-ValueBound, ValueBound)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int PairwiseScan()
    {
        var count = 0;

        for (var i = 0; i < _nums.Length; i++)
        {
            for (var j = i + 1; j < _nums.Length; j++)
            {
                if ((long)_nums[i] > ReversePairMultiplier * _nums[j])
                {
                    count++;
                }
            }
        }

        return count;
    }

    [Benchmark]
    public int FenwickTreeSweep()
    {
        var sortedDistinct = _nums.Select(value => (long)value).Distinct().OrderBy(value => value).ToArray();
        var sequence = new ArraySequence<long>(sortedDistinct);
        var tree = new FenwickTree<int, SumOperation<int>>(sortedDistinct.Length);
        var count = 0;

        foreach (var value in _nums)
        {
            var firstGreaterRank = BinarySearch.UpperBound(sequence, ReversePairMultiplier * value);

            if (firstGreaterRank < sortedDistinct.Length)
            {
                count += tree.Query(firstGreaterRank, sortedDistinct.Length - 1);
            }

            var rank = BinarySearch.LowerBound(sequence, (long)value);
            tree.Add(rank, 1);
        }

        return count;
    }
}
