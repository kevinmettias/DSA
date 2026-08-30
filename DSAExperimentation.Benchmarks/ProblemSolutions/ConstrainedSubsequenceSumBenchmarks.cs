using BenchmarkDotNet.Attributes;
using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Constrained Subsequence Sum (LC 1425): rescanning the trailing k-sized dp
// window from scratch at every position (O(n*k)) vs. this repo's own Deque<int>
// as a monotonic decreasing-dp-value index window (O(n), each index pushed and
// popped at most once) - the same technique SlidingWindowMaximumBenchmarks
// already proves, applied here to a dp array computed on the fly instead of a
// fixed input. Values are random over a wide signed range so the window's
// running maximum keeps changing instead of settling on one dominant early
// value that would make the rescan look artificially cheap.
[MemoryDiagnoser]
public class ConstrainedSubsequenceSumBenchmarks
{
    private const int K = 50;

    [Params(2_000, 20_000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1425);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(-1_000, 1_000)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int RescanWindowEachPosition()
    {
        var dp = new int[_nums.Length];
        var best = int.MinValue;

        for (var i = 0; i < _nums.Length; i++)
        {
            var windowMax = 0;
            for (var j = Math.Max(0, i - K); j < i; j++)
            {
                windowMax = Math.Max(windowMax, dp[j]);
            }

            dp[i] = _nums[i] + windowMax;
            best = Math.Max(best, dp[i]);
        }

        return best;
    }

    [Benchmark]
    public int MonotonicDequeDp()
    {
        var dp = new int[_nums.Length];
        var window = new RepoDeque();
        var best = int.MinValue;

        for (var i = 0; i < _nums.Length; i++)
        {
            if (window.TryPeekFront(out var frontIndex) && frontIndex < i - K)
            {
                window.TryPopFront(out _);
            }

            var windowMax = 0;
            if (window.TryPeekFront(out var maxIndex))
            {
                windowMax = Math.Max(0, dp[maxIndex]);
            }

            dp[i] = _nums[i] + windowMax;
            best = Math.Max(best, dp[i]);

            while (window.TryPeekBack(out var backIndex) && dp[backIndex] <= dp[i])
            {
                window.TryPopBack(out _);
            }

            window.PushBack(i);
        }

        return best;
    }
}
