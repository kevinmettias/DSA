using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignTwitter;

// LeetCode 355. Design Twitter: each user's own tweets live in a DynamicArray (append-only,
// already in post order), a follower's followees live in a HashMap<int,bool> (Set<TValue>
// itself exposes no enumeration, and getNewsFeed needs to walk every followee, so this
// composes HashMap directly the same way Set<Element> is itself built on top of it), and
// getNewsFeed is a k-way merge of "most recent tweet per followed source" via this repo's own
// max Heap<Element,TOrder> - the same engine ShortestPath's Dijkstra/A* frontier uses, just
// ordered by (Time, TweetId, SourceIndex, Position) via MaxHeapOrder's ValueTuple.CompareTo
// instead of ByPriorityOrder's (Node, Priority) projection.
public sealed partial class DesignTwitterTests
{
    [Fact]
    public void Twitter_LeetCodeExample_TracksFeedAcrossFollowAndUnfollow()
    {
        var twitter = new Twitter();

        twitter.PostTweet(1, 5);
        Assert.Equal([5], twitter.GetNewsFeed(1));

        twitter.Follow(1, 2);
        twitter.PostTweet(2, 6);
        Assert.Equal([6, 5], twitter.GetNewsFeed(1));

        twitter.Unfollow(1, 2);
        Assert.Equal([5], twitter.GetNewsFeed(1));
    }

    [Fact]
    public void GetNewsFeed_MoreThanTenTweetsFromOneUser_ReturnsOnlyMostRecentTen()
    {
        var twitter = new Twitter();

        for (var tweetId = 0; tweetId < 15; tweetId++)
        {
            twitter.PostTweet(1, tweetId);
        }

        var feed = twitter.GetNewsFeed(1);

        Assert.Equal(Enumerable.Range(5, 10).Reverse(), feed);
    }

    [Fact]
    public void GetNewsFeed_InterleavesTweetsFromSelfAndFollowees_MostRecentFirst()
    {
        var twitter = new Twitter();

        twitter.Follow(1, 2);
        twitter.PostTweet(1, 10);
        twitter.PostTweet(2, 20);
        twitter.PostTweet(1, 11);
        twitter.PostTweet(2, 21);

        Assert.Equal([21, 11, 20, 10], twitter.GetNewsFeed(1));
    }

    private sealed class Twitter
    {
        private const int FeedSize = 10;

        private int _clock;
        private readonly HashMap<int, DynamicArray<(int Time, int TweetId)>> _tweetsByUser = new();
        private readonly HashMap<int, HashMap<int, bool>> _followeesByUser = new();

        public void PostTweet(int userId, int tweetId)
        {
            if (!_tweetsByUser.TryGetValue(userId, out var tweets))
            {
                tweets = new DynamicArray<(int, int)>();
                _tweetsByUser.Set(userId, tweets);
            }

            tweets.Add((_clock++, tweetId));
        }

        public void Follow(int followerId, int followeeId)
        {
            if (!_followeesByUser.TryGetValue(followerId, out var followees))
            {
                followees = new HashMap<int, bool>();
                _followeesByUser.Set(followerId, followees);
            }

            followees.Set(followeeId, true);
        }

        public void Unfollow(int followerId, int followeeId)
        {
            if (_followeesByUser.TryGetValue(followerId, out var followees))
            {
                followees.TryRemove(followeeId);
            }
        }

        public List<int> GetNewsFeed(int userId)
        {
            var sourceIds = BuildSourceIds(userId);
            var sources = CollectSources(sourceIds);
            var heap = SeedHeap(sources);

            var feed = new List<int>();

            while (feed.Count < FeedSize && heap.TryPop(out var top))
            {
                ProcessFeedEntry(heap, sources, top, feed);
            }

            return feed;
        }

        private List<int> BuildSourceIds(int userId)
        {
            var sourceIds = new List<int> { userId };

            if (_followeesByUser.TryGetValue(userId, out var followees))
            {
                sourceIds.AddRange(followees.Keys);
            }

            return sourceIds;
        }

        private List<DynamicArray<(int Time, int TweetId)>> CollectSources(List<int> sourceIds)
        {
            var sources = new List<DynamicArray<(int Time, int TweetId)>>();

            foreach (var id in sourceIds)
            {
                if (_tweetsByUser.TryGetValue(id, out var tweets) && tweets.Count > 0)
                {
                    sources.Add(tweets);
                }
            }

            return sources;
        }

        private static Heap<(int Time, int TweetId, int SourceIndex, int Position), MaxHeapOrder<(int, int, int, int)>> SeedHeap(
            List<DynamicArray<(int Time, int TweetId)>> sources)
        {
            var heap = new Heap<(int Time, int TweetId, int SourceIndex, int Position), MaxHeapOrder<(int, int, int, int)>>();

            for (var i = 0; i < sources.Count; i++)
            {
                var position = sources[i].Count - 1;
                var (time, tweetId) = sources[i].Get(position);
                heap.Push((time, tweetId, i, position));
            }

            return heap;
        }

        private static void ProcessFeedEntry(
            Heap<(int Time, int TweetId, int SourceIndex, int Position), MaxHeapOrder<(int, int, int, int)>> heap,
            List<DynamicArray<(int Time, int TweetId)>> sources,
            (int Time, int TweetId, int SourceIndex, int Position) top,
            List<int> feed)
        {
            feed.Add(top.TweetId);

            if (top.Position == 0)
            {
                return;
            }

            var nextPosition = top.Position - 1;
            var (time, tweetId) = sources[top.SourceIndex].Get(nextPosition);
            heap.Push((time, tweetId, top.SourceIndex, nextPosition));
        }
    }
}
