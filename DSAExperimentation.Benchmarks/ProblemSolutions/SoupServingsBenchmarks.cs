using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SoupServings;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SoupServingsSolution's, the same methods
// SoupServingsTests proves correct. The two Milliliters values deliberately
// straddle the cliff: at 600 the un-memoized tree is still small (~2x slower
// than memoized in a local dry run), but at 850 it blows up to ~700x slower -
// the real exponential blowup PredictTheWinnerBenchmarks/CanIWinBenchmarks
// document for their own un-memoized game-tree baselines, here made sharper by
// soup's branching factor of 4 (vs. those two problems' factor of 2-3). Both
// sizes sit below the solution's large-amount short circuit, so each arm runs
// its full search.
//
// There is no [GlobalSetup] left to charge: the only preparation the old harness
// did was quantizing Milliliters to servings, one integer division that is part
// of the LeetCode-shaped call itself and identical for both arms.
[MemoryDiagnoser]
public class SoupServingsBenchmarks
{
    [Params(600, 850)]
    public int Milliliters { get; set; }

    [Benchmark(Baseline = true)]
    public double UnmemoizedRecursion() =>
        SoupServingsSolution.ProbabilityByUnmemoizedRecursion(Milliliters);

    [Benchmark]
    public double MemoizedRecursion() =>
        SoupServingsSolution.ProbabilityByMemoizedRecursion(Milliliters);
}
