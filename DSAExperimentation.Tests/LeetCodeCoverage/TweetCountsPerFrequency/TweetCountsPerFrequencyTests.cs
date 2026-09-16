using ITweetCountsStrategy = DSAExperimentation.LeetCode.TweetCountsPerFrequency.TweetCountsPerFrequencySolution.ITweetCountsStrategy;
using TweetCountsByFlatListFilter = DSAExperimentation.LeetCode.TweetCountsPerFrequency.TweetCountsPerFrequencySolution.TweetCountsByFlatListFilter;
using TweetCountsByHashMapGroupedByName = DSAExperimentation.LeetCode.TweetCountsPerFrequency.TweetCountsPerFrequencySolution.TweetCountsByHashMapGroupedByName;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TweetCountsPerFrequency;

// Harness only. Both strategies are TweetCountsPerFrequencySolution's - this file
// replays LeetCode's published call sequence, plus the edge cases the original
// test covered (a name with no recorded tweets) and two more (a window that starts
// after the first tweet, and two names recorded interleaved so a query has to
// filter by name), against each ITweetCountsStrategy implementation via a small
// operation script, so a failure still names the strategy that broke.
// TweetOp.Apply is pure dispatch - no bucketing logic of its own.
public sealed class TweetCountsPerFrequencyTests
{
    // LC 1348's own frequency names, stated here rather than read off the solution.
    // A test that asked the solution what "minute" is would still pass after the
    // solution's spelling changed, which is the one failure this file exists to
    // catch; these are the names the published call sequence uses, so the test
    // states them. A type of its own rather than a bare string, so a query's
    // frequency and the tweet name it filters by cannot be handed over swapped.
    private static readonly TweetFrequency Minute = new("minute");
    private static readonly TweetFrequency Hour = new("hour");
    private static readonly TweetFrequency Day = new("day");

    public static TheoryData<TweetOp[], List<int>?[]> Examples =>
        new()
        {
            {
                [
                    TweetOp.Record("tweet3", 0),
                    TweetOp.Record("tweet3", 60),
                    TweetOp.Record("tweet3", 10),
                    TweetOp.Query(Minute, "tweet3", 0, 59),
                    TweetOp.Query(Minute, "tweet3", 0, 60),
                    TweetOp.Record("tweet3", 120),
                    TweetOp.Query(Hour, "tweet3", 0, 210),
                ],
                [null, null, null, [2], [2, 1], null, [4]]
            },
            {
                [
                    TweetOp.Record("tweet1", 5),
                    TweetOp.Query(Day, "tweet2", 0, 86_400),
                ],
                [null, [0, 0]]
            },
            {
                [
                    TweetOp.Record("tweet5", 0),
                    TweetOp.Record("tweet5", 100),
                    TweetOp.Record("tweet5", 200),
                    TweetOp.Query(Minute, "tweet5", 100, 200),
                ],
                [null, null, null, [1, 1]]
            },
            {
                [
                    TweetOp.Record("a", 0),
                    TweetOp.Record("b", 0),
                    TweetOp.Record("a", 3_600),
                    TweetOp.Record("b", 3_600),
                    TweetOp.Record("b", 5_000),
                    TweetOp.Query(Hour, "a", 0, 7_199),
                    TweetOp.Query(Hour, "b", 0, 7_199),
                ],
                [null, null, null, null, null, [1, 1], [1, 2]]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void TweetCountsByFlatListFilter_LeetCodeExamples_BucketsByRequestedFrequency(
        TweetOp[] operations, List<int>?[] expected) =>
        RunScript(new TweetCountsByFlatListFilter(), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void TweetCountsByHashMapGroupedByName_LeetCodeExamples_BucketsByRequestedFrequency(
        TweetOp[] operations, List<int>?[] expected) =>
        RunScript(new TweetCountsByHashMapGroupedByName(), operations, expected);

    private static void RunScript(ITweetCountsStrategy strategy, TweetOp[] operations, List<int>?[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(strategy));
        }
    }

    // The bucket name a query groups by - LC's own "minute" / "hour" / "day". It is its
    // own type rather than a string so a query's frequency cannot be handed over in the
    // place of the tweet name it filters by.
    public readonly record struct TweetFrequency(string Name)
    {
        // A frequency with no name: what a recordTweet call carries, since LC's
        // recordTweet takes only a name and a time and Apply never reads the frequency
        // for one.
        public static TweetFrequency None => new(string.Empty);
    }

    // One call in a TweetCounts script: which method to invoke and with what
    // arguments. Pure dispatch, built via the two named factories below so a script
    // (like Examples above) reads like the LeetCode call sequence it replays. Nested,
    // because it is this harness's own way of stating a script.
    public readonly record struct TweetOp(
        TweetFrequency freq, string tweetName, int startTime, int endTime, bool isQuery)
    {
        public static TweetOp Record(string tweetName, int time) =>
            new(TweetFrequency.None, tweetName, time, time, false);

        public static TweetOp Query(TweetFrequency freq, string tweetName, int startTime, int endTime) =>
            new(freq, tweetName, startTime, endTime, true);

        // null for the void recordTweet call, the returned buckets for a query - so a
        // script runner can assert against one expected value per operation uniformly.
        // Internal, not public: ITweetCountsStrategy is internal to
        // TweetCountsPerFrequencySolution, and only this same assembly's RunScript
        // ever calls Apply.
        internal List<int>? Apply(ITweetCountsStrategy strategy)
        {
            if (isQuery)
            {
                return strategy.GetTweetCountsPerFrequency(freq.Name, tweetName, startTime, endTime);
            }

            strategy.RecordTweet(tweetName, startTime);
            return null;
        }
    }
}
