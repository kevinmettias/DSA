using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.StoneGame;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are StoneGameSolution's, the same methods StoneGameTests
// proves correct. UnmemoizedRecursion is plain minimax over (left, right) bounds -
// exponential, since the same sub-range recurs through many different pick orders -
// against this repo's own Memoizer<TState,TResult> caching that exact pair, the
// identical shape PredictTheWinnerBenchmarks uses for its own interval-DP game
// (LC 877 is LC 486's recurrence with a different win condition). N is kept modest
// for the same reason PredictTheWinnerBenchmarks documents: the un-memoized
// baseline's blowup is real.
[MemoryDiagnoser]
public class StoneGameBenchmarks
{
    private const int RandomSeed = 877; // LC problem number
    private const int PileValueUpperBoundExclusive = 100;

    [Params(22, 26)]
    public int N;

    private int[] _piles = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _piles = Enumerable.Range(0, N).Select(_ => random.Next(1, PileValueUpperBoundExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool UnmemoizedRecursion() => StoneGameSolution.AliceWinsByUnmemoizedRecursion(_piles);

    [Benchmark]
    public bool MemoizedRecursion() => StoneGameSolution.AliceWinsByMemoizedRecursion(_piles);
}
