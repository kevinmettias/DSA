using BenchmarkDotNet.Attributes;

using RepoIndexStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximum Width Ramp (LC 962): the textbook O(n^2) brute force checks every
// (i, j) pair directly vs. the O(n) two-pass approach that builds a
// monotonically-decreasing candidate stack of left-endpoint indices - over this
// repo's own Stack<int> (CarFleet precedent for this repo's own Stack instead of
// the CLR's own System.Collections.Generic.Stack) - then walks right-to-left
// popping every candidate that is <= the current value.
[MemoryDiagnoser]
public class MaximumWidthRampBenchmarks
{
    [Params(500, 5_000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(962);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(0, Length)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce()
    {
        var maxWidth = 0;

        for (var i = 0; i < _nums.Length; i++)
        {
            for (var j = i + 1; j < _nums.Length; j++)
            {
                if (_nums[i] <= _nums[j])
                {
                    maxWidth = Math.Max(maxWidth, j - i);
                }
            }
        }

        return maxWidth;
    }

    [Benchmark]
    public int MonotonicStack()
    {
        var candidates = new RepoIndexStack();

        for (var i = 0; i < _nums.Length; i++)
        {
            if (!candidates.TryPeek(out var topIndex) || _nums[topIndex] > _nums[i])
            {
                candidates.Push(i);
            }
        }

        var maxWidth = 0;

        for (var j = _nums.Length - 1; j >= 0; j--)
        {
            while (candidates.TryPeek(out var topIndex) && _nums[topIndex] <= _nums[j])
            {
                candidates.TryPop(out _);
                maxWidth = Math.Max(maxWidth, j - topIndex);
            }
        }

        return maxWidth;
    }
}
