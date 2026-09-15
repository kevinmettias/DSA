using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.StoneGameII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are StoneGameIISolution's, the same methods
// StoneGameIITests proves correct. UnmemoizedRecursion is plain minimax over
// (index, M) - exponential, since the same state recurs through many different
// pick-sequences reaching it - against this repo's own Memoizer<TState,TResult>
// caching that exact pair, the identical shape StoneGameBenchmarks/
// MinimumScoreTriangulationOfPolygonBenchmarks already use. PileCount is kept modest
// for the same reason those other interval/window-DP benchmarks' input sizes are:
// the un-memoized baseline's blowup is real (each state can branch into up to 2*M
// sub-states).
//
// [GlobalSetup] now builds the piles array - LeetCode's own input - rather than the
// derived suffix sums, which each strategy computes for itself. That precompute is
// one O(n) pass, identical in both arms, so what the comparison isolates is still
// memoization alone.
[MemoryDiagnoser]
public class StoneGameIIBenchmarks
{
    // LC problem number, used as the deterministic seed for pile generation.
    private const int RandomSeed = 1140;

    // Exclusive upper bound for a pile's stone count.
    private const int MaxPileSize = 100;

    private int[] _piles = [];

    [Params(10, 14)]
    public int PileCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _piles = Enumerable.Range(0, PileCount).Select(_ => random.Next(1, MaxPileSize)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() => StoneGameIISolution.MaxStonesByUnmemoizedRecursion(_piles);

    [Benchmark]
    public int MemoizedRecursion() => StoneGameIISolution.MaxStonesByMemoizedRecursion(_piles);
}
