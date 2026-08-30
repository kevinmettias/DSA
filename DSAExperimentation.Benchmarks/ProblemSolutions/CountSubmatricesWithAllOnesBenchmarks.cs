using BenchmarkDotNet.Attributes;
using RepoStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Count Submatrices With All Ones (LC 1504): the O(rows * cols^2) baseline (per row,
// walk every right boundary column back to every left boundary, tracking the running
// minimum height) vs. the O(rows * cols) reduction to LC 907 (Sum of Subarray Minimums)
// applied once per row's histogram - this repo's own Stack<int> as a previous-smaller-
// height monotonic stack, the same MaximalRectangleBenchmarks/LargestRectangleInHistogramBenchmarks
// precedent, just counting rectangles instead of finding the largest one.
[MemoryDiagnoser]
public class CountSubmatricesWithAllOnesBenchmarks
{
    [Params(50, 300)]
    public int Size;

    private int[][] _matrix = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _matrix = Enumerable.Range(0, Size)
            .Select(_ => Enumerable.Range(0, Size).Select(_ => random.Next(0, 2)).ToArray())
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int RunningMinScan()
    {
        var cols = _matrix[0].Length;
        var heights = new int[cols];
        var total = 0;

        foreach (var row in _matrix)
        {
            for (var col = 0; col < cols; col++)
            {
                heights[col] = row[col] == 1 ? heights[col] + 1 : 0;
            }

            for (var right = 0; right < cols; right++)
            {
                var minHeight = heights[right];
                for (var left = right; left >= 0 && heights[left] != 0; left--)
                {
                    minHeight = Math.Min(minHeight, heights[left]);
                    total += minHeight;
                }
            }
        }

        return total;
    }

    [Benchmark]
    public int MonotonicStackDp()
    {
        var cols = _matrix[0].Length;
        var heights = new int[cols];
        var total = 0;

        foreach (var row in _matrix)
        {
            for (var col = 0; col < cols; col++)
            {
                heights[col] = row[col] == 1 ? heights[col] + 1 : 0;
            }

            total += CountRowSubmatrices(heights);
        }

        return total;
    }

    private static int CountRowSubmatrices(int[] heights)
    {
        var indices = new RepoStack();
        var dp = new int[heights.Length];
        var rowTotal = 0;

        for (var j = 0; j < heights.Length; j++)
        {
            while (indices.TryPeek(out var top) && heights[top] >= heights[j])
            {
                indices.TryPop(out _);
            }

            dp[j] = indices.TryPeek(out var previousSmaller)
                ? dp[previousSmaller] + ((j - previousSmaller) * heights[j])
                : (j + 1) * heights[j];

            indices.Push(j);
            rowTotal += dp[j];
        }

        return rowTotal;
    }
}
