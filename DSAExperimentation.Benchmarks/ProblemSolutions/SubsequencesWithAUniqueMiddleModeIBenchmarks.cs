using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.SubsequencesWithAUniqueMiddleModeI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SubsequencesWithAUniqueMiddleModeISolution's, the
// same methods SubsequencesWithAUniqueMiddleModeISolutionTests proves correct. A small
// value range forces plenty of repeats, exercising the modular-combinatorics
// arm's distinct-pair bookkeeping instead of degenerating to the all-values-
// unique case.
//
// Sizes are per arm. The brute-force arm is O(n^5), so it stops at 14; the
// combinatorics arm is O(n^2) and runs on to LeetCode's real n = 1000. The two are
// compared at the sizes both run.
public class SubsequencesWithAUniqueMiddleModeIBenchmarks
{
    private const int RandomSeed = 3395; // LC problem number
    private const int ValueRange = 5;

    private Dictionary<int, int[]> _numsBySize = [];

    public static IEnumerable<int> BaselineSizes => [10, 14];

    public static IEnumerable<int> CombinatoricsSizes => [.. BaselineSizes, 100, 1_000];

    // Every size any arm runs is built here, outside the timed region; an arm looks its own up.
    [GlobalSetup]
    public void Setup() =>
        _numsBySize = CombinatoricsSizes.ToDictionary(
            length => length,
            length => SeededDraws.Values(length, 0, ValueRange, new Random(RandomSeed)));

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BaselineSizes))]
    public long BruteForce(int length) =>
        SubsequencesWithAUniqueMiddleModeISolution.CountMiddleModeSubsequencesByBruteForce(_numsBySize[length]);

    [Benchmark]
    [ArgumentsSource(nameof(CombinatoricsSizes))]
    public long ModularCombinatorics(int length) =>
        SubsequencesWithAUniqueMiddleModeISolution.CountMiddleModeSubsequencesByModularCombinatorics(
            _numsBySize[length]);
}
