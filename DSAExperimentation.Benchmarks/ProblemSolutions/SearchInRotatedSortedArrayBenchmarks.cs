using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SearchInRotatedSortedArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SearchInRotatedSortedArraySolution's, the same
// methods SearchInRotatedSortedArrayTests proves correct.
[MemoryDiagnoser]
public class SearchInRotatedSortedArrayBenchmarks
{
    // Rotates the sorted array by roughly a third so the pivot sits away from both ends.
    private const int PivotDivisor = 3;

    // The search target: an existing value two below Length, so it lands away from the
    // sorted range's own boundary and both strategies do genuine search work.
    private const int TargetOffsetFromLength = 2;

    private int[] _values = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var sorted = Enumerable.Range(0, Length).ToArray();
        var pivot = Length / PivotDivisor;
        _values = sorted[pivot..].Concat(sorted[..pivot]).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int LinearScan() => SearchInRotatedSortedArraySolution.SearchByLinearScan(_values, Length - TargetOffsetFromLength);

    [Benchmark]
    public int BinarySearchPivotAndSlice() =>
        SearchInRotatedSortedArraySolution.SearchByBinarySearchPivotAndSlice(_values, Length - TargetOffsetFromLength);
}
