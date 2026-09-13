using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SortIntegersByThePowerValue;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SortIntegersByThePowerValueSolution's, the same methods
// SortIntegersByThePowerValueTests proves correct - the textbook O(n^2) insertion sort
// of the (Power, Value) pairs against this repo's own MergeSort over
// ArrayIndexedSequence. Both arms recompute the same Collatz powers first, so the
// comparison isolates the sorting strategy rather than the power computation. k is the
// range length, so each arm reports the last pair in sorted order.
[MemoryDiagnoser]
public class SortIntegersByThePowerValueBenchmarks
{
    // The measured range is always [1, RangeLength].
    private const int Lo = 1;

    [Params(200, 5_000)]
    public int RangeLength;

    [Benchmark(Baseline = true)]
    public int InsertionSort() =>
        SortIntegersByThePowerValueSolution.GetKthByInsertionSort(Lo, Lo + RangeLength - 1, RangeLength);

    [Benchmark]
    public int MergeSortAscending() =>
        SortIntegersByThePowerValueSolution.GetKthByMergeSort(Lo, Lo + RangeLength - 1, RangeLength);
}
