using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.IntervalSet;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Data Stream as Disjoint Intervals (LC 352): addNum must leave the summary fully merged
// after every single call (LeetCode's SummaryRanges shape), not just once at the end.
// FullRebuildEachCall is the naive approach - track every distinct value seen and
// re-derive the disjoint-interval summary from scratch (re-sort + rescan for consecutive
// runs) on every addNum, O(n log n) per call. IntervalSetAddNum instead calls this repo's
// own IntervalSet<int>.Add once per value, encoding each value v as the half-open pair
// (v, v+1) so adjacent integers merge via IntervalSet's existing shared-boundary rule (see
// DataStreamAsDisjointIntervalsTests.cs for why that encoding is needed) - each call merges
// in place via BinarySearch.LowerBound instead of rebuilding the whole summary.
[MemoryDiagnoser]
public class DataStreamAsDisjointIntervalsBenchmarks
{
    [Params(200, 2_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(7);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(0, Length * 3)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int FullRebuildEachCall()
    {
        var raw = new List<int>();
        var lastCount = 0;

        foreach (var value in _values)
        {
            raw.Add(value);
            lastCount = CountDisjointIntervals(raw);
        }

        return lastCount;
    }

    [Benchmark]
    public int IntervalSetAddNum()
    {
        var set = new IntervalSet<int>();

        foreach (var value in _values)
        {
            set.Add(value, value + 1);
        }

        return set.Count;
    }

    private static int CountDisjointIntervals(List<int> raw)
    {
        var sorted = raw.Distinct().OrderBy(value => value).ToList();
        var count = 0;

        for (var i = 0; i < sorted.Count; i++)
        {
            if (i == 0 || sorted[i] != sorted[i - 1] + 1)
            {
                count++;
            }
        }

        return count;
    }
}
