using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumSumCircularSubarray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumSumCircularSubarraySolution's, the same
// methods MaximumSumCircularSubarrayTests proves correct - the O(n^2) brute force
// over every circular subarray vs. the O(n) two-Kadane-pass complement trick.
[MemoryDiagnoser]
public class MaximumSumCircularSubarrayBenchmarks
{
    private const int RandomSeed = 918; // LC problem number
    private const int ValueMagnitude = 50;

    [Params(200, 2_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(-ValueMagnitude, ValueMagnitude + 1)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForceAllCircularSubarrays() =>
        MaximumSumCircularSubarraySolution.MaxSubarraySumCircularByBruteForce(_values);

    [Benchmark]
    public int TwoPassKadane() =>
        MaximumSumCircularSubarraySolution.MaxSubarraySumCircularByTwoPassKadane(_values);
}
