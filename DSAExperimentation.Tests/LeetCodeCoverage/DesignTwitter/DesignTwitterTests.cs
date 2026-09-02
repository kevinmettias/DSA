using static DSAExperimentation.LeetCode.DesignTwitter.DesignTwitterSolution;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignTwitter;

// Harness only. Both strategies are DesignTwitterSolution's - this file replays
// LeetCode's published call sequence, plus the two edge cases the original test
// covered (more than ten tweets from one user, and interleaving self/followee
// tweets by time), against each ITwitterStrategy implementation via a small
// operation script, so a failure still names the strategy that broke.
// TwitterOp.Apply is pure dispatch (which method to call with which arguments) -
// no feed-ordering logic of its own.
public sealed class DesignTwitterTests
{
    public static TheoryData<TwitterOp[], List<int>?[]> Examples =>
        new()
        {
            {
                [
                    TwitterOp.PostTweet(1, 5),
                    TwitterOp.GetNewsFeed(1),
                    TwitterOp.Follow(1, 2),
                    TwitterOp.PostTweet(2, 6),
                    TwitterOp.GetNewsFeed(1),
                    TwitterOp.Unfollow(1, 2),
                    TwitterOp.GetNewsFeed(1),
                ],
                [null, [5], null, null, [6, 5], null, [5]]
            },
            {
                [
                    TwitterOp.PostTweet(1, 0), TwitterOp.PostTweet(1, 1), TwitterOp.PostTweet(1, 2),
                    TwitterOp.PostTweet(1, 3), TwitterOp.PostTweet(1, 4), TwitterOp.PostTweet(1, 5),
                    TwitterOp.PostTweet(1, 6), TwitterOp.PostTweet(1, 7), TwitterOp.PostTweet(1, 8),
                    TwitterOp.PostTweet(1, 9), TwitterOp.PostTweet(1, 10), TwitterOp.PostTweet(1, 11),
                    TwitterOp.PostTweet(1, 12), TwitterOp.PostTweet(1, 13), TwitterOp.PostTweet(1, 14),
                    TwitterOp.GetNewsFeed(1),
                ],
                [
                    null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                    [14, 13, 12, 11, 10, 9, 8, 7, 6, 5],
                ]
            },
            {
                [
                    TwitterOp.Follow(1, 2),
                    TwitterOp.PostTweet(1, 10),
                    TwitterOp.PostTweet(2, 20),
                    TwitterOp.PostTweet(1, 11),
                    TwitterOp.PostTweet(2, 21),
                    TwitterOp.GetNewsFeed(1),
                ],
                [null, null, null, null, null, [21, 11, 20, 10]]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void TwitterByGatherAllAndSort_LeetCodeExamples_ReturnsMostRecentTenAcrossFollowedSources(
        TwitterOp[] operations, List<int>?[] expected) =>
        RunScript(new TwitterByGatherAllAndSort(), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void TwitterByHeapKWayMerge_LeetCodeExamples_ReturnsMostRecentTenAcrossFollowedSources(
        TwitterOp[] operations, List<int>?[] expected) =>
        RunScript(new TwitterByHeapKWayMerge(), operations, expected);

    private static void RunScript(ITwitterStrategy strategy, TwitterOp[] operations, List<int>?[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(strategy));
        }
    }
}

// One call in a Twitter script: which method to invoke and with what arguments.
// Pure dispatch, built via the named factories below so a script (like Examples
// above) reads like the LeetCode call sequence it replays.
public readonly record struct TwitterOp
{
    private readonly Kind _kind;
    private readonly int _a;
    private readonly int _b;

    private TwitterOp(Kind kind, int a, int b)
    {
        _kind = kind;
        _a = a;
        _b = b;
    }

    public static TwitterOp PostTweet(int userId, int tweetId) => new(Kind.PostTweet, userId, tweetId);

    public static TwitterOp Follow(int followerId, int followeeId) => new(Kind.Follow, followerId, followeeId);

    public static TwitterOp Unfollow(int followerId, int followeeId) => new(Kind.Unfollow, followerId, followeeId);

    public static TwitterOp GetNewsFeed(int userId) => new(Kind.GetNewsFeed, userId, 0);

    // null for the three void calls, the returned feed for GetNewsFeed - so a
    // script runner can assert against one expected value per operation uniformly.
    // Internal, not public: ITwitterStrategy is internal to DesignTwitterSolution,
    // and only this same assembly's RunScript ever calls Apply.
    internal List<int>? Apply(ITwitterStrategy strategy)
    {
        switch (_kind)
        {
            case Kind.PostTweet:
                strategy.PostTweet(_a, _b);
                return null;
            case Kind.Follow:
                strategy.Follow(_a, _b);
                return null;
            case Kind.Unfollow:
                strategy.Unfollow(_a, _b);
                return null;
            default:
                return strategy.GetNewsFeed(_a);
        }
    }

    private enum Kind
    {
        PostTweet,
        Follow,
        Unfollow,
        GetNewsFeed,
    }
}
