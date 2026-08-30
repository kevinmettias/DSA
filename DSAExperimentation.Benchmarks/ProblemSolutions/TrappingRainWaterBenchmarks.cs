using BenchmarkDotNet.Attributes;
using RainStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Trapping Rain Water (LC 42): the textbook O(n^2) per-bar left/right rescan
// vs. the O(n) single sweep using this repo's own Stack<int> as a monotonic
// stack of candidate wall indices.
[MemoryDiagnoser]
public class TrappingRainWaterBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private int[] _heights = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _heights = Enumerable.Range(0, Length).Select(_ => random.Next(0, 1_000)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce()
    {
        var water = 0;

        for (var i = 0; i < _heights.Length; i++)
        {
            var leftMax = 0;
            for (var l = 0; l <= i; l++)
            {
                leftMax = Math.Max(leftMax, _heights[l]);
            }

            var rightMax = 0;
            for (var r = i; r < _heights.Length; r++)
            {
                rightMax = Math.Max(rightMax, _heights[r]);
            }

            water += Math.Min(leftMax, rightMax) - _heights[i];
        }

        return water;
    }

    [Benchmark]
    public int MonotonicStack()
    {
        var indices = new RainStack();
        var water = 0;

        for (var i = 0; i < _heights.Length; i++)
        {
            while (indices.TryPeek(out var top) && _heights[top] < _heights[i])
            {
                indices.TryPop(out _);

                if (!indices.TryPeek(out var left))
                {
                    break;
                }

                var width = i - left - 1;
                var boundedHeight = Math.Min(_heights[left], _heights[i]) - _heights[top];
                water += width * boundedHeight;
            }

            indices.Push(i);
        }

        return water;
    }
}
