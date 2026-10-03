using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.StoneGameII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are StoneGameIISolution's, the same methods
// StoneGameIISolutionTests proves correct. UnmemoizedRecursion is plain minimax over
// (index, M) - exponential, since the same state recurs through many different
// pick-sequences reaching it - against this repo's own Memoizer<TState,TResult>
// caching that exact pair, the identical shape StoneGameBenchmarks/
// MinimumScoreTriangulationOfPolygonBenchmarks already use.
//
// Sizes are per arm. The un-memoized baseline's blowup is real (each state can branch
// into up to 2*M sub-states), so it stops at 14 piles; the memoized arm's polynomial
// state space runs on to LC 1140's own bound of 100. The two are compared at the sizes
// both run.
//
// [GlobalSetup] now builds the piles array - LeetCode's own input - rather than the
// derived suffix sums, which each strategy computes for itself. That precompute is
// one O(n) pass, identical in both arms, so what the comparison isolates is still
// memoization alone.
public class StoneGameIIBenchmarks
{
    // LC problem number, used as the deterministic seed for pile generation.
    private const int RandomSeed = 1140;

    // Exclusive upper bound for a pile's stone count.
    private const int MaxPileSize = 100;

    private Dictionary<int, int[]> _pilesBySize = [];

    public static IEnumerable<int> BaselineSizes => [10, 14];

    public static IEnumerable<int> MemoizedSizes => [.. BaselineSizes, 40, 100];

    // Every size any arm runs is built here, outside the timed region; an arm looks its own up.
    [GlobalSetup]
    public void Setup() =>
        _pilesBySize = MemoizedSizes.ToDictionary(
            pileCount => pileCount,
            pileCount => SeededDraws.Values(pileCount, 1, MaxPileSize, new Random(RandomSeed)));

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BaselineSizes))]
    public int UnmemoizedRecursion(int pileCount) =>
        StoneGameIISolution.MaxStonesByUnmemoizedRecursion(_pilesBySize[pileCount]);

    [Benchmark]
    [ArgumentsSource(nameof(MemoizedSizes))]
    public int MemoizedRecursion(int pileCount) =>
        StoneGameIISolution.MaxStonesByMemoizedRecursion(_pilesBySize[pileCount]);
}
