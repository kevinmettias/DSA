using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.OneThreeTwoPattern;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are OneThreeTwoPatternSolution's, the same methods
// OneThreeTwoPatternTests proves correct. _nums is strictly increasing, which
// contains no 132 pattern at all, forcing both strategies through their full
// worst-case scan instead of an early exit on the first triple.
[MemoryDiagnoser]
public class OneThreeTwoPatternBenchmarks
{
    private int[] _nums = [];

    [Params(200, 3_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _nums = Enumerable.Range(0, Length).ToArray();

    [Benchmark(Baseline = true)]
    public bool BruteForceMinPrefixScan() => OneThreeTwoPatternSolution.HasPatternByBruteForce(_nums);

    [Benchmark]
    public bool MonotonicStack() => OneThreeTwoPatternSolution.HasPatternByMonotonicStack(_nums);
}
