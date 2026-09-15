using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.LongestBalancedSubarrayI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LongestBalancedSubarrayISolution's, the same
// methods LongestBalancedSubarrayITests proves correct. _nums alternates
// even/odd values so every prefix keeps both distinct-value sets growing
// together rather than one saturating out of a tiny alphabet early.
[MemoryDiagnoser]
public class LongestBalancedSubarrayIBenchmarks
{
    private int[] _nums = [];

    [Params(100, 1_500)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _nums = Enumerable.Range(0, Length).Select(i => 2 * i + (i % 2)).ToArray();

    [Benchmark(Baseline = true)]
    public int BruteForce() => LongestBalancedSubarrayISolution.FindLongestBalancedLengthByBruteForce(_nums);

    [Benchmark]
    public int DistinctSetScan() =>
        LongestBalancedSubarrayISolution.FindLongestBalancedLengthByDistinctSetScan(_nums);
}
