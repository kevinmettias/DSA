using BenchmarkDotNet.Attributes;
using RepoStack = DSAExperimentation.DataStructures.Stack.Stack<(int Price, int Span)>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Online Stock Span (LC 901): the canonical O(n) backward rescan per Next() call
// (O(n^2) total across a full price stream) vs. a single O(n) amortized sweep
// through this repo's own Stack<(int,int)> of still-standing (price, span) pairs
// (DailyTemperatures/NextGreaterElementI precedent) - each day's price is pushed
// once and popped at most once across the whole stream. Prices are strictly
// increasing here, deliberately - the mirror image of DailyTemperaturesBenchmarks'
// own random permutation, whose goal was avoiding a next-greater distance of 1 on
// every element. Every day's span here spans every prior day: the backward rescan's
// true O(n) per-call worst case, and simultaneously the case where the stack holds
// at most one element after every push (each new high pops the entire stack in one
// pass), so the amortized side pays its cheapest possible cost - the widest possible
// gap between the two, not an arbitrary input choice.
[MemoryDiagnoser]
public class OnlineStockSpanBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private int[] _prices = null!;

    [GlobalSetup]
    public void Setup()
    {
        _prices = Enumerable.Range(1, Length).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForceBackwardScan()
    {
        var spans = new int[_prices.Length];

        for (var day = 0; day < _prices.Length; day++)
        {
            var span = 1;
            var previous = day - 1;

            while (previous >= 0 && _prices[previous] <= _prices[day])
            {
                span++;
                previous--;
            }

            spans[day] = span;
        }

        return spans;
    }

    [Benchmark]
    public int[] MonotonicStackSweep()
    {
        var spans = new int[_prices.Length];
        var pending = new RepoStack();

        for (var day = 0; day < _prices.Length; day++)
        {
            var span = 1;

            while (pending.TryPeek(out var top) && top.Price <= _prices[day])
            {
                pending.TryPop(out _);
                span += top.Span;
            }

            pending.Push((_prices[day], span));
            spans[day] = span;
        }

        return spans;
    }
}
