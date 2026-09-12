using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.PartitionToKEqualSumSubsets;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PartitionToKEqualSumSubsetsSolution's. _nums is K
// interleaved copies of 1..NumbersPerSubset, so a perfect split always exists
// (each subset re-assembles the copy it came from) but the shuffled ordering
// still forces a real search rather than an immediate match.
[MemoryDiagnoser]
public class PartitionToKEqualSumSubsetsBenchmarks
{
    private const int NumbersPerSubset = 6;
    private const int RandomSeed = 2;

    [Params(3, 5)]
    public int K;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var perSubset = Enumerable.Range(1, NumbersPerSubset).ToArray();
        var all = new List<int>();

        for (var subset = 0; subset < K; subset++)
        {
            all.AddRange(perSubset);
        }

        var random = new Random(RandomSeed);
        _nums = all.OrderBy(_ => random.Next()).ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool NaiveBacktracking() =>
        PartitionToKEqualSumSubsetsSolution.CanPartitionKSubsetsByNaiveBacktracking(_nums, K);

    [Benchmark]
    public bool BacktrackPrimitive() =>
        PartitionToKEqualSumSubsetsSolution.CanPartitionKSubsetsByGenericBacktrack(_nums, K);
}
