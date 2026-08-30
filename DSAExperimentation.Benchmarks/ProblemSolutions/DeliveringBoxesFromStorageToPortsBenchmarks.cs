using BenchmarkDotNet.Attributes;
using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Delivering Boxes from Storage to Ports (LC 1687): rescanning the valid
// [left, i-1] window from scratch at every position (O(n*maxBoxes)) vs. this
// repo's own Deque<int> as a monotonic increasing-"dp minus switch-prefix"
// index window (O(n), each index pushed and popped at most once) - the same
// technique ConstrainedSubsequenceSumBenchmarks proves for LC 1425, applied
// here to a MINIMUM instead of a maximum, with the window's left edge driven
// by a two-pointer sweep over two constraints (box count and total weight)
// instead of one fixed k.
[MemoryDiagnoser]
public class DeliveringBoxesFromStorageToPortsBenchmarks
{
    private const int MaxBoxes = 50;
    private const int MaxWeight = 150;

    [Params(2_000, 20_000)]
    public int Length;

    private int[] _ports = null!;
    private int[] _switchPrefix = null!;
    private long[] _weightPrefix = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1687);
        _ports = Enumerable.Range(0, Length).Select(_ => random.Next(1, 6)).ToArray();
        var weights = Enumerable.Range(0, Length).Select(_ => random.Next(1, 6)).ToArray();

        _switchPrefix = new int[Length + 1];
        _weightPrefix = new long[Length + 1];

        for (var i = 1; i <= Length; i++)
        {
            _weightPrefix[i] = _weightPrefix[i - 1] + weights[i - 1];
            _switchPrefix[i] = i < 2
                ? 0
                : _switchPrefix[i - 1] + (_ports[i - 2] != _ports[i - 1] ? 1 : 0);
        }
    }

    [Benchmark(Baseline = true)]
    public int RescanWindowEachPosition()
    {
        var n = Length;
        var dp = new int[n + 1];
        var left = 0;

        for (var i = 1; i <= n; i++)
        {
            while (i - left > MaxBoxes || _weightPrefix[i] - _weightPrefix[left] > MaxWeight)
            {
                left++;
            }

            var best = int.MaxValue;
            for (var j = left; j < i; j++)
            {
                best = Math.Min(best, dp[j] - _switchPrefix[j + 1]);
            }

            dp[i] = 2 + _switchPrefix[i] + best;
        }

        return dp[n];
    }

    [Benchmark]
    public int MonotonicDequeDp()
    {
        var n = Length;
        var dp = new int[n + 1];
        var window = new RepoDeque();
        window.PushBack(0);
        var left = 0;

        for (var i = 1; i <= n; i++)
        {
            while (i - left > MaxBoxes || _weightPrefix[i] - _weightPrefix[left] > MaxWeight)
            {
                left++;
            }

            while (window.TryPeekFront(out var frontIndex) && frontIndex < left)
            {
                window.TryPopFront(out _);
            }

            window.TryPeekFront(out var bestIndex);
            dp[i] = 2 + _switchPrefix[i] + (dp[bestIndex] - _switchPrefix[bestIndex + 1]);

            if (i == n)
            {
                continue;
            }

            var candidate = dp[i] - _switchPrefix[i + 1];
            while (window.TryPeekBack(out var backIndex) && dp[backIndex] - _switchPrefix[backIndex + 1] >= candidate)
            {
                window.TryPopBack(out _);
            }

            window.PushBack(i);
        }

        return dp[n];
    }
}
