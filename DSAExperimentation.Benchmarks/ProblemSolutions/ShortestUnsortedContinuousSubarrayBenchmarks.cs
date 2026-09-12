using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ShortestUnsortedContinuousSubarray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ShortestUnsortedContinuousSubarraySolution's, the
// same methods ShortestUnsortedContinuousSubarrayTests proves correct. _values is
// random, so a genuinely already-sorted array is astronomically unlikely and both
// strategies do real work.
[MemoryDiagnoser]
public class ShortestUnsortedContinuousSubarrayBenchmarks
{
    private const int RandomSeed = 581; // LC problem number
    private const int ValueUpperBoundExclusive = 1_000_000;

    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(0, ValueUpperBoundExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int SelectionSortScan() =>
        ShortestUnsortedContinuousSubarraySolution.FindUnsortedSubarrayBySelectionSortScan(_values);

    [Benchmark]
    public int MergeSortScan() =>
        ShortestUnsortedContinuousSubarraySolution.FindUnsortedSubarrayByMergeSortScan(_values);
}
