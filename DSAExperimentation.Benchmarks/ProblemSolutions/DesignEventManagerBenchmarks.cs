using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.DesignEventManager;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DesignEventManagerSolution's, the same classes
// DesignEventManagerTests proves correct. [GlobalSetup] builds one fixed, random
// call script - every event re-prioritized once, in random order, then every
// event drained via PollHighest - so script construction is charged to setup
// rather than to the replay each [Benchmark] arm measures. There is no Add/Rmv
// pair to interleave (unlike DesignTaskManagerBenchmarks): this problem's events
// are fixed at construction, so the script only ever updates or polls, never
// grows the pool.
[MemoryDiagnoser]
public class DesignEventManagerBenchmarks
{
    private const int Seed = 3885;
    private const int PriorityUpperBound = 1_000_000_000;

    private (int EventId, int Priority)[] _initialEvents = [];

    private List<Func<DesignEventManagerSolution.IEventManagerStrategy, int?>> _script = new();
    [Params(200, 2_000)]
    public int EventCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _initialEvents = BuildInitialEvents(EventCount, random);
        _script = BuildScript(EventCount, random);
    }

    private static (int EventId, int Priority)[] BuildInitialEvents(int count, Random random)
        => Enumerable.Range(1, count)
            .Select(eventId => (EventId: eventId, Priority: random.Next(1, PriorityUpperBound)))
            .ToArray();

    private static List<Func<DesignEventManagerSolution.IEventManagerStrategy, int?>> BuildScript(int count, Random random)
    {
        var script = new List<Func<DesignEventManagerSolution.IEventManagerStrategy, int?>>();

        AppendUpdateRound(script, count, random);
        AppendDrainRound(script, count);
        return script;
    }

    // Every eventId re-prioritized exactly once, visited in random order -
    // exercises UpdatePriority's lazy-insertion path without ever touching an
    // eventId that has already been polled away (nothing is polled yet).
    private static void AppendUpdateRound(List<Func<DesignEventManagerSolution.IEventManagerStrategy, int?>> script, int count, Random random)
    {
        foreach (var eventId in Enumerable.Range(1, count).OrderBy(_ => random.Next()))
        {
            var newPriority = random.Next(1, PriorityUpperBound);
            script.Add(strategy =>
            {
                strategy.UpdatePriority(eventId, newPriority);
                return null;
            });
        }
    }

    // Drains every event via PollHighest - always has a target, since the pool
    // only shrinks and starts at exactly count events.
    private static void AppendDrainRound(List<Func<DesignEventManagerSolution.IEventManagerStrategy, int?>> script, int count)
    {
        for (var i = 0; i < count; i++)
        {
            script.Add(strategy => strategy.PollHighest());
        }
    }

    [Benchmark(Baseline = true)]
    public long LinearScan() => Replay(new DesignEventManagerSolution.EventManagerByLinearScan(_initialEvents));

    [Benchmark]
    public long LazyDeletionHeap() => Replay(new DesignEventManagerSolution.EventManagerByLazyDeletionHeap(_initialEvents));

    // Sums every polled eventId rather than discarding it, so the JIT can't
    // eliminate the replay as dead code - the same "return the real answer, not a
    // weaker proxy" shape OpenTheLockBenchmarks/DesignTaskManagerBenchmarks
    // already follow.
    private long Replay(DesignEventManagerSolution.IEventManagerStrategy strategy)
    {
        var polledEventIdSum = 0L;

        foreach (var op in _script)
        {
            polledEventIdSum += op(strategy) ?? 0;
        }

        return polledEventIdSum;
    }
}
