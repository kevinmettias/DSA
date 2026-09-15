using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.DesignTwitter;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DesignTwitterSolution's, the same classes
// DesignTwitterTests proves correct. LC 355's own Twitter() constructor takes no
// initial state, so - as with DesignAuctionSystem - there is no separate "prepared
// input" to hoist through; [GlobalSetup] instead builds one fixed call script: user
// 0 follows every one of FollowedUsers followees, then TweetsPerSource tweets per
// followee are posted in randomly interleaved order (so the top 10 genuinely draw
// from many different sources instead of always the most-recently-seeded one), and
// each [Benchmark] arm constructs a fresh strategy and replays that script before
// reading user 0's feed - so script construction, including the random interleave
// order, is charged to setup rather than to the replay each arm measures.
[MemoryDiagnoser]
public class DesignTwitterBenchmarks
{
    private const int SelfUserId = 0;
    private const int TweetsPerSource = 20;
    private const int RandomSeed = 13;

    private List<Action<DesignTwitterSolution.ITwitterStrategy>> _script = new();

    [Params(50, 500)]
    public int FollowedUsers { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _script = BuildScript(FollowedUsers, random);
    }

    private static List<Action<DesignTwitterSolution.ITwitterStrategy>> BuildScript(int followedUsers, Random random)
    {
        var script = new List<Action<DesignTwitterSolution.ITwitterStrategy>>();

        for (var followeeId = 1; followeeId <= followedUsers; followeeId++)
        {
            var capturedFolloweeId = followeeId;
            script.Add(strategy => strategy.Follow(SelfUserId, capturedFolloweeId));
        }

        var totalTweets = followedUsers * TweetsPerSource;

        for (var tweetId = 0; tweetId < totalTweets; tweetId++)
        {
            var sourceUserId = random.Next(1, followedUsers + 1);
            var capturedTweetId = tweetId;
            script.Add(strategy => strategy.PostTweet(sourceUserId, capturedTweetId));
        }

        return script;
    }

    [Benchmark(Baseline = true)]
    public int GatherAllAndSort() => Replay(new DesignTwitterSolution.TwitterByGatherAllAndSort());

    [Benchmark]
    public int HeapKWayMerge() => Replay(new DesignTwitterSolution.TwitterByHeapKWayMerge());

    // Sums the returned feed's tweetIds rather than discarding them, so the JIT
    // can't eliminate the replay as dead code - the same "return the real answer,
    // not a weaker proxy" shape DesignAuctionSystemBenchmarks already follows.
    private int Replay(DesignTwitterSolution.ITwitterStrategy strategy)
    {
        foreach (var op in _script)
        {
            op(strategy);
        }

        return strategy.GetNewsFeed(SelfUserId).Sum();
    }
}
