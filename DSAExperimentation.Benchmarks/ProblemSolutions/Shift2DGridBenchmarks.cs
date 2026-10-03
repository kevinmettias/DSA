using DSAExperimentation.LeetCode.Shift2DGrid;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are Shift2DGridSolution's, the same methods
// Shift2DGridSolutionTests proves correct. Direct row/col index arithmetic (compute each
// source cell's shifted destination and write straight into a fresh array) vs. this
// repo's own Deque<int> - flatten into it, right-rotate it shiftCount mod (rows*cols)
// times via TryPopBack + PushFront, then drain it back out into the grid shape.
//
// LeetCode's own input shape - a jagged grid and a shift count - is already what
// both strategies take, so [GlobalSetup] only decides how large the workload is and
// hands the finished input straight over; there is no construction left for a
// hoisted overload to lift out of the measured methods.
//
// The grid stops at LC 1260's 50 x 50 and the shift is its largest k, 100 - fewer cells
// than even the smaller grid holds, so both strategies do a genuine partial rotation
// rather than a degenerate no-op or full cycle.
public class Shift2DGridBenchmarks
{
    private const int MaxCellValueExclusive = 1_000;
    private const int RandomSeed = 1;

    // LC 1260's largest shift.
    private const int ShiftCount = 100;

    private int[][] _grid = [];

    [Params(20, 50)]
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

    }

    [Benchmark(Baseline = true)]
    public int[][] IndexArithmeticShift() => Shift2DGridSolution.ShiftGridByIndexArithmetic(_grid, ShiftCount);

    [Benchmark]
    public int[][] DequeRotationShift() => Shift2DGridSolution.ShiftGridByDequeRotation(_grid, ShiftCount);
}
