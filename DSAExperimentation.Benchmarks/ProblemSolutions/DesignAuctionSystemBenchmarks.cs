using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.DesignAuctionSystem;

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

    private List<Func<DesignAuctionSystemSolution.IAuctionSystemStrategy, int?>> _script = new();

    [Params(200, 2_000)]
    public int InitialBidCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _script = BuildScript(InitialBidCount, random);
    }

    private static List<Func<DesignAuctionSystemSolution.IAuctionSystemStrategy, int?>> BuildScript(int bidCount, Random random)
    {
        var script = new List<Func<DesignAuctionSystemSolution.IAuctionSystemStrategy, int?>>();
        var seededBids = new (int UserId, int ItemId)[bidCount];

        for (var userId = 0; userId < bidCount; userId++)
        {
            SeedBid(script, seededBids, userId, random);
        }

        var removeCount = bidCount / 10;
        var updateCount = bidCount / 10;

        // Removed/updated first, before the round loop below ever queries anything
        // - these (userId, itemId) pairs are guaranteed still live.
        AppendRemovals(script, seededBids, removeCount);
        AppendUpdates(script, seededBids, (removeCount, updateCount), random);
        AppendGrowthRounds(script, bidCount, random);

        return script;
    }

    // One seeded addBid call, remembering its (userId, itemId) pair so a later
    // removal or update can safely reference it.
    private static void SeedBid(
        List<Func<DesignAuctionSystemSolution.IAuctionSystemStrategy, int?>> script,
        (int UserId, int ItemId)[] seededBids,
        int userId,
        Random random)
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

    // The removals: the first removeCount seeded pairs, which nothing the script has
    // run so far has touched.
    private static void AppendRemovals(
        List<Func<DesignAuctionSystemSolution.IAuctionSystemStrategy, int?>> script,
        (int UserId, int ItemId)[] seededBids,
        int removeCount)
    {
        for (var i = 0; i < removeCount; i++)
        {
            var (userId, itemId) = seededBids[i];
            script.Add(strategy =>
            {
                strategy.RemoveBid(userId, itemId);
                return null;
            });
        }
    }

    // The updates: the next seeded pairs in the same order, each given a fresh amount
    // so a live bid changes rather than merely being rewritten.
    private static void AppendUpdates(
        List<Func<DesignAuctionSystemSolution.IAuctionSystemStrategy, int?>> script,
        (int UserId, int ItemId)[] seededBids,
        (int First, int Count) range,
        Random random)
    {
        for (var i = range.First; i < range.First + range.Count; i++)
        {
            var (userId, itemId) = seededBids[i];
            var newAmount = random.Next(1, BidAmountUpperBound);
            script.Add(strategy =>
            {
                strategy.UpdateBid(userId, itemId, newAmount);
                return null;
            });
        }
    }

    // Two fresh AddBid calls per query, each from a userId never seeded or used
    // before: the live bid count only grows round over round, so every query
    // always has at least one bidder to find regardless of which item it lands on.
    private static void AppendGrowthRounds(List<Func<DesignAuctionSystemSolution.IAuctionSystemStrategy, int?>> script, int bidCount, Random random)
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

    private static void AppendAddBid(List<Func<DesignAuctionSystemSolution.IAuctionSystemStrategy, int?>> script, Random random, int userId)
    {
        var itemId = random.Next(0, ItemPoolSize);
        var amount = random.Next(1, BidAmountUpperBound);
        script.Add(strategy =>
        {
            strategy.AddBid(userId, itemId, amount);
            return null;
        });
    }

    [Benchmark(Baseline = true)]
    public long LinearScan() => Replay(new DesignAuctionSystemSolution.AuctionSystemByLinearScan());

    [Benchmark]
    public long LazyDeletionHeap() => Replay(new DesignAuctionSystemSolution.AuctionSystemByLazyDeletionHeap());

    // Sums every returned userId (treating a void call's null as 0) rather than
    // discarding it, so the JIT can't eliminate the replay as dead code - the same
    // "return the real answer, not a weaker proxy" shape DesignTaskManagerBenchmarks
    // already follows.
    private long Replay(DesignAuctionSystemSolution.IAuctionSystemStrategy strategy)
    {
        var highestBidderSum = 0L;

        foreach (var op in _script)
        {
            highestBidderSum += op(strategy) ?? 0;
        }

        return highestBidderSum;
    }
}
