using DSAExperimentation.LeetCode.ZumaGame;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ZumaGameSolution's, the same methods ZumaGameSolutionTests
// proves correct. _board repeats "RRWW" (no run >=3 initially, per LC488's own
// precondition), so different insertion positions inside the same "WW"/"RR" run
// genuinely collapse to the identical resulting state - exactly the redundancy
// BruteForceDfs keeps re-exploring and QueueBfsDedup's Set<string> skips after the
// first time. BoardRepeats stops at 4, whose 16 balls are LC 488's longest board. That
// count is even, and an even count admits a 2-ball shortcut where 3 repeats need 3
// balls, so the larger board is also the shorter answer: no larger odd count fits the
// bound.
public class ZumaGameBenchmarks
{
    private const string Hand = "WWWWW";
    private const string BoardRepeatUnit = "RRWW";

    private string _board = "";

    [Params(3, 4)]
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
