using BenchmarkDotNet.Attributes;
using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Shortest Subarray with Sum at Least K (LC 862): the O(n^2) prefix-sum double
// scan vs. this repo's own Deque<int> holding prefix-sum indices in a monotonic
// increasing window (O(n), each index pushed/popped at most once) - the same
// "Deque<int> as a monotonic index window" composition SlidingWindowMaximum
// Benchmarks already uses, applied to prefix-sum indices instead of raw values.
// K is deliberately unreachable (values are small and bounded, K is far larger
// than any possible subarray sum) so BOTH strategies are forced through their
// full worst-case scan instead of an early exit on the first short answer found,
// the same "_target is deliberately unreachable" shape TwoSumBenchmarks uses.
[MemoryDiagnoser]
public class ShortestSubarrayWithSumAtLeastKBenchmarks
{
    private const int K = 1_000_000;
    private const int ValueLowerBound = -5;
    private const int ValueUpperBoundExclusive = 11;

    [Params(400, 3_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(ValueLowerBound, ValueUpperBoundExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForcePrefixScan()
    {
        var n = _values.Length;
        var prefix = new long[n + 1];
        for (var i = 0; i < n; i++)
        {
            prefix[i + 1] = prefix[i] + _values[i];
        }

        var best = n + 1;
        for (var i = 0; i <= n; i++)
        {
            for (var j = i + 1; j <= n; j++)
            {
                if (prefix[j] - prefix[i] >= K)
                {
                    best = Math.Min(best, j - i);
                    break;
                }
            }
        }

        return best > n ? -1 : best;
    }

    [Benchmark]
    public int MonotonicDequePrefixScan()
    {
        var n = _values.Length;
        var prefix = BuildPrefixSums(n);

        return FindShortestSubarrayViaWindow(prefix, n);
    }

    private long[] BuildPrefixSums(int n)
    {
        var prefix = new long[n + 1];
        for (var i = 0; i < n; i++)
        {
            prefix[i + 1] = prefix[i] + _values[i];
        }

        return prefix;
    }

    private static int FindShortestSubarrayViaWindow(long[] prefix, int n)
    {
        var best = n + 1;
        var window = new RepoDeque();

        for (var i = 0; i <= n; i++)
        {
            while (window.TryPeekFront(out var frontIndex) && prefix[i] - prefix[frontIndex] >= K)
            {
                best = Math.Min(best, i - frontIndex);
                window.TryPopFront(out _);
            }

            while (window.TryPeekBack(out var backIndex) && prefix[backIndex] >= prefix[i])
            {
                window.TryPopBack(out _);
            }

            window.PushBack(i);
        }

        return best > n ? -1 : best;
    }
}
