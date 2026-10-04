using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.SplitArrayLargestSum;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SplitArrayLargestSumSolution's, the same methods
// SplitArrayLargestSumSolutionTests proves correct. Length stops at LC 410's 1,000,
// where the subarray count Length / KDivisor reaches its cap of 50.
public class SplitArrayLargestSumBenchmarks
{
    private const int RandomSeed = 410; // LC problem number
    private const int MaxElementValue = 1_000; // exclusive upper bound passed to Random.Next
    private const int KDivisor = 20; private int[] _nums = [];

    private int _subarrayCount;
    // number of subarrays to split into, derived as a fraction of Length

    [Params(200, 1_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = SeededDraws.Values(Length, 1, MaxElementValue, random);
        _subarrayCount = Math.Max(1, Length / KDivisor);
    }

    [Benchmark(Baseline = true)]
    public int ManualBinarySearch() => SplitArrayLargestSumSolution.MinimizedLargestSumByManualBinarySearch(_nums, _subarrayCount);

    [Benchmark]
    public int PredicateSearch() => SplitArrayLargestSumSolution.MinimizedLargestSumByPredicateSearch(_nums, _subarrayCount);
}
