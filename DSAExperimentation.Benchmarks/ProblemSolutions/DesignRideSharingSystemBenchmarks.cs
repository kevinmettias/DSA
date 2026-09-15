using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode;
using DSAExperimentation.LeetCode.DesignRideSharingSystem;

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

    private List<Func<DesignRideSharingSystemSolution.IRideSharingStrategy, int[]?>> _script = new();

    [Params(500, 5_000)]
    public int RiderCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _script = BuildScript(RiderCount, random);
    }

    private static List<Func<DesignRideSharingSystemSolution.IRideSharingStrategy, int[]?>> BuildScript(int riderCount, Random random)
    {
        var script = new List<Func<DesignRideSharingSystemSolution.IRideSharingStrategy, int[]?>>();

        AppendIdOperations(script, 0, riderCount, new AddRiderOperation());
        AppendIdOperations(script, riderCount, riderCount, new AddDriverOperation());

        AppendCancelledRiders(script, riderCount, random);
        AppendMatchRounds(script, riderCount);

        return script;
    }

    // The one thing the two seeding passes below do differently: which single-id call
    // they make. Both the receiver and the id are named here, and the contract a bare
    // `Action<IRideSharingStrategy, int>` had nowhere to state - the id is a rider or
    // driver id, and the call is a seed, never a match - has somewhere to be written
    // down. Stateless, so a fresh instance costs nothing meaningful at setup time.
    private interface IRideOperation
    {
        void Apply(DesignRideSharingSystemSolution.IRideSharingStrategy strategy, int id);
    }

    // Appends one seeded call per id in `[firstId, firstId + count)`. Every id comes
    // from `foreach (var id in Enumerable.Range(...))` rather than a hand-rolled
    // `for`, deliberately: a `for` loop's declared variable is one shared slot for
    // the whole loop, so the closure below capturing it directly would see only its
    // final value once the loop finished - `foreach`'s iteration variable is a fresh
    // binding per element instead, the one this script actually needs.
    private static void AppendIdOperations(
        List<Func<DesignRideSharingSystemSolution.IRideSharingStrategy, int[]?>> script,
        int firstId,
        int count,
        IRideOperation operation)
    {
        foreach (var id in Enumerable.Range(firstId, count))
        {
            script.Add(strategy =>
            {
                operation.Apply(strategy, id);
                return null;
            });
        }
    }

    private sealed class AddRiderOperation : IRideOperation
    {
        public void Apply(DesignRideSharingSystemSolution.IRideSharingStrategy strategy, int id) =>
            strategy.AddRider(id);
    }

    private sealed class AddDriverOperation : IRideOperation
    {
        public void Apply(DesignRideSharingSystemSolution.IRideSharingStrategy strategy, int id) =>
            strategy.AddDriver(id);
    }

    // A tenth of the seeded riders, chosen at random, get cancelled before any
    // match call runs - guaranteed still pending, since seeding only ever adds.
    private static void AppendCancelledRiders(
        List<Func<DesignRideSharingSystemSolution.IRideSharingStrategy, int[]?>> script, int riderCount, Random random)
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

    private static void AppendMatchRounds(List<Func<DesignRideSharingSystemSolution.IRideSharingStrategy, int[]?>> script, int riderCount)
    {
        var rounds = Enumerable.Repeat<Func<DesignRideSharingSystemSolution.IRideSharingStrategy, int[]?>>(
            static strategy => strategy.MatchDriverWithRider(), riderCount);
        script.AddRange(rounds);
    }

    [Benchmark(Baseline = true)]
    public long LinearScanQueue() => Replay(new DesignRideSharingSystemSolution.RideSharingSystemByLinearScanQueue());

    [Benchmark]
    public long LazyDeletionQueue() => Replay(new DesignRideSharingSystemSolution.RideSharingSystemByLazyDeletionQueue());

    // Sums every returned [driverId, riderId] pair (treating a void call's null,
    // and a failed match's [-1, -1], as 0) rather than discarding it, so the JIT
    // can't eliminate the replay as dead code - the same "return the real answer,
    // not a weaker proxy" shape DesignAuctionSystemBenchmarks already follows.
    private long Replay(DesignRideSharingSystemSolution.IRideSharingStrategy strategy)
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
}
