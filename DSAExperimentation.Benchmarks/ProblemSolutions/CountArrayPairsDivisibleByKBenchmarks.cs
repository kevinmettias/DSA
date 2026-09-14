using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountArrayPairsDivisibleByK;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountArrayPairsDivisibleByKSolution's, the same methods
// CountArrayPairsDivisibleByKTests proves correct. The workload is a random array of
// values below 1_000 checked against k = 100, so the distinct Gcd(value, 100) groups
// stay bounded by that k's divisor count however long the array grows - which is the
// whole comparison: O(n^2) elementwise against O(n + d^2) group-wise.
[MemoryDiagnoser]
public class CountArrayPairsDivisibleByKBenchmarks
{
    private const int K = 100;
    private const int MaxValueExclusive = 1_000;
    private const int RandomSeed = 2183; // LC problem number

    [Params(200, 5_000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long BruteForcePairwiseCheck() =>
        CountArrayPairsDivisibleByKSolution.CountPairsByBruteForce(_nums, K);

    [Benchmark]
    public long GcdGroupedHashMapCount() =>
        CountArrayPairsDivisibleByKSolution.CountPairsByGcdGroups(_nums, K);
}
