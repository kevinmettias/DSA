using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SlidingWindowMaximum;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SlidingWindowMaximumSolution's, the same methods
// SlidingWindowMaximumTests proves correct. Values are random over a wide range
// so ties/early-exit shortcuts in BruteForceRescan can't make it look
// artificially competitive. Both arms now build the actual per-window maximum
// array (LeetCode's real answer shape) rather than the summed reduction the
// pre-migration arms measured.
[MemoryDiagnoser]
public class SlidingWindowMaximumBenchmarks
{
    private const int WindowSize = 50;
    private const int ValueBound = 1_000_000;

    private int[] _values = [];

    [Params(2_000, 20_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(-ValueBound, ValueBound)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForceRescan() =>
        SlidingWindowMaximumSolution.MaxSlidingWindowByBruteForceRescan(_values, WindowSize);

    [Benchmark]
    public int[] MonotonicDeque() =>
        SlidingWindowMaximumSolution.MaxSlidingWindowByMonotonicDeque(_values, WindowSize);
}
