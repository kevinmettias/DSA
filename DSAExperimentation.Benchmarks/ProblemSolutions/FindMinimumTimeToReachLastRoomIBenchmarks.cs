using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.FindMinimumTimeToReachLastRoomI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindMinimumTimeToReachLastRoomISolution's, the same
// methods FindMinimumTimeToReachLastRoomISolutionTests proves correct
// (MinimumTimeToVisitACellInAGridBenchmarks precedent). WaitCostGridWorkloads builds
// the grid - (0,0) always moveTime 0, LeetCode's own guarantee, and every other cell
// demanding a random wait - so each relaxation goes through ArrivalTime's wait branch
// instead of taking the constant-weight-1 shortcut. LC 3341 caps each side at 50
// rooms, so the larger Size is that cap.
public class FindMinimumTimeToReachLastRoomIBenchmarks
{
    private const int MaxMoveTimeExclusive = 200;
    private const int Seed = 3341;

    private int[][] _moveTime = [];

    [Params(20, 50)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup() => _moveTime = WaitCostGridWorkloads.WithZeroOrigin(Size, MaxMoveTimeExclusive, Seed);

    [Benchmark(Baseline = true)]
    public int BclPriorityQueue() => FindMinimumTimeToReachLastRoomISolution.MinimumTimeByBclPriorityQueue(_moveTime);

    [Benchmark]
    public int Heap() => FindMinimumTimeToReachLastRoomISolution.MinimumTimeByHeap(_moveTime);
}
