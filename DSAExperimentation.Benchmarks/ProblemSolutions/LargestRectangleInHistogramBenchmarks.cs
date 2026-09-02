using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.LargestRectangleInHistogram;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LargestRectangleInHistogramSolution's, the same
// methods LargestRectangleInHistogramTests proves correct - the O(n^2)
// per-bar left/right expansion baseline vs. the O(n) monotonic-stack sweep
// using this repo's own Stack<int>.
[MemoryDiagnoser]
public class LargestRectangleInHistogramBenchmarks
{
    private const int MaxHeight = 1_000;

    [Params(200, 5_000)]
    public int Length;

    private int[] _heights = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _heights = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxHeight)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => LargestRectangleInHistogramSolution.LargestRectangleAreaByBruteForce(_heights);

    [Benchmark]
    public int MonotonicStack() => LargestRectangleInHistogramSolution.LargestRectangleAreaByMonotonicStack(_heights);
}
