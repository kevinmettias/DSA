using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.FenwickTree;
using DSAExperimentation.LeetCode.RangeSumQueryImmutable;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RangeSumQueryImmutableSolution's, the same
// methods RangeSumQueryImmutableTests proves correct, run over a fixed batch of
// queries - NumArray is constructed once per arm and sumRange is called
// QueryCount times against it, the pattern LeetCode's own class exposes, so the
// comparison is "build + QueryCount O(log n) queries" against "no build +
// QueryCount O(n) rescans".
[MemoryDiagnoser]
public class RangeSumQueryImmutableBenchmarks
{
    private const int QueryCount = 500;

    // LC problem number, reused as the Random seed for reproducible benchmark input.
    private const int RandomSeed = 303;

    private const int ValueRange = 1_000;

    private int[] _nums = [];

    private (int Left, int Right)[] _queries = [];
    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(-ValueRange, ValueRange)).ToArray();

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
            total += RangeSumQueryImmutableSolution.SumRangeByBruteForceRescan(_nums, left, right);
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
            total += RangeSumQueryImmutableSolution.SumRangeByFenwickTree(tree, left, right);
        }

        return total;
    }
}
