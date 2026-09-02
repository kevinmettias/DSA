using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MergeIntervals;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MergeIntervalsSolution's, the same methods
// MergeIntervalsTests proves correct. Merge Intervals (LC 56) has two honest
// real-world shapes, not one "which is faster" answer: BatchSortAndMerge is the
// classic textbook solution - sort once, O(n log n), a single linear merge pass -
// built for "I have all N intervals up front." IncrementalIntervalSet instead
// inserts intervals one at a time into this repo's own IntervalSet<TKey> (Insert
// Interval / streaming-calendar shape, LC 57) - each Add is a real insertion into
// an already-sorted structure, O(n) worst case, making N one-at-a-time insertions
// O(n^2) overall. Same input, same output; BatchSortAndMerge should win here
// specifically because "insert one at a time, stay merged after every step" is a
// strictly harder guarantee to maintain than "merge once, at the end."
[MemoryDiagnoser]
public class MergeIntervalsBenchmarks
{
    private const int RandomSeed = 3;
    private const int StartRangeMultiplier = 2;
    private const int MaxIntervalLength = 5;

    [Params(200, 3_000)]
    public int Length;

    private (int Start, int End)[] _intervals = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);

        _intervals = Enumerable.Range(0, Length)
            .Select(_ =>
            {
                var start = random.Next(0, Length * StartRangeMultiplier);
                return (start, start + random.Next(1, MaxIntervalLength));
            })
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BatchSortAndMerge() => MergeIntervalsSolution.MergeByBatchSortAndMerge(_intervals).Count;

    [Benchmark]
    public int IncrementalIntervalSet() => MergeIntervalsSolution.MergeByIntervalSet(_intervals).Count;
}
