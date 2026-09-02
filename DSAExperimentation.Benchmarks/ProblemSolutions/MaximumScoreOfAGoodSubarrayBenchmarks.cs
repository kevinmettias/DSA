using BenchmarkDotNet.Attributes;
using RepoIntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximum Score of a Good Subarray (LC 1793): the O(n^2) baseline re-scans, from
// scratch, every left/right window anchored at k (the "just try every subarray"
// first-pass approach) against this repo's own Stack<int> used as a monotonic-
// increasing sweep (DailyTemperatures/CarFleetII precedent) to compute each index's
// nearest-smaller boundaries in a single O(n) pass each direction.
[MemoryDiagnoser]
public class MaximumScoreOfAGoodSubarrayBenchmarks
{
    private const int RandomSeed = 3;
    private const int MaxNumValue = 20_000;
    private const int MidpointDivisor = 2;

    [Params(200, 5_000)]
    public int Length;

    private int[] _nums = null!;
    private int _k;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxNumValue)).ToArray();
        _k = Length / MidpointDivisor;
    }

    [Benchmark(Baseline = true)]
    public int BruteForceExpand()
    {
        var nums = _nums;
        var k = _k;
        var best = 0;

        for (var left = 0; left <= k; left++)
        {
            var min = int.MaxValue;

            for (var i = left; i <= k; i++)
            {
                min = Math.Min(min, nums[i]);
            }

            for (var right = k; right < nums.Length; right++)
            {
                min = Math.Min(min, nums[right]);
                best = Math.Max(best, min * (right - left + 1));
            }
        }

        return best;
    }

    [Benchmark]
    public int MonotonicStackBoundaries()
    {
        var nums = _nums;
        var previousSmaller = BoundaryIndices(nums, left: true);
        var nextSmaller = BoundaryIndices(nums, left: false);

        var best = 0;

        for (var i = 0; i < nums.Length; i++)
        {
            if (previousSmaller[i] < _k && _k < nextSmaller[i])
            {
                var width = nextSmaller[i] - previousSmaller[i] - 1;
                best = Math.Max(best, nums[i] * width);
            }
        }

        return best;
    }

    private static int[] BoundaryIndices(int[] nums, bool left)
    {
        var n = nums.Length;
        var result = new int[n];
        var stack = new RepoIntStack();
        var outOfRange = left ? -1 : n;

        for (var step = 0; step < n; step++)
        {
            var i = left ? step : n - 1 - step;

            while (stack.TryPeek(out var top) && nums[top] >= nums[i])
            {
                stack.TryPop(out _);
            }

            result[i] = stack.TryPeek(out var boundary) ? boundary : outOfRange;
            stack.Push(i);
        }

        return result;
    }
}
