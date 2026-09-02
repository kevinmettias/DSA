using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumEleganceOfAKLengthSubsequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumEleganceOfAKLengthSubsequenceSolution's, the
// same methods MaximumEleganceOfAKLengthSubsequenceTests proves correct against
// LeetCode's own examples. Categories are drawn from a pool much smaller than the
// item count so the post-k scan actually walks the duplicates stack down instead
// of running out of duplicates after the very first swap.
[MemoryDiagnoser]
public class MaximumEleganceOfAKLengthSubsequenceBenchmarks
{
    private const int MaxProfitExclusive = 100_000;
    private const int CategoryPoolSize = 20;
    private const int Seed = 1;

    [Params(500, 5_000)]
    public int ItemCount;

    private int[][] _items = null!;
    private int _k;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _items = Enumerable.Range(0, ItemCount)
            .Select(_ => new[] { random.Next(1, MaxProfitExclusive), random.Next(0, CategoryPoolSize) })
            .ToArray();
        _k = ItemCount / 2;
    }

    [Benchmark(Baseline = true)]
    public long Bcl() => MaximumEleganceOfAKLengthSubsequenceSolution.MaximumEleganceByBcl(_items, _k);

    [Benchmark]
    public long RepoPrimitives() => MaximumEleganceOfAKLengthSubsequenceSolution.MaximumEleganceByRepoPrimitives(_items, _k);
}
