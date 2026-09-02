using BenchmarkDotNet.Attributes;
using static DSAExperimentation.LeetCode.DesignAuctionSystem.DesignAuctionSystemSolution;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DesignAuctionSystemSolution's, the same classes
// DesignAuctionSystemTests proves correct. Unlike DesignTaskManager, LC 3815's own
// AuctionSystem() constructor takes no initial state, so there is no separate
// "prepared input" to hoist a seed through - [GlobalSetup] instead builds one
// fixed, valid call script that starts with the seeding addBid calls themselves,
// then edits/removals against userId/itemId pairs nothing else has touched yet,
// then repeated add/add/query rounds so live bids only grow - so script
// construction, including tracking which pairs are still safe to reference, is
// charged to setup rather than to the replay each [Benchmark] arm measures. Every
// bid lands on one of a small fixed pool of itemIds, so both arms actually have to
// pick a winner among several live bidders rather than a single one.
[MemoryDiagnoser]
public class DesignAuctionSystemBenchmarks
{
    private const int Seed = 3815;
    private const int ItemPoolSize = 50;
    private const int BidAmountUpperBound = 1_000_000_000;

    [Params(200, 2_000)]
    public int InitialBidCount;

    private List<Func<IAuctionSystemStrategy, int?>> _script = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _script = BuildScript(InitialBidCount, random);
    }

    [Benchmark(Baseline = true)]
    public long LinearScan() => Replay(new AuctionSystemByLinearScan());

    [Benchmark]
    public long LazyDeletionHeap() => Replay(new AuctionSystemByLazyDeletionHeap());

    // Sums every returned userId (treating a void call's null as 0) rather than
    // discarding it, so the JIT can't eliminate the replay as dead code - the same
    // "return the real answer, not a weaker proxy" shape DesignTaskManagerBenchmarks
    // already follows.
    private long Replay(IAuctionSystemStrategy strategy)
    {
        var highestBidderSum = 0L;

        foreach (var op in _script)
        {
            highestBidderSum += op(strategy) ?? 0;
        }

        return highestBidderSum;
    }

    private static List<Func<IAuctionSystemStrategy, int?>> BuildScript(int bidCount, Random random)
    {
        var script = new List<Func<IAuctionSystemStrategy, int?>>();
        var seededBids = new (int UserId, int ItemId)[bidCount];

        for (var userId = 0; userId < bidCount; userId++)
        {
            var itemId = userId % ItemPoolSize;
            var amount = random.Next(1, BidAmountUpperBound);
            seededBids[userId] = (userId, itemId);
            script.Add(strategy =>
            {
                strategy.AddBid(userId, itemId, amount);
                return null;
            });
        }

        var removeCount = bidCount / 10;
        var updateCount = bidCount / 10;

        // Removed/updated first, before the round loop below ever queries anything
        // - these (userId, itemId) pairs are guaranteed still live.
        for (var i = 0; i < removeCount; i++)
        {
            var (userId, itemId) = seededBids[i];
            script.Add(strategy =>
            {
                strategy.RemoveBid(userId, itemId);
                return null;
            });
        }

        for (var i = removeCount; i < removeCount + updateCount; i++)
        {
            var (userId, itemId) = seededBids[i];
            var newAmount = random.Next(1, BidAmountUpperBound);
            script.Add(strategy =>
            {
                strategy.UpdateBid(userId, itemId, newAmount);
                return null;
            });
        }

        AppendGrowthRounds(script, bidCount, random);
        return script;
    }

    // Two fresh AddBid calls per query, each from a userId never seeded or used
    // before: the live bid count only grows round over round, so every query
    // always has at least one bidder to find regardless of which item it lands on.
    private static void AppendGrowthRounds(List<Func<IAuctionSystemStrategy, int?>> script, int bidCount, Random random)
    {
        var nextUserId = bidCount;

        for (var round = 0; round < bidCount; round++)
        {
            AppendAddBid(script, random, nextUserId++);
            AppendAddBid(script, random, nextUserId++);
            var itemId = random.Next(0, ItemPoolSize);
            script.Add(strategy => strategy.GetHighestBidder(itemId));
        }
    }

    private static void AppendAddBid(List<Func<IAuctionSystemStrategy, int?>> script, Random random, int userId)
    {
        var itemId = random.Next(0, ItemPoolSize);
        var amount = random.Next(1, BidAmountUpperBound);
        script.Add(strategy =>
        {
            strategy.AddBid(userId, itemId, amount);
            return null;
        });
    }
}
