using BenchmarkDotNet.Attributes;
using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Jump Game VI (LC 1696): rescanning the trailing k-sized dp window from
// scratch at every position (O(n*k)) vs. this repo's own Deque<int> as a
// monotonic decreasing-dp-value index window (O(n), each index pushed and
// popped at most once) - the identical technique
// ConstrainedSubsequenceSumBenchmarks proves for LC 1425. Values are random
// over a wide signed range so the window's running maximum keeps changing
// instead of settling on one dominant early value that would make the rescan
// look artificially cheap.
[MemoryDiagnoser]
public class JumpGameVIBenchmarks
{
    private const int K = 50;

    [Params(2_000, 20_000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1696);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(-1_000, 1_000)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int RescanWindowEachPosition()
    {
        var dp = new int[_nums.Length];
        dp[0] = _nums[0];

        for (var i = 1; i < _nums.Length; i++)
        {
            var windowMax = int.MinValue;
            for (var j = Math.Max(0, i - K); j < i; j++)
            {
                windowMax = Math.Max(windowMax, dp[j]);
            }

            dp[i] = _nums[i] + windowMax;
        }

        return dp[^1];
    }

    [Benchmark]
    public int MonotonicDequeDp()
    {
        var dp = new int[_nums.Length];
        dp[0] = _nums[0];
        var window = new RepoDeque();
        window.PushBack(0);

        for (var i = 1; i < _nums.Length; i++)
        {
            while (window.TryPeekFront(out var frontIndex) && frontIndex < i - K)
            {
                window.TryPopFront(out _);
            }

            window.TryPeekFront(out var maxIndex);
            dp[i] = _nums[i] + dp[maxIndex];

            while (window.TryPeekBack(out var backIndex) && dp[backIndex] <= dp[i])
            {
                window.TryPopBack(out _);
            }

            window.PushBack(i);
        }

        return dp[^1];
    }
}
