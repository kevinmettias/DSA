using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.TrappingRainWater;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are TrappingRainWaterSolution's, the same methods
// TrappingRainWaterTests proves correct - the textbook O(n^2) per-bar
// left/right rescan vs. the O(n) single sweep using this repo's own
// Stack<int> as a monotonic stack of candidate wall indices.
[MemoryDiagnoser]
public class TrappingRainWaterBenchmarks
{
    private const int MaxHeight = 1_000;

    private int[] _heights = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _heights = Enumerable.Range(0, Length).Select(_ => random.Next(0, MaxHeight)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => TrappingRainWaterSolution.TrapByBruteForce(_heights);

    [Benchmark]
    public int MonotonicStack() => TrappingRainWaterSolution.TrapByMonotonicStack(_heights);
}
