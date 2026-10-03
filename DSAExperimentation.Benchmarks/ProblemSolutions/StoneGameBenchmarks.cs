using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.StoneGame;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are StoneGameSolution's, the same methods StoneGameSolutionTests
// proves correct. CanAliceWinByUnmemoizedRecursion is plain minimax over (left, right)
// bounds - exponential, since the same sub-range recurs through many different pick
// orders - against this repo's own Memoizer<TState,TResult> caching that exact pair,
// the identical shape PredictTheWinnerBenchmarks uses for its own interval-DP game
// (LC 877 is LC 486's recurrence with a different win condition). PileCount is kept
// modest for the same reason PredictTheWinnerBenchmarks documents: the un-memoized
// baseline's blowup is real. StoneGameWorkloads keeps the total odd, as LC 877 promises.
public class StoneGameBenchmarks
{
    private const int RandomSeed = 877; // LC problem number

    private int[] _piles = [];

    [Params(22, 26)]
    public int PileCount { get; set; }

    [GlobalSetup]
    public void Setup() => _piles = StoneGameWorkloads.BuildPiles(PileCount, RandomSeed);

    [Benchmark(Baseline = true)]
    public bool CanAliceWinByUnmemoizedRecursion() => StoneGameSolution.CanAliceWinByUnmemoizedRecursion(_piles);

    [Benchmark]
    public bool CanAliceWinByMemoizedRecursion() => StoneGameSolution.CanAliceWinByMemoizedRecursion(_piles);
}
