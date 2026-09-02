using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ProductOfArrayExceptSelf;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the one arm is ProductOfArrayExceptSelfSolution's, the same
// method ProductOfArrayExceptSelfTests proves correct. The prior benchmark was
// a compile-smoke placeholder with no real workload; this backfills it with the
// prefix/suffix pass over a random array, including negative values and zero.
[MemoryDiagnoser]
public class ProductOfArrayExceptSelfBenchmarks
{
    private const int Seed = 238;
    private const int LowerBound = -1_000;
    private const int UpperBound = 1_000;

    [Params(200, 5_000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = [.. Enumerable.Range(0, Length).Select(_ => random.Next(LowerBound, UpperBound))];
    }

    [Benchmark]
    public int[] PrefixSuffixPass() =>
        ProductOfArrayExceptSelfSolution.ProductExceptSelfByPrefixSuffixPass(_nums);
}
