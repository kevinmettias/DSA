using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountIntegersInIntervals;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountIntegersInIntervalsSolution's, the same factories
// CountIntegersInIntervalsTests proves correct. [GlobalSetup] generates the fixed add()
// script - OperationCount random ranges - so script construction is charged to setup and
// only the replay is measured, the same "run the stateful object end to end" shape
// DataStreamAsDisjointIntervalsBenchmarks uses.
[MemoryDiagnoser]
public class CountIntegersInIntervalsBenchmarks
{
    private const int RandomSeed = 2276;

    // Widest possible single call, small enough to keep the per-integer HashSet baseline's
    // per-call cost bounded instead of dominating the whole benchmark on its own.
    private const int MaxRangeWidth = 20;

    private (int Left, int Right)[] _ranges = [];

    [Params(200, 2_000)]
    public int OperationCount { get; set; }

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
    public int HashSetPerInteger() => Replay(CountIntegersInIntervalsSolution.CreateByHashSetPerInteger());

    [Benchmark]
    public int IntervalSetMerge() => Replay(CountIntegersInIntervalsSolution.CreateByIntervalSetMerge());

    private int Replay(CountIntegersInIntervalsSolution.ICountIntervals countIntervals)
    {
        foreach (var (left, right) in _ranges)
        {
            countIntervals.Add(left, right);
        }

        return countIntervals.Count();
    }
}
