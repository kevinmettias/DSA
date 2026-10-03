using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.SlidingWindowMaximum;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SlidingWindowMaximumSolution's, the same methods
// SlidingWindowMaximumSolutionTests proves correct. Values are random across LC 239's
// whole [-10^4, 10^4] so ties/early-exit shortcuts in BruteForceRescan can't make it
// look artificially competitive. Both arms now build the actual per-window maximum
// array (LeetCode's real answer shape) rather than the summed reduction the
// pre-migration arms measured.
public class SlidingWindowMaximumBenchmarks
{
    private const int WindowSize = 50;
    private const int ValueBound = 10_000;

    private int[] _values = [];

    [Params(2_000, 20_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _values = SeededDraws.Values(Length, -ValueBound, ValueBound + 1, random);
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForceRescan() =>
        SlidingWindowMaximumSolution.MaxSlidingWindowByBruteForceRescan(_values, WindowSize);

    [Benchmark]
    public int[] MonotonicDeque() =>
        SlidingWindowMaximumSolution.MaxSlidingWindowByMonotonicDeque(_values, WindowSize);
}
