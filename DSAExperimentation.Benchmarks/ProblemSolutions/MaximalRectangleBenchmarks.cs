using BenchmarkDotNet.Attributes;
using HeightStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximal Rectangle (LC 85): an O(rows^2 * cols) row-pair-window baseline (collapse
// every top/bottom row pair into a per-column "all ones in this vertical strip"
// array, then scan for the widest contiguous run) vs. the O(rows * cols) reduction
// to LC 84 - one histogram-max-rectangle sweep per row using this repo's own
// Stack<int>, the same monotonic-stack routine LargestRectangleInHistogramBenchmarks
// already proves out.
[MemoryDiagnoser]
public class MaximalRectangleBenchmarks
{
    [Params(50, 300)]
    public int Size;

    private char[][] _matrix = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _matrix = Enumerable.Range(0, Size)
            .Select(_ => Enumerable.Range(0, Size).Select(_ => random.Next(0, 2) == 0 ? '0' : '1').ToArray())
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int RowPairScan()
    {
        var rows = _matrix.Length;
        var cols = _matrix[0].Length;
        var maxArea = 0;

        for (var top = 0; top < rows; top++)
        {
            var columnAllOnes = new bool[cols];
            Array.Fill(columnAllOnes, true);

            for (var bottom = top; bottom < rows; bottom++)
            {
                for (var c = 0; c < cols; c++)
                {
                    columnAllOnes[c] &= _matrix[bottom][c] == '1';
                }

                var height = bottom - top + 1;
                var run = 0;

                for (var c = 0; c < cols; c++)
                {
                    run = columnAllOnes[c] ? run + 1 : 0;
                    maxArea = Math.Max(maxArea, run * height);
                }
            }
        }

        return maxArea;
    }

    [Benchmark]
    public int RowHistogramStack()
    {
        var heights = new int[_matrix[0].Length];
        var maxArea = 0;

        foreach (var row in _matrix)
        {
            for (var col = 0; col < row.Length; col++)
            {
                heights[col] = row[col] == '1' ? heights[col] + 1 : 0;
            }

            maxArea = Math.Max(maxArea, LargestRectangleArea(heights));
        }

        return maxArea;
    }

    private static int LargestRectangleArea(int[] heights)
    {
        var indices = new HeightStack();
        var maxArea = 0;

        for (var i = 0; i <= heights.Length; i++)
        {
            var currentHeight = i == heights.Length ? 0 : heights[i];

            while (indices.TryPeek(out var top) && heights[top] >= currentHeight)
            {
                indices.TryPop(out _);
                var height = heights[top];
                var width = indices.TryPeek(out var left) ? i - left - 1 : i;
                maxArea = Math.Max(maxArea, height * width);
            }

            indices.Push(i);
        }

        return maxArea;
    }
}
