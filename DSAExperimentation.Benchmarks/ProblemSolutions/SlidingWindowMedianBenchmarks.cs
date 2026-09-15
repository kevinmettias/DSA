using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SlidingWindowMedian;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SlidingWindowMedianSolution's, the same methods
// SlidingWindowMedianTests proves correct - the O(n*k log k) baseline (copy each
// k-sized window and Array.Sort it from scratch) vs. the O(n log k) two-heap
// approach with lazy deletion. WindowSize is kept well below Length so both
// strategies do real repeated work across many windows, not one giant one.
[MemoryDiagnoser]
public class SlidingWindowMedianBenchmarks
{
    private const int WindowSize = 500;

    private const int RandomValueUpperBound = 10_000;

    private int[] _values = [];

    [Params(2_000, 8_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(0, RandomValueUpperBound)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public double[] SortEachWindow() =>
        SlidingWindowMedianSolution.MedianSlidingWindowBySortEachWindow(_values, WindowSize);

    [Benchmark]
    public double[] TwoHeapsLazyDeletion() =>
        SlidingWindowMedianSolution.MedianSlidingWindowByTwoHeapsLazyDeletion(_values, WindowSize);
}
