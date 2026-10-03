using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.FindTheNumberOfSubsequencesWithEqualGcd;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindTheNumberOfSubsequencesWithEqualGcdSolution's,
// the same methods FindTheNumberOfSubsequencesWithEqualGcdSolutionTests proves correct
// (TwoSumBenchmarks precedent).
//
// Sizes are per arm. Brute force is a genuine 3^n choice tree (each element: join
// seq1, join seq2, or join neither), so it stops at 12
// (CountTheNumberOfSquareFreeSubsetsBenchmarks' own precedent for "size the
// baseline can survive"); the memoized arm only pays for the distinct
// (index, gcd1, gcd2) states that actually occur and runs on to LC 3336's own bound
// of 200, and the two are compared at the lengths both run.
public class FindTheNumberOfSubsequencesWithEqualGcdBenchmarks
{
    private const int MinValueInclusive = 1;
    private const int MaxValueExclusive = 51;
    private const int Seed = 3336;

    private Dictionary<int, int[]> _numsByLength = [];

    public static IEnumerable<int> BruteForceSizes => [8, 12];

    public static IEnumerable<int> GcdMemoizationSizes => [.. BruteForceSizes, 50, 200];

    // Every length any arm runs is drawn here, outside the timed region, each from its own
    // generator on the same seed; an arm looks its own up.
    [GlobalSetup]
    public void Setup() =>
        _numsByLength = GcdMemoizationSizes.ToDictionary(
            length => length,
            length => SeededDraws.Values(length, MinValueInclusive, MaxValueExclusive, new Random(Seed)));

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BruteForceSizes))]
    public int BruteForce(int length) =>
        FindTheNumberOfSubsequencesWithEqualGcdSolution.CountPairsByBruteForce(_numsByLength[length]);

    [Benchmark]
    [ArgumentsSource(nameof(GcdMemoizationSizes))]
    public int GcdMemoization(int length) =>
        FindTheNumberOfSubsequencesWithEqualGcdSolution.CountPairsByGcdMemoization(_numsByLength[length]);
}
