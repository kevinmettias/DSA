using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.LeetCode.Solutions.NumberOfRecentCalls;

// LeetCode 933. Number of Recent Calls: a RecentCounter is fed strictly increasing
// request timestamps one at a time and answers, for each, how many requests fell in
// the inclusive window [t - 3000, t].
//
// This is a design problem - LeetCode's own shape is a stateful object answering one
// ping(t) at a time, constructed from nothing - so each strategy is a factory taking
// no input and returning an IRecentCounter, the object the catalog harness replays a
// call script on (ARCHITECTURE.md §17.13). A stream of pings is replayed by the
// harness, one call at a time, so each strategy is the one class answering them.
//
// CreateByFullHistoryRescan keeps every timestamp ever pinged and rescans all of it
// on every call (O(calls) per ping, O(calls^2) over the stream).
// CreateBySlidingWindowQueue instead uses this repo's own Queue<int> as a FIFO
// sliding window, evicting stale timestamps from the front the instant they fall
// outside the window - each timestamp is enqueued and dequeued exactly once across
// the whole stream, so the total cost is O(calls) amortized.
[LeetCodeQuestion("number-of-recent-calls", LeetCodeAnswerOrder.AsWritten)]
internal static class NumberOfRecentCallsSolution
{
    private const int WindowMilliseconds = 3000;

    // Deliberately written without this repo's primitives - a growing BCL List and a
    // full rescan per call is the baseline the sliding window has to justify itself
    // against.
    public static IRecentCounter CreateByFullHistoryRescan() => new FullHistoryRescanCounter();

    public static IRecentCounter CreateBySlidingWindowQueue() => new SlidingWindowQueueCounter();

    // LeetCode's RecentCounter: ping records a request at t and answers how many
    // requests fell in [t - 3000, t].
    internal interface IRecentCounter
    {
        int Ping(int time);
    }

    // Every timestamp ever pinged, in a BCL List, rescanned whole on each ping.
    private sealed class FullHistoryRescanCounter : IRecentCounter
    {
        private readonly List<int> _history = [];

        public int Ping(int time)
        {
            _history.Add(time);

            return CountWithinWindow(time);
        }

        // Walks the entire recorded history, counting every request that has not yet
        // aged out of the window ending at timestamp.
        private int CountWithinWindow(int timestamp)
        {
            var count = 0;

            foreach (var seen in _history)
            {
                if (seen >= timestamp - WindowMilliseconds)
                {
                    count++;
                }
            }

            return count;
        }
    }

    // The queue holds exactly the requests still inside the window, so its Count is
    // the answer and no scan is needed: Enqueue at the back, drop from the front.
    private sealed class SlidingWindowQueueCounter : IRecentCounter
    {
        private readonly RepoQueue _pings = new();

        public int Ping(int time)
        {
            _pings.Enqueue(time);
            DropExpired(time);

            return _pings.Count;
        }

        // Timestamps arrive in increasing order, so everything older than the window
        // is at the front and leaves in one uninterrupted run.
        private void DropExpired(int timestamp)
        {
            while (_pings.TryPeek(out var oldest) && oldest < timestamp - WindowMilliseconds)
            {
                _pings.TryDequeue(out _);
            }
        }
    }
}
