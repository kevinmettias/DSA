using DSAExperimentation.LeetCode.DesignTwitter;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DesignTwitterSolution's, the same classes
// DesignTwitterSolutionTests proves correct. LC 355's own Twitter() constructor takes no
// initial state, so - as with DesignAuctionSystem - there is no separate "prepared
// input" to hoist through; [GlobalSetup] instead builds one fixed call script: user
// 500 follows every one of FollowedUsers followees, then TweetsPerSource tweets per
// followee are posted in randomly interleaved order (so the top 10 genuinely draw
// from many different sources instead of always the most-recently-seeded one), and
// each [Benchmark] arm constructs a fresh strategy and replays that script before
// reading user 500's feed - so script construction, including the random interleave
// order, is charged to setup rather than to the replay each arm measures.
//
// LC 355 numbers users 1..500 and forbids following yourself, so the reader is the
// largest id and the followees 1..FollowedUsers stop one short of it, at 499.
public class DesignTwitterBenchmarks
{
    private const int SelfUserId = 500;
    private const int TweetsPerSource = 20;
    private const int RandomSeed = 13;

    private List<Action<DesignTwitterSolution.ITwitterStrategy>> _script = new();

    [Params(50, 499)]
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
    public List<int> GatherAllAndSort() => Replay(new DesignTwitterSolution.TwitterByGatherAllAndSort());

    [Benchmark]
    public List<int> HeapKWayMerge() => Replay(new DesignTwitterSolution.TwitterByHeapKWayMerge());

    // Returns the feed itself - the replay's only output, since Follow and PostTweet
    // answer nothing - so the JIT can't eliminate the replay as dead code.
    private List<int> Replay(DesignTwitterSolution.ITwitterStrategy strategy)
    {
        foreach (var op in _script)
        {
            op(strategy);
        }

        return strategy.GetNewsFeed(SelfUserId);
    }
}
