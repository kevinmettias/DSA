using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumProductSubarray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumProductSubarraySolution's, the same methods
// MaximumProductSubarrayTests proves correct - the O(n^2) all-subarrays brute
// force vs. the O(n) single pass tracking both a running min and a running max
// product.
[MemoryDiagnoser]
public class MaximumProductSubarrayBenchmarks
{
    private const int RandomSeed = 152; // LC problem number
    private const int ValueMagnitude = 10;

    private int[] _values = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(-ValueMagnitude, ValueMagnitude + 1)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForceAllSubarrays() => MaximumProductSubarraySolution.MaxProductByBruteForce(_values);

    [Benchmark]
    public int MinMaxSinglePass() => MaximumProductSubarraySolution.MaxProductByMinMaxScan(_values);
}
