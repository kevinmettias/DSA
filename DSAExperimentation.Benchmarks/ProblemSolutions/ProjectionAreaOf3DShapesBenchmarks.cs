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
    [Params(50, 500)]
    public int Size;

    private int[][] _grid = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _grid = Enumerable.Range(0, Size)
            .Select(_ => Enumerable.Range(0, Size).Select(_ => random.Next(0, 100)).ToArray())
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int ThreeSeparatePasses()
    {
        var rows = _grid.Length;
        var cols = _grid[0].Length;
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

        return top + front + side;
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
            var rowMax = 0;
            for (var c = 0; c < cols; c++)
            {
                var value = _grid[r][c];
                if (value > 0)
                {
                    top++;
                }

                rowMax = Math.Max(rowMax, value);
                colMax[c] = Math.Max(colMax[c], value);
            }

            front += rowMax;
        }

        var side = 0;
        for (var c = 0; c < cols; c++)
        {
            side += colMax[c];
        }

        return top + front + side;
    }
}
