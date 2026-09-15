using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NumberOfRecentCalls;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfRecentCallsSolution's, the same methods
// NumberOfRecentCallsTests proves correct. The workload is a stream of strictly
// non-decreasing timestamps with small random gaps, so the 3000ms window always
// holds a large slice of recent history - the case where rescanning the whole
// history costs O(calls) per ping (O(calls^2) over the run) while the queue window
// enqueues and dequeues each timestamp exactly once (O(calls) amortized).
[MemoryDiagnoser]
public class NumberOfRecentCallsBenchmarks
{
    private const int RandomSeed = 933; // LC problem number
    private const int MaxGapExclusive = 50;

    private int[] _timestamps = [];

    [Params(500, 5_000)]
    public int CallCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var t = 0;
        _timestamps = new int[CallCount];

        for (var i = 0; i < CallCount; i++)
        {
            t += random.Next(0, MaxGapExclusive);
            _timestamps[i] = t;
        }
    }

    [Benchmark(Baseline = true)]
    public int[] FullHistoryRescan() =>
        NumberOfRecentCallsSolution.PingCountsByFullHistoryRescan(_timestamps);

    [Benchmark]
    public int[] SlidingWindowQueue() =>
        NumberOfRecentCallsSolution.PingCountsBySlidingWindowQueue(_timestamps);
}
