using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.HeightChecker;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are HeightCheckerSolution's - the textbook O(n^2)
// insertion sort of a copy against this repo's MergeSort over ArrayIndexedSequence.
[MemoryDiagnoser]
public class HeightCheckerBenchmarks
{
    // LeetCode problem number, reused as the RNG seed for reproducible benchmark input.
    private const int RandomSeed = 1051;

    // Exclusive upper bound for the random height range: heights are 1..100.
    private const int HeightUpperBoundExclusive = 101;

    [Params(200, 5_000)]
    public int Length;

    private int[] _heights = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _heights = Enumerable.Range(0, Length).Select(_ => random.Next(1, HeightUpperBoundExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int InsertionSort() => HeightCheckerSolution.CountMismatchesByInsertionSort(_heights);

    [Benchmark]
    public int MergeSortComparison() => HeightCheckerSolution.CountMismatchesByMergeSort(_heights);
}
