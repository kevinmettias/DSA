using BenchmarkDotNet.Attributes;
using RangeIndexStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Sum of Subarray Ranges (LC 2104): the O(n^2) double loop tracking a running
// min/max per start index vs the O(n) monotonic-stack contribution technique
// (sum of subarray maximums minus sum of subarray minimums), using this repo's own
// Stack<int> over indices - the same primitive LargestRectangleInHistogramTests
// already proved out for a sibling monotonic-stack problem.
[MemoryDiagnoser]
public class SumOfSubarrayRangesBenchmarks
{
    private const int ValueMagnitude = 1_000;

    [Params(200, 2_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(-ValueMagnitude, ValueMagnitude)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long BruteForce()
    {
        var sum = 0L;

        for (var i = 0; i < _values.Length; i++)
        {
            var min = _values[i];
            var max = _values[i];

            for (var j = i; j < _values.Length; j++)
            {
                min = Math.Min(min, _values[j]);
                max = Math.Max(max, _values[j]);
                sum += max - min;
            }
        }

        return sum;
    }

    [Benchmark]
    public long MonotonicStack() => SumOfSubarrayMaximums(_values) - SumOfSubarrayMinimums(_values);

    private static long SumOfSubarrayMaximums(int[] nums) =>
        SumOfSubarrayContribution(nums, int.MaxValue, static (value, current) => value < current);

    private static long SumOfSubarrayMinimums(int[] nums) =>
        SumOfSubarrayContribution(nums, int.MinValue, static (value, current) => value > current);

    // Shared monotonic-stack shape behind SumOfSubarrayMaximums/Minimums: only the
    // "no more elements" sentinel and the pop condition differ between max and min.
    private static long SumOfSubarrayContribution(int[] nums, int sentinel, Func<int, int, bool> shouldPop)
    {
        var sum = 0L;
        var indices = new RangeIndexStack();

        for (var i = 0; i <= nums.Length; i++)
        {
            var current = i == nums.Length ? sentinel : nums[i];

            while (indices.TryPeek(out var top) && shouldPop(nums[top], current))
            {
                indices.TryPop(out _);
                var left = indices.TryPeek(out var previous) ? previous : -1;
                sum += (long)nums[top] * (top - left) * (i - top);
            }

            indices.Push(i);
        }

        return sum;
    }
}
