using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.TweetCountsPerFrequency;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Tweet Counts Per Frequency (LC 1348): both strategies record the same TweetCount
// tweets once in [GlobalSetup] (recording is O(1) amortized either way - one array
// append vs. one HashMap-bucketed append); the benchmark measures only what
// getTweetCountsPerFrequency itself pays per call. TweetCountsByFlatListFilter
// keeps every tweet in one flat list and filters it by name on every query - O(total
// tweets) per query, no matter how few belong to the requested name.
// TweetCountsByHashMapGroupedByName instead groups tweet times by name into their
// own DynamicArray inside a HashMap<string,DynamicArray<int>> as they're recorded
// (the same per-key-bucket shape DesignTwitterBenchmarks' _tweetsByUser already
// uses), so a query only ever scans that one name's own times - O(tweets for that
// name). Tweets are spread across NameCount distinct names so "that name's own
// times" is a small slice of the total. Every tweet falls inside the one queried
// window, which spans LC 1348's widest 10^4 seconds, and TweetCount stops at 9,999 so
// the recordings and the query make its 10^4 calls.
//
// Harness only: both arms are TweetCountsPerFrequencySolution's, the same classes
// TweetCountsPerFrequencySolutionTests proves correct. Each arm sums the returned buckets
// rather than discarding them, so the query can't be eliminated as dead code.
public class TweetCountsPerFrequencyBenchmarks
{
    private const int NameCount = 200;
    private const int RandomSeed = 1348; // LC 1348
    private const string NamePrefix = "tweet";
    private const int WindowStart = 0;

    // LC 1348's widest query window, endTime - startTime.
    private const int MaxWindowSpan = 10_000;
    private const int WindowEnd = WindowStart + MaxWindowSpan;

    private TweetCountsPerFrequencySolution.ITweetCountsStrategy _flatList = null!;

    private TweetCountsPerFrequencySolution.ITweetCountsStrategy _grouped = null!;
    [Params(2_000, 9_999)]
    public int TweetCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _flatList = new TweetCountsPerFrequencySolution.TweetCountsByFlatListFilter();
        _grouped = new TweetCountsPerFrequencySolution.TweetCountsByHashMapGroupedByName();

        for (var i = 0; i < TweetCount; i++)
        {
            var name = NamePrefix + random.Next(NameCount);
            var time = random.Next(WindowStart, WindowEnd + 1);

            _flatList.RecordTweet(name, time);
            _grouped.RecordTweet(name, time);
        }
    }

    [Benchmark(Baseline = true)]
    public int FlatListFilterPerQuery() => Query(_flatList);

    [Benchmark]
    public int HashMapGroupedByName() => Query(_grouped);

    private static int Query(TweetCountsPerFrequencySolution.ITweetCountsStrategy strategy) =>
        strategy.GetTweetCountsPerFrequency(
            TweetCountsScenario.QueryFrequency, TweetCountsScenario.QueryName, WindowStart, WindowEnd).Sum();
}
