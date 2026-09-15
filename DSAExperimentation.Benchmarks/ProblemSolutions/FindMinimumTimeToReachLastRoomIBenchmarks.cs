using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindMinimumTimeToReachLastRoomI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindMinimumTimeToReachLastRoomISolution's, the same
// methods FindMinimumTimeToReachLastRoomITests proves correct
// (MinimumTimeToVisitACellInAGridBenchmarks precedent). [GlobalSetup] builds a grid
// whose (0,0) is always moveTime 0 (LeetCode's own guarantee) and whose other cells
// demand a random wait, forcing every relaxation through ArrivalTime's wait branch
// instead of the constant-weight-1 shortcut.
[MemoryDiagnoser]
public class FindMinimumTimeToReachLastRoomIBenchmarks
{
    private const int MaxMoveTimeExclusive = 200;
    private const int Seed = 3341;

    private int[][] _moveTime = [];

    [Params(20, 60)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _moveTime = new int[Size][];

        for (var row = 0; row < Size; row++)
        {
            _moveTime[row] = new int[Size];

            for (var col = 0; col < Size; col++)
            {
                _moveTime[row][col] = row == 0 && col == 0 ? 0 : random.Next(0, MaxMoveTimeExclusive);
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int BclPriorityQueue() => FindMinimumTimeToReachLastRoomISolution.MinimumTimeByBclPriorityQueue(_moveTime);

    [Benchmark]
    public int Heap() => FindMinimumTimeToReachLastRoomISolution.MinimumTimeByHeap(_moveTime);
}
