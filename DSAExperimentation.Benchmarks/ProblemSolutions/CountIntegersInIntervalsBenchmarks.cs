using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.IntervalSet;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Count Integers in Intervals (LC 2276): HashSetPerInteger is the naive approach most people
// reach for first - add every individual integer in [left, right] to a HashSet<int> and read
// its Count, O(range width) per Add call. IntervalSetMerge instead calls this repo's own
// IntervalSet<int>.Add(left, right) once per call (same composition
// CountIntegersInIntervalsTests uses) and sums (End - Start + 1) across the resulting O(k)
// disjoint intervals, so a call never costs more than the number of intervals it actually
// touches - never the width of the range being added.
[MemoryDiagnoser]
public class CountIntegersInIntervalsBenchmarks
{
    private const int RandomSeed = 2276;

    // Widest possible single call, small enough to keep HashSetPerInteger's per-call cost
    // bounded instead of dominating the whole benchmark on its own.
    private const int MaxRangeWidth = 20;

    [Params(200, 2_000)]
    public int OperationCount;

    private (int Left, int Right)[] _ranges = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _ranges = Enumerable.Range(0, OperationCount)
            .Select(_ =>
            {
                var left = random.Next(0, OperationCount * MaxRangeWidth);
                var right = left + random.Next(0, MaxRangeWidth);
                return (left, right);
            })
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int HashSetPerInteger()
    {
        var seen = new HashSet<int>();

        foreach (var (left, right) in _ranges)
        {
            for (var value = left; value <= right; value++)
            {
                seen.Add(value);
            }
        }

        return seen.Count;
    }

    [Benchmark]
    public int IntervalSetMerge()
    {
        var intervals = new IntervalSet<int>();

        foreach (var (left, right) in _ranges)
        {
            intervals.Add(left, right);
        }

        var total = 0;

        for (var i = 0; i < intervals.Count; i++)
        {
            var (start, end) = intervals.Get(i);
            total += end - start + 1;
        }

        return total;
    }
}
