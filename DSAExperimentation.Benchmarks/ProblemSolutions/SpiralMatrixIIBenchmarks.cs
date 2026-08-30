using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Spiral Matrix II (LC 59): a direction-vector walk that re-derives "have I been
// here" via this repo's own Set<(int,int)> on every step vs. the boundary-
// shrinking one-pass fill (four fixed index-arithmetic loops per layer, nothing to
// look up). Both visit exactly n^2 cells - the gap is per-cell overhead, not
// algorithm class.
[MemoryDiagnoser]
public class SpiralMatrixIIBenchmarks
{
    [Params(10, 100)]
    public int N;

    [Benchmark(Baseline = true)]
    public int[][] DirectionVectorWithVisitedSet()
    {
        var n = N;
        var matrix = Enumerable.Range(0, n).Select(_ => new int[n]).ToArray();
        var visited = new Set<(int Row, int Col)>();
        int[] deltaRow = [0, 1, 0, -1];
        int[] deltaCol = [1, 0, -1, 0];
        var row = 0;
        var col = 0;
        var direction = 0;

        for (var value = 1; value <= n * n; value++)
        {
            matrix[row][col] = value;
            visited.TryAdd((row, col));

            var nextRow = row + deltaRow[direction];
            var nextCol = col + deltaCol[direction];

            if (nextRow < 0 || nextRow >= n || nextCol < 0 || nextCol >= n || visited.Has((nextRow, nextCol)))
            {
                direction = (direction + 1) % 4;
                nextRow = row + deltaRow[direction];
                nextCol = col + deltaCol[direction];
            }

            row = nextRow;
            col = nextCol;
        }

        return matrix;
    }

    [Benchmark]
    public int[][] BoundaryShrinking()
    {
        var n = N;
        var matrix = Enumerable.Range(0, n).Select(_ => new int[n]).ToArray();
        var value = 1;
        var top = 0;
        var bottom = n - 1;
        var left = 0;
        var right = n - 1;

        while (top <= bottom && left <= right)
        {
            for (var c = left; c <= right; c++)
            {
                matrix[top][c] = value++;
            }
            top++;

            for (var r = top; r <= bottom; r++)
            {
                matrix[r][right] = value++;
            }
            right--;

            for (var c = right; c >= left && top <= bottom; c--)
            {
                matrix[bottom][c] = value++;
            }
            bottom--;

            for (var r = bottom; r >= top && left <= right; r--)
            {
                matrix[r][left] = value++;
            }
            left++;
        }

        return matrix;
    }
}
