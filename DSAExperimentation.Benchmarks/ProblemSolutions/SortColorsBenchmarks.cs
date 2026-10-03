using DSAExperimentation.LeetCode.SortColors;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SortColorsSolution's, the same methods
// SortColorsSolutionTests proves correct. The workload is already color-sorted in
// descending order (2, 1, 0, 2, 1, 0, ...) so both strategies do real work over
// the whole length rather than short-circuiting on an already-ascending run.
public class SortColorsBenchmarks
{
    private const int MaxColorValue = 2;
    private const int ColorCount = 3;

    private int[] _values = [];

    // The array each arm copies _values into and sorts; allocated once in setup.
    private int[] _copy = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _values = Enumerable.Range(0, Length).Select(i => MaxColorValue - (i % ColorCount)).ToArray();
        _copy = new int[Length];
    }

    // Each arm sorts a fresh copy in place and returns it. The copy is timed on purpose: both
    // strategies mutate their input, so every arm pays the same O(n) copy.
    [Benchmark(Baseline = true)]
    public int[] ArraySort()
    {
        _values.CopyTo(_copy, 0);
        SortColorsSolution.SortByArraySort(_copy);
        return _copy;
    }

    [Benchmark]
    public int[] ArrayIndexedDutchFlag()
    {
        _values.CopyTo(_copy, 0);
        SortColorsSolution.SortByDutchFlagPartition(_copy);
        return _copy;
    }
}
