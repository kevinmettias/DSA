using DSAExperimentation.LeetCode.CountTheNumberOfArraysWithKMatchingAdjacentElements;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// CountTheNumberOfArraysWithKMatchingAdjacentElementsSolution's, the same
// methods CountTheNumberOfArraysWithKMatchingAdjacentElementsSolutionTests proves
// correct. AlphabetSize stays fixed at 2 and the match count at half of the array
// length.
//
// Sizes are per arm. The brute-force arm enumerates all 2^n arrays, so it stops at 18;
// the closed form is O(n) - one factorial table and one modular power - and runs on to
// LC 3405's own bound of 10^5, and the two are compared at the lengths both run.
public class CountTheNumberOfArraysWithKMatchingAdjacentElementsBenchmarks
{
    private const int AlphabetSize = 2;

    private Dictionary<int, int> _matchCountByLength = [];

    public static IEnumerable<int> BruteForceSizes => [10, 18];

    public static IEnumerable<int> ModularCombinatoricsSizes => [.. BruteForceSizes, 1_000, 100_000];

    // Every length any arm runs has its match count derived here, outside the timed region;
    // an arm looks its own up.
    [GlobalSetup]
    public void Setup() =>
        _matchCountByLength = ModularCombinatoricsSizes.ToDictionary(length => length, length => (length - 1) / 2);

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BruteForceSizes))]
    public long BruteForce(int arrayLength) =>
        CountTheNumberOfArraysWithKMatchingAdjacentElementsSolution.CountGoodArraysByBruteForce(
            arrayLength, AlphabetSize, _matchCountByLength[arrayLength]);

    [Benchmark]
    [ArgumentsSource(nameof(ModularCombinatoricsSizes))]
    public long ModularCombinatorics(int arrayLength) =>
        CountTheNumberOfArraysWithKMatchingAdjacentElementsSolution.CountGoodArraysByModularCombinatorics(
            arrayLength, AlphabetSize, _matchCountByLength[arrayLength]);
}
