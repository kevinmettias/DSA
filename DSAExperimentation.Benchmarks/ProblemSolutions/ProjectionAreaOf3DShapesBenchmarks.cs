using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Projection Area of 3D Shapes (LC 883): three independent full-grid passes
// (one for the top view's nonzero count, one per-row pass for front, one
// per-column pass for side) vs. a single combined pass that folds all three
// projections into one loop over rows*cols using a scratch colMax[] array.
// No repo primitive applies to either shape - the same precedent Spiral
// Matrix/Spiral Matrix II already establish - so this isolates the
// redundant-pass overhead rather than a genuine algorithm-class swap.
[MemoryDiagnoser]
public class ProjectionAreaOf3DShapesBenchmarks
{
    private const int CellHeightBound = 100;

    [Params(50, 500)]
    public int Size;

    private int[][] _grid = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _grid = Enumerable.Range(0, Size)
            .Select(_ => Enumerable.Range(0, Size).Select(_ => random.Next(0, CellHeightBound)).ToArray())
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int ThreeSeparatePasses()
    {
        var rows = _grid.Length;
        var cols = _grid[0].Length;

        var top = CountTopView(rows, cols);
        var front = SumFrontView(rows, cols);
        var side = SumSideView(rows, cols);

        return top + front + side;
    }

    private int CountTopView(int rows, int cols)
    {
        var top = 0;

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (_grid[r][c] > 0)
                {
                    top++;
                }
            }
        }

        return top;
    }

    private int SumFrontView(int rows, int cols)
    {
        var front = 0;

        for (var r = 0; r < rows; r++)
        {
            var rowMax = 0;
            for (var c = 0; c < cols; c++)
            {
                rowMax = Math.Max(rowMax, _grid[r][c]);
            }

            front += rowMax;
        }

        return front;
    }

    private int SumSideView(int rows, int cols)
    {
        var side = 0;

        for (var c = 0; c < cols; c++)
        {
            var colMax = 0;
            for (var r = 0; r < rows; r++)
            {
                colMax = Math.Max(colMax, _grid[r][c]);
            }

            side += colMax;
        }

        return side;
    }

    [Benchmark]
    public int SingleCombinedPass()
    {
        var rows = _grid.Length;
        var cols = _grid[0].Length;
        var top = 0;
        var front = 0;
        var colMax = new int[cols];

        for (var r = 0; r < rows; r++)
        {
            var rowState = ProcessRow(r, cols, colMax, top);
            top = rowState.Top;
            front += rowState.RowMax;
        }

        var side = 0;
        for (var c = 0; c < cols; c++)
        {
            side += colMax[c];
        }

        return top + front + side;
    }

    private RowScanState ProcessRow(int r, int cols, int[] colMax, int top)
    {
        var rowState = new RowScanState(top, 0);

        for (var c = 0; c < cols; c++)
        {
            rowState = ProcessCell(r, c, colMax, rowState);
        }

        return rowState;
    }

    private RowScanState ProcessCell(int r, int c, int[] colMax, RowScanState state)
    {
        var value = _grid[r][c];
        var top = state.Top + (value > 0 ? 1 : 0);
        var rowMax = Math.Max(state.RowMax, value);
        colMax[c] = Math.Max(colMax[c], value);

        return new RowScanState(top, rowMax);
    }

    private readonly record struct RowScanState(int Top, int RowMax);
}
