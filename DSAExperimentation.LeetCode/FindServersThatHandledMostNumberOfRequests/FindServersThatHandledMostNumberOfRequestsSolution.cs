using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.FenwickTree;
using DSAExperimentation.DataStructures.Heap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.FindServersThatHandledMostNumberOfRequests;

// LeetCode 1606. Find Servers That Handled Most Number of Requests: request i
// prefers server i % k, falls through to the next free server around the ring, and
// is dropped outright when every server is busy. Report the servers that handled
// the most requests.
//
// Both strategies simulate the same arrival sequence and differ only in how they
// answer "smallest free server index at or after `start`, wrapping": a walk around
// the ring, or a prefix-sum ceiling query.
internal static class FindServersThatHandledMostNumberOfRequestsSolution
{
    // A request that finds no free server is dropped, not queued.
    private const int NoFreeServer = -1;

    // The textbook answer: one `freeAt` timestamp per server, and a walk of up to k
    // servers around the ring per request - O(n*k), BCL-only, the arm the composed
    // solution below has to justify itself against.
    public static int[] BusiestServersByLinearScanRing(int k, int[] arrival, int[] load)
    {
        var freeAt = new int[k];
        var handled = new int[k];

        for (var i = 0; i < arrival.Length; i++)
        {
            var server = ScanRingForFreeServer(freeAt, k, i % k, arrival[i]);

            if (server == NoFreeServer)
            {
                continue;
            }

            freeAt[server] = arrival[i] + load[i];
            handled[server]++;
        }

        return BusiestServers(handled);
    }

    private static int ScanRingForFreeServer(int[] freeAt, int k, int start, int arrivalTime)
    {
        for (var offset = 0; offset < k; offset++)
        {
            var candidate = (start + offset) % k;

            if (freeAt[candidate] <= arrivalTime)
            {
                return candidate;
            }
        }

        return NoFreeServer;
    }

    // Two repo primitives standing in for the textbook "TreeSet of free servers +
    // TreeSet of (endTime, server) busy servers" this problem is usually solved with.
    //
    // Busy servers: a Heap<(int End, int Server), MinHeapOrder<(int,int)>> ordered by
    // end time (KClosestPointsToOrigin precedent - ValueTuple's own IComparable
    // already orders by End first, Server second) - its root is always the next
    // server due to free.
    //
    // Free servers: a FenwickTree<int, SumOperation<int>> of 0/1 availability.
    // Finding the first free server at or after `start` reduces to "smallest index
    // whose availability prefix sum has grown past the count already seen before
    // start" - PrefixQuery is monotonic non-decreasing (deltas are only 0/1), so that
    // index is found by wrapping the tree in an IRandomAccessSequence<int> (Get(i) =
    // PrefixQuery(i)) and handing it to this repo's own BinarySearch.LowerBound
    // (CountOfSmallerNumbersAfterSelf/ExamRoom precedent for "wrap a repo structure
    // as a Sequence view, then search it"), instead of a linear scan around the ring.
    public static int[] BusiestServersByFenwickCeilingAndHeap(int k, int[] arrival, int[] load)
    {
        var pool = new ServerPool(k);

        for (var i = 0; i < arrival.Length; i++)
        {
            pool.ProcessRequest(i, arrival[i], load[i]);
        }

        return BusiestServers(pool.Handled);
    }

    // LeetCode reports every server tied for the most requests, in ascending order.
    private static int[] BusiestServers(int[] handled)
    {
        var maxHandled = handled.Max();
        var busiest = new List<int>();

        for (var server = 0; server < handled.Length; server++)
        {
            if (handled[server] == maxHandled)
            {
                busiest.Add(server);
            }
        }

        return [.. busiest];
    }

    private sealed class ServerPool(int k)
    {
        private readonly FenwickTree<int, SumOperation<int>> _availability = new(Enumerable.Repeat(1, k).ToArray());
        private readonly Heap<(int End, int Server), MinHeapOrder<(int, int)>> _busy = new();

        public int[] Handled { get; } = new int[k];

        public void ProcessRequest(int i, int arrivalTime, int load)
        {
            while (_busy.TryPeek(out var freed) && freed.End <= arrivalTime)
            {
                _busy.TryPop(out freed);
                _availability.Add(freed.Server, 1);
            }

            if (!TryFindAvailableServer(_availability, k, i % k, out var server))
            {
                return;
            }

            _availability.Add(server, -1);
            _busy.Push((arrivalTime + load, server));
            Handled[server]++;
        }
    }

    // Smallest available server index >= start, wrapping to [0, start) when nothing
    // is free from start through the end of the ring.
    private static bool TryFindAvailableServer(
        FenwickTree<int, SumOperation<int>> availability, int k, int start, out int server)
    {
        var totalAvailable = availability.PrefixQuery(k - 1);

        if (totalAvailable == 0)
        {
            server = NoFreeServer;
            return false;
        }

        var beforeStart = start == 0 ? 0 : availability.PrefixQuery(start - 1);
        var threshold = beforeStart < totalAvailable ? beforeStart + 1 : 1;
        var sequence = new AvailabilityPrefixSequence(availability, k);

        server = BinarySearch.LowerBound(sequence, threshold);
        return true;
    }

    private readonly struct AvailabilityPrefixSequence(FenwickTree<int, SumOperation<int>> availability, int length)
        : IRandomAccessSequence<int>
    {
        public int Length => length;

        public int Get(int index) => availability.PrefixQuery(index);
    }
}
