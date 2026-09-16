using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SplitArrayLargestSum;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SplitArrayLargestSumSolution's, the same methods
// SplitArrayLargestSumTests proves correct.
[MemoryDiagnoser]
public class SplitArrayLargestSumBenchmarks
{
    private const int RandomSeed = 410; // LC problem number
    private const int MaxElementValue = 1_000; // exclusive upper bound passed to Random.Next
    private const int KDivisor = 20; private int[] _nums = [];

    private int _subarrayCount;
    // number of subarrays to split into, derived as a fraction of Length

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxElementValue)).ToArray();
        _subarrayCount = Math.Max(1, Length / KDivisor);
    }

    [Benchmark(Baseline = true)]
    public int ManualBinarySearch() => SplitArrayLargestSumSolution.MinimizedLargestSumByManualBinarySearch(_nums, _subarrayCount);

    [Benchmark]
    public int SequenceLowerBound() => SplitArrayLargestSumSolution.MinimizedLargestSumBySequenceLowerBound(_nums, _subarrayCount);
}
