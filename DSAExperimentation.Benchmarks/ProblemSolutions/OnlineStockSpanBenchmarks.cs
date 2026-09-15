using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.OnlineStockSpan;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are OnlineStockSpanSolution's, the same methods
// OnlineStockSpanTests proves correct. Prices are strictly increasing here,
// deliberately - the mirror image of DailyTemperaturesBenchmarks' own random
// permutation, whose goal was avoiding a next-greater distance of 1 on every
// element. Every day's span here spans every prior day: the backward rescan's true
// O(n) per-call worst case, and simultaneously the case where the stack holds at
// most one element after every push (each new high pops the entire stack in one
// pass), so the amortized side pays its cheapest possible cost - the widest
// possible gap between the two, not an arbitrary input choice.
[MemoryDiagnoser]
public class OnlineStockSpanBenchmarks
{
    private int[] _prices = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _prices = Enumerable.Range(1, Length).ToArray();

    [Benchmark(Baseline = true)]
    public int[] BruteForceBackwardScan() => OnlineStockSpanSolution.SpansByBackwardScan(_prices);

    [Benchmark]
    public int[] MonotonicStackSweep() => OnlineStockSpanSolution.SpansByMonotonicStack(_prices);
}
