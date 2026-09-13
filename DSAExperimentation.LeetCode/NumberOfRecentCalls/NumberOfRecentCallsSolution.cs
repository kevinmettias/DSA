using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.LeetCode.NumberOfRecentCalls;

// LeetCode 933. Number of Recent Calls: a RecentCounter is fed strictly increasing
// request timestamps one at a time and answers, for each, how many requests fell in
// the inclusive window [t - 3000, t]. The design-problem framing is a stream of
// ping(t) calls; the answer that stream produces is the sequence of counts, so both
// strategies here take the whole timestamp stream and return its counts in order -
// the shape that can actually be asserted and measured against itself, the same
// framing OnlineStockSpanSolution uses for LC 901's next(price) stream.
//
// PingCountsByFullHistoryRescan keeps every timestamp ever pinged and rescans all of
// it on every call (O(calls) per ping, O(calls^2) over the stream).
// PingCountsBySlidingWindowQueue instead uses this repo's own Queue<int> as a FIFO
// sliding window, evicting stale timestamps from the front the instant they fall
// outside the window - each timestamp is enqueued and dequeued exactly once across
// the whole stream, so the total cost is O(calls) amortized.
internal static class NumberOfRecentCallsSolution
{
    private const int WindowMilliseconds = 3000;

    // Deliberately written without this repo's primitives - a growing BCL List and a
    // full rescan per call is the baseline the sliding window has to justify itself
    // against.
    public static int[] PingCountsByFullHistoryRescan(int[] timestamps)
    {
        var counts = new int[timestamps.Length];
        var history = new List<int>();

        for (var call = 0; call < timestamps.Length; call++)
        {
            history.Add(timestamps[call]);
            counts[call] = CountWithinWindow(history, timestamps[call]);
        }

        return counts;
    }

    // Walks the entire recorded history, counting every request that has not yet
    // aged out of the window ending at t.
    private static int CountWithinWindow(List<int> history, int t)
    {
        var count = 0;

        foreach (var seen in history)
        {
            if (seen >= t - WindowMilliseconds)
            {
                count++;
            }
        }

        return count;
    }

    // The queue holds exactly the requests still inside the window, so its Count is
    // the answer and no scan is needed: Enqueue at the back, drop from the front.
    public static int[] PingCountsBySlidingWindowQueue(int[] timestamps)
    {
        var counts = new int[timestamps.Length];
        var pings = new RepoQueue();

        for (var call = 0; call < timestamps.Length; call++)
        {
            pings.Enqueue(timestamps[call]);
            DropExpired(pings, timestamps[call]);
            counts[call] = pings.Count;
        }

        return counts;
    }

    // Timestamps arrive in non-decreasing order, so everything older than the window
    // is at the front and leaves in one uninterrupted run.
    private static void DropExpired(RepoQueue pings, int t)
    {
        while (pings.TryPeek(out var oldest) && oldest < t - WindowMilliseconds)
        {
            pings.TryDequeue(out _);
        }
    }
}
