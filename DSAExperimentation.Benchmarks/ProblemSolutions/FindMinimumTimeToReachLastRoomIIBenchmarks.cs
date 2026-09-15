using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindMinimumTimeToReachLastRoomII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindMinimumTimeToReachLastRoomIISolution's, the
// same methods FindMinimumTimeToReachLastRoomIITests proves correct
// (TwoSumBenchmarks precedent). [GlobalSetup] builds a grid whose (0,0) stays 0
// (matching every LeetCode example) and whose other cells demand a random wait,
// forcing every relaxation through the max(currentTime, moveTime)-plus-
// alternating-cost path instead of a constant-weight shortcut
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
    public void Setup()
    {
        var random = new Random(Seed);
        _moveTime = new int[Size][];

        for (var row = 0; row < Size; row++)
        {
            _moveTime[row] = new int[Size];

            for (var col = 0; col < Size; col++)
            {
                _moveTime[row][col] = row == 0 && col == 0 ? 0 : random.Next(0, MaxWaitExclusive);
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int BclPriorityQueue() => FindMinimumTimeToReachLastRoomIISolution.MinTimeByBclPriorityQueue(_moveTime);

    [Benchmark]
    public int Heap() => FindMinimumTimeToReachLastRoomIISolution.MinTimeByHeap(_moveTime);
}
