using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MaximumGap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumGapSolution's, the same methods
// MaximumGapTests proves correct - O(n^2) selection sort vs. this repo's own
// O(n log n) MergeSort, each followed by the same linear adjacent-gap scan.
[MemoryDiagnoser]
public class MaximumGapBenchmarks
{
    private const int RandomSeed = 164; // LC problem number

    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup() => _values = MaximumGapWorkloads.BuildValues(Length, RandomSeed);

    [Benchmark(Baseline = true)]
    public int SelectionSort() => MaximumGapSolution.MaximumGapBySelectionSort(_values);

    [Benchmark]
    public int MergeSortScan() => MaximumGapSolution.MaximumGapByMergeSort(_values);
}
