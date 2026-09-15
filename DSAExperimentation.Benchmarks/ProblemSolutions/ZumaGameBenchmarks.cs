using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ZumaGame;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ZumaGameSolution's, the same methods ZumaGameTests
// proves correct. _board repeats "RRWW" (no run >=3 initially, per LC488's own
// precondition), so different insertion positions inside the same "WW"/"RR" run
// genuinely collapse to the identical resulting state - exactly the redundancy
// BruteForceDfs keeps re-exploring and QueueBfsDedup's Set<string> skips after the
// first time. BoardRepeats is kept odd at both sizes (this pattern happens to admit
// a same-move-count shortcut solution whenever the repeat count is even) so both
// sizes need the same 3-ball solution and the naive baseline's growth reflects board
// size, not a smaller answer getting lucky.
[MemoryDiagnoser]
public class ZumaGameBenchmarks
{
    private const string Hand = "WWWWW";
    private const string BoardRepeatUnit = "RRWW";

    private string _board = "";

    [Params(3, 9)]
    public int BoardRepeats { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var repeatedSegments = Enumerable.Repeat(BoardRepeatUnit, BoardRepeats);
        _board = string.Concat(repeatedSegments);
    }

    [Benchmark(Baseline = true)]
    public int BruteForceDfs() => ZumaGameSolution.FindMinStepByBruteForceDfs(new BallBoard(_board), new BallHand(Hand));

    [Benchmark]
    public int QueueBfsDedup() => ZumaGameSolution.FindMinStepByQueueBfsDedup(new BallBoard(_board), new BallHand(Hand));
}
