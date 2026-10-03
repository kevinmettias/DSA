using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.ShortestUnsortedContinuousSubarray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ShortestUnsortedContinuousSubarraySolution's, the
// same methods ShortestUnsortedContinuousSubarraySolutionTests proves correct. _values is
// random across LC 581's whole [-10^5, 10^5], so a genuinely already-sorted array is
// astronomically unlikely and both strategies do real work.
public class ShortestUnsortedContinuousSubarrayBenchmarks
{
    private const int RandomSeed = 581; // LC problem number
    private const int MinValue = -100_000;
    private const int MaxValue = 100_000;

    private int[] _values = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = SeededDraws.Values(Length, MinValue, MaxValue + 1, random);
    }

    [Benchmark(Baseline = true)]
    public int SelectionSortScan() =>
        ShortestUnsortedContinuousSubarraySolution.FindUnsortedSubarrayBySelectionSortScan(_values);

    [Benchmark]
    public int MergeSortScan() =>
        ShortestUnsortedContinuousSubarraySolution.FindUnsortedSubarrayByMergeSortScan(_values);
}
