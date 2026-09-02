using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TweetCountsPerFrequency;

// LeetCode 1348. Tweet Counts Per Frequency: each tweet name's recorded times live in
// their own DynamicArray inside a HashMap<string,DynamicArray<int>> - the same
// per-key-bucket shape DesignTwitter's _tweetsByUser already uses - so a query only
// ever scans that one name's own times instead of every tweet ever recorded, then
// buckets each in-range time by straight division into the requested chunk size.
public sealed partial class TweetCountsPerFrequencyTests
{
    [Fact]
    public void GetTweetCountsPerFrequency_LeetCodeExample_BucketsByRequestedFrequency()
    {
        var tweetCounts = new TweetCounts();

        tweetCounts.RecordTweet("tweet3", 0);
        tweetCounts.RecordTweet("tweet3", 60);
        tweetCounts.RecordTweet("tweet3", 10);

        var minuteBucketsNarrowWindow = tweetCounts.GetTweetCountsPerFrequency("minute", "tweet3", 0, 59);
        Assert.Equal([2], minuteBucketsNarrowWindow);

        var minuteBucketsWiderWindow = tweetCounts.GetTweetCountsPerFrequency("minute", "tweet3", 0, 60);
        Assert.Equal([2, 1], minuteBucketsWiderWindow);

        tweetCounts.RecordTweet("tweet3", 120);

        var hourBuckets = tweetCounts.GetTweetCountsPerFrequency("hour", "tweet3", 0, 210);
        Assert.Equal([4], hourBuckets);
    }

    [Fact]
    public void GetTweetCountsPerFrequency_NameWithNoRecordedTweets_ReturnsAllZeroBuckets()
    {
        var tweetCounts = new TweetCounts();

        tweetCounts.RecordTweet("tweet1", 5);

        var dayBucketsForUnrecordedName = tweetCounts.GetTweetCountsPerFrequency("day", "tweet2", 0, 86_400);
        Assert.Equal([0, 0], dayBucketsForUnrecordedName);
    }

    private sealed class TweetCounts
    {
        private const int SecondsPerMinute = 60;
        private const int SecondsPerHour = 3_600;
        private const int SecondsPerDay = 86_400;

        private readonly HashMap<string, DynamicArray<int>> _timesByName = new();

        public void RecordTweet(string tweetName, int time)
        {
            if (!_timesByName.TryGetValue(tweetName, out var times))
            {
                times = new DynamicArray<int>();
                _timesByName.Set(tweetName, times);
            }

            times.Add(time);
        }

        public List<int> GetTweetCountsPerFrequency(string freq, string tweetName, int startTime, int endTime)
        {
            var window = new TimeWindow(startTime, endTime);
            var chunkSeconds = ResolveChunkSeconds(freq);
            var buckets = CreateEmptyBuckets(window, chunkSeconds);

            if (!_timesByName.TryGetValue(tweetName, out var times))
            {
                return buckets;
            }

            BucketTweetTimes(times, window, chunkSeconds, buckets);

            return buckets;
        }

        private static int ResolveChunkSeconds(string freq)
            => freq switch
            {
                "minute" => SecondsPerMinute,
                "hour" => SecondsPerHour,
                _ => SecondsPerDay,
            };

        private static List<int> CreateEmptyBuckets(TimeWindow window, int chunkSeconds)
            => new(new int[(window.EndTime - window.StartTime) / chunkSeconds + 1]);

        private static void BucketTweetTimes(DynamicArray<int> times, TimeWindow window, int chunkSeconds, List<int> buckets)
        {
            for (var i = 0; i < times.Count; i++)
            {
                var time = times.Get(i);
                if (time >= window.StartTime && time <= window.EndTime)
                {
                    buckets[(time - window.StartTime) / chunkSeconds]++;
                }
            }
        }

        private readonly record struct TimeWindow(int StartTime, int EndTime);
    }
}
