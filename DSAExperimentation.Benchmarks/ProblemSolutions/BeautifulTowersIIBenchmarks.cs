using BenchmarkDotNet.Attributes;
using MonotonicStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Beautiful Towers II (LC 2866): identical algorithm to Beautiful Towers I (see
// BeautifulTowersIBenchmarks) at [Params] scaled up toward II's much larger
// official n <= 1e5 bound - large enough that the O(n) baseline's O(n^2) cost
// visibly dominates while staying inside a reasonable benchmark run.
[MemoryDiagnoser]
public class BeautifulTowersIIBenchmarks
{
    private const int MaxHeight = 1_000_000_000;

    [Params(1_000, 8_000)]
    public int Length;

    private int[] _maxHeights = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _maxHeights = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxHeight)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long BruteForce() => MaximumSumOfHeightsByBruteForce(_maxHeights);

    [Benchmark]
    public long MonotonicStack() => MaximumSumOfHeightsByMonotonicStack(_maxHeights);

    private static long MaximumSumOfHeightsByBruteForce(int[] maxHeights)
    {
        var n = maxHeights.Length;
        var best = 0L;

        for (var peak = 0; peak < n; peak++)
        {
            var sum = (long)maxHeights[peak];

            var cap = maxHeights[peak];
            for (var j = peak - 1; j >= 0; j--)
            {
                cap = Math.Min(cap, maxHeights[j]);
                sum += cap;
            }

            cap = maxHeights[peak];
            for (var j = peak + 1; j < n; j++)
            {
                cap = Math.Min(cap, maxHeights[j]);
                sum += cap;
            }

            best = Math.Max(best, sum);
        }

        return best;
    }

    private static long MaximumSumOfHeightsByMonotonicStack(int[] maxHeights)
    {
        var left = ComputeLeftSums(maxHeights);
        var right = ComputeRightSums(maxHeights);
        var best = 0L;

        for (var i = 0; i < maxHeights.Length; i++)
        {
            best = Math.Max(best, left[i] + right[i] - maxHeights[i]);
        }

        return best;
    }

    private static long[] ComputeLeftSums(int[] maxHeights)
    {
        var n = maxHeights.Length;
        var sums = new long[n];
        var stack = new MonotonicStack();

        for (var i = 0; i < n; i++)
        {
            while (stack.TryPeek(out var top) && maxHeights[top] > maxHeights[i])
            {
                stack.TryPop(out _);
            }

            sums[i] = stack.TryPeek(out var prev)
                ? sums[prev] + (long)maxHeights[i] * (i - prev)
                : (long)maxHeights[i] * (i + 1);

            stack.Push(i);
        }

        return sums;
    }

    private static long[] ComputeRightSums(int[] maxHeights)
    {
        var n = maxHeights.Length;
        var sums = new long[n];
        var stack = new MonotonicStack();

        for (var i = n - 1; i >= 0; i--)
        {
            while (stack.TryPeek(out var top) && maxHeights[top] > maxHeights[i])
            {
                stack.TryPop(out _);
            }

            sums[i] = stack.TryPeek(out var next)
                ? sums[next] + (long)maxHeights[i] * (next - i)
                : (long)maxHeights[i] * (n - i);

            stack.Push(i);
        }

        return sums;
    }
}
