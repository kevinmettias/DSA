using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Spiral Matrix (LC 54): a visited-grid simulation (turn clockwise when the next
// cell is out of bounds or already visited, O(rows*cols) extra memory for the
// visited flags) vs. the four-boundary-pointer shrink SpiralMatrixTests uses
// (O(1) extra memory, no visited tracking at all). No repo primitive applies to
// either shape - GridChildren/GridTopology model unordered orthogonal adjacency
// for graph walks, not a fixed clockwise visiting order, so forcing this through
// Grid/** would not be a genuine fit.
[MemoryDiagnoser]
public class SpiralMatrixBenchmarks
{
    private static readonly (int DRow, int DCol)[] Directions = [(0, 1), (1, 0), (0, -1), (-1, 0)];

    [Params(20, 100)]
    public int Size;

    private int[][] _matrix = null!;

    [GlobalSetup]
    public void Setup()
    {
        var value = 0;
        _matrix = Enumerable.Range(0, Size)
            .Select(_ => Enumerable.Range(0, Size).Select(_ => value++).ToArray())
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int VisitedGridWalk()
    {
        var rows = _matrix.Length;
        var cols = _matrix[0].Length;
        var visited = new bool[rows, cols];
        var row = 0;
        var col = 0;
        var direction = 0;
        var last = 0;

        for (var i = 0; i < rows * cols; i++)
        {
            visited[row, col] = true;
            last = _matrix[row][col];

            var (dRow, dCol) = Directions[direction];
            var nextRow = row + dRow;
            var nextCol = col + dCol;

            if (nextRow < 0 || nextRow >= rows || nextCol < 0 || nextCol >= cols || visited[nextRow, nextCol])
            {
                direction = (direction + 1) % Directions.Length;
                (dRow, dCol) = Directions[direction];
                nextRow = row + dRow;
                nextCol = col + dCol;
            }

            row = nextRow;
            col = nextCol;
        }

        return last;
    }

    [Benchmark]
    public int BoundaryPointerShrink()
    {
        var top = 0;
        var bottom = _matrix.Length - 1;
        var left = 0;
        var right = _matrix[0].Length - 1;
        var last = 0;

        while (top <= bottom && left <= right)
        {
            for (var c = left; c <= right; c++)
            {
                last = _matrix[top][c];
            }

            top++;

            for (var r = top; r <= bottom; r++)
            {
                last = _matrix[r][right];
            }

            right--;

            if (top <= bottom)
            {
                for (var c = right; c >= left; c--)
                {
                    last = _matrix[bottom][c];
                }

                bottom--;
            }

            if (left <= right)
            {
                for (var r = bottom; r >= top; r--)
                {
                    last = _matrix[r][left];
                }

                left++;
            }
        }

        return last;
    }
}
