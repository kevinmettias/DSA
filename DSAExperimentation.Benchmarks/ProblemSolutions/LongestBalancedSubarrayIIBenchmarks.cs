using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.LongestBalancedSubarrayII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LongestBalancedSubarrayIISolution's, the same
// methods LongestBalancedSubarrayIITests proves correct. _nums alternates
// even/odd values, the same workload shape LongestBalancedSubarrayIBenchmarks
// uses, kept small enough for BOTH arms to finish - this is where BruteForce's
// O(n^2) is meant to visibly lose to PrefixBalanceSegmentTree's O(n log^2 n).
[MemoryDiagnoser]
public class LongestBalancedSubarrayIIBenchmarks
{
    private int[] _nums = [];

    [Params(100, 2_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _nums = Enumerable.Range(0, Length).Select(i => 2 * i + (i % 2)).ToArray();

    [Benchmark(Baseline = true)]
    public int BruteForce() => LongestBalancedSubarrayIISolution.FindLongestBalancedLengthByBruteForce(_nums);

    [Benchmark]
    public int PrefixBalanceSegmentTree() =>
        LongestBalancedSubarrayIISolution.FindLongestBalancedLengthByPrefixBalanceSegmentTree(_nums);
}
