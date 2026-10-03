using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.HeightChecker;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are HeightCheckerSolution's - the textbook O(n^2)
// insertion sort of a copy against this repo's MergeSort over ArrayIndexedSequence.
// Length stops at LC 1051's 100-student cap, short of where the quadratic baseline
// pulls clearly away.
public class HeightCheckerBenchmarks
{
    // LeetCode problem number, reused as the RNG seed for reproducible benchmark input.
    private const int RandomSeed = 1051;

    // Exclusive upper bound for the random height range: heights are 1..100.
    private const int HeightUpperBoundExclusive = 101;

    private int[] _heights = [];

    [Params(10, 100)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _heights = SeededDraws.Values(Length, 1, HeightUpperBoundExclusive, random);
    }

    [Benchmark(Baseline = true)]
    public int InsertionSort() => HeightCheckerSolution.CountMismatchesByInsertionSort(_heights);

    [Benchmark]
    public int MergeSortComparison() => HeightCheckerSolution.CountMismatchesByMergeSort(_heights);
}
