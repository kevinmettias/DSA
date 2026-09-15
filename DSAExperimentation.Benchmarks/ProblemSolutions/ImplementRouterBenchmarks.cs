using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ImplementRouter;

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

    private int _memoryLimit;

    private List<Func<ImplementRouterSolution.IRouterStrategy, object?>> _script = new();
    [Params(200, 2_000)]
    public int PacketCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _memoryLimit = Math.Max(2, PacketCount / 4);
        _script = BuildScript(PacketCount, _memoryLimit, new Random(Seed));
    }

    // One add per packet (strictly increasing timestamps), a forward every
    // third packet, and a getCount every fifth packet over a recently-seen
    // destination/time window - a realistic mixed workload rather than an
    // add-only one.
    private static List<Func<ImplementRouterSolution.IRouterStrategy, object?>> BuildScript(int packetCount, int memoryLimit, Random random)
    {
        var script = new List<Func<ImplementRouterSolution.IRouterStrategy, object?>>(packetCount);
        var timestamp = 0;

        for (var i = 0; i < packetCount; i++)
        {
            timestamp = AppendPacketRound(script, random, (i, memoryLimit), timestamp);
        }

        return script;
    }

    // One packet's worth of the script: its add, plus the forward every third packet and the
    // getCount every fifth, reading a window that reaches back the memory limit. The running
    // timestamp is the packet clock, advanced and handed back to the caller's loop.
    private static int AppendPacketRound(
        List<Func<ImplementRouterSolution.IRouterStrategy, object?>> script, Random random, (int Index, int MemoryLimit) packet, int timestamp)
    {
        timestamp += random.Next(1, TimestampStep);
        var source = random.Next(1, DestinationUpperBound);
        var destination = random.Next(1, DestinationUpperBound);
        var packetTimestamp = timestamp;
        script.Add(strategy => strategy.AddPacket(source, destination, packetTimestamp));

        if (packet.Index % 3 == 2)
        {
            script.Add(strategy => strategy.ForwardPacket());
        }

        if (packet.Index % 5 == 4)
        {
            var windowDestination = destination;
            var start = Math.Max(1, packetTimestamp - (packet.MemoryLimit * TimestampStep));
            script.Add(strategy => strategy.GetCount(windowDestination, start, packetTimestamp));
        }

        return timestamp;
    }

    [Benchmark(Baseline = true)]
    public long LinearScan() => Replay(new ImplementRouterSolution.RouterByLinearScan(_memoryLimit));

    [Benchmark]
    public long BinarySearchIndex() => Replay(new ImplementRouterSolution.RouterByBinarySearchIndex(_memoryLimit));

    // Sums a numeric projection of every reply (a successful add or a
    // non-empty forward each contribute 1, getCount contributes its own
    // count) rather than discarding it, so the JIT can't eliminate the replay
    // as dead code - the same "return the real answer, not a weaker proxy"
    // shape DesignTaskManagerBenchmarks.Replay already follows.
    private long Replay(ImplementRouterSolution.IRouterStrategy strategy)
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
}
