using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.SortAnArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SortAnArraySolution's, the same methods
// SortAnArrayTests proves correct - the textbook O(n^2) insertion sort vs. this
// repo's own MergeSort over ArrayIndexedSequence. Each arm sorts a fresh copy of
// the same randomized input internally, so neither benefits from the other's
// partially-sorted leftovers.
[MemoryDiagnoser]
public class SortAnArrayBenchmarks
{
    // LC problem number, reused as the Random seed for reproducible benchmark input.
    private const int RandomSeed = 912;

    private int[] _values = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _values = SortAnArrayWorkloads.BuildValues(Length, RandomSeed);

    [Benchmark(Baseline = true)]
    public int[] InsertionSort() => SortAnArraySolution.SortArrayByInsertionSort(_values);

    [Benchmark]
    public int[] MergeSortAscending() => SortAnArraySolution.SortArrayByMergeSort(_values);
}
