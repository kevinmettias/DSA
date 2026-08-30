using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Design Twitter (LC 355): getNewsFeed is the operation worth benchmarking - merging the
// most recent tweets across every followed source into a top-10 feed. GatherAllAndSort is
// the naive approach: concatenate every source's entire tweet history into one list and
// sort it descending by time, O(m log m) where m is the total tweet count across all
// followed sources. HeapKWayMerge instead composes this repo's own max Heap<Element,TOrder>
// to pull only the FeedSize most recent tweets via a k-way merge seeded with one entry per
// source - O(FollowedUsers log FollowedUsers + FeedSize log FollowedUsers), independent of
// how many tweets each source has posted in total. Tweet times are interleaved randomly
// across sources (not one source's whole history at a time) so the top 10 genuinely draw
// from many different sources instead of always the most-recently-seeded one.
[MemoryDiagnoser]
public class DesignTwitterBenchmarks
{
    private const int FeedSize = 10;
    private const int TweetsPerSource = 20;

    [Params(50, 500)]
    public int FollowedUsers;

    private DynamicArray<(int Time, int TweetId)>[] _sources = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(13);
        var tweetsPerSource = Enumerable.Range(0, FollowedUsers)
            .Select(_ => new DynamicArray<(int Time, int TweetId)>())
            .ToArray();
        var totalTweets = FollowedUsers * TweetsPerSource;

        for (var time = 0; time < totalTweets; time++)
        {
            var source = random.Next(FollowedUsers);
            tweetsPerSource[source].Add((time, time));
        }

        _sources = tweetsPerSource.Where(tweets => tweets.Count > 0).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int GatherAllAndSort()
    {
        var all = new List<(int Time, int TweetId)>();

        foreach (var tweets in _sources)
        {
            for (var i = 0; i < tweets.Count; i++)
            {
                all.Add(tweets.Get(i));
            }
        }

        all.Sort((a, b) => b.Time.CompareTo(a.Time));
        return all.Take(FeedSize).Count();
    }

    [Benchmark]
    public int HeapKWayMerge()
    {
        var heap = new Heap<(int Time, int TweetId, int SourceIndex, int Position), MaxHeapOrder<(int, int, int, int)>>();

        for (var i = 0; i < _sources.Length; i++)
        {
            var position = _sources[i].Count - 1;
            var (time, tweetId) = _sources[i].Get(position);
            heap.Push((time, tweetId, i, position));
        }

        var feedCount = 0;

        while (feedCount < FeedSize && heap.TryPop(out var top))
        {
            feedCount++;

            if (top.Position == 0)
            {
                continue;
            }

            var nextPosition = top.Position - 1;
            var (time, tweetId) = _sources[top.SourceIndex].Get(nextPosition);
            heap.Push((time, tweetId, top.SourceIndex, nextPosition));
        }

        return feedCount;
    }
}
