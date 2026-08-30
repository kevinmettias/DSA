using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Tweet Counts Per Frequency (LC 1348): both strategies record the same TweetCount
// tweets once in GlobalSetup (recording is O(1) amortized either way - one array
// append vs. one HashMap-bucketed append); the benchmark measures only what
// getTweetCountsPerFrequency itself pays per call. FlatListFilterPerQuery keeps
// every tweet in one flat list and filters it by name on every query - O(total
// tweets) per query, no matter how few belong to the requested name.
// HashMapGroupedByName instead groups tweet times by name into their own
// DynamicArray inside a HashMap<string,DynamicArray<int>> as they're recorded (the
// same per-key-bucket shape DesignTwitterBenchmarks' _tweetsByUser already uses), so
// a query only ever scans that one name's own times - O(tweets for that name).
// Tweets are spread across NameCount distinct names so "that name's own times" is a
// small slice of the total, the same lopsided-grouping intent
// TweetCountsPerFrequencyTests' own composition demonstrates.
[MemoryDiagnoser]
public class TweetCountsPerFrequencyBenchmarks
{
    private const int NameCount = 200;
    private const int SecondsPerHour = 3_600;

    [Params(2_000, 20_000)]
    public int TweetCount;

    private (string Name, int Time)[] _flatTweets = null!;
    private HashMap<string, DynamicArray<int>> _timesByName = null!;
    private string _queryName = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1348);
        _flatTweets = new (string, int)[TweetCount];
        _timesByName = new HashMap<string, DynamicArray<int>>();

        for (var i = 0; i < TweetCount; i++)
        {
            var name = "tweet" + random.Next(NameCount);
            var time = random.Next(0, 10 * SecondsPerHour);
            _flatTweets[i] = (name, time);

            if (!_timesByName.TryGetValue(name, out var times))
            {
                times = new DynamicArray<int>();
                _timesByName.Set(name, times);
            }

            times.Add(time);
        }

        _queryName = "tweet0";
    }

    [Benchmark(Baseline = true)]
    public int FlatListFilterPerQuery()
    {
        var buckets = new int[10];

        foreach (var (name, time) in _flatTweets)
        {
            if (name == _queryName)
            {
                buckets[time / SecondsPerHour]++;
            }
        }

        return buckets.Sum();
    }

    [Benchmark]
    public int HashMapGroupedByName()
    {
        var buckets = new int[10];

        if (_timesByName.TryGetValue(_queryName, out var queryTimes))
        {
            for (var i = 0; i < queryTimes.Count; i++)
            {
                buckets[queryTimes.Get(i) / SecondsPerHour]++;
            }
        }

        return buckets.Sum();
    }
}
