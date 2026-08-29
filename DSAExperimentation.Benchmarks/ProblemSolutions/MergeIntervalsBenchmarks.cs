using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.IntervalSet;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Merge Intervals (LC 56) has two honest real-world shapes, not one "which is
// faster" answer: BatchSortAndMerge is the classic textbook solution - sort once,
// O(n log n), a single linear merge pass - built for "I have all N intervals up
// front." IncrementalIntervalSet instead inserts intervals one at a time into this
// repo's own IntervalSet<TKey> (Insert Interval / streaming-calendar shape, LC 57)
// - each Add is a real insertion into an already-sorted structure, O(n) worst case,
// making N one-at-a-time insertions O(n^2) overall. Same input, same output;
// BatchSortAndMerge should win here specifically because "insert one at a time,
// stay merged after every step" is a strictly harder guarantee to maintain than
// "merge once, at the end."
[MemoryDiagnoser]
public class MergeIntervalsBenchmarks
{
    [Params(200, 3_000)]
    public int Length;

    private (int Start, int End)[] _intervals = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(3);

        _intervals = Enumerable.Range(0, Length)
            .Select(_ =>
            {
                var start = random.Next(0, Length * 2);
                return (start, start + random.Next(1, 5));
            })
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BatchSortAndMerge()
    {
        var sorted = _intervals.OrderBy(interval => interval.Start).ToArray();
        var merged = new List<(int Start, int End)> { sorted[0] };

        for (var i = 1; i < sorted.Length; i++)
        {
            var last = merged[^1];
            var (start, end) = sorted[i];

            if (start <= last.End)
            {
                merged[^1] = (last.Start, Math.Max(last.End, end));
            }
            else
            {
                merged.Add((start, end));
            }
        }

        return merged.Count;
    }

    [Benchmark]
    public int IncrementalIntervalSet()
    {
        var set = new IntervalSet<int>();

        foreach (var (start, end) in _intervals)
        {
            set.Add(start, end);
        }

        return set.Count;
    }
}
