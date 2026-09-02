using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumAndMinimumSumsOfAtMostSizeKSubsequences;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumAndMinimumSumsOfAtMostSizeKSubsequencesSolution's,
// the same methods MaximumAndMinimumSumsOfAtMostSizeKSubsequencesTests proves
// correct.
[MemoryDiagnoser]
public class MaximumAndMinimumSumsOfAtMostSizeKSubsequencesBenchmarks
{
    private const int Seed = 3428;

    // LC caps k at min(70, n); 50 keeps both row-sum strategies doing real work
    // across every Length below.
    private const int K = 50;

    [Params(500, 5_000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = [.. Enumerable.Range(0, Length).Select(_ => random.Next(0, 1_000_000_000))];
    }

    [Benchmark(Baseline = true)]
    public long PascalTriangle() =>
        MaximumAndMinimumSumsOfAtMostSizeKSubsequencesSolution.SumByPascalTriangle(_nums, K);

    [Benchmark]
    public long FactorialCombinatorics() =>
        MaximumAndMinimumSumsOfAtMostSizeKSubsequencesSolution.SumByFactorialCombinatorics(_nums, K);
}
