using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.FenwickTree;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Count of Smaller Numbers After Self (LC 315): the textbook O(n^2) pairwise scan
// vs. a right-to-left sweep through this repo's own FenwickTree<int,SumOperation<int>>
// (a Binary Indexed Tree of counts), with each value's compressed rank found via
// BinarySearch.LowerBound over the sorted distinct values - O(n log n) overall,
// the same approach CountOfSmallerNumbersAfterSelfTests uses.
[MemoryDiagnoser]
public class CountOfSmallerNumbersAfterSelfBenchmarks
{
    private const int RandomSeed = 315; // LeetCode problem number
    private const int ValueBound = 10_000;

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
    public int[] PairwiseScan()
    {
        var counts = new int[_nums.Length];

        for (var i = 0; i < _nums.Length; i++)
        {
            var count = 0;
            for (var j = i + 1; j < _nums.Length; j++)
            {
                if (_nums[j] < _nums[i])
                {
                    count++;
                }
            }

            counts[i] = count;
        }

        return counts;
    }

    [Benchmark]
    public int[] FenwickTreeSweep()
    {
        var sortedDistinct = _nums.Distinct().OrderBy(value => value).ToArray();
        var sequence = new ArraySequence<int>(sortedDistinct);
        var tree = new FenwickTree<int, SumOperation<int>>(sortedDistinct.Length);
        var counts = new int[_nums.Length];

        for (var i = _nums.Length - 1; i >= 0; i--)
        {
            var rank = BinarySearch.LowerBound(sequence, _nums[i]);
            counts[i] = rank == 0 ? 0 : tree.PrefixQuery(rank - 1);
            tree.Add(rank, 1);
        }

        return counts;
    }
}
