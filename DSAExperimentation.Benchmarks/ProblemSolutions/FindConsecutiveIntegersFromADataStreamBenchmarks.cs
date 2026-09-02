using BenchmarkDotNet.Attributes;
using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Find Consecutive Integers from a Data Stream (LC 2526): appending every value to
// an unbounded history array and rescanning its last k entries on every call -
// O(n*k) - vs. this repo's own Deque<int> holding a fixed-size window (PushBack on
// arrival, TryPopFront once the window grows past k) with a running match count
// maintained incrementally as values enter/leave - O(n), the same windowing shape
// SlidingWindowMaximumBenchmarks already runs over Deque<int>. `Value` is rare in
// the stream so the window almost never has a chance to short-circuit either arm
// early.
[MemoryDiagnoser]
public class FindConsecutiveIntegersFromADataStreamBenchmarks
{
    private const int Value = 7;
    private const int K = 20;
    private const int ValueBoundExclusive = 1_000;

    [Params(2_000, 20_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(2526); // LC problem number
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(0, ValueBoundExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int UnboundedHistoryRescan()
    {
        var history = new List<int>(Length);
        var trueCount = 0;

        foreach (var num in _values)
        {
            history.Add(num);

            if (Consec(history))
            {
                trueCount++;
            }
        }

        return trueCount;
    }

    [Benchmark]
    public int FixedWindowIncrementalCount()
    {
        var window = new RepoDeque();
        var matchCount = 0;
        var trueCount = 0;

        foreach (var num in _values)
        {
            window.PushBack(num);

            if (num == Value)
            {
                matchCount++;
            }

            if (window.Count > K && window.TryPopFront(out var evicted) && evicted == Value)
            {
                matchCount--;
            }

            if (window.Count == K && matchCount == K)
            {
                trueCount++;
            }
        }

        return trueCount;
    }

    private static bool Consec(List<int> history)
    {
        if (history.Count < K)
        {
            return false;
        }

        for (var i = history.Count - K; i < history.Count; i++)
        {
            if (history[i] != Value)
            {
                return false;
            }
        }

        return true;
    }
}
