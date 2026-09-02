using BenchmarkDotNet.Attributes;
using static DSAExperimentation.LeetCode.ImplementRouter.ImplementRouterSolution;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ImplementRouterSolution's, the same classes
// ImplementRouterTests proves correct. [GlobalSetup] builds one fixed, valid
// call script - packets added in strictly increasing timestamp order (LC's own
// "calls arrive in non-decreasing timestamp order" guarantee), interleaved
// with forwardPacket/getCount calls - so script construction is charged to
// setup rather than to the replay each [Benchmark] arm measures.
[MemoryDiagnoser]
public class ImplementRouterBenchmarks
{
    private const int Seed = 3508;
    private const int DestinationUpperBound = 50;
    private const int TimestampStep = 3;

    [Params(200, 2_000)]
    public int PacketCount;

    private int _memoryLimit;
    private List<Func<IRouterStrategy, object?>> _script = null!;

    [GlobalSetup]
    public void Setup()
    {
        _memoryLimit = Math.Max(2, PacketCount / 4);
        _script = BuildScript(PacketCount, _memoryLimit, new Random(Seed));
    }

    [Benchmark(Baseline = true)]
    public long LinearScan() => Replay(new RouterByLinearScan(_memoryLimit));

    [Benchmark]
    public long BinarySearchIndex() => Replay(new RouterByBinarySearchIndex(_memoryLimit));

    // Sums a numeric projection of every reply (a successful add or a
    // non-empty forward each contribute 1, getCount contributes its own
    // count) rather than discarding it, so the JIT can't eliminate the replay
    // as dead code - the same "return the real answer, not a weaker proxy"
    // shape DesignTaskManagerBenchmarks.Replay already follows.
    private long Replay(IRouterStrategy strategy)
    {
        var total = 0L;

        foreach (var call in _script)
        {
            total += call(strategy) switch
            {
                true => 1,
                int[] { Length: > 0 } => 1,
                int count => count,
                _ => 0,
            };
        }

        return total;
    }

    // One add per packet (strictly increasing timestamps), a forward every
    // third packet, and a getCount every fifth packet over a recently-seen
    // destination/time window - a realistic mixed workload rather than an
    // add-only one.
    private static List<Func<IRouterStrategy, object?>> BuildScript(int packetCount, int memoryLimit, Random random)
    {
        var script = new List<Func<IRouterStrategy, object?>>(packetCount);
        var timestamp = 0;

        for (var i = 0; i < packetCount; i++)
        {
            timestamp += random.Next(1, TimestampStep);
            var source = random.Next(1, DestinationUpperBound);
            var destination = random.Next(1, DestinationUpperBound);
            var packetTimestamp = timestamp;
            script.Add(strategy => strategy.AddPacket(source, destination, packetTimestamp));

            if (i % 3 == 2)
            {
                script.Add(strategy => strategy.ForwardPacket());
            }

            if (i % 5 == 4)
            {
                var windowDestination = destination;
                var start = Math.Max(1, packetTimestamp - (memoryLimit * TimestampStep));
                script.Add(strategy => strategy.GetCount(windowDestination, start, packetTimestamp));
            }
        }

        return script;
    }
}
