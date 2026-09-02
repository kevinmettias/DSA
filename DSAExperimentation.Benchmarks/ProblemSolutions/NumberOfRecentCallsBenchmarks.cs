using BenchmarkDotNet.Attributes;

using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Number of Recent Calls (LC 933): FullHistoryRescan keeps every timestamp ever
// pinged in a growing list and rescans all of it on every call - O(calls) per
// Ping, O(calls^2) total across a full run. SlidingWindowQueue instead uses this
// repo's own Queue<int> as a FIFO sliding window, evicting stale timestamps from
// the front the instant they fall outside the 3000ms window - each timestamp is
// enqueued and dequeued exactly once across the whole run, so the total cost is
// O(calls) amortized instead of O(calls^2).
[MemoryDiagnoser]
public class NumberOfRecentCallsBenchmarks
{
    private const int RandomSeed = 933; // LC problem number
    private const int MaxGapExclusive = 50;
    private const int WindowMilliseconds = 3000;

    [Params(500, 5_000)]
    public int CallCount;

    private int[] _timestamps = null!;

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
    public int FullHistoryRescan()
    {
        var history = new List<int>();
        var lastCount = 0;

        foreach (var t in _timestamps)
        {
            history.Add(t);
            var count = 0;

            foreach (var seen in history)
            {
                if (seen >= t - WindowMilliseconds)
                {
                    count++;
                }
            }

            lastCount = count;
        }

        return lastCount;
    }

    [Benchmark]
    public int SlidingWindowQueue()
    {
        var pings = new RepoQueue();
        var lastCount = 0;

        foreach (var t in _timestamps)
        {
            pings.Enqueue(t);

            while (pings.TryPeek(out var oldest) && oldest < t - WindowMilliseconds)
            {
                pings.TryDequeue(out _);
            }

            lastCount = pings.Count;
        }

        return lastCount;
    }
}
