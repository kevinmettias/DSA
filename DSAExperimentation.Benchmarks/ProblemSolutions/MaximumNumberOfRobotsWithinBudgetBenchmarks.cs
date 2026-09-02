using BenchmarkDotNet.Attributes;
using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximum Number of Robots Within Budget (LC 2398): re-deriving the window max and
// sum from scratch at every left edge (O(n) per left, since runningCosts are
// positive and cost is strictly increasing in window size so each left can break
// out early once over budget) vs. this repo's own Deque<int> as a monotonic
// decreasing-chargeTimes index window (SlidingWindowMaximumBenchmarks' precedent),
// paired with a running runningCosts sum maintained as the window slides instead of
// resummed. Budget is sized to keep the average window a small, roughly constant
// fraction of Length across both Params (mirroring SlidingWindowMaximumBenchmarks'
// own fixed WindowSize), so the per-left rescan cost stays real instead of
// collapsing to O(1) via an always-tiny window.
[MemoryDiagnoser]
public class MaximumNumberOfRobotsWithinBudgetBenchmarks
{
    private const int ChargeTimeBoundExclusive = 50;
    private const int RunningCostBoundExclusive = 10;
    private const long Budget = 5_000;

    [Params(500, 4_000)]
    public int Length;

    private int[] _chargeTimes = null!;
    private int[] _runningCosts = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _chargeTimes = Enumerable.Range(0, Length).Select(_ => random.Next(1, ChargeTimeBoundExclusive)).ToArray();
        _runningCosts = Enumerable.Range(0, Length).Select(_ => random.Next(1, RunningCostBoundExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int RescanEveryLeftEdge()
    {
        var best = 0;

        for (var left = 0; left < _chargeTimes.Length; left++)
        {
            best = Math.Max(best, LongestBudgetedWindowFrom(left));
        }

        return best;
    }

    private int LongestBudgetedWindowFrom(int left)
    {
        var maxCharge = 0;
        long runningCostSum = 0;

        for (var right = left; right < _chargeTimes.Length; right++)
        {
            maxCharge = Math.Max(maxCharge, _chargeTimes[right]);
            runningCostSum += _runningCosts[right];

            if (maxCharge + ((long)(right - left + 1) * runningCostSum) > Budget)
            {
                return right - left;
            }
        }

        return _chargeTimes.Length - left;
    }

    [Benchmark]
    public int MonotonicDequeSlidingWindow()
    {
        var maxWindow = new RepoDeque();
        var best = 0;
        long runningCostSum = 0;
        var left = 0;

        for (var right = 0; right < _chargeTimes.Length; right++)
        {
            while (maxWindow.TryPeekBack(out var backIndex) && _chargeTimes[backIndex] <= _chargeTimes[right])
            {
                maxWindow.TryPopBack(out _);
            }

            maxWindow.PushBack(right);
            runningCostSum += _runningCosts[right];

            while (maxWindow.TryPeekFront(out var maxIndex)
                && _chargeTimes[maxIndex] + ((long)(right - left + 1) * runningCostSum) > Budget)
            {
                if (maxIndex == left)
                {
                    maxWindow.TryPopFront(out _);
                }

                runningCostSum -= _runningCosts[left];
                left++;
            }

            best = Math.Max(best, right - left + 1);
        }

        return best;
    }
}
