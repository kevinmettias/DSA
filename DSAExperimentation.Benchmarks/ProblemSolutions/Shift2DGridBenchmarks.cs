using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures;
using DSAExperimentation.LeetCode.Shift2DGrid;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are Shift2DGridSolution's, the same methods
// Shift2DGridTests proves correct. Direct row/col index arithmetic (compute each
// source cell's shifted destination and write straight into a fresh array) vs. this
// repo's own Deque<int> - flatten into it, right-rotate k mod (rows*cols) times via
// TryPopBack + PushFront, then drain it back out into the grid shape.
//
// LeetCode's own input shape - a jagged grid and a shift count - is already what
// both strategies take, so [GlobalSetup] only decides how large the workload is and
// hands the finished input straight over; there is no construction left for a
// hoisted overload to lift out of the measured methods.
[MemoryDiagnoser]
public class Shift2DGridBenchmarks
{
    private const int MaxCellValueExclusive = 1_000;
    private const int RandomSeed = 1;

    private int[][] _grid = [];

    private int _k;
    [Params(20, 200)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _grid = new int[Size][];

        for (var r = 0; r < Size; r++)
        {
            _grid[r] = new int[Size];

            for (var c = 0; c < Size; c++)
            {
                _grid[r][c] = random.Next(1, MaxCellValueExclusive);
            }
        }

        // Deliberately not a multiple of the grid's cell count, so both strategies
        // do a genuine partial rotation rather than a degenerate no-op/full-cycle.
        _k = (Size * Size / AlgorithmConstants.HalvingFactor) + 1;
    }

    [Benchmark(Baseline = true)]
    public int[][] IndexArithmeticShift() => Shift2DGridSolution.ShiftGridByIndexArithmetic(_grid, _k);

    [Benchmark]
    public int[][] DequeRotationShift() => Shift2DGridSolution.ShiftGridByDequeRotation(_grid, _k);
}
