using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumPrimeDifference;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumPrimeDifferenceSolution's, the same
// methods MaximumPrimeDifferenceTests proves correct. Values are drawn from
// the LeetCode-guaranteed [1, 100] range, which is dense enough with primes
// (25 of the 100 values) that the brute-force pairwise scan does real work
// rather than short-circuiting on a mostly-composite array.
[MemoryDiagnoser]
public class MaximumPrimeDifferenceBenchmarks
{
    private const int Seed = 3115; // LC problem number
    private const int MaxValueExclusive = 101;

    private int[] _nums = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForcePairs() => MaximumPrimeDifferenceSolution.MaxDistanceByBruteForcePairs(_nums);

    [Benchmark]
    public int EndpointScanWithSieve() => MaximumPrimeDifferenceSolution.MaxDistanceByEndpointScanWithSieve(_nums);
}
