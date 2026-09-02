using BenchmarkDotNet.Attributes;
using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Shift 2D Grid (LC 1260): direct row/col index arithmetic (compute each source
// cell's shifted (row,col) destination and write straight into a fresh array) vs.
// this repo's own Deque<int> - flatten into it, right-rotate k mod (rows*cols)
// times via TryPopBack + PushFront, then drain it back out into the grid shape.
[MemoryDiagnoser]
public class Shift2DGridBenchmarks
{
    private const int MaxCellValueExclusive = 1_000;
    private const int HalfDivisor = 2;

    [Params(20, 200)]
    public int Size;

    private int[][] _grid = null!;
    private int _k;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
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
        _k = (Size * Size / HalfDivisor) + 1;
    }

    [Benchmark(Baseline = true)]
    public int[][] IndexArithmeticShift()
    {
        var rows = _grid.Length;
        var cols = _grid[0].Length;
        var total = rows * cols;
        var result = new int[rows][];

        for (var r = 0; r < rows; r++)
        {
            result[r] = new int[cols];
        }

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                var flatIndex = (r * cols + c + _k) % total;
                result[flatIndex / cols][flatIndex % cols] = _grid[r][c];
            }
        }

        return result;
    }

    [Benchmark]
    public int[][] DequeRotationShift()
    {
        var rows = _grid.Length;
        var cols = _grid[0].Length;
        var total = rows * cols;

        var deque = new RepoDeque();
        FlattenIntoDeque(deque);

        var shifts = _k % total;
        RotateRight(deque, shifts);

        return DrainIntoGrid(deque, rows, cols);
    }

    private void FlattenIntoDeque(RepoDeque deque)
    {
        foreach (var row in _grid)
        {
            foreach (var value in row)
            {
                deque.PushBack(value);
            }
        }
    }

    private static void RotateRight(RepoDeque deque, int shifts)
    {
        for (var i = 0; i < shifts; i++)
        {
            deque.TryPopBack(out var last);
            deque.PushFront(last);
        }
    }

    private static int[][] DrainIntoGrid(RepoDeque deque, int rows, int cols)
    {
        var result = new int[rows][];

        for (var r = 0; r < rows; r++)
        {
            result[r] = new int[cols];

            for (var c = 0; c < cols; c++)
            {
                deque.TryPopFront(out result[r][c]);
            }
        }

        return result;
    }
}
