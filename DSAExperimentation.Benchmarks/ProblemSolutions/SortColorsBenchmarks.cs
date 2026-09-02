using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SortColors;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SortColorsSolution's, the same methods
// SortColorsTests proves correct. The workload is already color-sorted in
// descending order (2, 1, 0, 2, 1, 0, ...) so both strategies do real work over
// the whole length rather than short-circuiting on an already-ascending run.
[MemoryDiagnoser]
public class SortColorsBenchmarks
{
    private const int MaxColorValue = 2;
    private const int ColorCount = 3;

    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup() =>
        _values = Enumerable.Range(0, Length).Select(i => MaxColorValue - (i % ColorCount)).ToArray();

    [Benchmark(Baseline = true)]
    public int ArraySort()
    {
        var copy = (int[])_values.Clone();
        SortColorsSolution.SortByArraySort(copy);
        return copy[0];
    }

    [Benchmark]
    public int ArrayIndexedDutchFlag()
    {
        var copy = (int[])_values.Clone();
        SortColorsSolution.SortByDutchFlagPartition(copy);
        return copy[0];
    }
}
