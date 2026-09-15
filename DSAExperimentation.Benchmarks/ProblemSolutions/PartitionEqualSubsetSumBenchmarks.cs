using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.PartitionEqualSubsetSum;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PartitionEqualSubsetSumSolution's, the same methods
// PartitionEqualSubsetSumTests proves correct. Each arm takes the prepared-input
// overload with `half` already computed, so that summation is charged to
// [GlobalSetup] rather than to the search being measured.
[MemoryDiagnoser]
public class PartitionEqualSubsetSumBenchmarks
{
    private const int RandomSeed = 416; // LC problem number
    private const int MaxElementValue = 100;
    private const int SubsetSumDivisor = 2;

    private int[] _nums = [];

    private int _half;
    [Params(50, 400)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxElementValue)).ToArray();
        _half = _nums.Sum() / SubsetSumDivisor;
    }

    [Benchmark(Baseline = true)]
    public bool Tabulation() => PartitionEqualSubsetSumSolution.CanPartitionByTabulation(_nums, _half);

    [Benchmark]
    public bool Memoized() => PartitionEqualSubsetSumSolution.CanPartitionByMemoization(_nums, _half);
}
