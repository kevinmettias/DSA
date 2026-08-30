using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Range Sum Query - Immutable (LC 303): a brute-force rescan baseline (sums nums[left..right]
// directly, O(n) per SumRange call) vs. this repo's own FenwickTree<int,SumOperation<int>>, built
// once from the array (O(n log n)) so every SumRange afterward is an O(log n) FenwickTree.Query
// instead of a rescan. Both methods answer the same fixed batch of queries.
[MemoryDiagnoser]
public class RangeSumQueryImmutableBenchmarks
{
    private const int QueryCount = 500;

    [Params(200, 5_000)]
    public int Length;

    private int[] _nums = null!;
    private (int Left, int Right)[] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(303);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(-1_000, 1_000)).ToArray();

        _queries = new (int Left, int Right)[QueryCount];
        for (var i = 0; i < QueryCount; i++)
        {
            var left = random.Next(0, Length);
            var right = random.Next(left, Length);
            _queries[i] = (left, right);
        }
    }

    [Benchmark(Baseline = true)]
    public long BruteForceRescan()
    {
        var total = 0L;

        foreach (var (left, right) in _queries)
        {
            for (var i = left; i <= right; i++)
            {
                total += _nums[i];
            }
        }

        return total;
    }

    [Benchmark]
    public long FenwickTreeQuery()
    {
        var tree = new FenwickTree<int, SumOperation<int>>(_nums);
        var total = 0L;

        foreach (var (left, right) in _queries)
        {
            total += tree.Query(left, right);
        }

        return total;
    }
}
