using DSAExperimentation.LeetCode.Solutions.NumberOfRecentCalls;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfRecentCallsSolution's, the same factories
// NumberOfRecentCallsSolutionTests proves correct, each counter pinged once per
// timestamp, in order, its counts returned. The workload is a stream of strictly
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

    // Every ping's count; sized in setup so the replay allocates nothing.
    private int[] _counts = [];

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

        _counts = new int[CallCount];
    }

    [Benchmark(Baseline = true)]
    public int[] FullHistoryRescan()
    {
        var counter = NumberOfRecentCallsSolution.CreateByFullHistoryRescan();
        return PingEach(counter);
    }

    [Benchmark]
    public int[] SlidingWindowQueue()
    {
        var counter = NumberOfRecentCallsSolution.CreateBySlidingWindowQueue();
        return PingEach(counter);
    }

    private int[] PingEach(NumberOfRecentCallsSolution.IRecentCounter counter)
    {
        for (var i = 0; i < _timestamps.Length; i++)
        {
            _counts[i] = counter.Ping(_timestamps[i]);
        }

        return _counts;
    }
}
