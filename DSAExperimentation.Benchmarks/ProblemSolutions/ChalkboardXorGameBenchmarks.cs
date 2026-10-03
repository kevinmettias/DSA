using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.ChalkboardXorGame;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: all three arms are ChalkboardXorGameSolution's, the same methods
// ChalkboardXorGameSolutionTests proves correct. The two lengths every arm runs
// deliberately straddle the crossover: at 9 the unmemoized tree is still small enough
// to roughly match the Memoizer's own per-call overhead, but at 13 it is already ~32x
// slower in a local dry run - the same factorial blowup CanIWinBenchmarks documents
// for its own unmemoized bitmask baseline, here made visible sooner because every
// mask, not just a used-numbers subset, is a distinct memo key.
//
// Sizes are per arm. Both recursions are exponential - one in orderings, the other in
// remaining-element masks - so both stop at 13; the O(n) closed form shows what the
// whole search reduces to and runs on to LC 810's own bound of 1,000 numbers. All
// three are compared at the sizes they share.
public class ChalkboardXorGameBenchmarks
{
    private const int RandomSeed = 810; // LC problem number
    private const int RandomValueBitWidth = 16;

    private Dictionary<int, int[]> _numsByLength = [];

    public static IEnumerable<int> RecursionSizes => [9, 13];

    public static IEnumerable<int> FormulaSizes => [.. RecursionSizes, 100, 1_000];

    // Every length any arm runs is drawn here, outside the timed region, each from its own
    // generator on the same seed; an arm looks its own up.
    [GlobalSetup]
    public void Setup() =>
        _numsByLength = FormulaSizes.ToDictionary(
            length => length,
            length => SeededDraws.Values(length, 1, 1 << RandomValueBitWidth, new Random(RandomSeed)));

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(RecursionSizes))]
    public bool CanAliceWinByBruteForceRecursion(int length) =>
        ChalkboardXorGameSolution.CanAliceWinByBruteForceRecursion(_numsByLength[length]);

    [Benchmark]
    [ArgumentsSource(nameof(RecursionSizes))]
    public bool CanAliceWinByMemoizedRecursion(int length) =>
        ChalkboardXorGameSolution.CanAliceWinByMemoizedRecursion(_numsByLength[length]);

    [Benchmark]
    [ArgumentsSource(nameof(FormulaSizes))]
    public bool CanAliceWinByXorParityFormula(int length) =>
        ChalkboardXorGameSolution.CanAliceWinByXorParityFormula(_numsByLength[length]);
}
