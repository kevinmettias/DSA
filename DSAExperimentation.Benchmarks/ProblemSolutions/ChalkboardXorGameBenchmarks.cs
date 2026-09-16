using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ChalkboardXorGame;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: all three arms are ChalkboardXorGameSolution's, the same methods
// ChalkboardXorGameTests proves correct. The two Length values deliberately straddle
// the crossover: at 9 the unmemoized tree is still small enough to roughly match the
// Memoizer's own per-call overhead, but at 13 it is already ~32x slower in a local
// dry run - the same factorial blowup CanIWinBenchmarks documents for its own
// unmemoized bitmask baseline, here made visible sooner because every mask, not just
// a used-numbers subset, is a distinct memo key. The closed form is O(n) and shows
// what the whole search reduces to.
[MemoryDiagnoser]
public class ChalkboardXorGameBenchmarks
{
    private const int RandomSeed = 810; // LC problem number
    private const int RandomValueBitWidth = 16;

    private int[] _nums = [];

    [Params(9, 13)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, 1 << RandomValueBitWidth)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool CanAliceWinByBruteForceRecursion() =>
        ChalkboardXorGameSolution.CanAliceWinByBruteForceRecursion(_nums);

    [Benchmark]
    public bool CanAliceWinByMemoizedRecursion() =>
        ChalkboardXorGameSolution.CanAliceWinByMemoizedRecursion(_nums);

    [Benchmark]
    public bool CanAliceWinByXorParityFormula() =>
        ChalkboardXorGameSolution.CanAliceWinByXorParityFormula(_nums);
}
