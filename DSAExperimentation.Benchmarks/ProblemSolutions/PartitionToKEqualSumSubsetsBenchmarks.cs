using DSAExperimentation.LeetCode.PartitionToKEqualSumSubsets;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PartitionToKEqualSumSubsetsSolution's. _nums is
// BucketCount interleaved copies of 1..NumbersPerSubset, so a perfect split always
// exists (each subset re-assembles the copy it came from) but the shuffled
// ordering still forces a real search rather than an immediate match. LC 698 allows at
// most 16 numbers and at most four copies of any one, so four buckets of 1..4 is its
// largest such workload.
public class PartitionToKEqualSumSubsetsBenchmarks
{
    private const int NumbersPerSubset = 4;
    private const int RandomSeed = 2;

    private int[] _nums = [];

    [Params(2, 4)]
    public int BucketCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var perSubset = Enumerable.Range(1, NumbersPerSubset).ToArray();
        var all = new List<int>();

        for (var subset = 0; subset < BucketCount; subset++)
        {
            all.AddRange(perSubset);
        }

        var random = new Random(RandomSeed);
        _nums = all.OrderBy(_ => random.Next()).ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool CanPartitionKSubsetsByNaiveBacktracking() =>
        PartitionToKEqualSumSubsetsSolution.CanPartitionKSubsetsByNaiveBacktracking(
            _nums, BucketCount);

    [Benchmark]
    public bool CanPartitionKSubsetsByGenericBacktrack() =>
        PartitionToKEqualSumSubsetsSolution.CanPartitionKSubsetsByGenericBacktrack(
            _nums, BucketCount);
}
