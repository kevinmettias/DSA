using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode;
using static DSAExperimentation.LeetCode.DesignRideSharingSystem.DesignRideSharingSystemSolution;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DesignRideSharingSystemSolution's, the same classes
// DesignRideSharingSystemTests proves correct. LC 3829's own RideSharingSystem()
// constructor takes no initial state, so - as with DesignAuctionSystemBenchmarks -
// there is no separate "prepared input" to hoist a seed through; [GlobalSetup]
// instead builds one fixed, valid call script. A tenth of the seeded riders are
// cancelled before the match rounds ever run, so both arms have to walk past at
// least one stale/cancelled entry rather than always matching the immediate
// front.
[MemoryDiagnoser]
public class DesignRideSharingSystemBenchmarks
{
    private const int Seed = 3829;

    [Params(500, 5_000)]
    public int RiderCount;

    private List<Func<IRideSharingStrategy, int[]?>> _script = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _script = BuildScript(RiderCount, random);
    }

    [Benchmark(Baseline = true)]
    public long LinearScanQueue() => Replay(new RideSharingSystemByLinearScanQueue());

    [Benchmark]
    public long LazyDeletionQueue() => Replay(new RideSharingSystemByLazyDeletionQueue());

    // Sums every returned [driverId, riderId] pair (treating a void call's null,
    // and a failed match's [-1, -1], as 0) rather than discarding it, so the JIT
    // can't eliminate the replay as dead code - the same "return the real answer,
    // not a weaker proxy" shape DesignAuctionSystemBenchmarks already follows.
    private long Replay(IRideSharingStrategy strategy)
    {
        var idSum = 0L;

        foreach (var op in _script)
        {
            if (op(strategy) is [var driverId, var riderId] && driverId != LeetCodeAnswer.None)
            {
                idSum += driverId + riderId;
            }
        }

        return idSum;
    }

    // Every rider/driver id below comes from `foreach (var id in Enumerable.Range(...))`
    // rather than a hand-rolled `for`, deliberately: a `for` loop's declared
    // variable is one shared slot for the whole loop, so a closure capturing it
    // directly would see only its final value once the loop finished -
    // `foreach`'s iteration variable is a fresh binding per element instead, the
    // one this method actually needs.
    private static List<Func<IRideSharingStrategy, int[]?>> BuildScript(int riderCount, Random random)
    {
        var script = new List<Func<IRideSharingStrategy, int[]?>>();

        foreach (var riderId in Enumerable.Range(0, riderCount))
        {
            script.Add(strategy =>
            {
                strategy.AddRider(riderId);
                return null;
            });
        }

        foreach (var driverId in Enumerable.Range(riderCount, riderCount))
        {
            script.Add(strategy =>
            {
                strategy.AddDriver(driverId);
                return null;
            });
        }

        AppendCancelledRiders(script, riderCount, random);
        AppendMatchRounds(script, riderCount);

        return script;
    }

    // A tenth of the seeded riders, chosen at random, get cancelled before any
    // match call runs - guaranteed still pending, since seeding only ever adds.
    private static void AppendCancelledRiders(
        List<Func<IRideSharingStrategy, int[]?>> script, int riderCount, Random random)
    {
        var cancelCount = Math.Max(1, riderCount / 10);

        foreach (var _ in Enumerable.Range(0, cancelCount))
        {
            var riderId = random.Next(0, riderCount);
            script.Add(strategy =>
            {
                strategy.CancelRider(riderId);
                return null;
            });
        }
    }

    private static void AppendMatchRounds(List<Func<IRideSharingStrategy, int[]?>> script, int riderCount) =>
        script.AddRange(Enumerable.Repeat<Func<IRideSharingStrategy, int[]?>>(
            static strategy => strategy.MatchDriverWithRider(), riderCount));
}
