using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.FindTheMinimumCostArrayPermutation;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindTheMinimumCostArrayPermutationSolution's, the
// same methods FindTheMinimumCostArrayPermutationSolutionTests proves correct. Neither
// strategy needs input construction beyond the permutation itself, so [GlobalSetup]
// only builds a random permutation of each size - workload sizing that fixes no domain
// content, the same inline-randomized shape AddTwoNumbersBenchmarks uses for its own
// random digit list.
//
// Sizes are per arm. The brute-force arm is O(n!), so it stops at 8; the bitmask-TSP
// memo is O(2^n * n^2) per solve, but its lexicographic reconstruction re-solves from
// every candidate, so it runs on to 13 rather than LC 3149's own n <= 14, keeping a call
// near a benchmark's budget. The two are compared at the sizes both run.
public class FindTheMinimumCostArrayPermutationBenchmarks
{
    private const int Seed = 3149;

    private Dictionary<int, int[]> _numsBySize = [];

    public static IEnumerable<int> BruteForceSizes => [6, 8];

    public static IEnumerable<int> BitmaskMemoizationSizes => [.. BruteForceSizes, 11, 13];

    // Every size any arm runs is shuffled here, outside the timed region; an arm looks its
    // own up.
    [GlobalSetup]
    public void Setup() =>
        _numsBySize = BitmaskMemoizationSizes.ToDictionary(size => size, size => SeededSequences.ShuffledZeroTo(size, Seed));

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BruteForceSizes))]
    public int[] BruteForceSearch(int permutationSize) =>
        FindTheMinimumCostArrayPermutationSolution.FindPermutationByBruteForceSearch(_numsBySize[permutationSize]);

    [Benchmark]
    [ArgumentsSource(nameof(BitmaskMemoizationSizes))]
    public int[] BitmaskMemoization(int permutationSize) =>
        FindTheMinimumCostArrayPermutationSolution.FindPermutationByBitmaskMemoization(_numsBySize[permutationSize]);
}
