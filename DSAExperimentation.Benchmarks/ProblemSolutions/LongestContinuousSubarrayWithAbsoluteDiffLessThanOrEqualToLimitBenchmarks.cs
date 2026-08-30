using BenchmarkDotNet.Attributes;
using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Longest Continuous Subarray With Absolute Diff Less Than or Equal to Limit (LC 1438):
// the canonical O(n^2) baseline (every starting index expands its own window from
// scratch, tracking a running max/min until the diff exceeds Limit) vs. this repo's
// own Deque<int>, doubled into one decreasing-value (running max) and one
// increasing-value (running min) monotonic index window over a single expanding/
// shrinking range - the same SlidingWindowMaximumBenchmarks precedent, extended to
// track both ends of the window at once so each index is still pushed/popped from
// each deque at most once for O(n) total. Values are random over a narrow range
// relative to Limit so windows run long enough for the baseline's O(n^2) cost to
// actually show, rather than every start immediately violating the limit.
[MemoryDiagnoser]
public class LongestContinuousSubarrayWithAbsoluteDiffLessThanOrEqualToLimitBenchmarks
{
    private const int Limit = 100;

    [Params(500, 4_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(0, 2_000)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForceAllStartingPoints()
    {
        var best = 0;

        for (var start = 0; start < _values.Length; start++)
        {
            var windowMax = int.MinValue;
            var windowMin = int.MaxValue;

            for (var end = start; end < _values.Length; end++)
            {
                windowMax = Math.Max(windowMax, _values[end]);
                windowMin = Math.Min(windowMin, _values[end]);

                if (windowMax - windowMin > Limit)
                {
                    break;
                }

                best = Math.Max(best, end - start + 1);
            }
        }

        return best;
    }

    [Benchmark]
    public int DoubleMonotonicDeque()
    {
        var maxWindow = new RepoDeque();
        var minWindow = new RepoDeque();
        var left = 0;
        var best = 0;

        for (var right = 0; right < _values.Length; right++)
        {
            while (maxWindow.TryPeekBack(out var maxBack) && _values[maxBack] <= _values[right])
            {
                maxWindow.TryPopBack(out _);
            }

            maxWindow.PushBack(right);

            while (minWindow.TryPeekBack(out var minBack) && _values[minBack] >= _values[right])
            {
                minWindow.TryPopBack(out _);
            }

            minWindow.PushBack(right);

            while (maxWindow.TryPeekFront(out var maxFront) && minWindow.TryPeekFront(out var minFront)
                && _values[maxFront] - _values[minFront] > Limit)
            {
                left++;

                if (maxWindow.TryPeekFront(out var frontIndex) && frontIndex < left)
                {
                    maxWindow.TryPopFront(out _);
                }

                if (minWindow.TryPeekFront(out var frontIndex2) && frontIndex2 < left)
                {
                    minWindow.TryPopFront(out _);
                }
            }

            best = Math.Max(best, right - left + 1);
        }

        return best;
    }
}
