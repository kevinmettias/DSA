using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumNumberOfRobotsWithinBudget;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumNumberOfRobotsWithinBudgetSolution's, the same
// strategies MaximumNumberOfRobotsWithinBudgetTests proves correct. Budget is sized
// to keep the average window a small, roughly constant fraction of Length across
// both Params (mirroring SlidingWindowMaximumBenchmarks' own fixed WindowSize), so
// the baseline's per-left-edge rescan cost stays real instead of collapsing to O(1)
// via an always-tiny window.
[MemoryDiagnoser]
public class MaximumNumberOfRobotsWithinBudgetBenchmarks
{
    private const int ChargeTimeBoundExclusive = 50;
    private const int RunningCostBoundExclusive = 10;
    private const long Budget = 5_000;

    private int[] _chargeTimes = [];

    private int[] _runningCosts = [];
    [Params(500, 4_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _chargeTimes = Enumerable.Range(0, Length).Select(_ => random.Next(1, ChargeTimeBoundExclusive)).ToArray();
        _runningCosts = Enumerable.Range(0, Length).Select(_ => random.Next(1, RunningCostBoundExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int RescanEveryLeftEdge() =>
        MaximumNumberOfRobotsWithinBudgetSolution.MaximumRobotsByRescanEveryLeftEdge(
            _chargeTimes, _runningCosts, Budget);

    [Benchmark]
    public int MonotonicDequeSlidingWindow() =>
        MaximumNumberOfRobotsWithinBudgetSolution.MaximumRobotsByMonotonicDeque(
            _chargeTimes, _runningCosts, Budget);
}
