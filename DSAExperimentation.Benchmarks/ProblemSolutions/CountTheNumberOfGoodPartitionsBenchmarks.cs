using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountTheNumberOfGoodPartitions;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountTheNumberOfGoodPartitionsSolution's, the same
// methods CountTheNumberOfGoodPartitionsTests proves agree.
//
// A small 4-value alphabet forces repeat values often enough that most cut masks
// are invalid, exercising the brute-force check's early-reject path instead of
// degenerating to "every mask is good". ArrayLength stays small (brute force
// enumerates 2^(n-1) masks and would not finish otherwise); the merge strategy is
// O(n) regardless of how many distinct values repeat.
[MemoryDiagnoser]
public class CountTheNumberOfGoodPartitionsBenchmarks
{
    private const int RandomSeed = 2963; // LC problem number
    private const int AlphabetSize = 4;

    private int[] _nums = [];

    [Params(16, 20)]
    public int ArrayLength { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, ArrayLength).Select(_ => random.Next(AlphabetSize)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long BruteForce() => CountTheNumberOfGoodPartitionsSolution.CountGoodPartitionsByBruteForce(_nums);

    [Benchmark]
    public long LastOccurrenceMerge() => CountTheNumberOfGoodPartitionsSolution.CountGoodPartitionsByLastOccurrenceMerge(_nums);
}
