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
    private const int CellValueUpperBoundExclusive = 2;

    [Params(50, 300)]
    public int Size;

    private int[][] _matrix = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _matrix = Enumerable.Range(0, Size)
            .Select(_ => Enumerable.Range(0, Size).Select(_ => random.Next(0, CellValueUpperBoundExclusive)).ToArray())
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
            UpdateHeights(heights, row);
            total += CountRowSubmatricesRunningMin(heights);
        }

        return total;
    }

    private static void UpdateHeights(int[] heights, int[] row)
    {
        for (var col = 0; col < heights.Length; col++)
        {
            heights[col] = row[col] == 1 ? heights[col] + 1 : 0;
        }
    }

    private static int CountRowSubmatricesRunningMin(int[] heights)
    {
        var rowTotal = 0;

        for (var right = 0; right < heights.Length; right++)
        {
            var minHeight = heights[right];
            for (var left = right; left >= 0 && heights[left] != 0; left--)
            {
                minHeight = Math.Min(minHeight, heights[left]);
                rowTotal += minHeight;
            }
        }

        return rowTotal;
    }

    [Benchmark]
    public int MonotonicStackDp()
    {
        var cols = _matrix[0].Length;
        var heights = new int[cols];
        var total = 0;

        foreach (var row in _matrix)
        {
            UpdateHeights(heights, row);
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
