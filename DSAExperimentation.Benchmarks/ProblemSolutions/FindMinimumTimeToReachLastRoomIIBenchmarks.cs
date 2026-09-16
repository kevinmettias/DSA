using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.FindMinimumTimeToReachLastRoomII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindMinimumTimeToReachLastRoomIISolution's, the
// same methods FindMinimumTimeToReachLastRoomIITests proves correct
// (TwoSumBenchmarks precedent). WaitCostGridWorkloads builds the grid - (0,0)
// staying 0, matching every LeetCode example, and every other cell demanding a
// random wait - so each relaxation goes through the max(currentTime, moveTime)-plus-
// alternating-cost path instead of taking a constant-weight shortcut
// (MinimumTimeToVisitACellInAGridBenchmarks precedent).
[MemoryDiagnoser]
public class FindMinimumTimeToReachLastRoomIIBenchmarks
{
    private const int MaxWaitExclusive = 200;
    private const int Seed = 3342;

    private int[][] _moveTime = [];

    [Params(20, 60)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup() => _moveTime = WaitCostGridWorkloads.WithZeroOrigin(Size, MaxWaitExclusive, Seed);

    [Benchmark(Baseline = true)]
    public int BclPriorityQueue() => FindMinimumTimeToReachLastRoomIISolution.MinTimeByBclPriorityQueue(_moveTime);

    [Benchmark]
    public int Heap() => FindMinimumTimeToReachLastRoomIISolution.MinTimeByHeap(_moveTime);
}
