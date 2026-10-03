using DSAExperimentation.LeetCode.NumberOfRecentCalls;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfRecentCallsSolution's, the same methods
// NumberOfRecentCallsSolutionTests proves correct. The workload is a stream of strictly
// increasing timestamps with small random gaps, so the 3000ms window always
// holds a large slice of recent history - the case where rescanning the whole
// history costs O(calls) per ping (O(calls^2) over the run) while the queue window
// enqueues and dequeues each timestamp exactly once (O(calls) amortized). Every gap is
// at least one millisecond, because LC 933 pings with t >= 1 and strictly increasing.
public class NumberOfRecentCallsBenchmarks
{
    private const int RandomSeed = 933; // LC problem number
    private const int MinGap = 1;
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
            t += random.Next(MinGap, MaxGapExclusive);
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
