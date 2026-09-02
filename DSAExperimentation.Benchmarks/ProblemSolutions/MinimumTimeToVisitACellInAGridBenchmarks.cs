using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumTimeToVisitACellInAGrid;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumTimeToVisitACellInAGridSolution's, the same
// methods MinimumTimeToVisitACellInAGridTests proves correct (TwoSumBenchmarks
// precedent). [GlobalSetup] builds a grid whose (0,0) is always reachable (row 0 and
// column 0 both count up from 0, so grid[0][1] <= 1 always holds) and whose other
// cells demand a random, sometimes-large wait, forcing every relaxation through
// ArrivalTime's wait-and-parity logic instead of the constant-weight-1 shortcut.
[MemoryDiagnoser]
public class MinimumTimeToVisitACellInAGridBenchmarks
{
    private const int MaxWaitExclusive = 50;
    private const int Seed = 2577;

    [Params(20, 60)]
    public int Size;

    private int[][] _grid = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _grid = new int[Size][];

        for (var row = 0; row < Size; row++)
        {
            _grid[row] = new int[Size];

            for (var col = 0; col < Size; col++)
            {
                _grid[row][col] = row == 0 && col == 0 ? 0 : random.Next(0, MaxWaitExclusive);
            }
        }

        _grid[0][1] = 0;
    }

    [Benchmark(Baseline = true)]
    public int BclPriorityQueue() => MinimumTimeToVisitACellInAGridSolution.MinimumTimeByBclPriorityQueue(_grid);

    [Benchmark]
    public int Heap() => MinimumTimeToVisitACellInAGridSolution.MinimumTimeByHeap(_grid);
}
