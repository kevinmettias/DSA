using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.TweetCountsPerFrequency;

// LeetCode 1348. Tweet Counts Per Frequency: recordTweet(name, time) then
// getTweetCountsPerFrequency(freq, name, startTime, endTime), which chunks
// [startTime, endTime] into minute/hour/day buckets and counts that name's tweets
// per chunk.
//
// An instance API rather than a pure function, so - as with DesignTwitter -
// "every strategy for the problem" (§17.3) takes the form of two full classes
// implementing the shared ITweetCountsStrategy surface below. Recording is the
// step the two disagree about: the baseline appends every tweet to one flat list
// and pays for filtering it by name on every query; the composed strategy groups
// each name's times into their own bucket as they arrive, so a query only ever
// scans that one name's times.
//
// LC 1348's own TweetCounts() constructor takes no initial state, so there is no
// separate "prepared input" overload to hoist (§17.4): the recorded instance is
// itself the prepared input a benchmark builds in [GlobalSetup].
internal static class TweetCountsPerFrequencySolution
{
    private const int SecondsPerMinute = 60;
    private const int SecondsPerHour = 3_600;
    private const int SecondsPerDay = 86_400;

    // The three frequency names LC 1348 fixes. Private because nothing outside this
    // type needs the *declaration*: the tests and the benchmark each replay LC's own
    // call script, and a script states the names it is replaying. ResolveChunkSeconds
    // below is the only reader, and it is what makes these three a closed set the
    // program chooses between rather than three loose strings.
    private const string Minute = "minute";
    private const string Hour = "hour";
    private const string Day = "day";

    // The chunk width each frequency name stands for. LC 1348 fixes these three
    // names and nothing else, so both strategies read them from here rather than
    // each restating the conversion.
    private static int ResolveChunkSeconds(string freq)
        => freq switch
        {
            Minute => SecondsPerMinute,
            Hour => SecondsPerHour,
            _ => SecondsPerDay,
        };

    private static int BucketCount(int startTime, int endTime, int chunkSeconds)
        => (endTime - startTime) / chunkSeconds + 1;

    // A tweet only lands in a bucket when it was recorded inside the query window,
    // both ends included.
    private static bool IsWithinWindow(int time, int startTime, int endTime)
        => time >= startTime && time <= endTime;

    // The shared surface both strategies implement, so the test and benchmark
    // harnesses can replay one call script against either strategy without
    // restating it.
    internal interface ITweetCountsStrategy
    {
        void RecordTweet(string tweetName, int time);

        List<int> GetTweetCountsPerFrequency(string freq, string tweetName, int startTime, int endTime);
    }

    // The textbook answer: one flat BCL List of every (name, time) ever recorded,
    // re-scanned and filtered by name on every query - O(total tweets) per query,
    // no matter how few belong to the requested name. Deliberately written
    // without this repo's primitives, the arm the grouped strategy below has to
    // justify itself against.
    internal sealed class TweetCountsByFlatListFilter : ITweetCountsStrategy
    {
        private readonly List<(string Name, int Time)> _tweets = [];

        public void RecordTweet(string tweetName, int time) => _tweets.Add((tweetName, time));

        public List<int> GetTweetCountsPerFrequency(string freq, string tweetName, int startTime, int endTime)
        {
            var chunkSeconds = ResolveChunkSeconds(freq);
            var buckets = new List<int>(new int[BucketCount(startTime, endTime, chunkSeconds)]);

            foreach (var (name, time) in _tweets)
            {
                if (name == tweetName && IsWithinWindow(time, startTime, endTime))
                {
                    buckets[(time - startTime) / chunkSeconds]++;
                }
            }

            return buckets;
        }
    }

    // This repo's own HashMap keyed by tweet name, each name's times kept in their
    // own DynamicArray - the same per-key-bucket shape DesignTwitter's
    // _tweetsByUser uses - so a query only ever walks that one name's times and
    // buckets each in-range one by straight division into the requested chunk.
    internal sealed class TweetCountsByHashMapGroupedByName : ITweetCountsStrategy
    {
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
            var chunkSeconds = ResolveChunkSeconds(freq);
            var buckets = new List<int>(new int[BucketCount(startTime, endTime, chunkSeconds)]);

            if (!_timesByName.TryGetValue(tweetName, out var times))
            {
                return buckets;
            }

            BucketTweetTimes(times, (startTime, endTime), chunkSeconds, buckets);

            return buckets;
        }

        private static void BucketTweetTimes(
            DynamicArray<int> times, (int StartTime, int EndTime) window, int chunkSeconds, List<int> buckets)
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
    }
}
