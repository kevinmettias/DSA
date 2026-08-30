using BenchmarkDotNet.Attributes;
using OneThreeTwoStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// 132 Pattern (LC 456): the O(n^2) brute force (track the running prefix
// minimum as the "1" candidate, then rescan everything to its right for a "2,3"
// pair) vs. the O(n) right-to-left monotonic-stack sweep using this repo's own
// Stack<int> - the same LargestRectangleInHistogramBenchmarks/
// TrappingRainWaterBenchmarks precedent. _nums is strictly increasing, which
// contains no 132 pattern at all, forcing both strategies through their full
// worst-case scan instead of an early exit on the first triple.
[MemoryDiagnoser]
public class OneThreeTwoPatternBenchmarks
{
    [Params(200, 3_000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup() => _nums = Enumerable.Range(0, Length).ToArray();

    [Benchmark(Baseline = true)]
    public bool BruteForceMinPrefixScan()
    {
        var minLeft = _nums[0];

        for (var j = 1; j < _nums.Length; j++)
        {
            for (var k = j + 1; k < _nums.Length; k++)
            {
                if (minLeft < _nums[k] && _nums[k] < _nums[j])
                {
                    return true;
                }
            }

            minLeft = Math.Min(minLeft, _nums[j]);
        }

        return false;
    }

    [Benchmark]
    public bool MonotonicStack()
    {
        var stack = new OneThreeTwoStack();
        var third = int.MinValue;

        for (var i = _nums.Length - 1; i >= 0; i--)
        {
            if (_nums[i] < third)
            {
                return true;
            }

            while (stack.TryPeek(out var top) && top < _nums[i])
            {
                third = top;
                stack.TryPop(out _);
            }

            stack.Push(_nums[i]);
        }

        return false;
    }
}
