using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.LeetCode.DesignTwitter;

// LeetCode 355. Design Twitter: an instance API (postTweet/follow/unfollow/
// getNewsFeed) rather than a pure function, so - as with DesignAuctionSystem and
// DesignTaskManager - "every strategy for the problem" (§17.3) takes the form of
// two full classes implementing the shared ITwitterStrategy surface below. LC 355's
// own Twitter() constructor takes no initial state, so there is no separate
// "prepare input" step to hoist into a benchmark's [GlobalSetup]; each [Benchmark]
// arm constructs its own instance and replays the same call script instead.
// Only getNewsFeed's merge differs between the two arms - postTweet/follow/unfollow
// are the same bookkeeping in both, restated per class because each keeps its
// tweets in whatever container its own getNewsFeed strategy wants to walk.
internal static class DesignTwitterSolution
{
    private const int FeedSize = 10;

    // The shared surface both strategies implement, so the test and benchmark
    // harnesses can replay one call script against either strategy without
    // restating it.
    internal interface ITwitterStrategy
    {
        void PostTweet(int userId, int tweetId);

        void Follow(int followerId, int followeeId);

        void Unfollow(int followerId, int followeeId);

        List<int> GetNewsFeed(int userId);
    }

    // The textbook answer: BCL Dictionary/List/HashSet throughout, and
    // GetNewsFeed concatenates every followed source's entire tweet history into
    // one list before sorting it descending by time - deliberately without this
    // repo's Heap, the arm the k-way merge strategy below has to justify itself
    // against.
    internal sealed class TwitterByGatherAllAndSort : ITwitterStrategy
    {
        private int _clock;
        private readonly Dictionary<int, List<(int Time, int TweetId)>> _tweetsByUser = new();
        private readonly Dictionary<int, HashSet<int>> _followeesByUser = new();

        public void PostTweet(int userId, int tweetId)
        {
            if (!_tweetsByUser.TryGetValue(userId, out var tweets))
            {
                tweets = new List<(int, int)>();
                _tweetsByUser[userId] = tweets;
            }

            tweets.Add((_clock++, tweetId));
        }

        public void Follow(int followerId, int followeeId)
        {
            if (!_followeesByUser.TryGetValue(followerId, out var followees))
            {
                followees = new HashSet<int>();
                _followeesByUser[followerId] = followees;
            }

            followees.Add(followeeId);
        }

        public void Unfollow(int followerId, int followeeId)
        {
            if (_followeesByUser.TryGetValue(followerId, out var followees))
            {
                followees.Remove(followeeId);
            }
        }

        public List<int> GetNewsFeed(int userId)
        {
            var all = new List<(int Time, int TweetId)>();

            AppendSource(all, userId);

            if (_followeesByUser.TryGetValue(userId, out var followees))
            {
                foreach (var followeeId in followees)
                {
                    AppendSource(all, followeeId);
                }
            }

            all.Sort((a, b) => b.Time.CompareTo(a.Time));
            return all.Take(FeedSize).Select(tweet => tweet.TweetId).ToList();
        }

        private void AppendSource(List<(int Time, int TweetId)> all, int sourceUserId)
        {
            if (_tweetsByUser.TryGetValue(sourceUserId, out var tweets))
            {
                all.AddRange(tweets);
            }
        }
    }

    // This repo's own HashMap for per-user tweet history (a DynamicArray, already
    // in post order) and follow sets, and GetNewsFeed is a k-way merge of "most
    // recent tweet per followed source" via this repo's own max Heap<Element,
    // TOrder> - the same engine ShortestPath's Dijkstra/A* frontier uses, just
    // ordered by (Time, TweetId, SourceIndex, Position) via MaxHeapOrder's
    // ValueTuple.CompareTo instead of ByPriorityOrder's (Node, Priority)
    // projection.
    internal sealed class TwitterByHeapKWayMerge : ITwitterStrategy
    {
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
