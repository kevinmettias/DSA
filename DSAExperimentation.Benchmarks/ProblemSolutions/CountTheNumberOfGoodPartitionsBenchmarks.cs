using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.CountTheNumberOfGoodPartitions;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountTheNumberOfGoodPartitionsSolution's, the same
// methods CountTheNumberOfGoodPartitionsSolutionTests proves agree.
//
// A small 4-value alphabet forces repeat values often enough that most cut masks
// are invalid, exercising the brute-force check's early-reject path instead of
// degenerating to "every mask is good".
//
// Sizes are per arm. Brute force enumerates 2^(n-1) masks and would not finish past
// 20 elements, so it stops there; the merge strategy is O(n) regardless of how many
// distinct values repeat and runs on to LC 2963's own bound of 10^5. The two are
// compared at the lengths both run.
public class CountTheNumberOfGoodPartitionsBenchmarks
{
    private const int RandomSeed = 2963; // LC problem number
    private const int AlphabetSize = 4;

    private Dictionary<int, int[]> _numsByLength = [];

    public static IEnumerable<int> BruteForceSizes => [16, 20];

    public static IEnumerable<int> MergeSizes => [.. BruteForceSizes, 1_000, 100_000];

    // Every length any arm runs is drawn here, outside the timed region, each from its own
    // generator on the same seed; an arm looks its own up.
    [GlobalSetup]
    public void Setup() =>
        _numsByLength = MergeSizes.ToDictionary(
            length => length,
            length => SeededDraws.Values(length, 0, AlphabetSize, new Random(RandomSeed)));

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BruteForceSizes))]
    public long BruteForce(int arrayLength) =>
        CountTheNumberOfGoodPartitionsSolution.CountGoodPartitionsByBruteForce(_numsByLength[arrayLength]);

    [Benchmark]
    [ArgumentsSource(nameof(MergeSizes))]
    public long LastOccurrenceMerge(int arrayLength) =>
        CountTheNumberOfGoodPartitionsSolution.CountGoodPartitionsByLastOccurrenceMerge(_numsByLength[arrayLength]);
}
