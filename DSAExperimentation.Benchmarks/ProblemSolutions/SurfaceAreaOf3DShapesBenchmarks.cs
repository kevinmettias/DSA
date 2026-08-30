using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Surface Area of 3D Shapes (LC 892): summing top/bottom plus each cell's 4
// exposed side faces against its neighbors (or the height-0 ground outside the
// grid) is inherently O(n^2) either way - there is no better asymptotic algorithm
// available, the same "no complexity split to compare" shape TransposeMatrix
// Benchmarks already documents for this kind of fixed-shape grid arithmetic. So
// this instead compares two real constant-factor strategies: re-deriving each
// neighbor's bounds check inline per direction vs. padding the grid with a
// height-0 border once up front so the inner loop can index all 4 neighbors
// directly with zero per-cell branches.
[MemoryDiagnoser]
public class SurfaceAreaOf3DShapesBenchmarks
{
    [Params(50, 400)]
    public int Size;

    private int[][] _grid = null!;

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
                _grid[r][c] = random.Next(0, 50);
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int BoundsCheckedPerNeighbor()
    {
        var n = _grid.Length;
        var area = 0;

        for (var row = 0; row < n; row++)
        {
            for (var col = 0; col < n; col++)
            {
                var height = _grid[row][col];
                if (height == 0)
                {
                    continue;
                }

                area += 2;
                area += ExposedSide(height, row - 1, col, n);
                area += ExposedSide(height, row + 1, col, n);
                area += ExposedSide(height, row, col - 1, n);
                area += ExposedSide(height, row, col + 1, n);
            }
        }

        return area;
    }

    private int ExposedSide(int height, int row, int col, int n)
    {
        var neighborHeight = row >= 0 && row < n && col >= 0 && col < n ? _grid[row][col] : 0;
        return Math.Max(0, height - neighborHeight);
    }

    [Benchmark]
    public int PaddedGridNoBoundsChecks()
    {
        var n = _grid.Length;
        var padded = new int[n + 2][];

        for (var r = 0; r < n + 2; r++)
        {
            padded[r] = new int[n + 2];
        }

        for (var r = 0; r < n; r++)
        {
            for (var c = 0; c < n; c++)
            {
                padded[r + 1][c + 1] = _grid[r][c];
            }
        }

        var area = 0;

        for (var r = 1; r <= n; r++)
        {
            for (var c = 1; c <= n; c++)
            {
                var height = padded[r][c];
                if (height == 0)
                {
                    continue;
                }

                area += 2;
                area += Math.Max(0, height - padded[r - 1][c]);
                area += Math.Max(0, height - padded[r + 1][c]);
                area += Math.Max(0, height - padded[r][c - 1]);
                area += Math.Max(0, height - padded[r][c + 1]);
            }
        }

        return area;
    }
}
