using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.StoneGameV;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are StoneGameVSolution's, the same methods StoneGameVTests
// proves correct. UnmemoizedRecursion is plain interval recursion over (left, right) -
// the same range gets re-entered from many different parent splits (Best(0, 3) is
// reachable both directly and as the kept half of several larger ranges), so the call
// count grows far past the O(n^2) distinct-range count once un-cached (measured ~9.1M
// calls at PileCount=220 vs. only 24,310 distinct (i, j) states) - against this repo's
// own Memoizer<TState,TResult> caching each (Left, Right) range exactly once, the
// identical shape StoneGameIIIBenchmarks already uses for LC 1406, just keyed on a
// range instead of a single index.
[MemoryDiagnoser]
public class StoneGameVBenchmarks
{
    // LC problem number, used as the deterministic seed for stone-value generation.
    private const int RandomSeed = 1563;

    private const int MaxStoneValue = 100;

    [Params(120, 200)]
    public int PileCount;

    private int[] _stoneValue = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _stoneValue = Enumerable.Range(0, PileCount).Select(_ => random.Next(1, MaxStoneValue)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() => StoneGameVSolution.MaxScoreByUnmemoizedRecursion(_stoneValue);

    [Benchmark]
    public int MemoizedRecursion() => StoneGameVSolution.MaxScoreByMemoizedRecursion(_stoneValue);
}
