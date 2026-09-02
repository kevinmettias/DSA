using BenchmarkDotNet.Attributes;
using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Sliding Window Maximum (LC 239): rescanning every window from scratch
// (O(n*k)) vs. this repo's own Deque<int> as a monotonic decreasing-value index
// window (O(n), each index pushed/popped at most once). Values are random over
// a wide range so ties/early-exit shortcuts in BruteForceRescan can't make it
// look artificially competitive.
[MemoryDiagnoser]
public class SlidingWindowMaximumBenchmarks
{
    private const int WindowSize = 50;
    private const int ValueBound = 1_000_000;

    [Params(2_000, 20_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(-ValueBound, ValueBound)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long BruteForceRescan()
    {
        long sum = 0;
        for (var i = 0; i <= _values.Length - WindowSize; i++)
        {
            var windowMax = int.MinValue;
            for (var j = i; j < i + WindowSize; j++)
            {
                windowMax = Math.Max(windowMax, _values[j]);
            }

            sum += windowMax;
        }

        return sum;
    }

    [Benchmark]
    public long MonotonicDeque()
    {
        var window = new RepoDeque();
        long sum = 0;

        for (var i = 0; i < _values.Length; i++)
        {
            sum += WindowContributionForIndex(window, i);
        }

        return sum;
    }

    private long WindowContributionForIndex(RepoDeque window, int i)
    {
        while (window.TryPeekBack(out var backIndex) && _values[backIndex] <= _values[i])
        {
            window.TryPopBack(out _);
        }

        window.PushBack(i);

        if (window.TryPeekFront(out var frontIndex) && frontIndex <= i - WindowSize)
        {
            window.TryPopFront(out _);
        }

        if (i >= WindowSize - 1)
        {
            window.TryPeekFront(out var maxIndex);
            return _values[maxIndex];
        }

        return 0;
    }
}
