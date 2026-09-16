using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ContinuousSubarraySum;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ContinuousSubarraySumSolution's, the same methods
// ContinuousSubarraySumTests proves correct. K is chosen larger than any possible
// total sum of the generated values, so every prefix sum's remainder is just the
// prefix sum itself - strictly increasing since every value is positive, so no two
// prefix sums (nor the seeded {0: -1} entry, since every prefix sum stays positive)
// ever collide. Both methods are therefore forced through their full worst-case scan
// on every [Params] size instead of an early match letting either return early.
[MemoryDiagnoser]
public class ContinuousSubarraySumBenchmarks
{
    private const int K = 1_000_003;

    // Chosen so K comfortably exceeds any possible total sum of the generated
    // values, keeping every prefix sum's remainder equal to the prefix sum itself.
    private const int MaxGeneratedValueExclusive = 100;

    private int[] _values = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxGeneratedValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool HasSubarraySumMultipleOfKByBruteForce() =>
        ContinuousSubarraySumSolution.HasSubarraySumMultipleOfKByBruteForce(_values, K);

    [Benchmark]
    public bool HasSubarraySumMultipleOfKByHashMapPrefixRemainder() =>
        ContinuousSubarraySumSolution.HasSubarraySumMultipleOfKByHashMapPrefixRemainder(_values, K);
}
