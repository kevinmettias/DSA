using BenchmarkDotNet.Attributes;
using HistogramStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Largest Rectangle in Histogram (LC 84): the O(n^2) per-bar left/right expansion
// baseline vs. the O(n) monotonic-stack sweep using this repo's own Stack<int> -
// the same TrappingRainWaterBenchmarks precedent (Stack<int> of indices), applied
// to histogram area instead of trapped water.
[MemoryDiagnoser]
public class LargestRectangleInHistogramBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private int[] _heights = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _heights = Enumerable.Range(0, Length).Select(_ => random.Next(1, 1_000)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce()
    {
        var maxArea = 0;

        for (var i = 0; i < _heights.Length; i++)
        {
            var minHeight = _heights[i];

            for (var j = i; j < _heights.Length; j++)
            {
                minHeight = Math.Min(minHeight, _heights[j]);
                maxArea = Math.Max(maxArea, minHeight * (j - i + 1));
            }
        }

        return maxArea;
    }

    [Benchmark]
    public int MonotonicStack()
    {
        var indices = new HistogramStack();
        var maxArea = 0;

        for (var i = 0; i <= _heights.Length; i++)
        {
            var currentHeight = i == _heights.Length ? 0 : _heights[i];

            while (indices.TryPeek(out var top) && _heights[top] >= currentHeight)
            {
                indices.TryPop(out _);
                var height = _heights[top];
                var width = indices.TryPeek(out var left) ? i - left - 1 : i;
                maxArea = Math.Max(maxArea, height * width);
            }

            indices.Push(i);
        }

        return maxArea;
    }
}
